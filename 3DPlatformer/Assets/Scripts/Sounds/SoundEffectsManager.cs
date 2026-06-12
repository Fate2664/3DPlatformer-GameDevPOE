using System;
using UnityEngine;

namespace Platformer
{
    public class SoundEffectsManager : MonoBehaviour
    {
        [Serializable]
        private class SoundEffect
        {
            public string key;
            public AudioClip clip;
        }
        
        public static SoundEffectsManager Instance { get; private set; }
        
        [SerializeField] private SoundEffect[] soundEffects;
        
        private HashMapBase<string, AudioClip> soundMap;
        private AudioSource audioSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            soundMap = new HashMapBase<string, AudioClip>();

            foreach (SoundEffect soundEffect in soundEffects)
            {
                if (string.IsNullOrEmpty(soundEffect.key))
                    continue;
                
                soundMap.Insert_Update(soundEffect.key, soundEffect.clip);
            }
        }

        public bool Play(string soundName, float volume = 1f)
        {
            if (soundMap.TryGetValue(soundName, out AudioClip clip))
            {
                audioSource.PlayOneShot(clip, volume);
                return true;
            }
            return false;
        }
    }
}
