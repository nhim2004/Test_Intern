using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using WaterSort.Core;

namespace WaterSort.UI
{
    /// <summary>
    /// Hint system to help players
    /// </summary>
    public class HintSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button hintButton;
        [SerializeField] private int maxHints = 3;
        [SerializeField] private float hintCooldown = 5f;

        private int hintsRemaining;
        private bool isOnCooldown;

        private void Start()
        {
            hintsRemaining = maxHints;
            
            if (hintButton != null)
            {
                hintButton.onClick.AddListener(OnHintRequested);
                UpdateHintButton();
            }
        }

        /// <summary>
        /// Request a hint
        /// </summary>
        private void OnHintRequested()
        {
            if (isOnCooldown || hintsRemaining <= 0)
            {
                AudioManager.Instance?.PlaySound("error");
                return;
            }

            Bottle[] bottles = FindObjectsOfType<Bottle>();
            var validMove = FindValidMove(bottles);

            if (validMove.source != null && validMove.target != null)
            {
                // Highlight the bottles
                StartCoroutine(ShowHint(validMove.source, validMove.target));
                
                hintsRemaining--;
                UpdateHintButton();
                StartCoroutine(HintCooldown());
                
                AudioManager.Instance?.PlaySound("button");
            }
            else
            {
                Debug.Log("No valid moves found!");
                AudioManager.Instance?.PlaySound("error");
            }
        }

        /// <summary>
        /// Find a valid move
        /// </summary>
        private (Bottle source, Bottle target) FindValidMove(Bottle[] bottles)
        {
            // Try to find beneficial moves
            foreach (var source in bottles)
            {
                if (source.IsEmpty) continue;

                foreach (var target in bottles)
                {
                    if (source == target) continue;

                    if (target.CanReceiveFrom(source))
                    {
                        // Prefer moves that complete bottles or move to empty bottles
                        if (target.IsEmpty || WouldImproveTarget(source, target))
                        {
                            return (source, target);
                        }
                    }
                }
            }

            // If no beneficial move found, just find any valid move
            foreach (var source in bottles)
            {
                if (source.IsEmpty) continue;

                foreach (var target in bottles)
                {
                    if (source == target) continue;

                    if (target.CanReceiveFrom(source))
                    {
                        return (source, target);
                    }
                }
            }

            return (null, null);
        }

        /// <summary>
        /// Check if move would improve target bottle
        /// </summary>
        private bool WouldImproveTarget(Bottle source, Bottle target)
        {
            if (target.IsEmpty) return true;

            var targetColor = target.GetTopColor();
            var sourceColor = source.GetTopColor();

            return targetColor.IsSameColor(sourceColor);
        }

        /// <summary>
        /// Show hint animation
        /// </summary>
        private IEnumerator ShowHint(Bottle source, Bottle target)
        {
            // Highlight source and target
            for (int i = 0; i < 3; i++)
            {
                var sourceAnimator = source.GetComponent<WaterSort.Effects.BottleAnimator>();
                var targetAnimator = target.GetComponent<WaterSort.Effects.BottleAnimator>();

                if (sourceAnimator != null)
                    StartCoroutine(sourceAnimator.PulseAnimation());
                
                if (targetAnimator != null)
                    StartCoroutine(targetAnimator.PulseAnimation());

                yield return new WaitForSeconds(0.6f);
            }
        }

        /// <summary>
        /// Hint cooldown
        /// </summary>
        private IEnumerator HintCooldown()
        {
            isOnCooldown = true;
            yield return new WaitForSeconds(hintCooldown);
            isOnCooldown = false;
            UpdateHintButton();
        }

        /// <summary>
        /// Update hint button state
        /// </summary>
        private void UpdateHintButton()
        {
            if (hintButton != null)
            {
                hintButton.interactable = !isOnCooldown && hintsRemaining > 0;
                
                var buttonText = hintButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = $"Hint ({hintsRemaining})";
                }
            }
        }

        /// <summary>
        /// Reset hints for new level
        /// </summary>
        public void ResetHints()
        {
            hintsRemaining = maxHints;
            isOnCooldown = false;
            UpdateHintButton();
        }
    }
}
