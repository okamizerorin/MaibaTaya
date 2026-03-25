using UnityEngine;

public class NPCObstacleDetector : MonoBehaviour
{
    private NPCChaseMovement npc;

    void Start()
    {
        npc = GetComponentInParent<NPCChaseMovement>();
        if (npc == null)
            Debug.LogError("NPCChaseMovement not found on parent!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            float obstacleHeight = other.bounds.size.y;

            // decide action
            if (obstacleHeight < 1f)
                npc.Jump();
            else
                npc.Slide();
        }
    }
}
