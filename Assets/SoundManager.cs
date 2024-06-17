using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{  
    public static SoundManager Instance;
    private AudioSource audioSource;

    [SerializeField] private float time = 0.1f;
    [SerializeField] private bool isPlaying = false;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            print("SoundManager already exists, destroying this one.");
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    [SerializeField] private AudioClip clockSound;


    private void Update()
    {
        if (isPlaying) // Condition to play sound
        {
            isPlaying = false;
            PlaySound(clockSound);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        // Modify the clip pitch
        audioSource.clip = clip;
        audioSource.Play();
        StartCoroutine(PitchAndSpeedModSource(time));
    }

    /// <summary>
    /// Increase the pitch and speed of the audio source over time.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator PitchAndSpeedModSource(float playTime)  
    {
        audioSource.loop = true;
        audioSource.pitch = 1;

        float currentTime = 0;

        while (currentTime < playTime)
        {
            // Calculate the interpolation factor
            float t = currentTime / playTime;
            // Lerp the pitch of the audio source from 1 to 2 over the given playTime
            audioSource.pitch = Mathf.Lerp(1, 2, t);

            currentTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final pitch is exactly 2 after the loop completes
        audioSource.pitch = 2;
    }
}
