using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Lanes")]
    public Transform[] lanes;

    [Header("Points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public float coinHoverHeight = 0.5f;
    public float coinSpacing = 1.5f;
    public float patternGap = 3f;

    public LayerMask obstacleMask;

    private void Start()
    {
        GenerateCoinsForSegment();
    }

    void GenerateCoinsForSegment()
    {
        float totalLength = Vector3.Distance(startPoint.position, endPoint.position);
        float currentZ = 5f;
        float startZ = startPoint.position.z;

        int lastLane = 1;

        while (currentZ < totalLength - 10f)
        {
            float worldZ = startZ + currentZ;

            int lane = Mathf.Clamp(lastLane + Random.Range(-1, 2), 0, 2);

            float r = Random.value;

            if (r < 0.6f)
            {
                int length = Random.Range(8, 12);
                SpawnLine(lane, worldZ, length);
                currentZ += length * coinSpacing;
            }
            else
            {
                currentZ += SpawnZigZag(lane, worldZ);
            }

            lastLane = lane;
            currentZ += patternGap;
        }
    }

    void SpawnLine(int laneIndex, float worldZ, int length)
    {
        float y = lanes[laneIndex].position.y + coinHoverHeight;

        for (int i = 0; i < length; i++)
        {
            Vector3 pos = new Vector3(
                lanes[laneIndex].position.x,
                y,
                worldZ + i * coinSpacing
            );

            TrySpawnCoin(pos);
        }
    }

    float SpawnZigZag(int startLane, float worldZ)
    {
        int lane = startLane;
        int steps = 10;

        float y = lanes[0].position.y + coinHoverHeight;

        for (int i = 0; i < steps; i++)
        {
            if (i == 4) lane = 1;
            if (i == 8) lane = (startLane == 0) ? 2 : 0;

            Vector3 pos = new Vector3(
                lanes[lane].position.x,
                y,
                worldZ + i * coinSpacing
            );

            TrySpawnCoin(pos);
        }

        return steps * coinSpacing;
    }

    void TrySpawnCoin(Vector3 pos)
    {
        if (!Physics.CheckSphere(pos, 0.3f, obstacleMask))
        {
            Instantiate(coinPrefab, pos, Quaternion.identity, transform);
        }
    }
}