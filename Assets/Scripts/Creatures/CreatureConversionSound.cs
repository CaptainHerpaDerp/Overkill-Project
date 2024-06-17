using System.Collections;
using UnityEngine;

/// <summary>
/// Able to play a "conversion" sound that pitches over a period of given time, and can be cancelled when needed.
/// </summary>

namespace Creatures
{
    [RequireComponent(typeof(AudioSource))]
    public class CreatureConversionSound : MonoBehaviour
    {
        private AudioSource audioSource;

        [SerializeField] private bool isPlaying = false;
        private float interpolation;

        public void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        [SerializeField] private AudioClip conversionSound;
        [SerializeField] private AudioClip fadeOutSound;

        public void PlaySound(float interpolation)
        {
            this.interpolation = interpolation;

            // Return if already playing
            if (isPlaying) return;
            isPlaying = true;

            audioSource.clip = conversionSound;
            audioSource.Play();
            StartCoroutine(PitchAndSpeedModSource());
        }

        public void StopSound()
        {
            if (!isPlaying) return;
            // fade out the currently playing sound

            audioSource.loop = false;
            audioSource.clip = fadeOutSound;
            audioSource.Play();

            interpolation = 0;
            StopAllCoroutines();
            isPlaying = false;
        }

        /// <summary>
        /// Increase the pitch and speed of the audio source over time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator PitchAndSpeedModSource()
        {
            audioSource.loop = true;
            audioSource.pitch = 1;

            float currentTime = 0;

            while (audioSource.pitch < 2)
            {
                // Lerp the pitch of the audio source from 1 to 2 over the given playTime
                audioSource.pitch = Mathf.Lerp(1, 2, interpolation);

                currentTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the final pitch is exactly 2 after the loop completes
            audioSource.pitch = 2;
        }
    }
}