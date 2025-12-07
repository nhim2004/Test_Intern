using UnityEngine;
using System.Collections.Generic;

namespace WaterSort.Core
{
    /// <summary>
    /// Advanced level generator with difficulty parameters
    /// </summary>
    public class LevelGenerator : MonoBehaviour
    {
        [Header("Generation Parameters")]
        [SerializeField] private int minColors = 4;
        [SerializeField] private int maxColors = 8;
        [SerializeField] private int bottleCapacity = 4;
        [SerializeField] private int extraEmptyBottles = 2;

        [Header("Difficulty Settings")]
        [Range(0, 1)]
        [SerializeField] private float difficultyLevel = 0.5f;
        [SerializeField] private bool ensureSolvable = true;

        [Header("Color Palette")]
        [SerializeField] private List<Color> colorPalette = new List<Color>
        {
            new Color(1f, 0.2f, 0.2f),      // Red
            new Color(0.2f, 0.4f, 1f),      // Blue
            new Color(0.3f, 0.9f, 0.3f),    // Green
            new Color(1f, 0.9f, 0.2f),      // Yellow
            new Color(1f, 0.5f, 0f),        // Orange
            new Color(0.6f, 0.2f, 0.8f),    // Purple
            new Color(0.2f, 0.9f, 0.9f),    // Cyan
            new Color(1f, 0.4f, 0.7f),      // Pink
            new Color(0.5f, 0.3f, 0.1f),    // Brown
            new Color(0.9f, 0.9f, 0.9f)     // White
        };

        /// <summary>
        /// Generate a new level based on difficulty
        /// </summary>
        public LevelData GenerateLevel(int levelNumber)
        {
            LevelData level = ScriptableObject.CreateInstance<LevelData>();
            level.levelNumber = levelNumber;

            // Calculate parameters based on difficulty
            int numColors = Mathf.RoundToInt(Mathf.Lerp(minColors, maxColors, difficultyLevel));
            int emptyBottles = Mathf.Max(1, extraEmptyBottles - Mathf.RoundToInt(difficultyLevel * 2));
            
            level.numberOfColors = numColors;
            level.numberOfBottles = numColors + emptyBottles;
            level.bottleCapacity = bottleCapacity;
            level.availableColors = GetRandomColors(numColors);

            // Generate bottle setup
            level.bottleSetup = GenerateBottleSetup(numColors, bottleCapacity, emptyBottles);

            return level;
        }

        /// <summary>
        /// Get random colors from palette
        /// </summary>
        private List<Color> GetRandomColors(int count)
        {
            List<Color> colors = new List<Color>(colorPalette);
            List<Color> selected = new List<Color>();

            for (int i = 0; i < count && colors.Count > 0; i++)
            {
                int index = Random.Range(0, colors.Count);
                selected.Add(colors[index]);
                colors.RemoveAt(index);
            }

            return selected;
        }

        /// <summary>
        /// Generate bottle setup
        /// </summary>
        private List<LevelData.BottleData> GenerateBottleSetup(int numColors, int capacity, int emptyBottles)
        {
            List<LevelData.BottleData> bottles = new List<LevelData.BottleData>();

            // Create all color segments
            List<LevelData.ColorSegment> allSegments = new List<LevelData.ColorSegment>();
            
            for (int i = 0; i < numColors; i++)
            {
                Color color = colorPalette[i % colorPalette.Count];
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
            int bottlesWithColor = numColors;
            for (int i = 0; i < bottlesWithColor; i++)
            {
                LevelData.BottleData bottleData = new LevelData.BottleData
                {
                    segments = new List<LevelData.ColorSegment>()
                };

                int startIndex = i * capacity;
                for (int j = 0; j < capacity && startIndex + j < allSegments.Count; j++)
                {
                    bottleData.segments.Add(allSegments[startIndex + j]);
                }

                // Optionally merge consecutive same colors
                MergeConsecutiveColors(bottleData.segments);

                bottles.Add(bottleData);
            }

            // Add empty bottles
            for (int i = 0; i < emptyBottles; i++)
            {
                bottles.Add(new LevelData.BottleData 
                { 
                    segments = new List<LevelData.ColorSegment>() 
                });
            }

            return bottles;
        }

        /// <summary>
        /// Merge consecutive segments of same color
        /// </summary>
        private void MergeConsecutiveColors(List<LevelData.ColorSegment> segments)
        {
            for (int i = segments.Count - 1; i > 0; i--)
            {
                if (segments[i].color.Equals(segments[i - 1].color))
                {
                    segments[i - 1].amount += segments[i].amount;
                    segments.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Shuffle list using Fisher-Yates algorithm
        /// </summary>
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        /// <summary>
        /// Validate if generated level is solvable (basic heuristic)
        /// </summary>
        public bool ValidateLevel(LevelData level)
        {
            // Basic validation: ensure we have empty bottles
            int emptyCount = 0;
            foreach (var bottle in level.bottleSetup)
            {
                if (bottle.segments == null || bottle.segments.Count == 0)
                    emptyCount++;
            }

            return emptyCount >= 1;
        }

        /// <summary>
        /// Generate progressive difficulty levels
        /// </summary>
        public List<LevelData> GenerateLevelPack(int numberOfLevels)
        {
            List<LevelData> levels = new List<LevelData>();

            for (int i = 0; i < numberOfLevels; i++)
            {
                float difficulty = (float)i / (numberOfLevels - 1);
                difficultyLevel = difficulty;
                
                LevelData level = GenerateLevel(i + 1);
                
                if (ensureSolvable)
                {
                    int attempts = 0;
                    while (!ValidateLevel(level) && attempts < 10)
                    {
                        level = GenerateLevel(i + 1);
                        attempts++;
                    }
                }

                levels.Add(level);
            }

            return levels;
        }
    }
}
