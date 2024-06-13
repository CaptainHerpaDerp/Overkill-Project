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
        // Reduce the alpha of the smoke screen over time by the visualDuration

        Color color = meshRenderer.material.color;
        float startAlpha = color.a;

        float time = 0;

        while (time < visualDuration)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(startAlpha, 0, time / visualDuration);

            meshRenderer.material.color = color;

            yield return null;
        }

        if (color.a <= 0)
        {
            Destroy(transform.parent.gameObject);
        }

        yield break;
    }
}
