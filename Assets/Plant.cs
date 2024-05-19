using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private Renderer renderer;

    private const float SHIFT_SPEED = 0.1f;

    public void SetColor(Color color)
    {
        StopAllCoroutines();
        StartCoroutine(ShiftColor(color));
    }

    /// <summary>
    /// Change the color of the plant over time
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShiftColor(Color newColor)
    {
        while (true)
        {
            // lerp the color of the plant
            renderer.material.color = Color.Lerp(renderer.material.color, newColor, SHIFT_SPEED);
           
            yield return new WaitForFixedUpdate();

            if (renderer.material.color == newColor)
            {
                break;
            }
        }
    }
}
