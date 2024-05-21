using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private Renderer plantRenderer;

    private int plantOwner;

    public int PlantOwner
    {
        get => plantOwner;
        set
        {
            if (isGhostPlant)
            {
                return;
            }
            else
            {
                plantOwner = value;
            }
        }
    }

    private const float SHIFT_SPEED = 0.1f;

    private bool isGhostPlant;

    private void Start()
    {
        // Set the plant's y position to be 0.1f
        if (!IsOnSurface())
        {
            isGhostPlant = true;
            plantRenderer.enabled = false;
        }
    }

    public bool IsOnSurface()
    {
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f);
    }

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
            plantRenderer.material.color = Color.Lerp(plantRenderer.material.color, newColor, SHIFT_SPEED);
           
            yield return new WaitForFixedUpdate();

            if (plantRenderer.material.color == newColor)
            {
                break;
            }
        }
    }
}
