using UnityEngine;
using UnityEditor;
using WaterSort;
using WaterSort.Core;
using WaterSort.UI;

/// <summary>
/// Editor tool to test lose panel by triggering game over
/// </summary>
public class TestLosePanel
{
    [MenuItem("WaterSort/Test/Auto Lose (Time Limit)")]
    public static void TriggerTimeLimitLose()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Not Playing", 
                "❌ This tool only works in Play Mode!\n\n" +
                "Please enter Play Mode and start a level first.", "OK");
            return;
        }
        
        var uiManager = Object.FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager not found!");
            return;
        }
        
        // Show time limit panel
        uiManager.ShowTimeLimitPanel();
        Debug.Log("Time limit lose panel triggered!");
    }
    
    [MenuItem("WaterSort/Test/Auto Lose (Move Limit)")]
    public static void TriggerMoveLimitLose()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Not Playing", 
                "This tool only works in Play Mode!\n\n" +
                "Please enter Play Mode and start a level first.", "OK");
            return;
        }
        
        var uiManager = Object.FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager not found!");
            return;
        }
        
        // Show move limit panel
        uiManager.ShowMoveLimitPanel();
        Debug.Log("🚫 Move limit lose panel triggered!");
    }
    
    [MenuItem("WaterSort/Test/Auto Lose (General)")]
    public static void TriggerGeneralLose()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Not Playing", 
                "❌ This tool only works in Play Mode!\n\n" +
                "Please enter Play Mode and start a level first.", "OK");
            return;
        }
        
        var uiManager = Object.FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("❌ UIManager not found!");
            return;
        }
        
        // Show general lose panel
        uiManager.ShowLoseScreen();
        Debug.Log("💀 General lose panel triggered!");
    }
    
    [MenuItem("WaterSort/Test/Force Time Expire")]
    public static void ForceTimeExpire()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Not Playing", 
                "❌ This tool only works in Play Mode!\n\n" +
                "Please enter Play Mode and start a level first.", "OK");
            return;
        }
        
        var gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("❌ GameManager not found!");
            return;
        }
        
        // Use reflection to set time elapsed to max
        var timeElapsedField = typeof(GameManager).GetField("timeElapsed", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        var currentLevel = gameManager.GetCurrentLevel();
        if (currentLevel != null && currentLevel.timeLimit > 0)
        {
            timeElapsedField?.SetValue(gameManager, currentLevel.timeLimit + 1f);
            Debug.Log($"⏰ Forced time to expire! Time limit was: {currentLevel.timeLimit}s");
        }
        else
        {
            Debug.LogWarning("⚠️ Current level has no time limit!");
        }
    }
}
