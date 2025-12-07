using UnityEngine;
using UnityEditor;

/// <summary>
/// Tool for level unlocking management
/// </summary>
public class LevelUnlockTools
{
    [MenuItem("WaterSort/Debug/Unlock All Levels")]
    public static void UnlockAllLevels()
    {
        // Find all level data assets
        string[] guids = AssetDatabase.FindAssets("t:LevelData");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("No Levels Found", 
                "No LevelData assets found in project.", "OK");
            return;
        }
        
        int count = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            WaterSort.Core.LevelData level = AssetDatabase.LoadAssetAtPath<WaterSort.Core.LevelData>(path);
            
            if (level != null)
            {
                int levelIndex = level.levelNumber - 1;
                PlayerPrefs.SetInt($"Level_{levelIndex}_Completed", 1);
                PlayerPrefs.SetInt($"Level_{levelIndex}_Stars", 3);
                count++;
            }
        }
        
        PlayerPrefs.Save();
        
        Debug.Log($"✅ Unlocked all {count} levels with 3 stars!");
        EditorUtility.DisplayDialog("Success", 
            $"Unlocked all {count} levels!\n\nAll levels now have 3 stars.", "OK");
    }
    
    [MenuItem("WaterSort/Debug/Lock All Levels")]
    public static void LockAllLevels()
    {
        // Find all level data assets
        string[] guids = AssetDatabase.FindAssets("t:LevelData");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("No Levels Found", 
                "No LevelData assets found in project.", "OK");
            return;
        }
        
        int count = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            WaterSort.Core.LevelData level = AssetDatabase.LoadAssetAtPath<WaterSort.Core.LevelData>(path);
            
            if (level != null)
            {
                int levelIndex = level.levelNumber - 1;
                PlayerPrefs.DeleteKey($"Level_{levelIndex}_Completed");
                PlayerPrefs.DeleteKey($"Level_{levelIndex}_Stars");
                PlayerPrefs.DeleteKey($"Level_{levelIndex}_BestMoves");
                count++;
            }
        }
        
        PlayerPrefs.Save();
        
        Debug.Log($"🔒 Locked all {count} levels!");
        EditorUtility.DisplayDialog("Success", 
            $"Locked all {count} levels!\n\nOnly Level 1 is now unlocked.", "OK");
    }
    
    [MenuItem("WaterSort/Debug/Reset All Progress")]
    public static void ResetAllProgress()
    {
        bool confirm = EditorUtility.DisplayDialog("Reset All Progress?", 
            "This will delete ALL player progress including:\n" +
            "- Level completion\n" +
            "- Stars earned\n" +
            "- Best moves\n\n" +
            "This cannot be undone!", "Reset", "Cancel");
        
        if (!confirm) return;
        
        // Find all level data assets
        string[] guids = AssetDatabase.FindAssets("t:LevelData");
        
        int count = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            WaterSort.Core.LevelData level = AssetDatabase.LoadAssetAtPath<WaterSort.Core.LevelData>(path);
            
            if (level != null)
            {
                int levelIndex = level.levelNumber - 1;
                PlayerPrefs.DeleteKey($"Level_{levelIndex}_Completed");
                PlayerPrefs.DeleteKey($"Level_{levelIndex}_Stars");
                PlayerPrefs.DeleteKey($"Level_{levelIndex}_BestMoves");
                count++;
            }
        }
        
        PlayerPrefs.Save();
        
        Debug.Log($"♻️ Reset all progress for {count} levels!");
        EditorUtility.DisplayDialog("Progress Reset", 
            $"All progress has been reset!\n\nDeleted data for {count} levels.", "OK");
    }
    
    [MenuItem("WaterSort/Debug/Show Current Progress")]
    public static void ShowCurrentProgress()
    {
        // Find all level data assets
        string[] guids = AssetDatabase.FindAssets("t:LevelData");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("No Levels Found", 
                "No LevelData assets found in project.", "OK");
            return;
        }
        
        string report = "CURRENT LEVEL PROGRESS:\\n\\n";
        int unlockedCount = 0;
        int completedCount = 0;
        int totalStars = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            WaterSort.Core.LevelData level = AssetDatabase.LoadAssetAtPath<WaterSort.Core.LevelData>(path);
            
            if (level != null)
            {
                int levelIndex = level.levelNumber - 1;
                bool completed = PlayerPrefs.GetInt($"Level_{levelIndex}_Completed", 0) == 1;
                int stars = PlayerPrefs.GetInt($"Level_{levelIndex}_Stars", 0);
                int bestMoves = PlayerPrefs.GetInt($"Level_{levelIndex}_BestMoves", 999);
                
                bool unlocked = (levelIndex == 0) || PlayerPrefs.GetInt($"Level_{levelIndex - 1}_Completed", 0) == 1;
                
                string status = unlocked ? (completed ? "✅" : "🔓") : "🔒";
                string starStr = stars > 0 ? new string('⭐', stars) : "-";
                string movesStr = bestMoves < 999 ? bestMoves.ToString() : "-";
                
                report += $"{status} Level {level.levelNumber}: {starStr} (Best: {movesStr})\\n";
                
                if (unlocked) unlockedCount++;
                if (completed) completedCount++;
                totalStars += stars;
            }
        }
        
        report += $"\\nTotal: {completedCount}/{guids.Length} completed, {unlockedCount}/{guids.Length} unlocked\\n";
        report += $"Total Stars: {totalStars}/{guids.Length * 3}";
        
        Debug.Log(report.Replace("\\n", "\n"));
        EditorUtility.DisplayDialog("Level Progress", report, "OK");
    }
}
