using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public Renderer plantRenderer;

    [SerializeField] private MeshFilter plantMeshFilter;

    [SerializeField] private List<Mesh> plantMeshes = new();

    [SerializeField] private int plantOwner;

    [SerializeField] private Animator plantAnimator;

    private IEnumerator colorShiftRoutine;

    public int PlantID { get; private set; }

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
                plantMeshFilter.mesh = plantMeshes[value - 1];

                // If the value is new, grow the plant (So that the animation doesnt trigger when the player walks on their own plants)
                if (value != plantOwner)
                plantAnimator.SetTrigger("Grow");

                // Update the plant owner in the ScoreManager
                ScoreManager.Instance.UpdatePlantOwnership(PlantID, value);

                plantOwner = value;
            }
        }
    }

    private const float SHIFT_SPEED = 0.1f;

    private bool isGhostPlant;

    private void Awake()
    {
        PlantID = gameObject.GetInstanceID();
    }

    private void Start()
    {
        SetColorInstant(Color.white);
        plantRenderer.enabled = false;
    }

    public bool IsOnSurface()
    {
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f, layerMask: 6);
    }

    public void Activate(Color color, int PlayerNumber)
    {
        plantRenderer.enabled = true;
        PlantOwner = PlayerNumber;
        SetColor(color);
    }

    public void SetColorInstant(Color color)
    {
        plantRenderer.material.color = color;
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
