using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class NPCChaseMovement : MonoBehaviour
{
    [Header("Movement")]
    public float startSpeed = 12f;
    public float maxSpeed = 35f;
    public float speedIncreaseRate = 0.18f;
    private float currentSpeed;

    public float sideSpeed = 10f;
    public float laneDistance = 3f;
    private int desiredLane = 1;
    private Vector3 targetPosition;

    [Header("Jump/Slide")]
    public float jumpHeight = 5f;
    public float slideDuration = 1f;
    private bool isSliding = false;

    [Header("Obstacle Detection")]
    public float obstacleDetectionDistance = 5f;
    public LayerMask obstacleLayer;

    [Header("NPC Lane Change")]
    public float laneChangeInterval = 2f;
    private float laneChangeTimer;

    private CharacterController controller;
    private Animator anim;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        anim.SetBool("Run", true);

        laneChangeTimer = laneChangeInterval;
        targetPosition = transform.position;

        currentSpeed = startSpeed;
    }

    void Update()
    {
        GradualSpeedIncrease();
        MoveForward();
        RandomLaneChange();
        ObstacleDetection();
    }

    void GradualSpeedIncrease()
    {
        currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate * Time.deltaTime, maxSpeed);
    }

    void MoveForward()
    {
        targetPosition.x = (desiredLane - 1) * laneDistance;
        float newX = Mathf.MoveTowards(transform.position.x, targetPosition.x, sideSpeed * Time.deltaTime);

        Vector3 move = new Vector3(newX - transform.position.x, 0f, currentSpeed * Time.deltaTime);
        controller.Move(move);
    }

    void RandomLaneChange()
    {
        laneChangeTimer -= Time.deltaTime;
        if (laneChangeTimer <= 0f)
        {
            laneChangeTimer = laneChangeInterval;

            int newLane = Random.Range(0, 3);
            if (newLane != desiredLane)
                desiredLane = newLane;
        }
    }

    void ObstacleDetection()
    {
        if (isSliding) return;

        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, Vector3.forward, out hit, obstacleDetectionDistance, obstacleLayer))
        {
            if (hit.collider.CompareTag("Obstacles"))
            {
                float obstacleHeight = hit.collider.bounds.size.y;

                if (obstacleHeight < 1f)
                    Jump();
                else
                    Slide();
            }
        }
    }

    public void Jump()
    {
        anim.SetTrigger("Jump");
    }

    public void Slide()
    {
        if (!isSliding)
            StartCoroutine(SlideRoutine());
    }

    IEnumerator SlideRoutine()
    {
        isSliding = true;
        anim.SetTrigger("Slide");

        float originalHeight = controller.height;
        controller.height = originalHeight / 2f;
        yield return new WaitForSeconds(slideDuration);
        controller.height = originalHeight;

        isSliding = false;
    }
}
