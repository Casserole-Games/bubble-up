using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets._Scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class SFXManager : SingletonBehaviour<SFXManager>
    {
        [Header("Sounds")]
        public AudioClip bubblePopSound;
        public AudioClip bubbleInflatingSound;
        public AudioClip bubbleMergeSound;
        public AudioClip scoreSound;
        public AudioClip winSound;

        private AudioSource audioSource;

        override protected void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(AudioClip sound, float minPitch, float maxPitch, float volume = 1f)
        {
            if (sound == null) return;

            audioSource.Stop();
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.volume = volume;
            audioSource.PlayOneShot(sound);
        }

        public void StartSound(AudioClip music, float pitch, float volume = 1f)
        {
            if (audioSource.clip == music && audioSource.isPlaying) return;

            //audioSource.Stop();
            audioSource.clip = music;
            audioSource.loop = false;
            audioSource.pitch = pitch;
            audioSource.volume = volume;
            audioSource.Play();
        }

        public void StopSound()
        {
            audioSource.Stop();
        }
    }
}