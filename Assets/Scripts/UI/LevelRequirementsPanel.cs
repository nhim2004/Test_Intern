using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WaterSort.Core;

namespace WaterSort.UI
{
    /// <summary>
    /// Panel showing level requirements before starting
    /// </summary>
    public class LevelRequirementsPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI levelNumberText;
        [SerializeField] private TextMeshProUGUI difficultyText;
        [SerializeField] private TextMeshProUGUI requirementsText;
        [SerializeField] private TextMeshProUGUI optimalMovesText;
        [SerializeField] private TextMeshProUGUI starRequirementsText;
        [SerializeField] private TextMeshProUGUI timeLimitText;
        [SerializeField] private Button startButton;
        [SerializeField] private Button backButton;
        
        private LevelData currentLevel;
        private int levelIndex;
        
        private void Start()
        {
            if (startButton != null)
                startButton.onClick.AddListener(OnStartClicked);
            
            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);
            
            Hide();
        }
        
        /// <summary>
        /// Show requirements for a level
        /// </summary>
        public void ShowRequirements(LevelData level, int index)
        {
            currentLevel = level;
            levelIndex = index;
            
            if (level == null)
            {
                Debug.LogError("Level data is null!");
                return;
            }
            
            // Level number
            if (levelNumberText != null)
            {
                levelNumberText.text = $"LEVEL {index + 1}";
            }
            
            // Difficulty
            if (difficultyText != null)
            {
                string diffName = GetDifficultyName(level.difficulty);
                Color diffColor = GetDifficultyColor(level.difficulty);
                difficultyText.text = $"Difficulty: <color=#{ColorUtility.ToHtmlStringRGB(diffColor)}><b>{diffName}</b></color>";
            }
            
            // Basic requirements
            if (requirementsText != null)
            {
                string req = $"• Colors: <b>{level.numberOfColors}</b>\n";
                req += $"• Bottles: <b>{level.numberOfBottles}</b>\n";
                req += $"• Bottle Capacity: <b>{level.bottleCapacity}</b>\n";
                req += $"• Empty Bottles: <b>{level.minEmptyBottles}+</b>";
                requirementsText.text = req;
            }
            
            // Optimal moves
            if (optimalMovesText != null)
            {
                int currentBest = PlayerPrefs.GetInt($"Level_{levelIndex}_BestMoves", 999);
                
                if (currentBest < 999)
                {
                    optimalMovesText.text = $"<size=80%>Your Best:</size>\n<size=140%><color=#FFD700><b>{currentBest}</b></color></size> moves";
                }
                else
                {
                    optimalMovesText.gameObject.SetActive(false);
                }
            }
            
            // Star requirements
            if (starRequirementsText != null)
            {
                int threshold3 = level.GetThreeStarThreshold();
                int threshold2 = level.GetTwoStarThreshold();
                
                string starText = "<b>Star Requirements:</b>\n\n";
                starText += $"<color=#FFD700>***</color> Excellent: ≤ <b>{threshold3}</b> moves\n";
                starText += $"<color=#C0C0C0>**</color> Good: ≤ <b>{threshold2}</b> moves\n";
                starText += $"<color=#CD7F32>*</color> Complete: Any moves";
                
                starRequirementsText.text = starText;
            }
            
            // Time limit
            if (timeLimitText != null)
            {
                if (level.timeLimit > 0)
                {
                    int minutes = Mathf.FloorToInt(level.timeLimit / 60);
                    int seconds = Mathf.FloorToInt(level.timeLimit % 60);
                    timeLimitText.text = $"Time Limit: <color=#FF4444><b>{minutes}:{seconds:00}</b></color>";
                    timeLimitText.gameObject.SetActive(true);
                }
                else
                {
                    timeLimitText.gameObject.SetActive(false);
                }
            }
            
            Show();
        }
        
        /// <summary>
        /// Show panel
        /// </summary>
        public void Show()
        {
            if (panel != null)
            {
                panel.SetActive(true);
                Time.timeScale = 0; // Pause
                
                // Hide gameplay UI (timer and pause button)
                var uiManager = FindObjectOfType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.HideTimer();
                    uiManager.HidePauseButton();
                }
            }
        }
        
        /// <summary>
        /// Hide panel
        /// </summary>
        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
        
        private void OnStartClicked()
        {
            Hide();
            Time.timeScale = 1; // Resume
            
            // Show pause button when entering level
            var uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowPauseButton();
            }
            
            // Start the level
            if (GameManager.Instance != null && currentLevel != null)
            {
                GameManager.Instance.LoadSpecificLevel(currentLevel, levelIndex);
            }
        }
        
        private void OnBackClicked()
        {
            Hide();
            // Don't resume time here, let ShowMenu() handle it
            
            // Return to menu
            if (LevelMenuManager.Instance != null)
            {
                LevelMenuManager.Instance.ShowMenu();
            }
        }
        
        private string GetDifficultyName(int difficulty)
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
        
        private Color GetDifficultyColor(int difficulty)
        {
            switch (difficulty)
            {
                case 1: return new Color(0.3f, 0.8f, 0.3f); // Green
                case 2: return new Color(0.3f, 0.6f, 0.9f); // Blue
                case 3: return new Color(0.9f, 0.6f, 0.2f); // Orange
                case 4: return new Color(0.9f, 0.3f, 0.3f); // Red
                case 5: return new Color(0.7f, 0.2f, 0.9f); // Purple
                default: return Color.white;
            }
        }
    }
}
