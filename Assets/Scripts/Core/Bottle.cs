using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WaterSort.Core
{
    /// <summary>
    /// Represents a bottle that can hold water colors
    /// </summary>
    public class Bottle : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int maxCapacity = 4;
        [SerializeField] private Transform waterContainer;
        [SerializeField] private GameObject waterSegmentPrefab;
        
        [Header("Visual")]
        [SerializeField] private SpriteRenderer bottleRenderer;
        [SerializeField] private float waterSegmentHeight = 0.8f;
        
        private List<WaterColor> waterColors = new List<WaterColor>();
        private List<GameObject> waterVisuals = new List<GameObject>();
        private bool isSelected = false;

        public int CurrentAmount => waterColors.Count; // Mỗi WaterColor có amount = 1
        public int MaxCapacity => maxCapacity;
        public bool IsFull => CurrentAmount >= maxCapacity;
        public bool IsEmpty => waterColors.Count == 0;
        public bool IsComplete => IsFull && waterColors.All(w => w.IsSameColor(waterColors[0]));
        public bool IsSelected => isSelected;

        private Vector3 originalLocalPosition;
        
        private void Awake()
        {
            if (waterContainer == null)
            {
                GameObject container = new GameObject("WaterContainer");
                container.transform.SetParent(transform);
                container.transform.localPosition = Vector3.zero;
                waterContainer = container.transform;
            }
            
            // Save original position for animations
            originalLocalPosition = transform.localPosition;
        }

        /// <summary>
        /// Initialize bottle with water colors
        /// </summary>
        public void Initialize(List<WaterColor> colors)
        {
            ClearWater();
            // KHÔNG gộp colors - mỗi segment độc lập với amount = 1
            foreach (var color in colors)
            {
                for (int i = 0; i < color.amount; i++)
                {
                    waterColors.Add(new WaterColor(color.color, 1));
                }
            }
            UpdateVisuals();
        }

        /// <summary>
        /// Get the top water color
        /// </summary>
        public WaterColor GetTopColor()
        {
            if (IsEmpty) return null;
            return waterColors[waterColors.Count - 1];
        }
        
        /// <summary>
        /// Check if can pour water into this bottle
        /// </summary>
        public bool CanReceiveFrom(Bottle other)
        {
            // Không thể đổ nếu:
            // - Source bottle rỗng
            // - Target bottle đầy
            // - Source và target là cùng 1 bottle
            if (other == null || other.IsEmpty || this.IsFull || other == this)
            {
                return false;
            }
            
            // Target có chỗ trống -> có thể đổ
            return true;
        }

        /// <summary>
        /// Pour water from this bottle to target bottle
        /// </summary>
        public int PourTo(Bottle target)
        {
            if (!target.CanReceiveFrom(this)) return 0;

            WaterColor topColor = GetTopColor();
            
            // CHỈ CHUYỂN 1 SEGMENT MỖI LẦN
            int amountToPour = 1;
            int spaceAvailable = target.MaxCapacity - target.CurrentAmount;
            
            // Đảm bảo không vượt quá space available
            if (spaceAvailable < 1) return 0;

            // Remove from source
            RemoveTopAmount(amountToPour);

            // Add to target
            target.AddWater(new WaterColor(topColor.color, amountToPour));

            return amountToPour;
        }

        /// <summary>
        /// Get amount of top color segments
        /// </summary>
        private int GetTopColorAmount()
        {
            if (IsEmpty) return 0;
            
            WaterColor topColor = GetTopColor();
            int amount = topColor.amount;

            // Check if colors below are the same
            for (int i = waterColors.Count - 2; i >= 0; i--)
            {
                if (waterColors[i].IsSameColor(topColor))
                {
                    amount += waterColors[i].amount;
                }
                else
                {
                    break;
                }
            }

            return amount;
        }

        /// <summary>
        /// Remove amount from top
        /// </summary>
        private void RemoveTopAmount(int amount)
        {
            int remaining = amount;

            while (remaining > 0 && !IsEmpty)
            {
                WaterColor topColor = waterColors[waterColors.Count - 1];
                
                if (topColor.amount <= remaining)
                {
                    remaining -= topColor.amount;
                    waterColors.RemoveAt(waterColors.Count - 1);
                }
                else
                {
                    topColor.amount -= remaining;
                    remaining = 0;
                }
            }

            UpdateVisuals();
        }

        /// <summary>
        /// Add water to bottle
        /// </summary>
        private void AddWater(WaterColor newColor)
        {
            // KHÔNG gộp colors - luôn thêm mỗi segment độc lập với amount = 1
            for (int i = 0; i < newColor.amount; i++)
            {
                waterColors.Add(new WaterColor(newColor.color, 1));
            }
            UpdateVisuals();
        }

        /// <summary>
        /// Clear all water
        /// </summary>
        private void ClearWater()
        {
            waterColors.Clear();
            foreach (var visual in waterVisuals)
            {
                if (visual != null)
                    Destroy(visual);
            }
            waterVisuals.Clear();
        }

        /// <summary>
        /// Update water visuals
        /// </summary>
        public void UpdateVisuals()
        {
            // Clear existing visuals
            foreach (var visual in waterVisuals)
            {
                if (visual != null)
                    Destroy(visual);
            }
            waterVisuals.Clear();

            if (waterSegmentPrefab == null)
            {
                Debug.LogWarning($"⚠️ {gameObject.name}: waterSegmentPrefab is NULL!");
                return;
            }

            // Create new visuals
            float yOffset = 0;
            int totalSegments = 0;
            
            foreach (var waterColor in waterColors)
            {
                // Skip if amount is 0 or negative
                if (waterColor.amount <= 0)
                {
                    Debug.LogWarning($"⚠️ {gameObject.name}: WaterColor has invalid amount: {waterColor.amount}");
                    continue;
                }
                
                for (int i = 0; i < waterColor.amount; i++)
                {
                    GameObject segment = Instantiate(waterSegmentPrefab, waterContainer);
                    segment.transform.localPosition = new Vector3(0, yOffset, 0);
                    
                    SpriteRenderer sr = segment.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = waterColor.color;
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ {gameObject.name}: Water segment prefab missing SpriteRenderer!");
                    }
                    
                    waterVisuals.Add(segment);
                    yOffset += waterSegmentHeight;
                    totalSegments++;
                }
            }
            
            // Debug log to verify
            if (totalSegments != CurrentAmount)
            {
                Debug.LogError($"❌ {gameObject.name}: Mismatch! Created {totalSegments} segments but CurrentAmount = {CurrentAmount}");
                Debug.Log($"   WaterColors count: {waterColors.Count}");
                for (int i = 0; i < waterColors.Count; i++)
                {
                    Debug.Log($"   [{i}] Color: {waterColors[i].color}, Amount: {waterColors[i].amount}");
                }
            }
        }

        /// <summary>
        /// Set selected state
        /// </summary>
        public void SetSelected(bool selected, bool animate = true)
        {
            isSelected = selected;
            
            if (bottleRenderer != null)
            {
                bottleRenderer.color = selected ? new Color(1f, 1f, 0.5f) : Color.white;
            }
            
            // Add bounce animation
            StopAllCoroutines();
            
            if (!animate)
            {
                // Reset to original position immediately without animation
                transform.localPosition = originalLocalPosition;
                return;
            }
            
            if (selected)
            {
                StartCoroutine(SelectBounceAnimation());
            }
            else
            {
                StartCoroutine(DeselectAnimation());
            }
        }
        
        private System.Collections.IEnumerator SelectBounceAnimation()
        {
            Vector3 startPos = transform.localPosition;
            Vector3 targetPos = startPos + Vector3.up * 0.2f;
            float duration = 0.15f;
            float elapsed = 0;
            
            // Bounce up
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.localPosition = Vector3.Lerp(startPos, targetPos, Mathf.Sin(t * Mathf.PI * 0.5f));
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.localPosition = targetPos;
        }
        
        private System.Collections.IEnumerator DeselectAnimation()
        {
            Vector3 currentPos = transform.localPosition;
            Vector3 targetPos = originalLocalPosition; // Trở về vị trí ban đầu
            float duration = 0.15f;
            float elapsed = 0;
            
            // Settle down
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.localPosition = Vector3.Lerp(currentPos, targetPos, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.localPosition = targetPos;
        }

        /// <summary>
        /// Get all water colors for saving/undo
        /// </summary>
        public List<WaterColor> GetWaterColors()
        {
            return waterColors.Select(w => w.Clone()).ToList();
        }
    }
}
