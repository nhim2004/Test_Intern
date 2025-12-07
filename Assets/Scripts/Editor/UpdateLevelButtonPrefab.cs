using UnityEngine;
using UnityEditor;

/// <summary>
/// Tool to update existing level button prefab or recreate it
/// </summary>
public class UpdateLevelButtonPrefab
{
    [MenuItem("WaterSort/Update Level Button Prefab")]
    public static void UpdatePrefab()
    {
        string prefabPath = "Assets/Prefabs/LevelButton.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Prefab Not Found", 
                "LevelButton.prefab not found. Please create it first using:\nWaterSort → Create Level Button Prefab", "OK");
            return;
        }
        
        // Load prefab
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
        
        try
        {
            // Update text component settings
            TMPro.TextMeshProUGUI tmp = prefabInstance.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.fontSize = 24;
                tmp.enableWordWrapping = true;
                tmp.overflowMode = TMPro.TextOverflowModes.Overflow;
                tmp.alignment = TMPro.TextAlignmentOptions.Center;
                
                // Update RectTransform with padding
                RectTransform textRect = tmp.GetComponent<RectTransform>();
                if (textRect != null)
                {
                    textRect.anchorMin = Vector2.zero;
                    textRect.anchorMax = Vector2.one;
                    textRect.sizeDelta = new Vector2(-20, -20); // Add padding
                    textRect.anchoredPosition = Vector2.zero;
                }
                
                Debug.Log("✅ LevelButton prefab text updated successfully!");
            }
            else
            {
                Debug.LogWarning("⚠️ TextMeshProUGUI component not found in prefab!");
            }
            
            // Save changes
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            
            EditorUtility.DisplayDialog("Success", 
                "Level Button Prefab updated successfully!\n\nThe button now supports multi-line text with:\n" +
                "- Level number\n" +
                "- Difficulty\n" +
                "- Star requirements\n" +
                "- Time limit (if any)", "OK");
        }
        finally
        {
            // Clean up
            PrefabUtility.UnloadPrefabContents(prefabInstance);
        }
        
        // Refresh
        AssetDatabase.Refresh();
        
        // Select the prefab
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    }
}
