using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    /// <summary>
    /// Audio Manager using Singleton pattern
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Sound Clips")]
        [SerializeField] private AudioClip selectSound;
        [SerializeField] private AudioClip pourSound;
        [SerializeField] private AudioClip errorSound;
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip undoSound;
        [SerializeField] private AudioClip resetSound;
        [SerializeField] private AudioClip buttonClickSound;

        [Header("Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;

        private Dictionary<string, AudioClip> soundDictionary;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeSoundDictionary();
            SetupAudioSources();
        }

        private void InitializeSoundDictionary()
        {
            soundDictionary = new Dictionary<string, AudioClip>
            {
                { "select", selectSound },
                { "pour", pourSound },
                { "error", errorSound },
                { "win", winSound },
                { "undo", undoSound },
                { "reset", resetSound },
                { "button", buttonClickSound }
            };
        }

        private void SetupAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = true;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            UpdateVolumes();
        }

        /// <summary>
        /// Play a sound effect by name
        /// </summary>
        public void PlaySound(string soundName)
        {
            if (soundDictionary.TryGetValue(soundName, out AudioClip clip))
            {
                if (clip != null && sfxSource != null)
                {
                    sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
                }
            }
            else
            {
                Debug.LogWarning($"Sound '{soundName}' not found!");
            }
        }

        /// <summary>
        /// Play music
        /// </summary>
        public void PlayMusic(AudioClip music)
        {
            if (musicSource != null && music != null)
            {
                musicSource.clip = music;
                musicSource.volume = musicVolume * masterVolume;
                musicSource.Play();
            }
        }

        /// <summary>
        /// Stop music
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        /// <summary>
        /// Set master volume
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Set music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Set SFX volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }

        private void UpdateVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
        }

        /// <summary>
        /// Mute/Unmute all audio
        /// </summary>
        public void SetMute(bool mute)
        {
            AudioListener.volume = mute ? 0 : 1;
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
