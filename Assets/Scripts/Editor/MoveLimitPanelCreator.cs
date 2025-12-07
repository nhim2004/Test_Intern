using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor tool to automatically create Move Limit Panel in the scene
/// </summary>
public class MoveLimitPanelCreator
{
    [MenuItem("WaterSort/UI/Create Move Limit Panel")]
    public static void CreateMoveLimitPanel()
    {
        // Find or create Canvas
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ No Canvas found in scene!\n\nPlease create a Canvas first.", "OK");
            return;
        }

        // Check if panel already exists
        Transform existingPanel = canvas.transform.Find("MoveLimitPanel");
        if (existingPanel != null)
        {
            EditorUtility.DisplayDialog("Warning", "⚠️ Move Limit Panel already exists in scene!", "OK");
            return;
        }

        // Create main panel container
        GameObject panelContainer = new GameObject("MoveLimitPanel");
        panelContainer.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = panelContainer.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelRect.sizeDelta = new Vector2(300, 150);
        panelRect.anchoredPosition = new Vector2(0, -80);

        // Add background image
        Image bgImage = panelContainer.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        // Add MoveLimitPanel component
        var moveLimitPanelComponent = panelContainer.AddComponent<WaterSort.UI.MoveLimitPanel>();

        // Create Content background
        GameObject contentBg = new GameObject("Background");
        contentBg.transform.SetParent(panelContainer.transform, false);
        
        RectTransform contentBgRect = contentBg.AddComponent<RectTransform>();
        contentBgRect.anchorMin = Vector2.zero;
        contentBgRect.anchorMax = Vector2.one;
        contentBgRect.offsetMin = Vector2.zero;
        contentBgRect.offsetMax = Vector2.zero;
        
        Image contentBgImage = contentBg.AddComponent<Image>();
        contentBgImage.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

        // Create Progress Bar Background
        GameObject progressBarBg = new GameObject("ProgressBarBackground");
        progressBarBg.transform.SetParent(panelContainer.transform, false);
        
        RectTransform progressBgRect = progressBarBg.AddComponent<RectTransform>();
        progressBgRect.anchorMin = new Vector2(0.5f, 0.5f);
        progressBgRect.anchorMax = new Vector2(0.5f, 0.5f);
        progressBgRect.sizeDelta = new Vector2(250, 20);
        progressBgRect.anchoredPosition = new Vector2(0, -40);
        
        Image progressBgImage = progressBarBg.AddComponent<Image>();
        progressBgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        // Create Progress Bar Fill
        GameObject progressBarFill = new GameObject("Fill");
        progressBarFill.transform.SetParent(progressBarBg.transform, false);
        
        RectTransform progressFillRect = progressBarFill.AddComponent<RectTransform>();
        progressFillRect.anchorMin = new Vector2(0, 0.5f);
        progressFillRect.anchorMax = new Vector2(0, 0.5f);
        progressFillRect.sizeDelta = new Vector2(250, 20);
        progressFillRect.anchoredPosition = Vector2.zero;
        
