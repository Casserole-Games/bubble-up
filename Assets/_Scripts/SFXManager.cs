using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets._Scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class SFXManager : SingletonPersistent<SFXManager>
    {
        [Header("Sounds")]
        public AudioClip bubblePopSound;
        public AudioClip bubbleInflatingSound;
        public AudioClip bubbleMergeSound;
        public AudioClip scoreSound;
        public AudioClip winSound;
        public AudioClip pickupSound;
        public AudioClip[] duckSounds;

        private AudioSource _audioSource;
        private AudioClip _lastPlayedDuckSound;

        override protected void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(AudioClip sound, float minPitch, float maxPitch, float volume = 1f)
        {
            if (sound == null) return;

            _audioSource.Stop();
            _audioSource.pitch = Random.Range(minPitch, maxPitch);
            _audioSource.volume = volume;
            _audioSource.PlayOneShot(sound);
        }

        public void StartSound(AudioClip music, float pitch, float volume = 1f)
        {
            if (_audioSource.clip == music && _audioSource.isPlaying) return;

            //audioSource.Stop();
            _audioSource.clip = music;
            _audioSource.loop = false;
            _audioSource.pitch = pitch;
            _audioSource.volume = volume;
            _audioSource.Play();
        }

        public void StopSound()
        {
            _audioSource.Stop();
        }

        public void PlayDuckSound()
        {
            if (duckSounds.Length == 0) return;

            var availableSounds = duckSounds.Where(s => s != _lastPlayedDuckSound).ToArray();
            if (availableSounds.Length == 0)
                availableSounds = duckSounds;

            AudioClip duckSound = availableSounds[Random.Range(0, availableSounds.Length)];
            _lastPlayedDuckSound = duckSound;
            PlaySound(duckSound, 1f, 1f, GameParameters.Instance.DuckVolume);
        }
    }
}
