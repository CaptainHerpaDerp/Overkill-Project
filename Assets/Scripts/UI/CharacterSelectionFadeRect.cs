using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class CharacterSelectionFadeRect : MonoBehaviour
    {
        [SerializeField] private Image rect;

        [SerializeField] private float fadeSpeed;
        [SerializeField] private float targetDarkness;

        public void FadeInRect()
        {
            StopAllCoroutines();
            StartCoroutine(FadeInRectCR());
        }
        public void FadeOutRect()
        {
            StopAllCoroutines();    
            StartCoroutine(FadeOutRectCR());
        }
        
        public void ClearDarkness()
        {
            rect.color = new Color(rect.color.r, rect.color.g, rect.color.b, 0);
        }

        private IEnumerator FadeInRectCR()
        {
            rect.color = new Color(rect.color.r, rect.color.g, rect.color.b, 0);

            while (rect.color.a < targetDarkness)
            {
                Color color = rect.color;

                float fadeVar = fadeSpeed * Time.deltaTime;

                if (color.a + fadeVar > targetDarkness)
                {
                    color.a = targetDarkness;
                    rect.color = color;
                    yield break;
                }

                color.a += fadeSpeed * Time.deltaTime;
                rect.color = color;

                yield return new WaitForFixedUpdate();
            }

            rect.color = new Color(rect.color.r, rect.color.g, rect.color.b, targetDarkness);

            yield return null;
        }

        private IEnumerator FadeOutRectCR()
        {
            rect.color = new Color(rect.color.r, rect.color.g, rect.color.b, targetDarkness);

            while (rect.color.a > 0)
            {
                Color color = rect.color;

                float fadeVar = fadeSpeed * Time.deltaTime;

                if (color.a - fadeVar < 0)
                {
                    color.a = 0;
                    rect.color = color;
                    yield break;
                }

                color.a -= fadeSpeed * Time.deltaTime;
                rect.color = color; 

                yield return new WaitForFixedUpdate();
            }

            rect.color = new Color(rect.color.r, rect.color.g, rect.color.b, 0);

            yield return null;
        }
    }
}