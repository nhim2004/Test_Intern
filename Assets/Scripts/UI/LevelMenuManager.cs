using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WaterSort;
using WaterSort.Core;
using WaterSort.UI;

public class LevelMenuManager : MonoBehaviour
{
    public static LevelMenuManager Instance { get; private set; }
    
    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject gameUI;
    [HideInInspector] public LevelRequirementsPanel requirementsPanel;
    [HideInInspector] public LockedLevelPopup lockedLevelPopup;
    
    [Header("Level Selection")]
    public Transform levelButtonContainer;
    public GameObject levelButtonPrefab;
    
    [Header("Levels")]
    public LevelData[] allLevels;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Auto-find requirements panel if not assigned
        if (requirementsPanel == null)
        {
            requirementsPanel = FindObjectOfType<LevelRequirementsPanel>(true);
            if (requirementsPanel != null)
            {
                Debug.Log("✅ LevelRequirementsPanel auto-found");
            }
        }
        
        // Auto-find locked level popup
        if (lockedLevelPopup == null)
        {
            lockedLevelPopup = FindObjectOfType<LockedLevelPopup>(true);
            if (lockedLevelPopup != null)
            {
                Debug.Log("✅ LockedLevelPopup auto-found");
            }
        }
    }
    
    private void Start()
    {
        // Ensure panels are hidden initially
        if (requirementsPanel != null)
        {
            requirementsPanel.Hide();
        }
        
        if (lockedLevelPopup != null)
        {
            lockedLevelPopup.Hide();
        }
        
        ShowMenu();
        GenerateLevelButtons();
    }
    
    public void ShowMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        
        // Hide requirements panel and popups
        if (requirementsPanel != null)
        {
            requirementsPanel.Hide();
        }
        
        if (lockedLevelPopup != null)
        {
            lockedLevelPopup.Hide();
        }
        
        Time.timeScale = 0;
    }
    
    public void HideMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        
        Time.timeScale = 1;
    }
    
    void GenerateLevelButtons()
    {
        if (levelButtonContainer == null || levelButtonPrefab == null)
        {
            Debug.LogWarning("Level button container or prefab not assigned!");
            return;
        }
        
        // Clear existing
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        if (allLevels == null || allLevels.Length == 0)
        {
            Debug.LogWarning("No levels assigned!");
            return;
        }
        
        // Create buttons
        for (int i = 0; i < allLevels.Length; i++)
        {
            int levelIndex = i;
            LevelData level = allLevels[i];
            GameObject btnObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            
            // Check if level is unlocked
            bool isUnlocked = IsLevelUnlocked(levelIndex);
            bool completed = PlayerPrefs.GetInt($"Level_{levelIndex}_Completed", 0) == 1;
            int stars = PlayerPrefs.GetInt($"Level_{levelIndex}_Stars", 0);
            
            // Set level number and requirements
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                if (isUnlocked)
                {
                    string requirements = GetLevelRequirements(level);
                    btnText.text = $"<size=120%><b>{i + 1}</b></size>\n{requirements}";
                }
                else
                {
                    // Locked level display
                    btnText.text = $"<size=120%><b>{i + 1}</b></size>\n<size=80%>[LOCKED]</size>";
                }
                btnText.alignment = TextAlignmentOptions.Center;
            }
            
            // Set click
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                if (isUnlocked)
                {
                    btn.onClick.AddListener(() => OnLevelSelected(levelIndex));
                    btn.interactable = true;
                }
                else
                {
                    btn.onClick.AddListener(() => OnLockedLevelClicked(levelIndex));
                    btn.interactable = true; // Still clickable to show locked message
                }
            }
            
            // Color based on completion, stars, or locked status
            Image btnImg = btnObj.GetComponent<Image>();
            if (btnImg != null)
            {
                if (!isUnlocked)
                {
                    // Locked: dark gray
                    btnImg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
                }
                else if (completed)
                {
                    // Color based on stars earned
                    if (stars == 3)
                        btnImg.color = new Color(1f, 0.84f, 0f); // Gold
                    else if (stars == 2)
                        btnImg.color = new Color(0.75f, 0.75f, 0.75f); // Silver
                    else
                        btnImg.color = new Color(0.8f, 0.5f, 0.3f); // Bronze
                }
                else
                {
                    btnImg.color = Color.white; // Not completed but unlocked
                }
            }
            
            // Add lock icon overlay if locked
            if (!isUnlocked)
            {
                AddLockIcon(btnObj);
            }
        }
    }
    
    /// <summary>
    /// Check if a level is unlocked
    /// </summary>
    bool IsLevelUnlocked(int levelIndex)
    {
        // Level 0 is always unlocked
        if (levelIndex == 0) return true;
        
        // Check if previous level is completed
        int previousLevel = levelIndex - 1;
        bool previousCompleted = PlayerPrefs.GetInt($"Level_{previousLevel}_Completed", 0) == 1;
        
        return previousCompleted;
    }
    
    /// <summary>
    /// Add lock icon to button
    /// </summary>
    void AddLockIcon(GameObject btnObj)
    {
        // Create lock icon object
        GameObject lockObj = new GameObject("LockIcon");
        lockObj.transform.SetParent(btnObj.transform, false);
        
        RectTransform lockRect = lockObj.AddComponent<RectTransform>();
        lockRect.anchorMin = new Vector2(0.5f, 0.5f);
        lockRect.anchorMax = new Vector2(0.5f, 0.5f);
        lockRect.sizeDelta = new Vector2(60, 60);
        lockRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI lockText = lockObj.AddComponent<TextMeshProUGUI>();
        lockText.text = "X";
        lockText.fontSize = 60;
        lockText.alignment = TextAlignmentOptions.Center;
        lockText.color = new Color(1f, 1f, 1f, 0.8f);
    }
    
    /// <summary>
    /// Handle locked level click
    /// </summary>
    void OnLockedLevelClicked(int levelIndex)
    {
        Debug.Log($"Level {levelIndex + 1} is locked!");
        AudioManager.Instance?.PlaySound("error");
        
        // Show locked message
        if (levelIndex > 0)
        {
            ShowLockedMessage(levelIndex);
        }
    }
    
    /// <summary>
    /// Show locked level message
    /// </summary>
    void ShowLockedMessage(int levelIndex)
    {
        int requiredLevel = levelIndex;
        string message = $"<b>Level {levelIndex + 1} is Locked!</b>\n\nComplete <color=#FFD700>Level {requiredLevel}</color> to unlock this level.";
        
        Debug.Log($"⚠️ Complete Level {requiredLevel} to unlock Level {levelIndex + 1}!");
        
        // Show popup if available
        if (lockedLevelPopup != null)
        {
            lockedLevelPopup.Show(message);
        }
    }
    
    /// <summary>
    /// Get formatted level requirements text
    /// </summary>
    string GetLevelRequirements(LevelData level)
    {
        if (level == null) return "";
        
        string text = "";
        
        // Difficulty
        string difficultyName = GetDifficultyName(level.difficulty);
        text += $"<size=70%>{difficultyName}</size>\n";
        
        // Star requirements
        int threshold3 = level.GetThreeStarThreshold();
        int threshold2 = level.GetTwoStarThreshold();
        text += $"<size=60%>3* <={threshold3} | 2* <={threshold2}</size>";
        
        // Time limit if exists
        if (level.timeLimit > 0)
        {
            int minutes = Mathf.FloorToInt(level.timeLimit / 60);
            int seconds = Mathf.FloorToInt(level.timeLimit % 60);
            text += $"\n<size=60%>⏱{minutes}:{seconds:00}</size>";
        }
        
        return text;
    }
    
    /// <summary>
    /// Get difficulty display name
    /// </summary>
    string GetDifficultyName(int difficulty)
    {
        switch (difficulty)
        {
            case 1: return "Easy";
            case 2: return "Medium";
            case 3: return "Hard";
            case 4: return "Expert";
            case 5: return "Master";
            default: return "Normal";
        }
    }
    
    void OnLevelSelected(int levelIndex)
    {
        Debug.Log($"Level {levelIndex + 1} selected");
        
        if (levelIndex < 0 || levelIndex >= allLevels.Length)
        {
            Debug.LogError("Invalid level index!");
            return;
        }
        
        // Double-check if level is unlocked
        if (!IsLevelUnlocked(levelIndex))
        {
            Debug.LogWarning($"Level {levelIndex + 1} is locked!");
            OnLockedLevelClicked(levelIndex);
            return;
        }
        
        // Hide menu
        if (menuPanel != null) menuPanel.SetActive(false);
        
        // Show requirements panel
        if (requirementsPanel != null)
        {
            requirementsPanel.ShowRequirements(allLevels[levelIndex], levelIndex);
        }
        else
        {
            // Fallback: start immediately if no requirements panel
            Debug.LogWarning("No requirements panel found, starting level directly");
            HideMenu();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadSpecificLevel(allLevels[levelIndex], levelIndex);
            }
        }
    }
    
    public void OnQuitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
