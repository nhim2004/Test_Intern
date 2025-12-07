using UnityEngine;
using UnityEditor;
using WaterSort.Core;

namespace WaterSort.Editor
{
    /// <summary>
    /// Editor tool to generate levels automatically
    /// </summary>
    public class LevelGeneratorEditor : EditorWindow
    {
        private int startLevel = 1;
        private int numberOfLevels = 10;
        private int minColors = 4;
        private int maxColors = 10;
        private int bottleCapacity = 4;
        private int extraEmptyBottles = 2;
        private bool ensureSolvable = true;

        [MenuItem("WaterSort/Level Generator")]
        public static void ShowWindow()
        {
            GetWindow<LevelGeneratorEditor>("Level Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Auto Level Generator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Generation Settings", EditorStyles.boldLabel);
            startLevel = EditorGUILayout.IntField("Start Level Number", startLevel);
            numberOfLevels = EditorGUILayout.IntField("Number of Levels", numberOfLevels);
            
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Difficulty Settings", EditorStyles.boldLabel);
            minColors = EditorGUILayout.IntSlider("Min Colors", minColors, 3, 8);
            maxColors = EditorGUILayout.IntSlider("Max Colors", maxColors, minColors, 12);
            bottleCapacity = EditorGUILayout.IntSlider("Bottle Capacity", bottleCapacity, 3, 5);
            extraEmptyBottles = EditorGUILayout.IntSlider("Extra Empty Bottles", extraEmptyBottles, 1, 4);
            
            GUILayout.Space(10);
            ensureSolvable = EditorGUILayout.Toggle("Ensure Solvable", ensureSolvable);

            GUILayout.Space(20);
            
            if (GUILayout.Button("Generate Levels", GUILayout.Height(40)))
            {
                GenerateLevels();
            }

            GUILayout.Space(10);
            
            if (GUILayout.Button("Generate Single Random Level", GUILayout.Height(30)))
            {
                GenerateSingleLevel();
            }
        }

        private void GenerateLevels()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Levels"))
            {
                AssetDatabase.CreateFolder("Assets", "Levels");
            }

            for (int i = 0; i < numberOfLevels; i++)
            {
                int levelNum = startLevel + i;
                float difficulty = numberOfLevels > 1 ? (float)i / (numberOfLevels - 1) : 0.5f;
                
                LevelData level = GenerateLevel(levelNum, difficulty);
                
                string assetPath = $"Assets/Levels/Level_{levelNum:D2}.asset";
                
                // Check if exists
                LevelData existing = AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);
                if (existing != null)
                {
                    if (!EditorUtility.DisplayDialog("Level Exists", 
                        $"Level {levelNum} already exists. Overwrite?", "Yes", "No"))
                    {
                        continue;
                    }
                    AssetDatabase.DeleteAsset(assetPath);
                }
                
                AssetDatabase.CreateAsset(level, assetPath);
                Debug.Log($"✅ Generated Level {levelNum} at {assetPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", 
                $"Generated {numberOfLevels} levels!", "OK");
        }

        private void GenerateSingleLevel()
        {
            int levelNum = startLevel;
            float difficulty = 0.5f;
            
            LevelData level = GenerateLevel(levelNum, difficulty);
            
            if (!AssetDatabase.IsValidFolder("Assets/Levels"))
            {
                AssetDatabase.CreateFolder("Assets", "Levels");
            }
            
            string assetPath = $"Assets/Levels/Level_Random_{System.DateTime.Now.Ticks}.asset";
            AssetDatabase.CreateAsset(level, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = level;
            
            Debug.Log($"✅ Generated random level at {assetPath}");
        }

        private LevelData GenerateLevel(int levelNumber, float difficulty)
        {
            LevelData level = ScriptableObject.CreateInstance<LevelData>();
            level.name = $"Level_{levelNumber:D2}";
            level.levelNumber = levelNumber;

            // Calculate parameters based on difficulty
            int numColors = Mathf.RoundToInt(Mathf.Lerp(minColors, maxColors, difficulty));
            int emptyBottles = Mathf.Max(1, extraEmptyBottles - Mathf.RoundToInt(difficulty * 1.5f));
            
            level.numberOfColors = numColors;
            level.numberOfBottles = numColors + emptyBottles;
            level.bottleCapacity = bottleCapacity;
            level.availableColors = GetRandomColors(numColors);

            // Generate bottle setup
            level.bottleSetup = GenerateBottleSetup(numColors, bottleCapacity, emptyBottles, level.availableColors);

            return level;
        }

        private System.Collections.Generic.List<Color> GetRandomColors(int count)
        {
            System.Collections.Generic.List<Color> colorPalette = new System.Collections.Generic.List<Color>
            {
                new Color(1f, 0.2f, 0.2f),      // Red
                new Color(0.2f, 0.5f, 1f),      // Blue
                new Color(0.2f, 1f, 0.2f),      // Green
                new Color(1f, 0.9f, 0.2f),      // Yellow
                new Color(1f, 0.5f, 0f),        // Orange
                new Color(0.7f, 0.2f, 0.9f),    // Purple
                new Color(0.2f, 1f, 1f),        // Cyan
                new Color(1f, 0.4f, 0.7f),      // Pink
                new Color(0.5f, 0.3f, 0.1f),    // Brown
                new Color(0.9f, 0.9f, 0.9f),    // White
                new Color(0.3f, 0.3f, 0.3f),    // Gray
                new Color(1f, 0.6f, 0.8f)       // Light Pink
            };

            System.Collections.Generic.List<Color> colors = new System.Collections.Generic.List<Color>(colorPalette);
            System.Collections.Generic.List<Color> selected = new System.Collections.Generic.List<Color>();

            for (int i = 0; i < count && colors.Count > 0; i++)
            {
                int index = Random.Range(0, colors.Count);
                selected.Add(colors[index]);
                colors.RemoveAt(index);
            }

            return selected;
        }

        private System.Collections.Generic.List<LevelData.BottleData> GenerateBottleSetup(
            int numColors, int capacity, int emptyBottles, System.Collections.Generic.List<Color> colors)
        {
            System.Collections.Generic.List<LevelData.BottleData> bottles = 
                new System.Collections.Generic.List<LevelData.BottleData>();

            // Create all color segments
            System.Collections.Generic.List<LevelData.ColorSegment> allSegments = 
                new System.Collections.Generic.List<LevelData.ColorSegment>();
            
            for (int i = 0; i < numColors; i++)
            {
                Color color = colors[i];
                for (int j = 0; j < capacity; j++)
                {
                    allSegments.Add(new LevelData.ColorSegment 
                    { 
                        color = color, 
                        amount = 1 
                    });
                }
            }

            // Shuffle segments
            ShuffleList(allSegments);

            // Distribute to bottles
            for (int i = 0; i < numColors; i++)
            {
                LevelData.BottleData bottleData = new LevelData.BottleData
                {
                    segments = new System.Collections.Generic.List<LevelData.ColorSegment>()
                };

                int startIndex = i * capacity;
                for (int j = 0; j < capacity && startIndex + j < allSegments.Count; j++)
                {
                    bottleData.segments.Add(allSegments[startIndex + j]);
                }

                bottles.Add(bottleData);
            }

            // Add empty bottles
            for (int i = 0; i < emptyBottles; i++)
            {
                bottles.Add(new LevelData.BottleData 
                { 
                    segments = new System.Collections.Generic.List<LevelData.ColorSegment>() 
                });
            }

            return bottles;
        }

        private void ShuffleList<T>(System.Collections.Generic.List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
