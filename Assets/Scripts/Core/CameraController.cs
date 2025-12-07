using UnityEngine;

namespace WaterSort.Core
{
    /// <summary>
    /// Camera controller for the game
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float targetOrthographicSize = 6f;
        [SerializeField] private Vector3 targetPosition = new Vector3(0, 0, -10);
        [SerializeField] private bool autoAdjust = true;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Start()
        {
            if (autoAdjust)
            {
                AdjustCamera();
            }
        }

        /// <summary>
        /// Adjust camera to fit all bottles
        /// </summary>
        public void AdjustCamera()
        {
            if (mainCamera == null) return;

            Bottle[] bottles = FindObjectsOfType<Bottle>();
            
            if (bottles.Length == 0) return;

            // Calculate bounds
            Bounds bounds = new Bounds(bottles[0].transform.position, Vector3.zero);
            foreach (var bottle in bottles)
            {
                bounds.Encapsulate(bottle.transform.position);
            }

            // Set camera position
            Vector3 center = bounds.center;
            transform.position = new Vector3(center.x, center.y, targetPosition.z);

            // Adjust orthographic size
            float verticalSize = bounds.size.y / 2f + 1f;
            float horizontalSize = bounds.size.x / (2f * mainCamera.aspect) + 1f;
            
            mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize, targetOrthographicSize);
        }

        /// <summary>
        /// Shake camera effect
        /// </summary>
        public void Shake(float duration = 0.3f, float magnitude = 0.1f)
        {
            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }

        private System.Collections.IEnumerator ShakeCoroutine(float duration, float magnitude)
        {
            Vector3 originalPosition = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = originalPosition + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
        }
    }
}
