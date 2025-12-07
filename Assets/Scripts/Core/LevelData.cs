using System.Collections.Generic;
using UnityEngine;

namespace WaterSort.Core
{
    /// <summary>
    /// Represents level configuration data
    /// </summary>
    [CreateAssetMenu(fileName = "Level", menuName = "WaterSort/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Info")]
        public int levelNumber;
        public int numberOfBottles = 8;
        public int numberOfColors = 5;
        public int bottleCapacity = 4;
        
        [Header("Difficulty Settings")]
        [Tooltip("Difficulty: 1=Easy, 2=Medium, 3=Hard, 4=Expert, 5=Master")]
        [Range(1, 5)]
        public int difficulty = 1;
        
        [Tooltip("Optimal/minimum moves to complete (0 = auto-calculate)")]
        public int optimalMoves = 0;
        
        [Tooltip("Maximum moves to get 3 stars (0 = auto-calculate)")]
        public int threeStarMoves = 0;
        
        [Tooltip("Maximum moves to get 2 stars (0 = auto-calculate)")]
        public int twoStarMoves = 0;
        
        [Tooltip("Maximum moves allowed (0 = unlimited)")]
        public int maxMoves = 0;
        
        [Tooltip("Time limit in seconds (0 = no limit)")]
        public float timeLimit = 0f;
        
        [Tooltip("Minimum empty bottles for this level")]
        public int minEmptyBottles = 2;
        
        [Header("Colors")]
        public List<Color> availableColors = new List<Color>
        {
            new Color(1f, 0.2f, 0.2f),      // Bright Red
            new Color(0.2f, 0.5f, 1f),      // Bright Blue
            new Color(0.2f, 1f, 0.2f),      // Bright Green
            new Color(1f, 0.9f, 0.2f),      // Bright Yellow
            new Color(1f, 0.5f, 0f),        // Orange
            new Color(0.7f, 0.2f, 0.9f),    // Purple
            new Color(0.2f, 1f, 1f),        // Cyan
            new Color(1f, 0.4f, 0.7f)       // Pink
        };

        [Header("Predefined Level (Optional)")]
        public List<BottleData> bottleSetup;

        [System.Serializable]
        public class BottleData
        {
            public List<ColorSegment> segments;
        }

        [System.Serializable]
        public class ColorSegment
        {
            public Color color;
            public int amount;
        }

        /// <summary>
        /// Generate random level
        /// </summary>
        public List<List<WaterColor>> GenerateLevel()
        {
            if (bottleSetup != null && bottleSetup.Count > 0)
            {
                return GeneratePredefinedLevel();
            }
            
            return GenerateRandomLevel();
        }

        private List<List<WaterColor>> GeneratePredefinedLevel()
        {
            List<List<WaterColor>> result = new List<List<WaterColor>>();
            
            foreach (var bottleData in bottleSetup)
            {
                List<WaterColor> colors = new List<WaterColor>();
                foreach (var segment in bottleData.segments)
                {
                    colors.Add(new WaterColor(segment.color, segment.amount));
                }
                result.Add(colors);
            }
            
            return result;
        }

        private List<List<WaterColor>> GenerateRandomLevel()
        {
            List<List<WaterColor>> result = new List<List<WaterColor>>();
            
            // Create all water segments
            List<WaterColor> allSegments = new List<WaterColor>();
            for (int i = 0; i < numberOfColors; i++)
            {
                Color color = availableColors[i % availableColors.Count];
                for (int j = 0; j < bottleCapacity; j++)
                {
                    allSegments.Add(new WaterColor(color, 1));
                }
            }

            // Shuffle
            for (int i = allSegments.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = allSegments[i];
                allSegments[i] = allSegments[j];
                allSegments[j] = temp;
            }

            // Distribute to bottles (numberOfColors bottles + empty bottles)
            int segmentsPerBottle = bottleCapacity;
            int bottlesWithWater = numberOfColors;
            
            for (int i = 0; i < bottlesWithWater; i++)
            {
                List<WaterColor> bottleColors = new List<WaterColor>();
                int startIndex = i * segmentsPerBottle;
                
                for (int j = 0; j < segmentsPerBottle && startIndex + j < allSegments.Count; j++)
                {
                    bottleColors.Add(allSegments[startIndex + j]);
                }
                
                result.Add(bottleColors);
            }

            // Add empty bottles
            int emptyBottles = numberOfBottles - bottlesWithWater;
            for (int i = 0; i < emptyBottles; i++)
            {
                result.Add(new List<WaterColor>());
            }

            return result;
        }

