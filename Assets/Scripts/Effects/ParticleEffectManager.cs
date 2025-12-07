using UnityEngine;

namespace WaterSort.Effects
{
    /// <summary>
    /// Particle effect manager for game events
    /// </summary>
    public class ParticleEffectManager : MonoBehaviour
    {
        public static ParticleEffectManager Instance { get; private set; }

        [Header("Particle Prefabs")]
        [SerializeField] private ParticleSystem splashParticle;
        [SerializeField] private ParticleSystem celebrationParticle;
        [SerializeField] private ParticleSystem sparkleParticle;
        [SerializeField] private ParticleSystem errorParticle;

        [Header("Settings")]
        [SerializeField] private float particleLifetime = 2f;

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
        /// Play splash effect at position
        /// </summary>
        public void PlaySplash(Vector3 position, Color color)
        {
            if (splashParticle != null)
            {
                ParticleSystem ps = Instantiate(splashParticle, position, Quaternion.identity);
                var main = ps.main;
                main.startColor = color;
                Destroy(ps.gameObject, particleLifetime);
            }
        }

        /// <summary>
        /// Play celebration effect
        /// </summary>
        public void PlayCelebration(Vector3 position)
        {
            if (celebrationParticle != null)
            {
                ParticleSystem ps = Instantiate(celebrationParticle, position, Quaternion.identity);
                Destroy(ps.gameObject, particleLifetime);
            }
        }

        /// <summary>
        /// Play sparkle effect
        /// </summary>
        public void PlaySparkle(Vector3 position)
        {
            if (sparkleParticle != null)
            {
                ParticleSystem ps = Instantiate(sparkleParticle, position, Quaternion.identity);
                Destroy(ps.gameObject, particleLifetime);
            }
        }

        /// <summary>
        /// Play error effect
        /// </summary>
        public void PlayError(Vector3 position)
        {
            if (errorParticle != null)
            {
                ParticleSystem ps = Instantiate(errorParticle, position, Quaternion.identity);
                Destroy(ps.gameObject, particleLifetime);
            }
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
