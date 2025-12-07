using UnityEngine;

namespace WaterSort.Core
{
    /// <summary>
    /// Represents a water color segment in a bottle
    /// </summary>
    [System.Serializable]
    public class WaterColor
    {
        public Color color;
        public int amount;

        public WaterColor(Color color, int amount)
        {
            this.color = color;
            this.amount = amount;
        }

        public WaterColor Clone()
        {
            return new WaterColor(color, amount);
        }

        public bool IsSameColor(WaterColor other)
        {
            return color.Equals(other.color);
        }
    }
}