        /// <summary>
        /// Calculate optimal/minimum moves needed to solve
        /// </summary>
        public int CalculateOptimalMoves()
        {
            if (optimalMoves > 0) return optimalMoves;
            
            // Formula based on game theory:
            // Each color needs to be moved to its own bottle
            // Minimum moves = numberOfColors * (bottleCapacity - 1)
            // But can be optimized with empty bottles
            
            int baseOptimal = numberOfColors * 2; // Conservative estimate
            
            // Adjust based on bottle capacity
            if (bottleCapacity > 4)
            {
                baseOptimal += (bottleCapacity - 4) * numberOfColors;
            }
            
            // Adjust based on empty bottles
            int emptyBottles = numberOfBottles - numberOfColors;
            if (emptyBottles >= 2)
            {
                // More empty bottles = easier optimization
                baseOptimal = Mathf.Max(numberOfColors * 2, baseOptimal - emptyBottles);
            }
            else if (emptyBottles < 2)
            {
                // Fewer empty bottles = more moves needed
                baseOptimal += (2 - emptyBottles) * 3;
            }
            
            return baseOptimal;
        }
        
        /// <summary>
        /// Get move threshold for 3 stars (auto-calculated if not set)
        /// </summary>
        public int GetThreeStarThreshold()
        {
            if (threeStarMoves > 0) return threeStarMoves;
            
            // Based on optimal moves + small buffer
            int optimal = CalculateOptimalMoves();
            int buffer = difficulty * 2 + 3; // Smaller buffer for 3 stars
            return optimal + buffer;
        }
        
        /// <summary>
        /// Get move threshold for 2 stars (auto-calculated if not set)
        /// </summary>
        public int GetTwoStarThreshold()
        {
            if (twoStarMoves > 0) return twoStarMoves;
            
            int threeStarThreshold = GetThreeStarThreshold();
            int additionalBuffer = difficulty * 3 + 5;
            return threeStarThreshold + additionalBuffer;
        }
        
        /// <summary>
        /// Apply difficulty scaling to level parameters
        /// </summary>
        public void ApplyDifficultyScaling()
        {
            switch (difficulty)
            {
                case 1: // Easy
                    numberOfBottles = Mathf.Max(numberOfColors + 3, 6);
                    minEmptyBottles = 3;
                    break;
                    
                case 2: // Medium
                    numberOfBottles = Mathf.Max(numberOfColors + 2, 7);
                    minEmptyBottles = 2;
                    break;
                    
                case 3: // Hard
                    numberOfBottles = numberOfColors + 2;
                    minEmptyBottles = 2;
                    break;
                    
                case 4: // Expert
                    numberOfBottles = numberOfColors + 1;
                    minEmptyBottles = 1;
                    bottleCapacity = 5; // Larger bottles = more complexity
                    break;
                    
                case 5: // Master
                    numberOfBottles = numberOfColors + 1;
                    minEmptyBottles = 1;
                    bottleCapacity = 5;
                    // Master levels can have time limits
                    if (timeLimit == 0) timeLimit = 180f; // 3 minutes
                    break;
            }
        }

        /// <summary>
        /// Validate if level is solvable (basic check)
        /// </summary>
        public bool ValidateLevel(List<List<WaterColor>> bottles)
        {
            // Check for minimum empty bottles based on difficulty
            int emptyCount = 0;
            foreach (var bottle in bottles)
            {
                if (bottle.Count == 0) emptyCount++;
            }

            return emptyCount >= minEmptyBottles;
        }
    }
}
