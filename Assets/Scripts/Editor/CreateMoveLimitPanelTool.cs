using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple editor tool to create Move Limit Panel
/// </summary>
public class CreateMoveLimitPanelTool
{
    [MenuItem("WaterSort/UI Tools/Create Move Limit Panel in Scene")]
    public static void CreatePanel()
    {
        // Find Canvas
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ No Canvas found!\n\nCreate a Canvas first.", "OK");
            return;
        }

        // Check existing
        if (canvas.transform.Find("MoveLimitPanel") != null)
        {
            EditorUtility.DisplayDialog("Warning", "⚠️ Panel already exists!", "OK");
            return;
        }

        // Create panel root
        GameObject panel = new GameObject("MoveLimitPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(300, 120);
        rt.anchoredPosition = new Vector2(0, -60);

        // Background
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        // Add component
        var moveLimitComponent = panel.AddComponent<WaterSort.UI.MoveLimitPanel>();

        // Create panel root field reference
        var panelRootField = typeof(WaterSort.UI.MoveLimitPanel).GetField("panelRoot",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        panelRootField?.SetValue(moveLimitComponent, panel);

        // Moves remaining text
        GameObject remainingGO = new GameObject("MovesRemaining");
        remainingGO.transform.SetParent(panel.transform, false);
        RectTransform remainingRT = remainingGO.AddComponent<RectTransform>();
        remainingRT.anchorMin = new Vector2(0.5f, 0.5f);
        remainingRT.anchorMax = new Vector2(0.5f, 0.5f);
        remainingRT.sizeDelta = new Vector2(150, 80);
        remainingRT.anchoredPosition = new Vector2(-20, 10);

        TextMeshProUGUI remainingText = remainingGO.AddComponent<TextMeshProUGUI>();
        remainingText.text = "20";
        remainingText.fontSize = 60;
        remainingText.alignment = TextAlignmentOptions.MidlineRight;
        remainingText.color = Color.white;

        // Max moves text
        GameObject maxGO = new GameObject("MaxMoves");
        maxGO.transform.SetParent(panel.transform, false);
        RectTransform maxRT = maxGO.AddComponent<RectTransform>();
        maxRT.anchorMin = new Vector2(0.5f, 0.5f);
        maxRT.anchorMax = new Vector2(0.5f, 0.5f);
        maxRT.sizeDelta = new Vector2(100, 80);
        maxRT.anchoredPosition = new Vector2(40, 10);

        TextMeshProUGUI maxText = maxGO.AddComponent<TextMeshProUGUI>();
        maxText.text = "20";
        maxText.fontSize = 50;
        maxText.alignment = TextAlignmentOptions.MidlineLeft;
        maxText.color = new Color(0.7f, 0.7f, 0.7f, 1f);

        // Progress bar background
        GameObject progressBgGO = new GameObject("ProgressBarBg");
        progressBgGO.transform.SetParent(panel.transform, false);
        RectTransform progressBgRT = progressBgGO.AddComponent<RectTransform>();
        progressBgRT.anchorMin = new Vector2(0.5f, 0.5f);
        progressBgRT.anchorMax = new Vector2(0.5f, 0.5f);
        progressBgRT.sizeDelta = new Vector2(250, 15);
        progressBgRT.anchoredPosition = new Vector2(0, -30);

        Image progressBgImage = progressBgGO.AddComponent<Image>();
        progressBgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        // Progress bar fill
        GameObject progressFillGO = new GameObject("Fill");
        progressFillGO.transform.SetParent(progressBgGO.transform, false);
        RectTransform progressFillRT = progressFillGO.AddComponent<RectTransform>();
        progressFillRT.anchorMin = new Vector2(0, 0.5f);
        progressFillRT.anchorMax = new Vector2(0, 0.5f);
        progressFillRT.sizeDelta = new Vector2(250, 15);
        progressFillRT.anchoredPosition = Vector2.zero;

        Image progressFillImage = progressFillGO.AddComponent<Image>();
        progressFillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);

        // Assign fields
        var movesRemainingField = typeof(WaterSort.UI.MoveLimitPanel).GetField("movesRemainingText",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var maxMovesField = typeof(WaterSort.UI.MoveLimitPanel).GetField("maxMovesText",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var barFillField = typeof(WaterSort.UI.MoveLimitPanel).GetField("movesBarFill",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        movesRemainingField?.SetValue(moveLimitComponent, remainingText);
        maxMovesField?.SetValue(moveLimitComponent, maxText);
        barFillField?.SetValue(moveLimitComponent, progressFillImage);

        EditorUtility.SetDirty(panel);
        
        Debug.Log("✅ Move Limit Panel created!");
        EditorUtility.DisplayDialog("Success", "✅ Move Limit Panel created!\n\nAssign it in UIManager Inspector.", "OK");
    }

    [MenuItem("WaterSort/UI Tools/Create Simple Move Counter (No Limit)")]
    public static void CreateSimpleCounter()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ No Canvas found!", "OK");
            return;
        }

        if (canvas.transform.Find("MoveCounter") != null)
        {
            EditorUtility.DisplayDialog("Warning", "⚠️ Counter already exists!", "OK");
            return;
        }

        GameObject counter = new GameObject("MoveCounter");
        counter.transform.SetParent(canvas.transform, false);
        
        RectTransform rt = counter.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.sizeDelta = new Vector2(200, 70);
        rt.anchoredPosition = new Vector2(100, -50);

        Image bg = counter.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);

        TextMeshProUGUI text = counter.AddComponent<TextMeshProUGUI>();
        text.text = "Moves: 0";
        text.fontSize = 40;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        Debug.Log("✅ Simple Move Counter created!");
        EditorUtility.DisplayDialog("Success", "✅ Simple Move Counter created!", "OK");
    }
}
