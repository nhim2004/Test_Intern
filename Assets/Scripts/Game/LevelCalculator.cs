using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace WaterSort.Game
{
    /// <summary>
    /// Tính toán minimum moves và difficulty cho level
    /// </summary>
    public static class LevelCalculator
    {
        /// <summary>
        /// Tính số moves tối thiểu để hoàn thành level dựa trên số lượng bottles và màu
        /// 
        /// Công thức:
        /// - Mỗi màu cần 1 move để ghép hoàn chỉnh
        /// - Mỗi bottle không hoàn chỉnh cần thêm moves để sort
        /// - Bottle trống cần để chứa từng phần khi sort
        /// 
        /// minMoves = numberOfColors + (numberOfBottles - numberOfEmptyBottles - numberOfColors) * 2
        /// </summary>
        public static int CalculateMinimumMoves(int numberOfBottles, int numberOfColors, int numberOfEmptyBottles = 0)
        {
            if (numberOfColors == 0) return 0;
            if (numberOfBottles <= numberOfColors) return numberOfColors;

            // Mỗi màu cần 1 move để ghép hoàn chỉnh
            int minMoveForColors = numberOfColors;

            // Mỗi bottle không hoàn chỉnh cần thêm moves
            // Tối đa: (bottles - colors) * 2 = số moves cần để sort hết
            int bottlesNeeded = numberOfBottles - numberOfEmptyBottles;
            int incompleteMoves = (bottlesNeeded - numberOfColors) * 2;

            int totalMinMoves = minMoveForColors + Mathf.Max(0, incompleteMoves);

            return totalMinMoves;
        }

        /// <summary>
        /// Tính số moves được phép dựa trên minimum moves và difficulty level
        /// Difficulty: 1=Easy (150%), 2=Normal (120%), 3=Hard (100%), 4=Expert (80%), 5=Master (60%)
        /// </summary>
        public static int CalculateMaxMovesFromDifficulty(int minimumMoves, int difficulty)
        {
            float multiplier = difficulty switch
            {
                1 => 1.5f,  // Easy: 150% of min
                2 => 1.2f,  // Normal: 120% of min
                3 => 1.0f,  // Hard: 100% of min
                4 => 0.8f,  // Expert: 80% of min
                5 => 0.6f,  // Master: 60% of min
                _ => 1.0f
            };

            int maxMoves = Mathf.Max(minimumMoves, Mathf.CeilToInt(minimumMoves * multiplier));
            return maxMoves;
        }

        /// <summary>
        /// Tính difficulty dựa trên level number
        /// Levels 1-10: Difficulty 1
        /// Levels 11-20: Difficulty 2
        /// Levels 21-30: Difficulty 3
        /// Levels 31-40: Difficulty 4
        /// Levels 41+: Difficulty 5
        /// </summary>
        public static int CalculateDifficultyFromLevel(int levelNumber)
        {
            return Mathf.Clamp((levelNumber - 1) / 10 + 1, 1, 5);
        }

        /// <summary>
        /// Tính toán đầy đủ thông tin level:
        /// - Minimum moves
        /// - Maximum moves (cho phép)
        /// - Difficulty
        /// - Time limit (option: mỗi 5 level một lần)
        /// </summary>
        public static LevelCalculationResult CalculateLevelStats(
            int levelNumber,
            int numberOfBottles,
            int numberOfColors,
            int numberOfEmptyBottles = 0,
            bool enableTimeLimitEvery5Levels = true)
        {
            int difficulty = CalculateDifficultyFromLevel(levelNumber);
            int minimumMoves = CalculateMinimumMoves(numberOfBottles, numberOfColors, numberOfEmptyBottles);
            int maxMoves = CalculateMaxMovesFromDifficulty(minimumMoves, difficulty);

            // Time limit: mỗi level thứ 5, 10, 15... của mỗi 10 level
            int timeLimitSeconds = (enableTimeLimitEvery5Levels && levelNumber % 5 == 0) 
                ? (60 + difficulty * 30)  // 60s Easy, 90s Normal, 120s Hard, 150s Expert, 180s Master
                : 0;

            return new LevelCalculationResult
            {
                levelNumber = levelNumber,
                difficulty = difficulty,
                minimumMoves = minimumMoves,
                maxMoves = maxMoves,
                timeLimitSeconds = timeLimitSeconds,
                numberOfBottles = numberOfBottles,
                numberOfColors = numberOfColors
            };
        }

        /// <summary>
        /// Tính số bottles và colors cho level dựa trên level number
        /// </summary>
        public static void GetLevelConfiguration(int levelNumber, out int bottles, out int colors)
        {
            int difficulty = CalculateDifficultyFromLevel(levelNumber);

            bottles = difficulty switch
            {
                1 => 5,   // Easy: 5 bottles
                2 => 6,   // Normal: 6 bottles
                3 => 7,   // Hard: 7 bottles
                4 => 8,   // Expert: 8 bottles
                5 => 9,   // Master: 9 bottles
                _ => 5
            };

            colors = difficulty switch
            {
                1 => 4,   // Easy: 4 colors
                2 => 5,   // Normal: 5 colors
                3 => 6,   // Hard: 6 colors
                4 => 7,   // Expert: 7 colors
                5 => 8,   // Master: 8 colors
                _ => 4
            };
        }
    }

    /// <summary>
    /// Kết quả tính toán thông tin level
    /// </summary>
    public class LevelCalculationResult
    {
        public int levelNumber;
        public int difficulty;
        public int minimumMoves;
        public int maxMoves;
        public int timeLimitSeconds;
        public int numberOfBottles;
        public int numberOfColors;

        public override string ToString()
        {
            string difficultyName = difficulty switch
            {
                1 => "Easy",
                2 => "Normal",
                3 => "Hard",
                4 => "Expert",
                5 => "Master",
                _ => "Unknown"
            };

            return $"Level {levelNumber} ({difficultyName})\n" +
                   $"Bottles: {numberOfBottles}, Colors: {numberOfColors}\n" +
                   $"Min Moves: {minimumMoves}, Max Moves (Allowed): {maxMoves}\n" +
                   $"Time Limit: {(timeLimitSeconds > 0 ? timeLimitSeconds + "s" : "None")}";
        }
    }
}
