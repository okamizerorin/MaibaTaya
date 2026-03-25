using UnityEngine;
using System.Collections.Generic;

public class RoadTheme : MonoBehaviour
{
    public bool isBonusLevel = false;

    [Header("Lanes")]
    public Transform laneLeft;
    public Transform laneMiddle;
    public Transform laneRight;

    [Header("Obstacle Prefabs")]
    public GameObject[] hardPrefabs;
    public GameObject[] jumpPrefabs;
    public GameObject[] slidePrefabs;
    public GameObject[] movingPrefabs;

    [Header("Patterns")]
    public List<ObstaclePattern> themePatterns;

    public Transform GetLaneAnchor(int laneIndex)
    {
        if (laneIndex == 0) return laneLeft;
        if (laneIndex == 1) return laneMiddle;
        return laneRight;
    }
}