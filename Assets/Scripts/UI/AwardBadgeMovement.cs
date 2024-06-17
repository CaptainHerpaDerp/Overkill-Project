using System.Collections;
using UnityEngine;

namespace UI
{
    public class AwardBadgeMovement : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float animationTime;

        [SerializeField] private float holdTime = 1.5f;

        [SerializeField] private TMPro.TextMeshProUGUI ValueText;

        public void AnimateToPosition(Vector3 pos)
        {
            StartCoroutine(MoveToTarget(pos));
        }

        public void SetBadgeText(string text)
        {
            if (ValueText == null)
            {
                Debug.LogWarning("ValueText is null. Cannot set badge text value.");
                return;
            }

            ValueText.text = text;
        }


        /// <summary>
        /// Move to the target position over the given animation time
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveToTarget(Vector3 toPosition)
        {
            animator.SetTrigger("Drop");
            yield return new WaitForSeconds(holdTime);
            animator.SetTrigger("Shrink");

            // Set the animator speed to match the animation time
            animator.speed = 1 / animationTime;

            Vector3 startPosition = transform.position;

            float elapsedTime = 0;

            while (elapsedTime < animationTime)
            {
                transform.position = Vector3.Lerp(startPosition, toPosition, elapsedTime / animationTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = toPosition;

            yield break;
        }
    }
}