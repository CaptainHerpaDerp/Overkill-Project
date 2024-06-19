using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Multiple SoundManager instances detected. Destroying the new one.");
                Destroy(gameObject);
            }
        }

        [SerializeField] private List<Transform> players = new();
        private Dictionary<AudioSource, float> audioSourceVolumePairs = new();

        private void Start()
        {
            AudioSource[] audioSources;
            audioSources = FindObjectsOfType<AudioSource>();

            foreach (AudioSource source in audioSources)
            {
                // Add the audio source to the dictionary with its initial volume
                audioSourceVolumePairs.Add(source, source.volume);
            }
        }

        private void FixedUpdate()
        {
            foreach (AudioSource source in audioSourceVolumePairs.Keys)
            {
                CheckPlayerDistances(source);
            }
        }

        public void RegisterAudioSource(AudioSource source)
        {
            if (!audioSourceVolumePairs.ContainsKey(source))
            {
                audioSourceVolumePairs.Add(source, source.volume);
            }
        }

        public void AddPlayer(Transform playerTransform)
        {
            players.Add(playerTransform);
        }

        public void ClearPlayers()
        {
            players.Clear();
        }

        private void CheckPlayerDistances(AudioSource source)
        {
            if (players.Count == 0)
                return;

            float minDistance = float.MaxValue;
            Transform closestPlayer = null;

            foreach (var player in players)
            {
                if (player.gameObject == null)
                    continue;

                float distance = Vector3.Distance(player.position, source.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPlayer = player;
                }
            }

            if (closestPlayer != null)
            {
                UpdateAudioForPlayer(source, closestPlayer);
            }
        }

        private void UpdateAudioForPlayer(AudioSource source, Transform player)
        {
            float distance = Vector3.Distance(player.position, source.transform.position);
            float maxDistance = source.maxDistance;

            float targetMaxVolume = audioSourceVolumePairs[source];

            // Adjust the volume based on distance (simple linear falloff)
            float volume = Mathf.Clamp01(1 - distance / maxDistance);
            volume *= targetMaxVolume;

            source.volume = volume;

            // Adjust the spatial blend based on distance
            float spatialBlend = Mathf.Clamp01(distance / maxDistance);
            source.spatialBlend = spatialBlend;
        }
    }
}