using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObstaclePattern", menuName = "Runner System/Obstacle Pattern")]
public class ObstaclePattern : ScriptableObject
{
    public enum ObsType { None, Hard, Jump, Slide, Moving }

    [System.Serializable]
    public struct RowData
    {
        public ObsType LeftLane;
        public ObsType MiddleLane;
        public ObsType RightLane;
    }

    [Range(1, 5)] public int difficultyRating = 1;

    public float internalSpacingMultiplier = 1.0f;

    [Header("Patterns")]
    public List<RowData> rows = new List<RowData>();

    public void AddRow(ObsType l, ObsType m, ObsType r)
    {
        rows.Add(new RowData
        {
            LeftLane = l,
            MiddleLane = m,
            RightLane = r
        });
    }
}