using UnityEngine;

public class SharedAudioSource : MonoBehaviour
{
    public static SharedAudioSource Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple SharedAudioSource instances detected. Destroying the new one.");
            Destroy(this);
        }
    }

    public GameObject audioSpot;
    public AudioClip Clip;

}
