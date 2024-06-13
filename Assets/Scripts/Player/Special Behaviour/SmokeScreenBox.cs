using System.Collections;
using UnityEngine;

public class SmokeScreenBox : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    //[SerializeField] private float fadeSpeed;
    [SerializeField] private float visualDuration;

    private void Start()
    {
        StartCoroutine(ShiftAlpha());

        // Randomize parent Y rotation
        transform.parent.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    // Reduce the alpha of the smoke screen over time, destroy it when it reaches 0
    private IEnumerator ShiftAlpha()
    {
        // Lerp the alpha over a period of time

        Color color = meshRenderer.material.color;

        while (color.a > 0)
        {
            // Lerp the alpha over a period of time
            Mathf.Lerp(color.a, 0, visualDuration);
            meshRenderer.material.color = color;

            yield return new WaitForFixedUpdate();
        }

        if (color.a <= 0)
        {
            Destroy(transform.parent.gameObject);
        }

        yield break;
    }
}
