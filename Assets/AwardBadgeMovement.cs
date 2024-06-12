using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwardBadgeMovement : MonoBehaviour
{
    [SerializeField] private bool doMovement;
    [SerializeField] private Animator animator;

    [SerializeField] private Transform targetPosition;
    [SerializeField] private float animationTime;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (doMovement)
        {
            transform.position = startPosition;
            Animate();
            doMovement = false; 
        }
    }

    public void Animate()
    {
        animator.SetTrigger("Shrink");
        StartCoroutine(MoveToTarget());
    }

    /// <summary>
    /// Move to the target position over the given animation time
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveToTarget()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = this.targetPosition.position;

        float elapsedTime = 0;

        while (elapsedTime < animationTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        yield break;
    }
}
