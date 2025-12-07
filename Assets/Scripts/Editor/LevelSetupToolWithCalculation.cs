using UnityEngine;
using UnityEditor;
using WaterSort.Game;
using WaterSort.Core;
using System.Collections.Generic;

/// <summary>
/// Tool setup levels với calculated minimum/maximum moves
/// </summary>
public class LevelSetupToolWithCalculation
{
    [MenuItem("WaterSort/Setup Levels/Calculate & Setup Level (Single)")]
    public static void CalculateAndSetupSingleLevel()
    {
        var level = Selection.activeObject as LevelData;
        if (level == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ Select a LevelData asset!", "OK");
            return;
        }

        int levelNumber = ExtractLevelNumber(level.name);
        if (levelNumber <= 0)
        {
            EditorUtility.DisplayDialog("Error", "❌ Invalid level name format!\nUse 'Level_1', 'Level_11', etc.", "OK");
            return;
        }

        var result = LevelCalculator.CalculateLevelStats(
            levelNumber,
            level.numberOfBottles,
            level.numberOfColors,
            numberOfEmptyBottles: 1,
            enableTimeLimitEvery5Levels: true
        );

        level.difficulty = result.difficulty;
        level.maxMoves = result.maxMoves;
        level.timeLimit = result.timeLimitSeconds;

        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("✅ Success",
            result.ToString() + "\n\n" +
            $"Updated:\n" +
            $"• Difficulty: {result.difficulty}\n" +
            $"• Max Moves: {result.maxMoves}\n" +
            $"• Time Limit: {(result.timeLimitSeconds > 0 ? result.timeLimitSeconds + "s" : "None")}", 
            "OK");
    }

    [MenuItem("WaterSort/Setup Levels/Calculate & Setup Level Range (11-50)")]
    public static void CalculateAndSetupLevelRange()
    {
        string[] folders = AssetDatabase.FindAssets("LevelData", new[] { "Assets/Levels" });
        if (folders.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "❌ No LevelData assets found in Assets/Levels!", "OK");
            return;
        }

        List<LevelData> levels = new List<LevelData>();
        foreach (var guid in folders)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var level = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (level != null)
            {
                int levelNum = ExtractLevelNumber(level.name);
                if (levelNum >= 11 && levelNum <= 50)
                {
                    levels.Add(level);
                }
            }
        }

        if (levels.Count == 0)
        {
            EditorUtility.DisplayDialog("Info", "⚠️ No levels 11-50 found!", "OK");
            return;
        }

        int updated = 0;
        foreach (var level in levels)
        {
            int levelNumber = ExtractLevelNumber(level.name);
            
            var result = LevelCalculator.CalculateLevelStats(
                levelNumber,
                level.numberOfBottles,
                level.numberOfColors,
                numberOfEmptyBottles: 1,
                enableTimeLimitEvery5Levels: true
            );

            level.difficulty = result.difficulty;
            level.maxMoves = result.maxMoves;
            level.timeLimit = result.timeLimitSeconds;
            EditorUtility.SetDirty(level);

            updated++;
            Debug.Log($"✅ Level {levelNumber}: Min={result.minimumMoves}, Max={result.maxMoves}, Difficulty={result.difficulty}");
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("✅ Success",
            $"Configured {updated} levels (11-50)!\n\n" +
            $"Each level now has:\n" +
            $"• Calculated difficulty (1-5)\n" +
            $"• Max moves based on min moves\n" +
            $"• Time limit on levels 5, 10, 15... (optional)", "OK");
    }

    [MenuItem("WaterSort/Setup Levels/Show Level Calculation Stats")]
    public static void ShowLevelStats()
    {
        var level = Selection.activeObject as LevelData;
        if (level == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ Select a LevelData asset!", "OK");
            return;
        }

        int levelNumber = ExtractLevelNumber(level.name);
        if (levelNumber <= 0)
        {
            EditorUtility.DisplayDialog("Error", "❌ Invalid level name!", "OK");
            return;
        }

        var result = LevelCalculator.CalculateLevelStats(
            levelNumber,
            level.numberOfBottles,
            level.numberOfColors,
            numberOfEmptyBottles: 1,
            enableTimeLimitEvery5Levels: true
        );

        Debug.Log("======== LEVEL CALCULATION STATS ========\n" + result.ToString());

        EditorUtility.DisplayDialog("📊 Level Stats", result.ToString(), "OK");
    }

    [MenuItem("WaterSort/Setup Levels/Calculate Difficulty Table")]
    public static void ShowDifficultyTable()
    {
        string table = "LEVEL DIFFICULTY TABLE\n" +
                       "======================\n\n";

        for (int level = 1; level <= 50; level += 5)
        {
            int diff = LevelCalculator.CalculateDifficultyFromLevel(level);
            LevelCalculator.GetLevelConfiguration(level, out int bottles, out int colors);
            
            string diffName = diff switch
            {
                1 => "Easy",
                2 => "Normal",
                3 => "Hard",
                4 => "Expert",
                5 => "Master",
                _ => "?"
            };

            table += $"Level {level:D2}: {diffName,-8} | Bottles: {bottles}, Colors: {colors}\n";
        }

        Debug.Log(table);
        EditorUtility.DisplayDialog("📊 Difficulty Table", table, "OK");
    }

    // Helper: Extract level number from asset name "Level_1", "Level_11", etc.
    private static int ExtractLevelNumber(string levelName)
    {
        if (!levelName.StartsWith("Level_")) return -1;
        
        string numStr = levelName.Substring(6);
        if (int.TryParse(numStr, out int num)) return num;
        
        return -1;
    }
}
