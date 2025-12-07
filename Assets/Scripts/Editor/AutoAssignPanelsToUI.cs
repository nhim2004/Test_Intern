using UnityEngine;
using UnityEditor;

/// <summary>
/// Auto-assign panels to UIManager
/// </summary>
public class AutoAssignPanelsToUI
{
    [MenuItem("WaterSort/UI Tools/Auto-Assign Panels to UIManager")]
    public static void AutoAssignPanels()
    {
        var uiManager = Object.FindObjectOfType<WaterSort.UI.UIManager>();
        if (uiManager == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ UIManager not found in scene!", "OK");
            return;
        }

        var canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ Canvas not found!", "OK");
            return;
        }

        int assigned = 0;

        // Find and assign MoveLimitPanel
        Transform moveLimitPanelTransform = canvas.transform.Find("MoveLimitPanel");
        if (moveLimitPanelTransform != null)
        {
            var moveLimitPanelComponent = moveLimitPanelTransform.GetComponent<WaterSort.UI.MoveLimitPanel>();
            if (moveLimitPanelComponent != null)
            {
                var moveLimitPanelField = typeof(WaterSort.UI.UIManager).GetField("moveLimitPanel",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                moveLimitPanelField?.SetValue(uiManager, moveLimitPanelComponent);
                assigned++;
                Debug.Log("✅ Assigned MoveLimitPanel");
            }
        }

        // Find and assign MoveCounter text
        Transform moveCounterTransform = canvas.transform.Find("MoveCounter");
        if (moveCounterTransform != null)
        {
            var moveCounterText = moveCounterTransform.GetComponent<TMPro.TextMeshProUGUI>();
            if (moveCounterText != null)
            {
                var moveCountTextField = typeof(WaterSort.UI.UIManager).GetField("moveCountText",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                moveCountTextField?.SetValue(uiManager, moveCounterText);
                assigned++;
                Debug.Log("✅ Assigned MoveCounter");
            }
        }

        EditorUtility.SetDirty(uiManager);

        if (assigned > 0)
        {
            EditorUtility.DisplayDialog("Success",
                $"✅ Assigned {assigned} panel(s) to UIManager!\n\n" +
                $"Now set level maxMoves > 0 to enable move limit.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Info", "⚠️ No panels found to assign.\n\nCreate panels first using UI Tools menu.", "OK");
        }
    }
}
