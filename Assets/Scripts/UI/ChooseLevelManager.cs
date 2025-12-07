using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using WaterSort.Core;

public class ChooseLevelManager : MonoBehaviour
{
    [Header("UI")]
    public Transform levelButtonContainer;
    public GameObject levelButtonPrefab;
    public TextMeshProUGUI titleText;
    
    [Header("Levels")]
    public LevelData[] allLevels;
    
    void Start()
    {
        if (titleText != null)
            titleText.text = "CHOOSE LEVEL";
            
        GenerateLevelButtons();
    }
    
    void GenerateLevelButtons()
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
            
            // Set button text
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = (i + 1).ToString();
            }
            
            // Setup button click
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnLevelClicked(levelIndex));
            }
            
            // Color based on completion
            bool isCompleted = PlayerPrefs.GetInt($"Level_{levelIndex}_Completed", 0) == 1;
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null && isCompleted)
            {
                buttonImage.color = new Color(0.2f, 0.8f, 0.3f); // Green for completed
            }
        }
    }
    
    void OnLevelClicked(int levelIndex)
    {
        Debug.Log($"Loading level {levelIndex + 1}");
        
        // Store selected level
        PlayerPrefs.SetInt("SelectedLevel", levelIndex + 1);
        PlayerPrefs.Save();
        
        // Load game scene
        SceneManager.LoadScene("Game");
    }
    
    public void OnBackClicked()
    {
        // Go back to main menu or previous scene
        SceneManager.LoadScene(0); // Load first scene (typically main menu)
    }
}
