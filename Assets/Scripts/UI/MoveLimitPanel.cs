using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WaterSort.Core;

namespace WaterSort.UI
{
    /// <summary>
    /// Move Limit Panel - displays remaining moves with visual feedback
    /// </summary>
    public class MoveLimitPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI movesRemainingText;
        [SerializeField] private TextMeshProUGUI maxMovesText;
        [SerializeField] private Image movesBarFill;
        [SerializeField] private CanvasGroup canvasGroup;

        private int maxMoves = 0;
        private Color normalColor = Color.white;
        private Color warningColor = Color.yellow;
        private Color criticalColor = Color.red;

        private void Start()
        {
            // Initialize canvas group if not assigned
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            HidePanel();
        }

        /// <summary>
        /// Initialize panel with move limit
        /// </summary>
        public void Initialize(int maxMovesLimit)
        {
            maxMoves = maxMovesLimit;
            maxMovesText.text = $"/ {maxMoves}";
            ShowPanel();
        }

        /// <summary>
        /// Update display with current moves remaining
        /// </summary>
        public void UpdateMoves(int currentMoves)
        {
            if (maxMoves <= 0) return;

            int remaining = maxMoves - currentMoves;
            movesRemainingText.text = remaining.ToString();

            // Update progress bar
            if (movesBarFill != null)
            {
                float fillAmount = Mathf.Max(0, (float)remaining / maxMoves);
                movesBarFill.fillAmount = fillAmount;
            }

            // Update color based on remaining moves
            Color textColor = normalColor;
            float fontSize = 60f;

            if (remaining <= 0)
            {
                textColor = criticalColor;
                fontSize = 70f;
            }
            else if (remaining <= 3)
            {
                textColor = criticalColor;
                fontSize = 68f;
            }
            else if (remaining <= 5)
            {
                textColor = warningColor;
                fontSize = 65f;
            }
            else if (remaining <= 10)
            {
                textColor = warningColor;
                fontSize = 62f;
            }

            movesRemainingText.color = textColor;
            movesRemainingText.fontSize = fontSize;
        }

        /// <summary>
        /// Show the panel
        /// </summary>
        public void ShowPanel()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(true);
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        /// <summary>
        /// Hide the panel
        /// </summary>
        public void HidePanel()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
        }
    }
}
