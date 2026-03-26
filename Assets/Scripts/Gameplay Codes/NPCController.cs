using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class NPCController : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerRef;

    [Header("Movement Settings")]
    public float laneChangeSpeed = 15f;
    public float laneDistance = 3f;
    public float roadCenterX;
    private float targetPositionX;

    [Header("Behavior")]
    public float lookAheadDistance = 10f;
    public LayerMask obstacleLayer;
    public float reactionTime = 0.1f;
    private Collider lastObstacle;

    public float decisionCooldown = 0.5f;
    private bool isChangingLane = false;

    [Header("Speed Boost (try lang)")]
    public bool enableSpeedBoosts = true;
    public float boostMultiplier = 0.9f;
    public float minBoostInterval = 8f;
    public float maxBoostInterval = 20f;
    public float boostDuration = 2.5f;

    [Header("Physics")]
    public float gravity = -20f;
    public float jumpHeight = 2f;
    public float slideDuration = 1.0f;
    public float slideHeight = 0.5f;

    private CharacterController controller;
    private Animator anim;
    public int currentLane = 1;
    private float currentSpeed;
    private float verticalVelocity;
    private bool isSliding = false;
    private bool isBoosting = false;
    private float originalHeight;
    private Vector3 originalCenter;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        if (playerRef == null) playerRef = FindObjectOfType<PlayerMovement>();

        targetPositionX = transform.position.x;

        originalHeight = controller.height;
        originalCenter = controller.center;

        StartCoroutine(SpeedBoostRoutine());
    }

    void Update()
    {
        if (controller == null || !controller.enabled) return;

        SyncSpeedWithPlayer();
        DetectObstacles();
        MoveNPC();
        CheckIfCaught();
        CheckIfPlayerPassed();
    }

    void SyncSpeedWithPlayer()
    {
        if (playerRef == null) return;

        float baseSpeed = playerRef.currentSpeed;

        // tinanggal q rubber banding
        currentSpeed = isBoosting ? baseSpeed * 1.05f : baseSpeed;
    }

    void CheckIfPlayerPassed()
    {
        if (playerRef == null) return;

        if (transform.position.z < playerRef.transform.position.z)
        {
            // if malagpasan ng player
            RevivalSystem revival = RevivalSystem.Instance;
            if (revival != null)
            {
                Destroy(gameObject);
                revival.SpawnChaseNPC(); 
            }
        }
    }

    // movements
    void MoveNPC()
    {
        targetPositionX = roadCenterX + (currentLane - 1) * laneDistance;

        Vector3 moveDir = Vector3.zero;

        float xDiff = targetPositionX - transform.position.x;

        if (Mathf.Abs(xDiff) > 0.01f)
        {
            float smoothX = xDiff * laneChangeSpeed;
            moveDir.x = smoothX;
        }
        else
        {
            Vector3 snappedPos = transform.position;
            snappedPos.x = targetPositionX;
            transform.position = snappedPos;
            moveDir.x = 0;
        }

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        moveDir.y = verticalVelocity;
        moveDir.z = currentSpeed;

        controller.Move(moveDir * Time.deltaTime);
    }

    // more detection
    void DetectObstacles()
    {
        if (isChangingLane) return;

        float dynamicDistance = 15f + (currentSpeed * 0.8f);
        RaycastHit hit;

        Vector3 highOrigin = transform.position + Vector3.up * 1.2f;
        bool hitSomething = Physics.SphereCast(highOrigin, 0.4f, Vector3.forward, out hit, dynamicDistance, obstacleLayer);

        if (!hitSomething)
        {
            Vector3 lowOrigin = transform.position + Vector3.up * 0.2f; // Check near feet
            hitSomething = Physics.SphereCast(lowOrigin, 0.2f, Vector3.forward, out hit, dynamicDistance, obstacleLayer);
        }

        if (hitSomething)
        {
            if (hit.collider != lastObstacle)
            {
                lastObstacle = hit.collider;
                StartCoroutine(LaneChangeCooldownRoutine(hit.collider));
            }
        }
    }

    IEnumerator LaneChangeCooldownRoutine(Collider obstacle)
    {
        isChangingLane = true;
        DecideAvoidanceAction(obstacle);

        yield return new WaitForSeconds(0.2f);

        isChangingLane = false;
        lastObstacle = null;
    }

    void DecideAvoidanceAction(Collider obstacle)
    {
        float npcFeetY = transform.position.y;

        float obstacleHeight = obstacle.bounds.size.y;
        float obstacleTop = obstacle.bounds.max.y;
        float obstacleBottom = obstacle.bounds.min.y;

        float dist = Vector3.Distance(transform.position, obstacle.transform.position);
        float timeToHit = dist / Mathf.Max(currentSpeed, 0.1f);

        if (obstacleHeight <= 1.0f)
        {
            if (timeToHit < 0.9f && controller.isGrounded)
            {
                Jump();
                Debug.Log("NPC: Low obstacle - JUMP");
                return;
            }
        }

        if (obstacleBottom > npcFeetY + 1.0f)
        {
            Slide();
            Debug.Log("NPC: Floating obstacle - SLIDE");
            return;
        }

        TryChangeLaneRandomly();
        Debug.Log("NPC: Big obstacle - SWERVE");
    }

    IEnumerator WaitAndJump(float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        Jump();
    }

    void Jump()
    {
        anim.SetTrigger("Jump");
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void Slide()
    {
        if (verticalVelocity > 0f) return;
        if (!isSliding && controller.isGrounded) 
            StartCoroutine(SlideRoutine());
    }

    IEnumerator SlideRoutine()
    {
        isSliding = true;
        anim.SetTrigger("Slide");

        controller.height = slideHeight;
        controller.center = new Vector3(0, slideHeight / 2, 0);

        yield return new WaitForSeconds(slideDuration);

        controller.height = originalHeight;
        controller.center = originalCenter;
        isSliding = false;
    }

    // laning nya
    bool TryChangeLaneRandomly()
    {
        int nextLane = currentLane;

        if (currentLane == 0 || currentLane == 2)
        {
            nextLane = 1;
        }
        else
        {
            if (IsLaneClear(0)) nextLane = 0;
            else if (IsLaneClear(2)) nextLane = 2;
        }

        if (nextLane != currentLane)
        {
            currentLane = nextLane;
            return true;
        }
        return false;
    }

    bool IsLaneClear(int laneIndex)
    {
        float laneX = roadCenterX + (laneIndex - 1) * laneDistance;
        Vector3 checkOrigin = new Vector3(laneX, transform.position.y + 1.2f, transform.position.z);

        return !Physics.Raycast(checkOrigin, Vector3.forward, 15f, obstacleLayer);
    }

    void CheckIfCaught()
    {
        if (playerRef == null || !this.enabled) return;

        float distance = Vector3.Distance(transform.position, playerRef.transform.position);

        if (distance < 2.5f)
        {
            if (RevivalSystem.Instance != null)
            {
                RevivalSystem.Instance.OnNPCCaught();
            }

            this.enabled = false;
        }
    }

    IEnumerator SpeedBoostRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minBoostInterval, maxBoostInterval);
            yield return new WaitForSeconds(waitTime);

            if (enableSpeedBoosts)
            {
                isBoosting = true;
                yield return new WaitForSeconds(boostDuration);
                isBoosting = false;
            }
        }
    }
}