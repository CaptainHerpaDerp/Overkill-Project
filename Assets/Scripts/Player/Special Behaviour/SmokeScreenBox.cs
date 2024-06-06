using System.Collections;
using UnityEngine;

public class SmokeScreenBox : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float lerpTime;

    private void Start()
    {
        StartCoroutine(ShiftAlpha());
    }

    // Reduce the alpha of the smoke screen over time, destroy it when it reaches 0
    private IEnumerator ShiftAlpha()
    {
        // Lerp the alpha over a period of time
        while (true)
        {
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(color.a, 0, lerpTime * Time.deltaTime);
            meshRenderer.material.color = color;

            if (color.a <= 0.01f)
            {
                Destroy(gameObject);
            }

            yield return null;
        }
    }
}
