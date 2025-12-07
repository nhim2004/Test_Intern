using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace WaterSort.Utilities
{
    /// <summary>
    /// Simple tween utility without external dependencies
    /// Can be replaced with DOTween for better performance
    /// </summary>
    public class SimpleTween : MonoBehaviour
    {
        public static SimpleTween Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Tween position
        /// </summary>
        public Coroutine TweenPosition(Transform target, Vector3 to, float duration, AnimationCurve ease = null, UnityAction onComplete = null)
        {
            return StartCoroutine(TweenPositionCoroutine(target, to, duration, ease, onComplete));
        }

        private IEnumerator TweenPositionCoroutine(Transform target, Vector3 to, float duration, AnimationCurve ease, UnityAction onComplete)
        {
            Vector3 from = target.position;
            float elapsed = 0;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                if (ease != null)
                    t = ease.Evaluate(t);

                target.position = Vector3.Lerp(from, to, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.position = to;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Tween local position
        /// </summary>
        public Coroutine TweenLocalPosition(Transform target, Vector3 to, float duration, AnimationCurve ease = null, UnityAction onComplete = null)
        {
            return StartCoroutine(TweenLocalPositionCoroutine(target, to, duration, ease, onComplete));
        }

        private IEnumerator TweenLocalPositionCoroutine(Transform target, Vector3 to, float duration, AnimationCurve ease, UnityAction onComplete)
        {
            Vector3 from = target.localPosition;
            float elapsed = 0;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                if (ease != null)
                    t = ease.Evaluate(t);

                target.localPosition = Vector3.Lerp(from, to, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.localPosition = to;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Tween scale
        /// </summary>
        public Coroutine TweenScale(Transform target, Vector3 to, float duration, AnimationCurve ease = null, UnityAction onComplete = null)
        {
            return StartCoroutine(TweenScaleCoroutine(target, to, duration, ease, onComplete));
        }

        private IEnumerator TweenScaleCoroutine(Transform target, Vector3 to, float duration, AnimationCurve ease, UnityAction onComplete)
        {
            Vector3 from = target.localScale;
            float elapsed = 0;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                if (ease != null)
                    t = ease.Evaluate(t);

                target.localScale = Vector3.Lerp(from, to, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.localScale = to;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Tween rotation
        /// </summary>
        public Coroutine TweenRotation(Transform target, Quaternion to, float duration, AnimationCurve ease = null, UnityAction onComplete = null)
        {
            return StartCoroutine(TweenRotationCoroutine(target, to, duration, ease, onComplete));
        }

        private IEnumerator TweenRotationCoroutine(Transform target, Quaternion to, float duration, AnimationCurve ease, UnityAction onComplete)
        {
            Quaternion from = target.rotation;
            float elapsed = 0;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                if (ease != null)
                    t = ease.Evaluate(t);

                target.rotation = Quaternion.Lerp(from, to, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.rotation = to;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Tween color
        /// </summary>
        public Coroutine TweenColor(SpriteRenderer target, Color to, float duration, AnimationCurve ease = null, UnityAction onComplete = null)
        {
            return StartCoroutine(TweenColorCoroutine(target, to, duration, ease, onComplete));
        }

        private IEnumerator TweenColorCoroutine(SpriteRenderer target, Color to, float duration, AnimationCurve ease, UnityAction onComplete)
        {
            Color from = target.color;
            float elapsed = 0;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                if (ease != null)
                    t = ease.Evaluate(t);

                target.color = Color.Lerp(from, to, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.color = to;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Punch animation (like a bounce)
        /// </summary>
        public Coroutine Punch(Transform target, Vector3 punch, float duration, UnityAction onComplete = null)
        {
            return StartCoroutine(PunchCoroutine(target, punch, duration, onComplete));
        }

        private IEnumerator PunchCoroutine(Transform target, Vector3 punch, float duration, UnityAction onComplete)
        {
            Vector3 originalScale = target.localScale;
            float elapsed = 0;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float strength = Mathf.Sin(t * Mathf.PI);
                target.localScale = originalScale + punch * strength;
                
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.localScale = originalScale;
            onComplete?.Invoke();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
