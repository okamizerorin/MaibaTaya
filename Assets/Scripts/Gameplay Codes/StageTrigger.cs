using UnityEngine;

public class StageTrigger : MonoBehaviour
{
    [Header("Platform Prefabs")]
    public GameObject[] platformPrefabs;

    // old script, naiba na na approach on another script!
    private static int currentIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlatformTrigger"))
        {
            GameObject prefabToSpawn = platformPrefabs[currentIndex];

            Vector3 spawnPos = new Vector3(0, 0, 100);

            // pang rotatelang
            Instantiate(prefabToSpawn, spawnPos, Quaternion.Euler(0, 90, 0));

            currentIndex++;
            if (currentIndex >= platformPrefabs.Length)
                currentIndex = 0;

            other.enabled = false;
        }
    }
}
