using TMPro;
using UnityEngine;

public class DistanceMeter : MonoBehaviour
{
    public PlayerMovement player;
    public TextMeshProUGUI distanceText;

    int lastDistance = 0;

    void Update()
    {
        if (player == null || distanceText == null) return;

        int currentDistance = player.DistanceInMeters();

        distanceText.text = currentDistance + " m";

        if (currentDistance > lastDistance)
        {
            int gained = currentDistance - lastDistance;

            MissionSystem.Instance.AddProgress(MissionType.Distance, gained);

            lastDistance = currentDistance;
        }
    }

    public void ResetDistanceTracker()
    {
        lastDistance = 0;
    }
}