using System.Collections;
using TMPro;
using UnityEngine;

namespace UIElements
{
    public class FadingText : MonoBehaviour
    {
        [SerializeField] private float fadeSpeed = 1f;

        private Coroutine fadeCoroutine;

        [SerializeField] private TextMeshProUGUI textObject;

        private bool fadeOut = true;

        private void OnEnable()
        {
            fadeCoroutine ??= StartCoroutine(FadeCoroutine());
        }

        private void OnDisable()
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        private IEnumerator FadeCoroutine()
        {
            while (true)
            {
                Color textColor = textObject.color;

                if (fadeOut)
                {
                    textColor.a -= fadeSpeed * Time.deltaTime;
                }
                else
                {
                    textColor.a += fadeSpeed * Time.deltaTime;
                }

                if (textColor.a <= 0)
                {
                    fadeOut = false;
                }
                else if (textColor.a >= 1)
                {
                    fadeOut = true;
                }

                textObject.color = textColor;

                yield return new WaitForFixedUpdate();
            }
        }
    }
}