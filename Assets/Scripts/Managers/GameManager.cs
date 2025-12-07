using System.Collections.Generic;
using UnityEngine;
using WaterSort.Core;
using WaterSort.UI;
using WaterSort.Effects;
using System.Linq;

namespace WaterSort
{
    /// <summary>
    /// Main game manager using Singleton pattern
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private LevelData currentLevel;
        [SerializeField] private LevelData[] allLevels; // Array of all levels
        [SerializeField] private GameObject bottlePrefab;
        [SerializeField] private Transform bottleContainer;
        [SerializeField] private UIManager uiManager;

        [Header("Settings")]
        [SerializeField] private float bottleSpacing = 1.8f;
        [SerializeField] private int bottlesPerRow = 5;
        
        private int currentLevelIndex = 0;

        private List<Bottle> bottles = new List<Bottle>();
        private Bottle selectedBottle = null;
        private int moveCount = 0;
        private float timeElapsed = 0f;
        private float levelStartRealTime = 0f; // Track real time when level starts (not affected by pause)
        private bool timeLimitEnabled = false;
        private bool moveLimitEnabled = false;
        private GameState currentState = GameState.Playing;
        private bool isAnimating = false;
        private bool isPaused = false;
        private bool timeLimitChecked = false; // Flag to ensure time limit is checked only once

        // History for undo
        private Stack<GameStateSnapshot> history = new Stack<GameStateSnapshot>();

        public int MoveCount => moveCount;
        public float TimeElapsed => timeElapsed;
        public float TimeRemaining => timeLimitEnabled && currentLevel != null ? currentLevel.timeLimit - timeElapsed : 0f;
        public int MovesRemaining => moveLimitEnabled && currentLevel != null ? currentLevel.maxMoves - moveCount : 0;
        public bool HasTimeLimit => timeLimitEnabled;
        public bool HasMoveLimit => moveLimitEnabled;
        public GameState CurrentState => currentState;
        public int CurrentLevelIndex => currentLevelIndex;
        public bool IsPaused => isPaused;
        public LevelData GetCurrentLevel() => currentLevel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Check if level was selected from main menu
            if (PlayerPrefs.HasKey("SelectedLevel"))
            {
                int selectedLevel = PlayerPrefs.GetInt("SelectedLevel");
                currentLevelIndex = selectedLevel - 1; // Convert to 0-based index
                
                if (allLevels != null && currentLevelIndex >= 0 && currentLevelIndex < allLevels.Length)
                {
                    currentLevel = allLevels[currentLevelIndex];
                    Debug.Log($"✅ Loaded level {selectedLevel} from main menu");
                }
                
                PlayerPrefs.DeleteKey("SelectedLevel"); // Clear after use
            }
            
