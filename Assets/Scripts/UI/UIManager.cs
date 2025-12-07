using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WaterSort.Core;

namespace WaterSort.UI
{
    /// <summary>
    /// Manages UI elements
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Gameplay UI")]
        [SerializeField] private TextMeshProUGUI moveCountText;
        [SerializeField] private TextMeshProUGUI moveLimitWarningText;
        [SerializeField] private MoveLimitPanel moveLimitPanel;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button autoPlayButton;
        
        [Header("Pause Panel")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button pauseMenuButton;

        [Header("Win Screen")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private TextMeshProUGUI winMoveCountText;
        [SerializeField] private GameObject[] winStars; // Array of 3 star GameObjects
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button replayButton;

        [Header("Lose Screen")]
        [SerializeField] private GameObject losePanel;
        [SerializeField] private GameObject timeLimitLosePanel;
        [SerializeField] private GameObject moveLimitLosePanel;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button loseMenuButton;

        [Header("Settings Panel")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle muteToggle;

        private void Start()
        {
            SetupButtons();
            HideAllPanels();
            
            // Hide pause button initially - will show when level starts
            if (pauseButton != null)
            {
                pauseButton.gameObject.SetActive(false);
            }
            
            // Hide timer initially - will show when level starts (if needed)
            if (timerText != null)
            {
                timerText.gameObject.SetActive(false);
            }
        }

        private void SetupButtons()
        {
            if (undoButton != null)
                undoButton.onClick.AddListener(OnUndoClicked);

            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetClicked);

            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);
                
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);
            
            if (autoPlayButton != null)
                autoPlayButton.onClick.AddListener(OnAutoPlayClicked);
                
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
                
            if (pauseMenuButton != null)
                pauseMenuButton.onClick.AddListener(OnPauseMenuClicked);
                
            if (nextLevelButton != null)
                nextLevelButton.onClick.AddListener(OnNextLevelClicked);
                
            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetryClicked);

            if (loseMenuButton != null)
                loseMenuButton.onClick.AddListener(OnLoseMenuClicked);

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

