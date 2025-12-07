using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using WaterSort.Core;

namespace WaterSort.UI
{
    /// <summary>
    /// Level select scene manager
    /// </summary>
    public class LevelSelectManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform levelButtonContainer;
        [SerializeField] private GameObject levelButtonPrefab;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI titleText;
        
        [Header("Level Configuration")]
        [SerializeField] private LevelData[] allLevels;
        [SerializeField] private Sprite lockedLevelIcon;
        [SerializeField] private Sprite unlockedLevelIcon;
        [SerializeField] private Sprite completedLevelIcon;
        
        [Header("Level Info Panel")]
        [SerializeField] private GameObject levelInfoPanel;
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private TextMeshProUGUI levelDifficultyText;
        [SerializeField] private TextMeshProUGUI levelDescriptionText;
        [SerializeField] private Button playLevelButton;
        [SerializeField] private Button closeLevelInfoButton;
        
        private int selectedLevelIndex = -1;
        
        private void Start()
        {
            SetupButtons();
            GenerateLevelButtons();
            
            if (levelInfoPanel != null)
                levelInfoPanel.SetActive(false);
        }
        
        private void SetupButtons()
        {
            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);
                
            if (playLevelButton != null)
                playLevelButton.onClick.AddListener(OnPlayLevelClicked);
                
