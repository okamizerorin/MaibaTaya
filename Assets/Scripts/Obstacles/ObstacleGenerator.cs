using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ObstacleGenerator : MonoBehaviour
{
    [Header("Spacing")]
    public float startBaseSpacing = 15f;
    public float endBaseSpacing = 9f;
    public float patternGap = 18f;
    public float globalXOffset = 0f;

    [Header("Difficulty")]
    public Transform player;
    public float difficultyDistanceStep = 200f;
    public int maxDifficulty = 5;

    private ObstaclePattern lastPattern;

    public void GenerateObstaclesForPlatform(Transform platform)
    {
        RoadTheme theme = platform.GetComponent<RoadTheme>();
        if (theme == null || theme.isBonusLevel) return;
        if (theme.themePatterns.Count == 0) return;

        Transform start = platform.Find("StartPoint");
        Transform end = platform.Find("EndPoint");

        if (start == null || end == null)
        {
            Debug.LogError("Missing StartPoint or EndPoint!");
            return;
        }

        float platformLength = end.position.z - start.position.z;

        float safeEndZ = platformLength - 10f;

        float currentZOffset = 15f;

        int difficulty = GetDifficultyLevel();

        var validPatterns = theme.themePatterns
            .Where(p => p.difficultyRating <= difficulty)
            .ToList();

        if (validPatterns.Count == 0) return;

        int patternsToSpawn = 3;

        for (int p = 0; p < patternsToSpawn; p++)
        {
            if (currentZOffset >= safeEndZ) break;

            ObstaclePattern chosenPattern = GetNonRepeatingPattern(validPatterns);

            ObstaclePattern.RowData? previousRow = null;

            foreach (var rawRow in chosenPattern.rows)
            {
                // if lagpas
                if (currentZOffset >= safeEndZ)
                    break;

                var row = SmartShuffleRow(rawRow);
                row = EnsureSafeLane(row);

                // combos
                if (previousRow.HasValue)
                {
                    if (IsActionRow(previousRow.Value) && IsHeavyRow(row))
                    {
                        currentZOffset += 10f;
                    }
                }

                float rowLength = SpawnRow(row, currentZOffset, platform, theme);

                float dynamicSpacing = Mathf.Lerp(
                    startBaseSpacing,
                    endBaseSpacing,
                    difficulty / (float)maxDifficulty
                );

                float spacing = Mathf.Max(
                    dynamicSpacing * chosenPattern.internalSpacingMultiplier,
                    rowLength + 4f
                );

                currentZOffset += spacing;

                previousRow = row;
            }

            currentZOffset += patternGap;

            // if no space
            if (currentZOffset >= safeEndZ)
                break;
        }
    }

    int GetDifficultyLevel()
    {
        if (player == null) return 1;

        float distance = player.position.z;
        int level = Mathf.FloorToInt(distance / difficultyDistanceStep) + 1;

        return Mathf.Clamp(level, 1, maxDifficulty);
    }

    ObstaclePattern GetNonRepeatingPattern(List<ObstaclePattern> patterns)
    {
        ObstaclePattern chosen;

        do
        {
            chosen = patterns[Random.Range(0, patterns.Count)];
        }
        while (chosen == lastPattern && patterns.Count > 1);

        lastPattern = chosen;
        return chosen;
    }

    // rows and laning partt
    bool IsActionRow(ObstaclePattern.RowData row)
    {
        return row.LeftLane == ObstaclePattern.ObsType.Jump ||
               row.MiddleLane == ObstaclePattern.ObsType.Jump ||
               row.RightLane == ObstaclePattern.ObsType.Jump ||
               row.LeftLane == ObstaclePattern.ObsType.Slide ||
               row.MiddleLane == ObstaclePattern.ObsType.Slide ||
               row.RightLane == ObstaclePattern.ObsType.Slide;
    }

    bool IsHeavyRow(ObstaclePattern.RowData row)
    {
        return row.LeftLane == ObstaclePattern.ObsType.Moving ||
               row.MiddleLane == ObstaclePattern.ObsType.Moving ||
               row.RightLane == ObstaclePattern.ObsType.Moving ||
               row.LeftLane == ObstaclePattern.ObsType.Hard ||
               row.MiddleLane == ObstaclePattern.ObsType.Hard ||
               row.RightLane == ObstaclePattern.ObsType.Hard;
    }

    ObstaclePattern.RowData EnsureSafeLane(ObstaclePattern.RowData row)
    {
        bool leftBlocked = row.LeftLane == ObstaclePattern.ObsType.Hard || row.LeftLane == ObstaclePattern.ObsType.Moving;
        bool midBlocked = row.MiddleLane == ObstaclePattern.ObsType.Hard || row.MiddleLane == ObstaclePattern.ObsType.Moving;
        bool rightBlocked = row.RightLane == ObstaclePattern.ObsType.Hard || row.RightLane == ObstaclePattern.ObsType.Moving;

        if (leftBlocked && midBlocked && rightBlocked)
        {
            int safeLane = Random.Range(0, 3);

            if (safeLane == 0) row.LeftLane = ObstaclePattern.ObsType.None;
            if (safeLane == 1) row.MiddleLane = ObstaclePattern.ObsType.None;
            if (safeLane == 2) row.RightLane = ObstaclePattern.ObsType.None;
        }

        return row;
    }

    ObstaclePattern.RowData SmartShuffleRow(ObstaclePattern.RowData row)
    {
        int filled =
            (row.LeftLane != ObstaclePattern.ObsType.None ? 1 : 0) +
            (row.MiddleLane != ObstaclePattern.ObsType.None ? 1 : 0) +
            (row.RightLane != ObstaclePattern.ObsType.None ? 1 : 0);

        if (filled <= 1)
            return row;

        var lanes = new ObstaclePattern.ObsType[]
        {
            row.LeftLane,
            row.MiddleLane,
            row.RightLane
        };

        for (int i = 0; i < lanes.Length; i++)
        {
            int rand = Random.Range(i, lanes.Length);
            var temp = lanes[i];
            lanes[i] = lanes[rand];
            lanes[rand] = temp;
        }

        return new ObstaclePattern.RowData
        {
            LeftLane = lanes[0],
            MiddleLane = lanes[1],
            RightLane = lanes[2]
        };
    }

    float SpawnRow(ObstaclePattern.RowData row, float zOffset, Transform platform, RoadTheme theme)
    {
        float maxLength = 0f;

        maxLength = Mathf.Max(maxLength, SpawnInLane(row.LeftLane, 0, zOffset, platform, theme));
        maxLength = Mathf.Max(maxLength, SpawnInLane(row.MiddleLane, 1, zOffset, platform, theme));
        maxLength = Mathf.Max(maxLength, SpawnInLane(row.RightLane, 2, zOffset, platform, theme));

        return maxLength;
    }

    float SpawnInLane(ObstaclePattern.ObsType type, int laneIndex, float zOffset, Transform platform, RoadTheme theme)
    {
        if (type == ObstaclePattern.ObsType.None) return 0f;

        GameObject prefab = GetPrefabFromPool(type, theme);
        if (prefab == null) return 0f;

        Transform laneAnchor = theme.GetLaneAnchor(laneIndex);
        if (laneAnchor == null) return 0f;

        Transform groundAnchor = laneAnchor.Find("GroundAnchor");
        if (groundAnchor == null) return 0f;

        Vector3 spawnPos = groundAnchor.position + new Vector3(globalXOffset, 0f, zOffset);

        Quaternion rot = Quaternion.Euler(0f, 90f, 0f);
        GameObject obj = ObstaclePooler.Instance.SpawnFromPool(prefab, spawnPos, rot);

        Transform gp = obj.transform.Find("GroundPoint");
        if (gp != null)
        {
            float yOffset = gp.position.y - obj.transform.position.y;
            obj.transform.position -= new Vector3(0, yOffset, 0);
        }

        obj.transform.SetParent(platform);

        return GetObstacleLength(prefab);
    }

    GameObject GetPrefabFromPool(ObstaclePattern.ObsType type, RoadTheme theme)
    {
        GameObject[] pool = null;

        switch (type)
        {
            case ObstaclePattern.ObsType.Hard: pool = theme.hardPrefabs; break;
            case ObstaclePattern.ObsType.Jump: pool = theme.jumpPrefabs; break;
            case ObstaclePattern.ObsType.Slide: pool = theme.slidePrefabs; break;
            case ObstaclePattern.ObsType.Moving: pool = theme.movingPrefabs; break;
        }

        if (pool != null && pool.Length > 0)
            return pool[Random.Range(0, pool.Length)];

        return null;
    }

    float GetObstacleLength(GameObject prefab)
    {
        Collider col = prefab.GetComponentInChildren<Collider>();
        if (col != null) return col.bounds.size.z;

        Renderer rend = prefab.GetComponentInChildren<Renderer>();
        if (rend != null) return rend.bounds.size.z;

        return 5f;
    }
}