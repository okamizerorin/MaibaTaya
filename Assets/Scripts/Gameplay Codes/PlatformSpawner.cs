using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class PlatformSpawner : MonoBehaviour
{
    public static PlatformSpawner Instance;

    public ObstacleGenerator obstacleSpawner;
    public PowerupSpawner powerupSpawner;
    public Transform player;
    public GameObject[] platformPrefabs;

    [Header("Spawn Settings")]
    public float spawnAheadDistance = 60f;
    public float deleteBehindDistance = 50f;
    public float bonusDeleteDistance = 200f;
    public int initialPlatforms = 4;

    [HideInInspector]
    public Transform lastPlatformSpawned;

    public Vector3 platformRotation = new Vector3(0, 90, 0);

    private List<GameObject> spawnedPlatforms = new List<GameObject>();
    private Transform lastEndPoint;

    [Header("Road Levels by Duration")]
    public float[] roadDurations = { 20f, 20f, 20f, 20f };

    private int currentRoadIndex = 0;
    private float roadTimer = 0f;
    private bool triggerBonusNext = false;

    [Header("Road UI")]
    public CanvasGroup road0UI;
    public CanvasGroup road1UI;
    public CanvasGroup road2UI;
    public CanvasGroup road3UI;
    public CanvasGroup bonusRoadUI;

    public float firstRoadDelay = 2f;
    public float otherRoadDelay = 4f;

    bool road0Shown;
    bool road1Shown;
    bool road2Shown;
    bool road3Shown;
    bool bonusShown;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (player == null || platformPrefabs == null || platformPrefabs.Length == 0)
        {
            enabled = false;
            return;
        }

        SpawnSpecificPlatform(platformPrefabs[0], true);

        for (int i = 0; i < initialPlatforms - 1; i++)
        {
            SpawnNextPlatform();
        }
    }

    void Update()
    {
        if (!triggerBonusNext)
        {
            roadTimer += Time.deltaTime;

            if (roadTimer >= roadDurations[currentRoadIndex])
            {
                roadTimer = 0f;
                currentRoadIndex++;

                if (currentRoadIndex >= 4)
                {
                    triggerBonusNext = true;
                }
            }
        }

        if (player.position.z + spawnAheadDistance > lastEndPoint.position.z)
        {
            SpawnNextPlatform();
        }

        CleanupOldPlatforms();
    }

    void SpawnNextPlatform()
    {
        bool isBonusRoad = triggerBonusNext;
        GameObject prefabToSpawn = isBonusRoad ? platformPrefabs[4] : platformPrefabs[currentRoadIndex];

        SpawnSpecificPlatform(prefabToSpawn, false);

        //ui platform namee, level switch label
        if (currentRoadIndex == 0 && !road0Shown)
        {
            road0Shown = true;
            StartCoroutine(ShowRoadUI(road0UI, firstRoadDelay));
        }

        if (currentRoadIndex == 1 && !road1Shown)
        {
            road1Shown = true;
            StartCoroutine(ShowRoadUI(road1UI, otherRoadDelay));
        }

        if (currentRoadIndex == 2 && !road2Shown)
        {
            road2Shown = true;
            StartCoroutine(ShowRoadUI(road2UI, otherRoadDelay));
        }

        if (currentRoadIndex == 3 && !road3Shown)
        {
            road3Shown = true;
            StartCoroutine(ShowRoadUI(road3UI, otherRoadDelay));
        }

        if (isBonusRoad && !bonusShown)
        {
            bonusShown = true;
            StartCoroutine(ShowRoadUI(bonusRoadUI, otherRoadDelay));
        }

        if (isBonusRoad)
        {
            triggerBonusNext = false;
            currentRoadIndex = 0;
            roadTimer = 0f;
        }
    }

    //ui coroutine
    IEnumerator ShowRoadUI(CanvasGroup ui, float delay)
    {
        yield return new WaitForSeconds(delay);

        ui.gameObject.SetActive(true);
        ui.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        seq.Append(ui.DOFade(1f, 0.5f));
        seq.AppendInterval(2f);
        seq.Append(ui.DOFade(0f, 0.5f));

        yield return seq.WaitForCompletion();

        ui.gameObject.SetActive(false);
    }

    void SpawnSpecificPlatform(GameObject prefab, bool isFirstPlatform)
    {
        GameObject platform = Instantiate(prefab, Vector3.zero, Quaternion.Euler(platformRotation));

        Transform startPoint = platform.transform.Find("StartPoint");
        Transform endPoint = platform.transform.Find("EndPoint");

        if (startPoint == null || endPoint == null)
        {
            Debug.LogError($"Platform '{platform.name}' must have StartPoint and EndPoint.");
            Destroy(platform);
            return;
        }

        if (isFirstPlatform)
        {
            lastEndPoint = endPoint;
        }
        else
        {
            Vector3 offset = platform.transform.position - startPoint.position;
            platform.transform.position = lastEndPoint.position + offset;
            lastEndPoint = endPoint;
        }

        spawnedPlatforms.Add(platform);
        lastPlatformSpawned = platform.transform;

        if (powerupSpawner != null)
        {
            powerupSpawner.AttemptSpawn(platform.transform);
        }

        // obs gen if not bonus road
        if (obstacleSpawner != null && prefab != platformPrefabs[4])
        {
            obstacleSpawner.GenerateObstaclesForPlatform(platform.transform);
        }
    }

    void CleanupOldPlatforms()
    {
        if (spawnedPlatforms.Count == 0) return;

        GameObject oldestPlatform = spawnedPlatforms[0];
        float deleteDistance = oldestPlatform.name.Contains(platformPrefabs[4].name) ? bonusDeleteDistance : deleteBehindDistance;

        if (player.position.z - oldestPlatform.transform.position.z > deleteDistance)
        {
            spawnedPlatforms.RemoveAt(0);
            Destroy(oldestPlatform);
        }
    }
}