using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public float driveSpeed = 15f;
    public float activationDistance = 45f;

    private Transform player;
    private bool isMoving = false;

    void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void OnEnable()
    {
        isMoving = false;
    }

    void Update()
    {
        if (player == null || driveSpeed <= 0) 
            return;

        if (!isMoving && Vector3.Distance(transform.position, player.position) < activationDistance)
        {
            isMoving = true;
        }

        if (isMoving)
        {
            transform.position += Vector3.back * driveSpeed * Time.deltaTime;
        }
    }
}