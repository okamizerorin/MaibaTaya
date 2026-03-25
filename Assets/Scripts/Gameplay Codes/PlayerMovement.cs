using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public enum PlayerState
{
    Running,
    Staggered,
    Dead
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("States")]
    public PlayerState currentState = PlayerState.Running;
    public bool isFrozen { get; private set; }

    [Header("Touch Input")]
    public float swipeThreshold = 30f;
    private Vector2 touchStartPos;
    private bool touchActive;

    [Header("Lanes")]
    public float laneDistance = 3f;
    public float sideSpeed = 14f;
    private int desiredLane = 1;
    private float sideVelocity;

    [Header("Speed")]
    public float startSpeed = 7f;
    public float maxSpeed = 35f;
    public float speedIncreaseRate = 0.18f;
    public float currentSpeed;

    // distance meterz
    public float distanceTraveled = 0f;
    private float startZ;

    [Header("Jump")]
    public float jumpHeight = 8f;
    private float verticalVelocity;
    private float gravity = -20f;
    private bool jumpCanceled = false;

    [Header("Slide")]
    public float slideDuration = 0.6f;
    private bool isSliding;
    private float slideTimer;
    public float slideHeight = 1f;
    public float slideCenterY = 0.5f;

    private float originalHeight;
    private Vector3 originalCenter;

    private Animator anim;
    private CharacterController controller;
    private Vector3 targetPosition;
    private Vector3 startPos;

    private int sideMoveHash;
    private bool hasSideMoveParam;

    public LayerMask platformLayer;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        if (anim != null)
        {
            sideMoveHash = Animator.StringToHash("SideMove");

            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.name == "SideMove") hasSideMoveParam = true;
            }

            if (!hasSideMoveParam)
                Debug.LogError($"CRITICAL: 'SideMove' is missing from {anim.runtimeAnimatorController.name}!");
        }

        currentSpeed = startSpeed;
        startZ = transform.position.z;

        startPos = transform.position;
        targetPosition = startPos;

        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        if (isFrozen) return;

        HandleKeyboardInput();
        HandleTouchInput();

        HandleLaneMovement();
        HandleSlideTimer();
        HandleGravity();
        Move();
        IncreaseSpeed();

    }

    // movement controls
    void HandleTouchInput()
    {
        if (Touch.activeTouches.Count == 0) return;

        var touch = Touch.activeTouches[0];

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            touchStartPos = touch.screenPosition;
            touchActive = true;
        }
        else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && touchActive)
        {
            Vector2 delta = touch.screenPosition - touchStartPos;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (Mathf.Abs(delta.x) > swipeThreshold)
                {
                    if (delta.x > 0) MoveRight();
                    else MoveLeft();

                    touchActive = false;
                }
            }
            else
            {
                if (Mathf.Abs(delta.y) > swipeThreshold)
                {
                    if (delta.y > 0) Jump();
                    else Slide();

                    touchActive = false;
                }
            }
        }
    }

    void HandleKeyboardInput()
    {
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) MoveLeft();
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) MoveRight();
        if (Keyboard.current.upArrowKey.wasPressedThisFrame) Jump();
        if (Keyboard.current.downArrowKey.wasPressedThisFrame) Slide();
    }

    void MoveLeft() => desiredLane = Mathf.Max(0, desiredLane - 1);
    void MoveRight() => desiredLane = Mathf.Min(2, desiredLane + 1);

    void HandleLaneMovement()
    {
        targetPosition.x = startPos.x + (desiredLane - 1) * laneDistance;
    }

    void Move()
    {
        Vector3 move = Vector3.zero;

        float oldX = transform.position.x;

        float newX = Mathf.SmoothDamp(
            oldX,
            targetPosition.x,
            ref sideVelocity,
            0.08f
        );

        move.x = newX - oldX;

        float targetSideValue = 0f;
        float xDiff = targetPosition.x - oldX;

        if (Mathf.Abs(xDiff) > 0.01f)
        {
            targetSideValue = Mathf.Sign(xDiff);
        }

        if (anim != null && hasSideMoveParam)
        {
            anim.SetFloat(sideMoveHash, targetSideValue, 0.12f, Time.deltaTime);
        }

        if (currentState != PlayerState.Dead)
        {
            float finalSpeed = currentSpeed;

            GamePowerups powerups = GetComponent<GamePowerups>();
            if (powerups != null && powerups.isSpeedBoostActive)
            {
                finalSpeed *= powerups.speedMultiplier;
            }

            move.z = finalSpeed * Time.deltaTime;
        }

        move.y = verticalVelocity * Time.deltaTime;
        controller.Move(move);
    }

    void IncreaseSpeed()
    {
        currentSpeed = Mathf.Min(
            currentSpeed + speedIncreaseRate * Time.deltaTime,
            maxSpeed
        );
    }

    // jump
    void Jump()
    {
        if (!controller.isGrounded || isSliding || jumpCanceled) return;

        verticalVelocity = jumpHeight;
        MusicBGManager.Instance.PlayJump();
        anim.SetTrigger("Jump");
    }

    void Slide()
    {
        if (isSliding)
        {
            slideTimer = 0f;
            return;
        }

        if (!controller.isGrounded)
        {
            jumpCanceled = true;
            verticalVelocity = -20f;

            anim.SetTrigger("JumpRoll");
        }
        else
        {
            anim.SetTrigger("Slide");
        }

        anim.ResetTrigger("Jump");

        isSliding = true;
        slideTimer = 0f;
        controller.height = slideHeight;
        controller.center = new Vector3(0, slideCenterY, 0);
    }

    void HandleSlideTimer()
    {
        if (!isSliding) return;

        slideTimer += Time.deltaTime;
        if (slideTimer >= slideDuration)
            EndSlide();
    }

    void EndSlide()
    {
        isSliding = false;
        controller.height = originalHeight;
        controller.center = originalCenter;
    }

    void HandleGravity()
    {
        if (controller.isGrounded)
        {
            if (jumpCanceled) jumpCanceled = false;

            if (verticalVelocity < 0)
                verticalVelocity = -2f; 
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    // stagger movementt
    public void OnStagger(float slowdown)
    {
        if (currentState != PlayerState.Running) return;

        currentState = PlayerState.Staggered;
        currentSpeed *= slowdown;
    }

    public void RecoverFromStagger(float originalSpeed)
    {
        currentSpeed = originalSpeed;
        currentState = PlayerState.Running;
    }

    // freezing-
    public void Freeze()
    {
        isFrozen = true;
        currentState = PlayerState.Dead;
        verticalVelocity = 0f;
    }

    public void Unfreeze()
    {
        isFrozen = false;
        currentState = PlayerState.Running;
    }

    // distance meter sana gumana
    public int DistanceInMeters()
    {
        float distance = transform.position.z - startZ;
        return Mathf.Max(0, Mathf.FloorToInt(distance));
    }

    public void ResetDistance()
    {
        startZ = transform.position.z;
    }

    // reset after revive + laning 
    public void ResetSpeed()
    {
        currentSpeed = startSpeed;
    }

    public void ResetMovementState()
    {
        verticalVelocity = 0f;
        desiredLane = 1;
    }

    public void ResetAfterRevive()
    {
        ResetMovementState();

        Animator anim = GetComponentInChildren<Animator>();
        anim.Rebind();
        anim.Update(0f);
    }

    public Transform GetCurrentPlatformTransform()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 2f; 
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 5f, platformLayer))
        {
            return hit.collider.transform;
        }
        return null;
    }

    public int currentLane
    {
        get { return desiredLane; }
    }

    public float forwardSpeed
    {
        get { return currentSpeed; }
    }
}
