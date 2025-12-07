using UnityEngine;
using System.Collections;

namespace WaterSort.Effects
{
    /// <summary>
    /// Water visual effects and liquid simulation
    /// </summary>
    public class WaterEffect : MonoBehaviour
    {
        [Header("Water Settings")]
        [SerializeField] private SpriteRenderer waterRenderer;
        [SerializeField] private float waveSpeed = 2f;
        [SerializeField] private float waveAmplitude = 0.05f;
        [SerializeField] private AnimationCurve waveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Flow Effect")]
        [SerializeField] private bool animateFlow = true;
        [SerializeField] private float flowSpeed = 1f;

        private Material waterMaterial;
        private Color currentColor;
        private float time;

        private void Awake()
        {
            if (waterRenderer != null)
            {
                waterMaterial = waterRenderer.material;
            }
        }

        private void Update()
        {
            if (animateFlow && waterMaterial != null)
            {
                time += Time.deltaTime * flowSpeed;
                
                // Simple wave effect using shader or transform
                float wave = Mathf.Sin(time * waveSpeed) * waveAmplitude;
                transform.localScale = new Vector3(1f, 1f + wave, 1f);
            }
        }

        /// <summary>
        /// Set water color with smooth transition
        /// </summary>
        public void SetColor(Color color, float duration = 0.3f)
        {
            if (duration <= 0)
            {
                currentColor = color;
                if (waterRenderer != null)
                {
                    waterRenderer.color = color;
                }
            }
            else
            {
                StartCoroutine(TransitionColor(color, duration));
            }
        }

        private IEnumerator TransitionColor(Color targetColor, float duration)
        {
            Color startColor = currentColor;
            float elapsed = 0;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                Color newColor = Color.Lerp(startColor, targetColor, t);
                
                if (waterRenderer != null)
                {
                    waterRenderer.color = newColor;
                }
                
                currentColor = newColor;
                elapsed += Time.deltaTime;
                yield return null;
            }

            currentColor = targetColor;
            if (waterRenderer != null)
            {
                waterRenderer.color = targetColor;
            }
        }

        /// <summary>
        /// Play filling animation
        /// </summary>
        public IEnumerator PlayFillAnimation(float targetHeight, float duration)
        {
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = new Vector3(startScale.x, targetHeight, startScale.z);
            
            float elapsed = 0;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, targetScale, waveCurve.Evaluate(t));
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = targetScale;
        }

        /// <summary>
        /// Play draining animation
        /// </summary>
        public IEnumerator PlayDrainAnimation(float targetHeight, float duration)
        {
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = new Vector3(startScale.x, targetHeight, startScale.z);
            
            float elapsed = 0;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = targetScale;
        }

        /// <summary>
        /// Play splash effect
        /// </summary>
        public void PlaySplash()
        {
            StartCoroutine(SplashAnimation());
        }

        private IEnumerator SplashAnimation()
        {
            float elapsed = 0;
            float duration = 0.3f;
            Vector3 originalScale = transform.localScale;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.2f;
                transform.localScale = new Vector3(originalScale.x * scale, originalScale.y, originalScale.z);
                
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = originalScale;
        }
    }
}
