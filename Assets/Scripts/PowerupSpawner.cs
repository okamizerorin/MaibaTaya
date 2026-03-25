using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerupSpawner : MonoBehaviour
{
    [Header("Powerups")]
    public GameObject[] powerupPrefabs;
    private List<int> powerupShuffleBag = new List<int>();

    [Header("Spawn Interval")]
    public int minPlatformsBetween = 1;
    public int maxPlatformsBetween = 3;
    private int platformsSinceLast;
    private int targetInterval;

    [Header("Lane & Obstacle")]
    public LayerMask obstacleMask;
    public LayerMask coinMask;
    public float checkRadius = 0.8f;
    public float hoverOffset = 0.6f;

    private void Start()
    {
        targetInterval = Random.Range(minPlatformsBetween, maxPlatformsBetween + 1);
        RefillShuffleBag();
    }

    public void AttemptSpawn(Transform platform)
    {
        platformsSinceLast++;

        if (platformsSinceLast < targetInterval) return;

        // laningg
        string[] laneNames = { "LaneLeft", "LaneMiddle", "LaneRight" };
        ShuffleArray(laneNames);

        foreach (string laneName in laneNames)
        {
            Transform lane = platform.Find(laneName);
            if (lane != null && IsLaneClear(lane))
            {
                SpawnPowerup(lane, platform);

                platformsSinceLast = 0;
                targetInterval = Random.Range(minPlatformsBetween, maxPlatformsBetween + 1);
                return;
            }
        }
    }

    bool IsLaneClear(Transform lane)
    {
        return !Physics.CheckSphere(lane.position + Vector3.up, checkRadius, obstacleMask | coinMask);
    }

    void SpawnPowerup(Transform lane, Transform platform)
    {
        if (powerupPrefabs.Length == 0) return;

        Transform start = platform.Find("StartPoint");
        Transform end = platform.Find("EndPoint");
        if (start == null || end == null) return;

        Vector3 spawnPos = Vector3.zero;
        bool spotFound = false;

        // checkingg pang spawn
        for (int i = 0; i < 5; i++)
        {
            float randomT = Random.Range(0.1f, 0.9f);
            float zPos = Mathf.Lerp(start.position.z, end.position.z, randomT);

            spawnPos = new Vector3(lane.position.x, lane.position.y + hoverOffset, zPos);

            if (!Physics.CheckSphere(spawnPos, 1.0f, obstacleMask | coinMask))
            {
                spotFound = true;
                break;
            }
        }

        if (spotFound)
        {
            if (powerupShuffleBag.Count == 0) RefillShuffleBag();
            int index = powerupShuffleBag[0];
            powerupShuffleBag.RemoveAt(0);

            GameObject powerup = Instantiate(powerupPrefabs[index], spawnPos, Quaternion.identity, platform);

            if (!powerup.GetComponent<PowerupFloat>())
                powerup.AddComponent<PowerupFloat>();
        }
        else
        {
            platformsSinceLast--;
            Debug.Log("Powerup spawn blocked by obstacles, retrying next platform.");
        }
    }

    void RefillShuffleBag()
    {
        for (int i = 0; i < powerupPrefabs.Length; i++)
        {
            powerupShuffleBag.Add(i);
        }

        for (int i = 0; i < powerupShuffleBag.Count; i++)
        {
            int temp = powerupShuffleBag[i];
            int randomIndex = Random.Range(i, powerupShuffleBag.Count);
            powerupShuffleBag[i] = powerupShuffleBag[randomIndex];
            powerupShuffleBag[randomIndex] = temp;
        }
    }

    void ShuffleArray(string[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            string temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}