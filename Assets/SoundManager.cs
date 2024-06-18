using UnityEngine;

namespace Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        private AudioSource audioSource;

        [SerializeField] private AudioClip greenSphereSound;

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

        public void GreenExpandSound(Vector3 pos)
        {
            AudioSource.PlayClipAtPoint(greenSphereSound, pos);
        }
    }
}