using UnityEngine;

namespace WaterSort.Core
{
    /// <summary>
    /// Handles player input and interactions
    /// </summary>
    public class InputController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask bottleLayer;
        [SerializeField] private bool enableTouch = true;
        [SerializeField] private bool enableMouse = true;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleInput();
        }

        /// <summary>
        /// Handle input from mouse or touch
        /// </summary>
        private void HandleInput()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
                return;

            // Mouse input
            if (enableMouse && Input.GetMouseButtonDown(0))
            {
                Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                CheckBottleClick(worldPoint);
            }

            // Touch input
            if (enableTouch && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                
                if (touch.phase == TouchPhase.Began)
                {
                    Vector2 worldPoint = mainCamera.ScreenToWorldPoint(touch.position);
                    CheckBottleClick(worldPoint);
                }
            }
        }

        /// <summary>
        /// Check if a bottle was clicked
        /// </summary>
        private void CheckBottleClick(Vector2 worldPoint)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                Bottle bottle = hit.collider.GetComponent<Bottle>();
                if (bottle != null)
                {
                    GameManager.Instance.OnBottleClicked(bottle);
                }
            }
        }

        /// <summary>
        /// Enable/disable input
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            this.enabled = enabled;
        }
    }
}