            if (muteToggle != null)
                muteToggle.onValueChanged.AddListener(OnMuteToggled);
        }

        /// <summary>
        /// <summary>
        /// Update move count display
        /// </summary>
        public void UpdateMoveCount(int count)
        {
            if (moveCountText != null)
            {
                // Show move limit if enabled
                if (GameManager.Instance != null && GameManager.Instance.HasMoveLimit)
                {
                    var level = GameManager.Instance.GetCurrentLevel();
                    if (level != null)
                    {
                        int remaining = level.maxMoves - count;
                        moveCountText.text = $"Moves: {count}/{level.maxMoves}";
                        
                        // Warning color when running out
                        if (remaining <= 5)
                        {
                            moveCountText.color = Color.red;
                        }
                        else if (remaining <= 10)
                        {
                            moveCountText.color = Color.yellow;
                        }
                        else
                        {
                            moveCountText.color = Color.white;
                        }
                        
                        // Update move limit panel with remaining moves
                        if (moveLimitPanel != null)
                        {
                            moveLimitPanel.UpdateMoves(count);
                        }
                    }
                }
                else
                {
                    moveCountText.text = $"Moves: {count}";
                    moveCountText.color = Color.white;
                    
                    if (moveLimitPanel != null)
                    {
                        moveLimitPanel.HidePanel();
                    }
                }
            }
        }
        
        /// <summary>
        /// Show pause button (called when level starts)
        /// </summary>
        public void ShowPauseButton()
        {
            if (pauseButton != null)
            {
                pauseButton.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// Hide pause button
        /// </summary>
        public void HidePauseButton()
        {
            if (pauseButton != null)
            {
                pauseButton.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Show auto-play button (called when level starts with move limit)
        /// </summary>
        public void ShowAutoPlayButton()
        {
            if (autoPlayButton != null)
            {
                autoPlayButton.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// Hide auto-play button
        /// </summary>
        public void HideAutoPlayButton()
        {
            if (autoPlayButton != null)
            {
                autoPlayButton.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Update timer display
        /// </summary>
        public void UpdateTimer(float timeRemaining)
        {
            if (timerText != null)
            {
                // Show timer
                timerText.gameObject.SetActive(true);
                
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                timerText.text = $"TIME: {minutes:00}:{seconds:00}";
                
                // Change color when time is running out
                if (timeRemaining < 30f)
                {
                    timerText.color = Color.red;
                }
                else if (timeRemaining < 60f)
                {
                    timerText.color = Color.yellow;
                }
                else
                {
                    timerText.color = Color.white;
                }
            }
        }
        
        /// <summary>
        /// Hide timer (for levels without time limit)
        /// </summary>
        public void HideTimer()
        {
            if (timerText != null)
            {
                timerText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Show win screen
        /// </summary>
        public void ShowWinScreen()
        {
            if (winPanel != null)
            {
                StartCoroutine(ShowPanelWithAnimation(winPanel));
                
                if (winMoveCountText != null && GameManager.Instance != null)
                {
                    int moves = GameManager.Instance.MoveCount;
                    winMoveCountText.text = $"Completed in {moves} moves!";
                    
                    // Show stars based on performance
                    ShowStars(CalculateStars(moves));
                }
                
                // Check if Next Level button should be shown
                UpdateNextLevelButton();
            }
        }
        
        /// <summary>
        /// Update Next Level button visibility based on unlock status
        /// </summary>
        private void UpdateNextLevelButton()
        {
            if (nextLevelButton == null || GameManager.Instance == null)
                return;
            
            int currentIndex = GameManager.Instance.CurrentLevelIndex;
            int nextLevelIndex = currentIndex + 1;
            
            // Check if there is a next level
            var allLevels = GameManager.Instance.GetType()
                .GetField("allLevels", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(GameManager.Instance) as LevelData[];
            
            if (allLevels == null || nextLevelIndex >= allLevels.Length)
            {
                // No more levels - hide button or show "Back to Menu"
                nextLevelButton.gameObject.SetActive(false);
                Debug.Log("🏁 No more levels - hiding Next Level button");
                return;
            }
            
            // Check if next level is unlocked
            // After completing current level, next level should be unlocked
            // But we verify to be safe
            bool isUnlocked = true;
            if (nextLevelIndex > 0)
            {
                // Current level should be completed (we just won it)
                bool currentCompleted = PlayerPrefs.GetInt($"Level_{currentIndex}_Completed", 0) == 1;
                isUnlocked = currentCompleted;
            }
            
            if (isUnlocked)
            {
                nextLevelButton.gameObject.SetActive(true);
                Debug.Log($"✅ Next Level button shown - Level {nextLevelIndex + 1} is available");
            }
            else
            {
                nextLevelButton.gameObject.SetActive(false);
                Debug.Log($"🔒 Next Level button hidden - Level {nextLevelIndex + 1} is locked");
            }
        }
        
        /// <summary>
        /// Show stars on win screen
        /// </summary>
        private void ShowStars(int starCount)
        {
            if (winStars != null && winStars.Length == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (winStars[i] != null)
                    {
                        winStars[i].SetActive(i < starCount);
                    }
                }
            }
        }
        
        /// <summary>
        /// Calculate stars earned
        /// </summary>
        private int CalculateStars(int moves)
        {
            if (GameManager.Instance == null) return 1;
            
            var level = GameManager.Instance.GetCurrentLevel();
            if (level == null) return 1;
            
            int threeStarThreshold = level.GetThreeStarThreshold();
            int twoStarThreshold = level.GetTwoStarThreshold();
            
            if (moves <= threeStarThreshold) return 3;
            if (moves <= twoStarThreshold) return 2;
            return 1;
        }
        
        /// <summary>
        /// Show time limit reached panel
        /// </summary>
        public void ShowTimeLimitPanel()
        {
            Debug.Log($"ShowTimeLimitPanel called - timeLimitLosePanel: {(timeLimitLosePanel != null ? timeLimitLosePanel.name : "NULL")}, losePanel: {(losePanel != null ? losePanel.name : "NULL")}");
            
            if (timeLimitLosePanel != null)
            {
                // Make sure parent losePanel is active first
                if (losePanel != null)
                {
                    losePanel.SetActive(true);
                }
                
                Debug.Log("   Showing timeLimitLosePanel");
                StartCoroutine(ShowPanelWithAnimation(timeLimitLosePanel));
            }
            else if (losePanel != null)
            {
                // Fallback to lose panel if time limit panel doesn't exist
                losePanel.SetActive(true);
                Debug.Log("   timeLimitLosePanel is NULL, falling back to losePanel");
                StartCoroutine(ShowPanelWithAnimation(losePanel));
            }
            else
            {
                Debug.LogError("   ❌ Both timeLimitLosePanel and losePanel are NULL!");
            }
        }
        
        /// <summary>
        /// Show move limit reached panel
        /// </summary>
        public void ShowMoveLimitPanel()
        {
            if (moveLimitLosePanel != null)
            {
                // Make sure parent losePanel is active first
                if (losePanel != null)
                {
                    losePanel.SetActive(true);
                }
                
                Debug.Log("   Showing moveLimitLosePanel");
                StartCoroutine(ShowPanelWithAnimation(moveLimitLosePanel));
            }
            else if (losePanel != null)
            {
                // Fallback to lose panel if move limit panel doesn't exist
                losePanel.SetActive(true);
                Debug.Log("   moveLimitLosePanel is NULL, falling back to losePanel");
                StartCoroutine(ShowPanelWithAnimation(losePanel));
            }
        }

        /// <summary>
        /// Show lose screen
        /// </summary>
        public void ShowLoseScreen()
        {
            if (losePanel != null)
            {
                StartCoroutine(ShowPanelWithAnimation(losePanel));
            }
        }

        /// <summary>
        /// Hide all panels
        /// </summary>
        private void HideAllPanels()
        {
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (timeLimitLosePanel != null) timeLimitLosePanel.SetActive(false);
            if (moveLimitLosePanel != null) moveLimitLosePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            
            // Hide move limit panel
            if (moveLimitPanel != null)
            {
                moveLimitPanel.HidePanel();
            }
        }

        // Button callbacks
        private void OnUndoClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            GameManager.Instance?.UndoMove();
        }

        private void OnResetClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            GameManager.Instance?.ResetLevel();
        }

        private void OnMenuClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            
            // Show level menu
            if (LevelMenuManager.Instance != null)
            {
                LevelMenuManager.Instance.ShowMenu();
            }
            
            HideAllPanels();
        }
        
        private void OnPauseClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
                if (pausePanel != null)
                    pausePanel.SetActive(true);
            }
        }
        
        private void OnResumeClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
                if (pausePanel != null)
                    pausePanel.SetActive(false);
            }
        }
        
        private void OnAutoPlayClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            Debug.Log("🤖 Auto-play feature coming soon!");
            // Future implementation: Auto solver/demo mode
        }
        
        private void OnPauseMenuClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
        
        private void OnNextLevelClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            HideAllPanels();
            GameManager.Instance?.LoadNextLevel();
        }

        private void OnReplayClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            HideAllPanels();
            
            // Show requirements panel for replay
            if (GameManager.Instance != null)
            {
                var currentLevel = GameManager.Instance.GetCurrentLevel();
                int levelIndex = GameManager.Instance.CurrentLevelIndex;
                
                if (LevelMenuManager.Instance != null && LevelMenuManager.Instance.requirementsPanel != null && currentLevel != null)
                {
                    LevelMenuManager.Instance.requirementsPanel.ShowRequirements(currentLevel, levelIndex);
                }
                else
                {
                    // Fallback: reset directly
                    GameManager.Instance.ResetLevel();
                }
            }
        }

        private void OnRetryClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            HideAllPanels();
            
            // Show requirements panel for retry
            if (GameManager.Instance != null)
            {
                var currentLevel = GameManager.Instance.GetCurrentLevel();
                int levelIndex = GameManager.Instance.CurrentLevelIndex;
                
                if (LevelMenuManager.Instance != null && LevelMenuManager.Instance.requirementsPanel != null && currentLevel != null)
                {
                    LevelMenuManager.Instance.requirementsPanel.ShowRequirements(currentLevel, levelIndex);
                }
                else
                {
                    // Fallback: reset directly
                    GameManager.Instance.ResetLevel();
                }
            }
        }
        
        private void OnLoseMenuClicked()
        {
            AudioManager.Instance?.PlaySound("button");
            HideAllPanels();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }

        private void OnMusicVolumeChanged(float value)
        {
            AudioManager.Instance?.SetMusicVolume(value);
        }

        private void OnSFXVolumeChanged(float value)
        {
            AudioManager.Instance?.SetSFXVolume(value);
        }

        private void OnMuteToggled(bool muted)
        {
            AudioManager.Instance?.SetMute(muted);
        }

        /// <summary>
        /// Show message to player
        /// </summary>
        public void ShowMessage(string message, float duration = 2f)
        {
            // Implement tooltip or message system
            Debug.Log($"Message: {message}");
        }
        
        /// <summary>
        /// Show panel with fade and scale animation
        /// </summary>
        private System.Collections.IEnumerator ShowPanelWithAnimation(GameObject panel)
        {
            Debug.Log($"ShowPanelWithAnimation: Starting animation for {panel.name}");
            
            panel.SetActive(true);
            Debug.Log($"   Panel active: {panel.activeInHierarchy}");
            
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
            {
                canvasGroup = panel.AddComponent<CanvasGroup>();
            }
            
            // Ensure CanvasGroup doesn't block interactions
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = true;
            
            canvasGroup.alpha = 0f;
            panel.transform.localScale = Vector3.one * 0.8f;
            
            float duration = 0.3f;
            float startTime = Time.realtimeSinceStartup; // Use real time, not affected by pause
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed = Time.realtimeSinceStartup - startTime;
                float t = elapsed / duration;
                float easedT = 1f - Mathf.Pow(1f - t, 3f); // Ease out cubic
                
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, easedT);
                panel.transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, easedT);
                
                yield return null;
            }
            
            Debug.Log($"ShowPanelWithAnimation: Animation completed for {panel.name}");
            Debug.Log($"   Final: alpha={canvasGroup.alpha}, active={panel.activeInHierarchy}, scale={panel.transform.localScale}");
            
            canvasGroup.alpha = 1f;
            panel.transform.localScale = Vector3.one;
            
            // Ensure button interactions are enabled
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        
        /// <summary>
        /// Hide panel with fade out animation
        /// </summary>
        private System.Collections.IEnumerator HidePanelWithAnimation(GameObject panel)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
            {
                panel.SetActive(false);
                yield break;
            }
            
            float duration = 0.2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                panel.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.9f, t);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            panel.SetActive(false);
        }
    }
}
