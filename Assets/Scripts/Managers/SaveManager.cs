using UnityEngine;
using System.IO;

namespace WaterSort.Core
{
    /// <summary>
    /// Save and load game progress
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private string saveFilePath;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);

            saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        }

        /// <summary>
        /// Save game data
        /// </summary>
        public void SaveGame(GameSaveData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(saveFilePath, json);
                Debug.Log($"Game saved to {saveFilePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        /// <summary>
        /// Load game data
        /// </summary>
        public GameSaveData LoadGame()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    string json = File.ReadAllText(saveFilePath);
                    GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
                    Debug.Log("Game loaded successfully");
                    return data;
                }
                else
                {
                    Debug.Log("No save file found, creating new save");
                    return new GameSaveData();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return new GameSaveData();
            }
        }

        /// <summary>
        /// Delete save file
        /// </summary>
        public void DeleteSave()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                    Debug.Log("Save file deleted");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to delete save: {e.Message}");
            }
        }

        /// <summary>
        /// Check if save file exists
        /// </summary>
        public bool HasSaveFile()
        {
            return File.Exists(saveFilePath);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }

    /// <summary>
    /// Save data structure
    /// </summary>
    [System.Serializable]
    public class GameSaveData
    {
        public int currentLevel = 1;
        public int highestLevel = 1;
        public int totalStars = 0;
        public float musicVolume = 0.7f;
        public float sfxVolume = 1f;
        public bool isMuted = false;

        // Per-level data
        public LevelProgress[] levelProgress;

        public GameSaveData()
        {
            levelProgress = new LevelProgress[0];
        }
    }

    [System.Serializable]
    public class LevelProgress
    {
        public int levelNumber;
        public bool isCompleted;
        public int bestMoves;
        public int stars;
    }
}