            if (closeLevelInfoButton != null)
                closeLevelInfoButton.onClick.AddListener(OnCloseLevelInfo);
        }
        
        private void GenerateLevelButtons()
        {
            if (levelButtonContainer == null || levelButtonPrefab == null)
            {
                Debug.LogWarning("Level button container or prefab not assigned!");
                return;
            }
            
            // Clear existing buttons
            foreach (Transform child in levelButtonContainer)
            {
                Destroy(child.gameObject);
            }
            
            if (allLevels == null || allLevels.Length == 0)
            {
                Debug.LogWarning("No levels assigned!");
                return;
            }
            
            // Create level buttons
            for (int i = 0; i < allLevels.Length; i++)
            {
                int levelIndex = i;
                GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
                
                // Setup button
                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
                }
                
                // Setup button visuals
                SetupLevelButton(buttonObj, levelIndex);
            }
        }
        
        private void SetupLevelButton(GameObject buttonObj, int levelIndex)
        {
            LevelData level = allLevels[levelIndex];
            
            // Get button components
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            Image buttonImage = buttonObj.GetComponent<Image>();
            
            // Set level number
            if (buttonText != null)
            {
                buttonText.text = $"{levelIndex + 1}";
            }
            
            // Check if level is unlocked
            bool isUnlocked = IsLevelUnlocked(levelIndex);
            bool isCompleted = IsLevelCompleted(levelIndex);
            
            // Set button state
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = isUnlocked;
            }
            
            // Set button color based on state
            if (buttonImage != null)
            {
                if (isCompleted)
                {
                    buttonImage.color = new Color(0.2f, 0.8f, 0.3f, 1f); // Green for completed
                }
                else if (isUnlocked)
                {
                    buttonImage.color = new Color(0.3f, 0.6f, 0.9f, 1f); // Blue for unlocked
                }
                else
                {
                    buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gray for locked
                }
            }
            
            // Add stars for completed levels
            if (isCompleted)
            {
                AddStarsToButton(buttonObj, GetLevelStars(levelIndex));
            }
            
            // Add lock icon for locked levels
            if (!isUnlocked && buttonText != null)
            {
                buttonText.text = "🔒";
            }
        }
        
        private void AddStarsToButton(GameObject buttonObj, int stars)
        {
            GameObject starsContainer = new GameObject("Stars");
            starsContainer.transform.SetParent(buttonObj.transform, false);
            
            RectTransform starsRect = starsContainer.AddComponent<RectTransform>();
            starsRect.anchorMin = new Vector2(0.5f, 0);
            starsRect.anchorMax = new Vector2(0.5f, 0);
            starsRect.anchoredPosition = new Vector2(0, 20);
            starsRect.sizeDelta = new Vector2(150, 30);
            
            HorizontalLayoutGroup layout = starsContainer.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            
            for (int i = 0; i < 3; i++)
            {
                GameObject starObj = new GameObject($"Star{i + 1}");
                starObj.transform.SetParent(starsContainer.transform, false);
                
                TextMeshProUGUI starText = starObj.AddComponent<TextMeshProUGUI>();
                starText.text = i < stars ? "⭐" : "☆";
                starText.fontSize = 20;
                starText.alignment = TextAlignmentOptions.Center;
                
                RectTransform starRect = starObj.GetComponent<RectTransform>();
                starRect.sizeDelta = new Vector2(30, 30);
            }
        }
        
        private void OnLevelButtonClicked(int levelIndex)
        {
            AudioManager.Instance?.PlaySound("button");
            
            if (!IsLevelUnlocked(levelIndex))
            {
                AudioManager.Instance?.PlaySound("error");
                Debug.Log($"Level {levelIndex + 1} is locked!");
                return;
            }
            
            selectedLevelIndex = levelIndex;
            ShowLevelInfo(levelIndex);
        }
        
        private void ShowLevelInfo(int levelIndex)
        {
            if (levelInfoPanel == null) return;
            
            LevelData level = allLevels[levelIndex];
            
            if (levelNameText != null)
                levelNameText.text = $"LEVEL {levelIndex + 1}";
                
            if (levelDifficultyText != null)
            {
                string difficulty = GetLevelDifficulty(level);
                levelDifficultyText.text = $"Difficulty: {difficulty}";
                
                // Set color based on difficulty
                Color difficultyColor = difficulty switch
                {
                    "Easy" => new Color(0.2f, 0.8f, 0.3f),
                    "Medium" => new Color(0.9f, 0.7f, 0.2f),
                    "Hard" => new Color(0.9f, 0.3f, 0.2f),
                    _ => Color.white
                };
                levelDifficultyText.color = difficultyColor;
            }
            
            if (levelDescriptionText != null)
            {
                int bottles = level.numberOfBottles;
                int colors = level.numberOfColors;
                levelDescriptionText.text = $"Bottles: {bottles}\nColors: {colors}\n\nSort all colors into separate bottles!";
            }
            
            levelInfoPanel.SetActive(true);
        }
        
        private void OnCloseLevelInfo()
        {
            AudioManager.Instance?.PlaySound("button");
            
            if (levelInfoPanel != null)
                levelInfoPanel.SetActive(false);
                
            selectedLevelIndex = -1;
        }
        
        private void OnPlayLevelClicked()
        {
            if (selectedLevelIndex < 0) return;
            
            AudioManager.Instance?.PlaySound("button");
            
            // Store selected level
            PlayerPrefs.SetInt("SelectedLevel", selectedLevelIndex + 1);
            PlayerPrefs.Save();
            
            // Load game scene
            SceneManager.LoadScene("GameScene");
        }
        
        private void OnBackClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            
            // Load main menu
            SceneManager.LoadScene("MainMenu");
        }
        
        private bool IsLevelUnlocked(int levelIndex)
        {
            // First level is always unlocked
            if (levelIndex == 0) return true;
            
            // Check if previous level is completed
            int previousLevel = levelIndex - 1;
            return PlayerPrefs.GetInt($"Level_{previousLevel}_Completed", 0) == 1;
        }
        
        private bool IsLevelCompleted(int levelIndex)
        {
            return PlayerPrefs.GetInt($"Level_{levelIndex}_Completed", 0) == 1;
        }
        
        private int GetLevelStars(int levelIndex)
        {
            return PlayerPrefs.GetInt($"Level_{levelIndex}_Stars", 0);
        }
        
        private string GetLevelDifficulty(LevelData level)
        {
            if (level == null) return "Unknown";
            
            int bottles = level.numberOfBottles;
            int colors = level.numberOfColors;
            
            // Simple difficulty calculation
            if (colors <= 4 && bottles <= 6)
                return "Easy";
            else if (colors <= 6 && bottles <= 10)
                return "Medium";
            else
                return "Hard";
        }
    }
}