            // Auto-find UIManager if not assigned
            if (uiManager == null)
            {
                uiManager = FindObjectOfType<UIManager>();
                if (uiManager != null)
                {
                    Debug.Log("✅ UIManager auto-found and assigned");
                }
                else
                {
                    Debug.LogWarning("⚠️ UIManager not found in scene");
                }
            }
        }

        private void Start()
        {
            // Don't auto-start, wait for menu
            // InitializeLevel();
        }
        
        private void Update()
        {
            // Track time if playing and time limit is enabled
            if (currentState == GameState.Playing && timeLimitEnabled)
            {
                // Use realtimeSinceStartup to track time independently of pause
                timeElapsed = Time.realtimeSinceStartup - levelStartRealTime;
                
                // Update UI
                if (uiManager != null)
                {
                    uiManager.UpdateTimer(TimeRemaining);
                }
                
                // Warning sound when time is running out (last 10 seconds)
                if (TimeRemaining <= 10f && TimeRemaining > 0f)
                {
                    // Play warning every second
                    float prevTime = timeElapsed - Time.deltaTime;
                    if (Mathf.FloorToInt(currentLevel.timeLimit - prevTime) != Mathf.FloorToInt(TimeRemaining))
                    {
                        AudioManager.Instance?.PlaySound("error");
                    }
                }
                
                // Check time limit - call it even if animation is running
                // Use flag to ensure it's called only once
                if (timeElapsed >= currentLevel.timeLimit && !timeLimitChecked)
                {
                    timeLimitChecked = true;
                    OnTimeLimitReached();
                }
            }
        }
        
        /// <summary>
        /// Handle time limit reached
        /// </summary>
        private void OnTimeLimitReached()
        {
            currentState = GameState.Lost;
            Debug.Log("⏰ TIME LIMIT REACHED - GAME OVER!");
            Debug.Log($"   timeElapsed: {timeElapsed}, timeLimit: {currentLevel.timeLimit}");
            Debug.Log($"   uiManager: {(uiManager != null ? "NOT NULL" : "NULL")}");
            
            // Play lose sound and effects
            AudioManager.Instance?.PlaySound("lose");
            
            // Play error effects on all incomplete bottles
            foreach (var bottle in bottles)
            {
                if (!bottle.IsComplete)
                {
                    ParticleEffectManager.Instance?.PlayError(bottle.transform.position + Vector3.up * 0.5f);
                }
            }
            
            if (uiManager != null)
            {
                Debug.Log("   Calling ShowTimeLimitPanel()");
                uiManager.ShowTimeLimitPanel();
            }
            else
            {
                Debug.LogError("❌ UIManager is NULL - cannot show time limit panel!");
            }
        }
        
        /// <summary>
        /// Load specific level from menu
        /// </summary>
        public void LoadSpecificLevel(LevelData levelData, int levelIndex)
        {
            currentLevel = levelData;
            currentLevelIndex = levelIndex;
            InitializeLevel();
        }

        /// <summary>
        /// Initialize level
        public void InitializeLevel()
        {
            ClearBottles();
            moveCount = 0;
            timeElapsed = 0f;
            levelStartRealTime = Time.realtimeSinceStartup; // Record when level starts
            timeLimitEnabled = currentLevel != null && currentLevel.timeLimit > 0;
            moveLimitEnabled = currentLevel != null && currentLevel.maxMoves > 0;
            currentState = GameState.Playing;
            timeLimitChecked = false; // Reset flag for new level
            history.Clear();

            if (currentLevel == null)
            {
                Debug.LogError("No level data assigned!");
                return;
            }
            
            // Apply difficulty scaling
            currentLevel.ApplyDifficultyScaling();

            List<List<WaterColor>> bottleData = currentLevel.GenerateLevel();
            CreateBottles(bottleData);
            
            SaveState(); // Save initial state
            
            if (uiManager != null)
            {
                uiManager.UpdateMoveCount(moveCount);
                
                // Initialize move limit panel if level has move limit
                if (moveLimitEnabled)
                {
                    var moveLimitPanelComponent = uiManager.GetComponent<WaterSort.UI.MoveLimitPanel>();
                    if (moveLimitPanelComponent != null)
                    {
                        moveLimitPanelComponent.Initialize(currentLevel.maxMoves);
                    }
                }
                
                // Show/hide timer based on level
                if (timeLimitEnabled)
                {
                    uiManager.UpdateTimer(currentLevel.timeLimit);
                }
                else
                {
                    uiManager.HideTimer();
                }
                
                // Show auto-play button if level has move limit
                if (moveLimitEnabled)
                {
                    uiManager.ShowAutoPlayButton();
                }
                else
                {
                    uiManager.HideAutoPlayButton();
                }
            }
        }

        /// <summary>
        /// Create bottles from data
        /// </summary>
        private void CreateBottles(List<List<WaterColor>> bottleData)
        {
            if (bottlePrefab == null)
            {
                Debug.LogError("Bottle prefab not assigned!");
                return;
            }

            for (int i = 0; i < bottleData.Count; i++)
            {
                Vector3 position = CalculateBottlePosition(i);
                GameObject bottleObj = Instantiate(bottlePrefab, position, Quaternion.identity, bottleContainer);
                bottleObj.name = $"Bottle_{i}";

                Bottle bottle = bottleObj.GetComponent<Bottle>();
                if (bottle != null)
                {
                    bottle.Initialize(bottleData[i]);
                    bottles.Add(bottle);
                }
            }
        }

        /// <summary>
        /// Calculate bottle position in grid layout
        /// </summary>
        private Vector3 CalculateBottlePosition(int index)
        {
            int row = index / bottlesPerRow;
            int col = index % bottlesPerRow;
            
            float xOffset = (col - bottlesPerRow / 2f + 0.5f) * bottleSpacing;
            float yOffset = -row * bottleSpacing;
            
            return new Vector3(xOffset, yOffset, 0);
        }

        /// <summary>
        /// Handle bottle click
        /// </summary>
        public void OnBottleClicked(Bottle bottle)
        {
            Debug.Log($"🎮 GameManager.OnBottleClicked: {bottle.name}");
            Debug.Log($"   Current State: {currentState}");
            Debug.Log($"   Selected Bottle: {(selectedBottle != null ? selectedBottle.name : "None")}");
            
            if (currentState != GameState.Playing)
            {
                Debug.LogWarning("⚠️ Game not in Playing state!");
                return;
            }

            if (isAnimating)
            {
                Debug.LogWarning("⚠️ Animation is running, please wait!");
                return;
            }

            if (selectedBottle == null)
            {
                // Select bottle if it has water
                if (!bottle.IsEmpty)
                {
                    Debug.Log($"✅ Selecting bottle: {bottle.name}");
                    selectedBottle = bottle;
                    bottle.SetSelected(true);
                    AudioManager.Instance?.PlaySound("select");
                }
                else
                {
                    Debug.Log($"⚠️ Cannot select empty bottle");
                }
            }
            else
            {
                if (bottle == selectedBottle)
                {
                    // Deselect
                    Debug.Log($"⭕ Deselecting bottle");
                    selectedBottle.SetSelected(false);
                    selectedBottle = null;
                }
                else
                {
                    Debug.Log($"🔄 Trying to pour from {selectedBottle.name} to {bottle.name}");
                    // Try to pour
                    TryPour(selectedBottle, bottle);
                }
            }
        }

        /// <summary>
        /// Try to pour water from source to target
        /// </summary>
        private void TryPour(Bottle source, Bottle target)
        {
            Debug.Log($"💧 TryPour: {source.name} → {target.name}");
            Debug.Log($"   Source: {source.CurrentAmount} units, Top: {source.GetTopColor()?.color}");
            Debug.Log($"   Target: {target.CurrentAmount} units, Top: {target.GetTopColor()?.color}");
            
            bool canReceive = target.CanReceiveFrom(source);
            Debug.Log($"   Can receive: {canReceive}");
            
            if (canReceive)
            {
                SaveState(); // Save state before move
                
                int amountPoured = source.PourTo(target);
                Debug.Log($"✅ Poured {amountPoured} units");
                
                if (amountPoured > 0)
                {
                    moveCount++;
                    
                    if (uiManager != null)
                    {
                        uiManager.UpdateMoveCount(moveCount);
                    }
                    
                    // Check move limit
                    if (moveLimitEnabled && moveCount >= currentLevel.maxMoves)
                    {
                        Debug.Log($"⚠️ Move limit reached: {moveCount}/{currentLevel.maxMoves}");
                        // Will check after animation completes
                    }
                    
                    // Deselect bottle BEFORE animation (without animation)
                    source.SetSelected(false, animate: false);
                    selectedBottle = null;
                    
                    // Play animation and sound, then check win condition after animation
                    StartCoroutine(AnimatePouringAndCheckWin(source, target));
                    AudioManager.Instance?.PlaySound("pour");
                }
                else
                {
                    Debug.LogWarning("⚠️ amountPoured is 0!");
                    source.SetSelected(false);
                    selectedBottle = null;
                }
            }
            else
            {
                Debug.LogWarning("❌ Cannot pour - invalid move!");
                AudioManager.Instance?.PlaySound("error");
                
                // Shake animation for error
                StartCoroutine(ShakeBottle(source.transform));
                ParticleEffectManager.Instance?.PlayError(target.transform.position);
                
                source.SetSelected(false);
                selectedBottle = null;
            }
        }
        
        /// <summary>
        /// Shake bottle for error feedback
        /// </summary>
        private System.Collections.IEnumerator ShakeBottle(Transform bottle)
        {
            Vector3 originalPos = bottle.position;
            float duration = 0.3f;
            float elapsed = 0;
            float magnitude = 0.15f;
            
            while (elapsed < duration)
            {
                float x = Mathf.Sin(elapsed * 50f) * magnitude * (1f - elapsed / duration);
                bottle.position = originalPos + new Vector3(x, 0, 0);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            bottle.position = originalPos;
        }

        /// <summary>
        /// Animate pouring water and check win condition after animation completes
        /// </summary>
        private System.Collections.IEnumerator AnimatePouringAndCheckWin(Bottle source, Bottle target)
        {
            yield return StartCoroutine(AnimatePouring(source, target));
            
            // Check time limit first (in case it expired during animation)
            if (timeLimitEnabled && timeElapsed >= currentLevel.timeLimit && currentState == GameState.Playing)
            {
                OnTimeLimitReached();
                yield break; // Don't check win if time expired
            }
            
            // Check win condition AFTER animation completes
            CheckWinCondition();
        }

        /// <summary>
        /// Animate pouring water
        /// </summary>
        private System.Collections.IEnumerator AnimatePouring(Bottle source, Bottle target)
        {
            isAnimating = true;
            
            Vector3 sourcePos = source.transform.position;
            Vector3 targetPos = target.transform.position;
            
            // Determine direction: is target on the left or right of source?
            bool targetIsOnRight = targetPos.x > sourcePos.x;
            float direction = targetIsOnRight ? 1f : -1f;
            
            // Calculate pour position - above and slightly to the side of target bottle
            float pourHeight = 1.0f; // Height above target
            float sideOffset = 0.4f; // Distance to the side (closer to target)
            Vector3 pourPos = targetPos + Vector3.up * pourHeight + Vector3.right * (sideOffset * direction);
            
            // Tilt angle based on direction
            float tiltAngle = targetIsOnRight ? -60f : 60f;
            
            float duration = 0.2f;
            float elapsed = 0;
            AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

            // Move source bottle to pour position and tilt
            Quaternion startRot = source.transform.rotation;
            Quaternion tiltRot = Quaternion.Euler(0, 0, tiltAngle);
            
            while (elapsed < duration)
            {
                float t = ease.Evaluate(elapsed / duration);
                source.transform.position = Vector3.Lerp(sourcePos, pourPos, t);
                source.transform.rotation = Quaternion.Lerp(startRot, tiltRot, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            source.transform.position = pourPos;
            source.transform.rotation = tiltRot;

            // Pour effect at the mouth of target bottle
            Color waterColor = source.GetTopColor()?.color ?? Color.white;
            Vector3 splashPos = targetPos + Vector3.up * 0.8f; // Splash at top of target
            ParticleEffectManager.Instance?.PlaySplash(splashPos, waterColor);
            
            // Update visuals during pour
            yield return new WaitForSeconds(0.2f);
            source.UpdateVisuals();
            target.UpdateVisuals();
            
            // Play target bottle reaction animation
            StartCoroutine(BounceBottle(target.transform, 0.1f, 0.15f));

            yield return new WaitForSeconds(0.1f);

            // Return to original position
            elapsed = 0;
            while (elapsed < duration)
            {
                float t = ease.Evaluate(elapsed / duration);
                source.transform.position = Vector3.Lerp(pourPos, sourcePos, t);
                source.transform.rotation = Quaternion.Lerp(tiltRot, startRot, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            source.transform.position = sourcePos;
            source.transform.rotation = startRot;
            
            isAnimating = false;
        }

        /// <summary>
        /// Bounce animation for bottle
        /// </summary>
        private System.Collections.IEnumerator BounceBottle(Transform bottle, float height, float duration)
        {
            Vector3 startPos = bottle.position;
            Vector3 peakPos = startPos + Vector3.up * height;
            float elapsed = 0;
            AnimationCurve bounce = AnimationCurve.EaseInOut(0, 0, 1, 1);
            
            // Up
            while (elapsed < duration * 0.5f)
            {
                float t = bounce.Evaluate(elapsed / (duration * 0.5f));
                bottle.position = Vector3.Lerp(startPos, peakPos, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Down
            elapsed = 0;
            while (elapsed < duration * 0.5f)
            {
                float t = bounce.Evaluate(elapsed / (duration * 0.5f));
                bottle.position = Vector3.Lerp(peakPos, startPos, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            bottle.position = startPos;
        }

        /// <summary>
        /// Check if player won
        /// </summary>
        private void CheckWinCondition()
        {
            // Check if all bottles are complete
            bool allComplete = true;
            foreach (var bottle in bottles)
            {
                if (!bottle.IsEmpty && !bottle.IsComplete)
                {
                    allComplete = false;
                    break;
                }
            }
            
            // Check move limit first
            if (moveLimitEnabled && moveCount >= currentLevel.maxMoves)
            {
                if (!allComplete)
                {
                    // Lost: reached move limit without completing
                    currentState = GameState.Lost;
                    Debug.Log($"🚫 MOVE LIMIT REACHED WITHOUT COMPLETING - GAME OVER! ({moveCount}/{currentLevel.maxMoves})");
                    
                    // Play lose sound and effects
                    AudioManager.Instance?.PlaySound("lose");
                    
                    // Play error effects on all incomplete bottles
                    foreach (var bottle in bottles)
                    {
                        if (!bottle.IsComplete)
                        {
                            ParticleEffectManager.Instance?.PlayError(bottle.transform.position + Vector3.up * 0.5f);
                        }
                    }
                    
                    if (uiManager != null)
                    {
                        uiManager.ShowMoveLimitPanel();
                    }
                    else
                    {
                        Debug.LogError("❌ UIManager is NULL - cannot show move limit panel!");
                    }
                    return;
                }
            }

            Debug.Log($"🏆 CheckWinCondition: allComplete = {allComplete}");

            if (allComplete)
            {
                Debug.Log("🎉 WIN CONDITION MET!");
                currentState = GameState.Won;
                AudioManager.Instance?.PlaySound("win");
                
                // Save level progress
                SaveLevelProgress();
                
                // Play celebration effects
                foreach (var bottle in bottles)
                {
                    if (bottle.IsComplete)
                    {
                        ParticleEffectManager.Instance?.PlayCelebration(bottle.transform.position + Vector3.up * 0.5f);
                        ParticleEffectManager.Instance?.PlaySparkle(bottle.transform.position);
                    }
                }
                
                if (uiManager != null)
                {
                    Debug.Log("✅ Calling uiManager.ShowWinScreen()");
                    uiManager.ShowWinScreen();
                }
                else
                {
                    Debug.LogError("❌ UIManager is NULL!");
                }
            }
        }

        /// <summary>
        /// Undo last move
        /// </summary>
        public void UndoMove()
        {
            if (history.Count <= 1) return; // Keep initial state

            history.Pop(); // Remove current state
            
            if (history.Count > 0)
            {
                GameStateSnapshot previousState = history.Peek();
                RestoreState(previousState);
                
                AudioManager.Instance?.PlaySound("undo");
            }
        }

        /// <summary>
        /// Save current game state
        /// </summary>
        private void SaveState()
        {
            GameStateSnapshot snapshot = new GameStateSnapshot
            {
                moveCount = this.moveCount,
                bottleStates = new List<List<WaterColor>>()
            };

            foreach (var bottle in bottles)
            {
                snapshot.bottleStates.Add(bottle.GetWaterColors());
            }

            history.Push(snapshot);
        }

        /// <summary>
        /// Restore game state
        /// </summary>
        private void RestoreState(GameStateSnapshot snapshot)
        {
            this.moveCount = snapshot.moveCount;
            
            for (int i = 0; i < bottles.Count && i < snapshot.bottleStates.Count; i++)
            {
                bottles[i].Initialize(snapshot.bottleStates[i]);
            }

            if (selectedBottle != null)
            {
                selectedBottle.SetSelected(false);
                selectedBottle = null;
            }

            if (uiManager != null)
            {
                uiManager.UpdateMoveCount(moveCount);
            }
        }

        /// <summary>
        /// Reset current level
        /// </summary>
        public void ResetLevel()
        {
            while (history.Count > 1)
            {
                history.Pop();
            }

            if (history.Count > 0)
            {
                RestoreState(history.Peek());
            }
            
            currentState = GameState.Playing;
            AudioManager.Instance?.PlaySound("reset");
        }

        /// <summary>
        /// Clear all bottles
        /// </summary>
        private void ClearBottles()
        {
            foreach (var bottle in bottles)
            {
                if (bottle != null)
                    Destroy(bottle.gameObject);
            }
            bottles.Clear();
            selectedBottle = null;
        }

        /// <summary>
        /// Load next level
        /// </summary>
        public void LoadNextLevel()
        {
            // If we have allLevels array configured
            if (allLevels != null && allLevels.Length > 0)
            {
                int nextLevelIndex = currentLevelIndex + 1;
                
                // Check if we've completed all levels
                if (nextLevelIndex >= allLevels.Length)
                {
                    Debug.Log("🎉 All levels completed! Returning to menu.");
                    
                    // Return to level menu
                    if (LevelMenuManager.Instance != null)
                    {
                        LevelMenuManager.Instance.ShowMenu();
                    }
                    return;
                }
                
                // Check if next level is unlocked
                bool isNextLevelUnlocked = true;
                if (nextLevelIndex > 0)
                {
                    // Check if current level (previous to next) is completed
                    int previousLevel = nextLevelIndex - 1;
                    bool previousCompleted = PlayerPrefs.GetInt($"Level_{previousLevel}_Completed", 0) == 1;
                    isNextLevelUnlocked = previousCompleted;
                }
                
                if (!isNextLevelUnlocked)
                {
                    Debug.LogWarning($"⚠️ Level {nextLevelIndex + 1} is locked! Returning to menu.");
                    
                    // Show locked message and return to menu
                    if (LevelMenuManager.Instance != null)
                    {
                        LevelMenuManager.Instance.ShowMenu();
                        
                        // Show popup explaining why
                        if (LevelMenuManager.Instance.lockedLevelPopup != null)
                        {
                            string message = $"<b>Level {nextLevelIndex + 1}</b> is locked.\n\nComplete <b>Level {nextLevelIndex}</b> to unlock it!";
                            LevelMenuManager.Instance.lockedLevelPopup.Show(message);
                        }
                    }
                    return;
                }
                
                // Level is unlocked, proceed
                currentLevelIndex = nextLevelIndex;
                currentLevel = allLevels[currentLevelIndex];
                Debug.Log($"📋 Loading Level {currentLevelIndex + 1}");
                
                // Show requirements panel instead of starting immediately
                if (LevelMenuManager.Instance != null && LevelMenuManager.Instance.requirementsPanel != null)
                {
                    LevelMenuManager.Instance.requirementsPanel.ShowRequirements(currentLevel, currentLevelIndex);
                }
                else
                {
                    // Fallback: start immediately if no requirements panel
                    InitializeLevel();
                }
            }
            else
            {
                // No level array, just reinitialize current level
                InitializeLevel();
            }
        }
        
        /// <summary>
        /// Save level completion and stars
        /// </summary>
        private void SaveLevelProgress()
        {
            if (currentLevel == null) return;
            
            // Mark level as completed
            PlayerPrefs.SetInt($"Level_{currentLevelIndex}_Completed", 1);
            
            // Calculate stars based on level's difficulty thresholds
            int stars = 1; // Default 1 star for completion
            int threeStarThreshold = currentLevel.GetThreeStarThreshold();
            int twoStarThreshold = currentLevel.GetTwoStarThreshold();
            
            if (moveCount <= threeStarThreshold)
            {
                stars = 3;
            }
            else if (moveCount <= twoStarThreshold)
            {
                stars = 2;
            }
            
            // Time bonus: reduce stars if over time limit (for timed levels)
            if (timeLimitEnabled && timeElapsed >= currentLevel.timeLimit * 0.9f)
            {
                stars = Mathf.Max(1, stars - 1);
            }
            
            // Save stars only if better than previous
            int previousStars = PlayerPrefs.GetInt($"Level_{currentLevelIndex}_Stars", 0);
            if (stars > previousStars)
            {
                PlayerPrefs.SetInt($"Level_{currentLevelIndex}_Stars", stars);
            }
            
            // Save best move count
            int previousBestMoves = PlayerPrefs.GetInt($"Level_{currentLevelIndex}_BestMoves", 999);
            if (moveCount < previousBestMoves)
            {
                PlayerPrefs.SetInt($"Level_{currentLevelIndex}_BestMoves", moveCount);
            }
            
            PlayerPrefs.Save();
            Debug.Log($"✅ Level {currentLevelIndex + 1} (Difficulty {currentLevel.difficulty}) completed!");
            Debug.Log($"   Moves: {moveCount} (3⭐≤{threeStarThreshold}, 2⭐≤{twoStarThreshold})");
            Debug.Log($"   Stars: {stars}");
            if (timeLimitEnabled)
            {
                Debug.Log($"   Time: {timeElapsed:F1}s / {currentLevel.timeLimit}s");
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        /// <summary>
        /// Pause the game
        /// </summary>
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                isPaused = true;
                Time.timeScale = 0;
                Debug.Log("⏸️ Game paused");
            }
        }
        
        /// <summary>
        /// Resume the game
        /// </summary>
        public void ResumeGame()
        {
            if (isPaused)
            {
                isPaused = false;
                Time.timeScale = 1;
                Debug.Log("▶️ Game resumed");
            }
        }
        
    }

    public enum GameState
    {
        Playing,
        Won,
        Lost,
        Paused
    }

    [System.Serializable]
    public class GameStateSnapshot
    {
        public int moveCount;
        public List<List<WaterColor>> bottleStates;
    }
}
