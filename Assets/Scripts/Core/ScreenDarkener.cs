using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Crore
{
    public class ScreenDarkener : MonoBehaviour
    {
        public static ScreenDarkener Instance { get; private set; }

        [SerializeField] private Image image;

        [Header("The time spend in darkness")]
        [SerializeField] private float darkWaitTime;

        [Header("The speed at which the screen darkens")]
        [SerializeField] private float darkenSpeed;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning("Multiple ScreenDarkener instances found. Destroying the new one.");
            }
        }

        // Action to be invoked when the screen has gone dark
        public Action OnDarkened;

        public void DarkenScreen(float delay = 0) => StartCoroutine(DarkenScreenCoroutine(delay));

        private IEnumerator DarkenScreenCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            Color color = image.color;
            while (color.a < 1)
            {
                float targetAlpha = darkenSpeed * Time.deltaTime;

                if (color.a + targetAlpha > 1)
                {
                    color.a = 1;
                }
                else
                {
                    color.a += targetAlpha;
                }

                image.color = color;

                yield return new WaitForFixedUpdate();
            }

            if (color.a >= 1)
            {
                //print("invoked");
                OnDarkened?.Invoke();
                StartCoroutine(UnDarkenScreenCoroutine());
            }

            yield return null;
        }

        private IEnumerator UnDarkenScreenCoroutine()
        {
            // Clear anyone listening to the OnDarkened event
            OnDarkened = null;

            yield return new WaitForSeconds(darkWaitTime);

            Color color = image.color;

            while (color.a > 0)
            {
                float targetAlpha = darkenSpeed * Time.deltaTime;

                if (color.a - targetAlpha < 0)
                {
                    color.a = 0;
                }
                else
                {
                    color.a -= targetAlpha;
                }

                image.color = color;

                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }
    }
}