        Image progressFillImage = progressBarFill.AddComponent<Image>();
        progressFillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);

        // Create "Moves:" label
        GameObject movesLabelGO = new GameObject("MovesLabel");
        movesLabelGO.transform.SetParent(panelContainer.transform, false);
        
        RectTransform movesLabelRect = movesLabelGO.AddComponent<RectTransform>();
        movesLabelRect.anchorMin = new Vector2(0.5f, 0.5f);
        movesLabelRect.anchorMax = new Vector2(0.5f, 0.5f);
        movesLabelRect.sizeDelta = new Vector2(200, 50);
        movesLabelRect.anchoredPosition = new Vector2(0, 15);
        
        TextMeshProUGUI movesLabel = movesLabelGO.AddComponent<TextMeshProUGUI>();
        movesLabel.text = "Moves:";
        movesLabel.fontSize = 40;
        movesLabel.alignment = TextAlignmentOptions.Center;
        movesLabel.color = Color.white;

        // Create "Remaining" text
        GameObject remainingGO = new GameObject("MovesRemaining");
        remainingGO.transform.SetParent(panelContainer.transform, false);
        
        RectTransform remainingRect = remainingGO.AddComponent<RectTransform>();
        remainingRect.anchorMin = new Vector2(0.5f, 0.5f);
        remainingRect.anchorMax = new Vector2(0.5f, 0.5f);
        remainingRect.sizeDelta = new Vector2(150, 60);
        remainingRect.anchoredPosition = new Vector2(-40, 5);
        
        TextMeshProUGUI remainingText = remainingGO.AddComponent<TextMeshProUGUI>();
        remainingText.text = "20";
        remainingText.fontSize = 60;
        remainingText.alignment = TextAlignmentOptions.MidlineRight;
        remainingText.color = Color.white;

        // Create "/" separator
        GameObject separatorGO = new GameObject("Separator");
        separatorGO.transform.SetParent(panelContainer.transform, false);
        
        RectTransform separatorRect = separatorGO.AddComponent<RectTransform>();
        separatorRect.anchorMin = new Vector2(0.5f, 0.5f);
        separatorRect.anchorMax = new Vector2(0.5f, 0.5f);
        separatorRect.sizeDelta = new Vector2(30, 60);
        separatorRect.anchoredPosition = new Vector2(20, 5);
        
        TextMeshProUGUI separatorText = separatorGO.AddComponent<TextMeshProUGUI>();
        separatorText.text = "/";
        separatorText.fontSize = 50;
        separatorText.alignment = TextAlignmentOptions.Center;
        separatorText.color = Color.white;

        // Create Max Moves text
        GameObject maxMovesGO = new GameObject("MaxMoves");
        maxMovesGO.transform.SetParent(panelContainer.transform, false);
        
        RectTransform maxMovesRect = maxMovesGO.AddComponent<RectTransform>();
        maxMovesRect.anchorMin = new Vector2(0.5f, 0.5f);
        maxMovesRect.anchorMax = new Vector2(0.5f, 0.5f);
        maxMovesRect.sizeDelta = new Vector2(100, 60);
        maxMovesRect.anchoredPosition = new Vector2(60, 5);
        
        TextMeshProUGUI maxMovesText = maxMovesGO.AddComponent<TextMeshProUGUI>();
        maxMovesText.text = "20";
        maxMovesText.fontSize = 50;
        maxMovesText.alignment = TextAlignmentOptions.MidlineLeft;
        maxMovesText.color = new Color(0.7f, 0.7f, 0.7f, 1f);

        // Assign components to MoveLimitPanel
        var panelRootField = typeof(WaterSort.UI.MoveLimitPanel).GetField("panelRoot", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var movesRemainingField = typeof(WaterSort.UI.MoveLimitPanel).GetField("movesRemainingText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var maxMovesField = typeof(WaterSort.UI.MoveLimitPanel).GetField("maxMovesText", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var barFillField = typeof(WaterSort.UI.MoveLimitPanel).GetField("movesBarFill", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        panelRootField?.SetValue(moveLimitPanelComponent, panelContainer);
        movesRemainingField?.SetValue(moveLimitPanelComponent, remainingText);
        maxMovesField?.SetValue(moveLimitPanelComponent, maxMovesText);
        barFillField?.SetValue(moveLimitPanelComponent, progressFillImage);

        EditorUtility.SetDirty(panelContainer);
        
        Debug.Log("✅ Move Limit Panel created successfully!");
        EditorUtility.DisplayDialog("Success", 
            "✅ Move Limit Panel created!\n\n" +
            "Next steps:\n" +
            "1. Find 'MoveLimitPanel' in hierarchy\n" +
            "2. Assign it to UIManager > Move Limit Panel\n" +
            "3. Set level maxMoves > 0 to enable", "OK");
    }

    [MenuItem("WaterSort/UI/Create Simple Move Counter (No Limit)")]
    public static void CreateSimpleMoveCounter()
    {
        // Find Canvas
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ No Canvas found!", "OK");
            return;
        }

        // Check if counter exists
        Transform existingCounter = canvas.transform.Find("MoveCounter");
        if (existingCounter != null)
        {
            EditorUtility.DisplayDialog("Warning", "⚠️ Move Counter already exists!", "OK");
            return;
        }

        // Create simple counter
        GameObject counterGO = new GameObject("MoveCounter");
        counterGO.transform.SetParent(canvas.transform, false);
        
        RectTransform counterRect = counterGO.AddComponent<RectTransform>();
        counterRect.anchorMin = new Vector2(0, 1);
        counterRect.anchorMax = new Vector2(0, 1);
        counterRect.sizeDelta = new Vector2(200, 80);
        counterRect.anchoredPosition = new Vector2(100, -50);

        // Background
        Image bgImage = counterGO.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);

        // Text
        TextMeshProUGUI moveText = counterGO.AddComponent<TextMeshProUGUI>();
        moveText.text = "Moves: 0";
        moveText.fontSize = 40;
        moveText.alignment = TextAlignmentOptions.Center;
        moveText.color = Color.white;

        Debug.Log("✅ Simple Move Counter created!");
        EditorUtility.DisplayDialog("Success", 
            "✅ Simple Move Counter created!\n\n" +
            "This counter shows total moves\n" +
            "without move limit.", "OK");
    }

    [MenuItem("WaterSort/UI/Setup Level - Add Move Limit")]
    public static void SetupLevelWithMoveLimit()
    {
        var gameManager = Object.FindObjectOfType<WaterSort.GameManager>();
        if (gameManager == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ GameManager not found!", "OK");
            return;
        }

        var currentLevel = gameManager.GetCurrentLevel();
        if (currentLevel == null)
        {
            EditorUtility.DisplayDialog("Error", "❌ No level loaded!", "OK");
            return;
        }

        // Show current level info
        EditorUtility.DisplayDialog("Setup Move Limit", 
            $"Current level: {currentLevel.levelNumber}\n" +
            $"Current move limit: {(currentLevel.maxMoves > 0 ? currentLevel.maxMoves.ToString() : "Unlimited")}\n\n" +
            $"To change: Edit the level asset directly in Inspector.", "OK");
    }
}
