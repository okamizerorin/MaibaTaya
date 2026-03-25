using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PatternBuilderTool : EditorWindow
{
    private string patternText = "";
    private int difficulty = 1;
    private float spacingMultiplier = 1f;

    [MenuItem("Tools/Pattern Builder")]
    public static void ShowWindow()
    {
        GetWindow<PatternBuilderTool>("Pattern Builder");
    }

    void OnGUI()
    {
        GUILayout.Label("Paste Pattern (Top Row First)", EditorStyles.boldLabel);

        patternText = EditorGUILayout.TextArea(patternText, GUILayout.Height(150));

        difficulty = EditorGUILayout.IntSlider("Difficulty", difficulty, 1, 5);
        spacingMultiplier = EditorGUILayout.FloatField("Spacing Multiplier", spacingMultiplier);

        if (GUILayout.Button("Create Pattern"))
        {
            CreatePattern();
        }
    }

    void CreatePattern()
    {
        ObstaclePattern pattern = ScriptableObject.CreateInstance<ObstaclePattern>();

        pattern.difficultyRating = difficulty;
        pattern.internalSpacingMultiplier = spacingMultiplier;

        string[] lines = patternText.Split('\n');

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            string[] parts = trimmed.Split(' ');

            if (parts.Length != 3) continue;

            pattern.rows.Add(new ObstaclePattern.RowData
            {
                LeftLane = Convert(parts[0]),
                MiddleLane = Convert(parts[1]),
                RightLane = Convert(parts[2])
            });
        }

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Pattern",
            "NewPattern",
            "asset",
            "Choose location"
        );

        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(pattern, path);
            AssetDatabase.SaveAssets();
        }
    }

    ObstaclePattern.ObsType Convert(string s)
    {
        switch (s)
        {
            case "H": return ObstaclePattern.ObsType.Hard;
            case "J": return ObstaclePattern.ObsType.Jump;
            case "S": return ObstaclePattern.ObsType.Slide;
            case "M": return ObstaclePattern.ObsType.Moving;
            default: return ObstaclePattern.ObsType.None;
        }
    }
}