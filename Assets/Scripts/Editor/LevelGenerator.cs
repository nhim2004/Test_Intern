using UnityEngine;
using UnityEditor;
using WaterSort.Core;
using WaterSort.Game;
using System.Collections.Generic;

/// <summary>
/// Editor tool to generate new level assets
/// </summary>
public class LevelGenerator
{
    private static readonly string LEVELS_PATH = "Assets/Levels/";

    [MenuItem("WaterSort/Generate Levels/Create Levels 11-20")]
    public static void CreateLevels11To20()
    {
        CreateLevelRange(11, 20);
        EditorUtility.DisplayDialog("Success", "✅ Created levels 11-20!", "OK");
    }

    [MenuItem("WaterSort/Generate Levels/Create Levels 11-13")]
    public static void CreateLevels11To13()
    {
        CreateLevelRange(11, 13);
        EditorUtility.DisplayDialog("Success", "✅ Created levels 11-13!", "OK");
    }

    [MenuItem("WaterSort/Generate Levels/Create Levels 21-30")]
    public static void CreateLevels21To30()
    {
        CreateLevelRange(21, 30);
        EditorUtility.DisplayDialog("Success", "✅ Created levels 21-30!", "OK");
    }

    [MenuItem("WaterSort/Generate Levels/Create Levels 31-50")]
    public static void CreateLevels31To50()
    {
        CreateLevelRange(31, 50);
        EditorUtility.DisplayDialog("Success", "✅ Created levels 31-50!", "OK");
    }

    private static void CreateLevelRange(int startLevel, int endLevel)
    {
        for (int i = startLevel; i <= endLevel; i++)
        {
            CreateLevel(i);
        }
        AssetDatabase.Refresh();
        Debug.Log($"✅ Created levels {startLevel}-{endLevel}");
    }

    private static void CreateLevel(int levelNumber)
    {
        // Create new LevelData
        LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
        levelData.levelNumber = levelNumber;

        // Get configuration từ calculator
        LevelCalculator.GetLevelConfiguration(levelNumber, out int bottles, out int colors);
        levelData.numberOfBottles = bottles;
        levelData.numberOfColors = colors;
        levelData.bottleCapacity = 4; // Always 4

        // Tính toán stats: difficulty, minMoves, maxMoves, timeLimit
        var stats = LevelCalculator.CalculateLevelStats(
            levelNumber,
            levelData.numberOfBottles,
            levelData.numberOfColors,
            numberOfEmptyBottles: 1,
            enableTimeLimitEvery5Levels: true
        );

        levelData.difficulty = stats.difficulty;
        levelData.maxMoves = stats.maxMoves;
        levelData.timeLimit = stats.timeLimitSeconds;

        // Set empty bottles requirement
        levelData.minEmptyBottles = 1 + (levelData.numberOfBottles - 6) / 2;

        // Calculate star thresholds based on minimum moves
        CalculateStarThresholds(levelData, stats.minimumMoves);

        // Save asset
        string assetPath = $"{LEVELS_PATH}Level_{levelNumber:D2}.asset";
        AssetDatabase.CreateAsset(levelData, assetPath);

        string timeStr = levelData.timeLimit > 0 ? $", Time: {levelData.timeLimit}s" : "";
        Debug.Log($"✅ Created: {assetPath}\n" +
                  $"   Difficulty: {stats.difficulty}, Bottles: {bottles}, Colors: {colors}\n" +
                  $"   Min Moves: {stats.minimumMoves}, Max Moves: {levelData.maxMoves}{timeStr}");
    }

    private static void CalculateStarThresholds(LevelData level, int minimumMoves)
    {
        // Sao dựa trên minimum moves
        // 3 sao: min moves (optimal)
        // 2 sao: min moves * 1.5
        // 1 sao: min moves * 2
        
        level.optimalMoves = minimumMoves;
        level.threeStarMoves = minimumMoves;
        level.twoStarMoves = Mathf.RoundToInt(minimumMoves * 1.5f);
    }

    [MenuItem("WaterSort/Generate Levels/Show All Levels Info")]
    public static void ShowAllLevelsInfo()
    {
        string[] levelGUIDs = AssetDatabase.FindAssets("Level_", new[] { "Assets/Levels/" });
        List<LevelData> levels = new List<LevelData>();

        foreach (var guid in levelGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (level != null)
            {
                levels.Add(level);
            }
        }

        // Sort by level number
        levels.Sort((a, b) => a.levelNumber.CompareTo(b.levelNumber));

        string report = "📋 ALL LEVELS INFO:\n\n";
        report += "Level\tDiff\tBottles\tColors\tMoves\tTime\n";
        report += "────────────────────────────────────────\n";

        foreach (var level in levels)
        {
            string movesStr = level.maxMoves > 0 ? level.maxMoves.ToString() : "∞";
            string timeStr = level.timeLimit > 0 ? level.timeLimit + "s" : "-";
            report += $"{level.levelNumber:D2}\t{level.difficulty}\t{level.numberOfBottles}\t{level.numberOfColors}\t{movesStr}\t{timeStr}\n";
        }

        Debug.Log(report);
        EditorUtility.DisplayDialog("Levels Info", report, "OK");
    }
}
