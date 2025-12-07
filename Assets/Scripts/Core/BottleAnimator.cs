using UnityEngine;
using System.Collections;

namespace WaterSort.Effects
{
    /// <summary>
    /// Handles bottle animations and visual effects
    /// </summary>
    public class BottleAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float selectBounceHeight = 0.3f;
        [SerializeField] private float selectBounceDuration = 0.2f;
        [SerializeField] private float pourTiltAngle = 45f;
        [SerializeField] private float pourDuration = 0.5f;
        [SerializeField] private AnimationCurve bounceEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem splashEffect;
        [SerializeField] private GameObject glowEffect;
        [SerializeField] private SpriteRenderer bottleRenderer;

        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Coroutine currentAnimation;

        private void Awake()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
        }

        /// <summary>
        /// Play select animation
        /// </summary>
        public void PlaySelectAnimation()
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            
            currentAnimation = StartCoroutine(SelectBounceAnimation());
        }

        /// <summary>
        /// Play deselect animation
        /// </summary>
        public void PlayDeselectAnimation()
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            
            currentAnimation = StartCoroutine(DeselectAnimation());
        }

        /// <summary>
        /// Play pour animation
        /// </summary>
        public IEnumerator PlayPourAnimation(Transform target)
        {
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            // Move up
            float elapsed = 0;
            float liftHeight = 1f;
            
            while (elapsed < pourDuration * 0.3f)
            {
                float t = elapsed / (pourDuration * 0.3f);
                transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * liftHeight, bounceEase.Evaluate(t));
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Move towards target and tilt
            Vector3 liftedPos = startPos + Vector3.up * liftHeight;
            Vector3 targetPos = target.position + Vector3.up * liftHeight + Vector3.right * 0.5f;
            
            elapsed = 0;
            while (elapsed < pourDuration * 0.4f)
            {
                float t = elapsed / (pourDuration * 0.4f);
                transform.position = Vector3.Lerp(liftedPos, targetPos, t);
                transform.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(0, 0, pourTiltAngle), t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Pour effect
            if (splashEffect != null)
            {
                ParticleSystem splash = Instantiate(splashEffect, target.position + Vector3.up * 0.5f, Quaternion.identity);
                Destroy(splash.gameObject, 2f);
            }

            yield return new WaitForSeconds(0.3f);

            // Return to original position
            elapsed = 0;
            while (elapsed < pourDuration * 0.3f)
            {
                float t = elapsed / (pourDuration * 0.3f);
                transform.position = Vector3.Lerp(targetPos, startPos, bounceEase.Evaluate(t));
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, pourTiltAngle), startRot, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = startPos;
            transform.rotation = startRot;
        }

        /// <summary>
        /// Play complete animation (when bottle is full of single color)
        /// </summary>
        public void PlayCompleteAnimation()
        {
            StartCoroutine(CompleteAnimation());
        }

        private IEnumerator SelectBounceAnimation()
        {
            float elapsed = 0;
            Vector3 startPos = transform.localPosition;
            Vector3 targetPos = originalPosition + Vector3.up * selectBounceHeight;

            // Bounce up
            while (elapsed < selectBounceDuration)
            {
                float t = elapsed / selectBounceDuration;
                transform.localPosition = Vector3.Lerp(startPos, targetPos, bounceEase.Evaluate(t));
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = targetPos;

            // Enable glow
            if (glowEffect != null)
                glowEffect.SetActive(true);
        }

        private IEnumerator DeselectAnimation()
        {
            float elapsed = 0;
            Vector3 startPos = transform.localPosition;

            // Disable glow
            if (glowEffect != null)
                glowEffect.SetActive(false);

            // Bounce down
            while (elapsed < selectBounceDuration)
            {
                float t = elapsed / selectBounceDuration;
                transform.localPosition = Vector3.Lerp(startPos, originalPosition, bounceEase.Evaluate(t));
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
        }

        private IEnumerator CompleteAnimation()
        {
            // Celebration animation
            float elapsed = 0;
            float duration = 0.5f;
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = startScale * 1.2f;

            // Scale up
            while (elapsed < duration * 0.5f)
            {
                float t = elapsed / (duration * 0.5f);
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Scale back
            elapsed = 0;
            while (elapsed < duration * 0.5f)
            {
                float t = elapsed / (duration * 0.5f);
                transform.localScale = Vector3.Lerp(targetScale, startScale, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = startScale;

            // Spawn particles
            if (splashEffect != null)
            {
                ParticleSystem particles = Instantiate(splashEffect, transform.position, Quaternion.identity);
                Destroy(particles.gameObject, 2f);
            }
        }

        /// <summary>
        /// Shake animation for invalid move
        /// </summary>
        public void PlayShakeAnimation()
        {
            StartCoroutine(ShakeAnimation());
        }

        private IEnumerator ShakeAnimation()
        {
            float elapsed = 0;
            float duration = 0.3f;
            float magnitude = 0.1f;

            Vector3 startPos = transform.localPosition;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                transform.localPosition = startPos + new Vector3(x, 0, 0);
                
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = startPos;
        }

        /// <summary>
        /// Pulse animation for hint
        /// </summary>
        public IEnumerator PulseAnimation()
        {
            float elapsed = 0;
            float duration = 0.5f;
            Color originalColor = bottleRenderer != null ? bottleRenderer.color : Color.white;
            Color highlightColor = Color.yellow;

            while (elapsed < duration)
            {
                float t = Mathf.PingPong(elapsed / duration, 1f);
                if (bottleRenderer != null)
                {
                    bottleRenderer.color = Color.Lerp(originalColor, highlightColor, t);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (bottleRenderer != null)
            {
                bottleRenderer.color = originalColor;
            }
        }
    }
}
