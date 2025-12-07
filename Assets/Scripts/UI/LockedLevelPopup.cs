using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WaterSort.UI
{
    /// <summary>
    /// Simple popup for locked level messages
    /// </summary>
    public class LockedLevelPopup : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button closeButton;
        
        private void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
            
            Hide();
        }
        
        /// <summary>
        /// Show popup with message
        /// </summary>
        public void Show(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }
            
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }
        
        /// <summary>
        /// Hide popup
        /// </summary>
        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
}
