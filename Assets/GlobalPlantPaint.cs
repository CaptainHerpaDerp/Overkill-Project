using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalPlantPaint : MonoBehaviour
{
    public GameObject plantPrefab;
    public float radius = 5f;
    public int targetCount = 10;

    [SerializeField] private List<Transform> plantSurfaces;

    [SerializeField] private string LayerName;

    [SerializeField] private Vector3 offset;
    [SerializeField] private float randomizationOffset = 0.5f;
    [SerializeField] private float randomizationScale;
    [SerializeField] private float boundaryPadding = 1f;


    [SerializeField] private bool doRandomRotation;

    [SerializeField] Transform plantParent;

    ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = ScoreManager.Instance;
        PlacePlants();
        VerifyPlantPositions();
    }

    public void PlacePlants()
    {
        // Find all surfaces with the "plant" layer
        plantSurfaces = new List<Transform>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PlantSurface"))
        {
            plantSurfaces.Add(obj.transform);
        }

        float spacing = CalculateOptimalSpacing();
        List<Vector3> points = GenerateGridPoints(spacing);

        // Clear the current plants
        ClearPlants();

        foreach (Vector3 point in points)
        {
            // Ensure each point is on the surface
            Ray ray = new Ray(point + Vector3.up * 10, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask(LayerName)))
            {
                Vector3 randomOffset = new(Random.Range(-randomizationOffset, randomizationOffset), 0, Random.Range(-randomizationOffset, randomizationOffset));

                Plant newPlant = Instantiate(plantPrefab, hit.point + offset + randomOffset, Quaternion.identity, parent: plantParent).GetComponent<Plant>();

                if (doRandomRotation)
                {
                    newPlant.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                }

                // Set the plant's default team to "None"
                if (scoreManager != null)
                scoreManager.RegisterPlant(newPlant.PlantID, ColorEnum.TEAMCOLOR.DEFAULT);

                // Randomize the position

            }
        }
    }

    public void ClearPlants()
    {
        while (plantParent.childCount > 0)
        {
            DestroyImmediate(plantParent.GetChild(0).gameObject);
        }
    }

    private float CalculateOptimalSpacing()
    {
        // Calculate the optimal spacing based on radius and target count
        // Assuming a square grid layout
        float area = Mathf.PI * Mathf.Pow(radius, 2);
        float density = targetCount / area;
        return Mathf.Sqrt(1 / density);
    }


    private List<Vector3> GenerateGridPoints(float spacing)
    {
        List<Vector3> points = new List<Vector3>();


        foreach (Transform surface in plantSurfaces)
        {
            Renderer renderer = surface.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Account for padding
                Bounds bounds = renderer.bounds;
                for (float x = bounds.min.x + boundaryPadding; x < bounds.max.x - boundaryPadding; x += spacing)
                {
                    for (float z = bounds.min.z + boundaryPadding; z < bounds.max.z - boundaryPadding; z += spacing)
                    {
                        Vector3 point = new Vector3(x, bounds.center.y, z);
                        points.Add(point);
                    }
                }
            }
        }
        return points;
    }

    /// <summary>
    /// For each plant, verify that it is on the surface using a raycast, if not, destroy it
    /// </summary>
    public void VerifyPlantPositions()
    {
        Ray ray;
        RaycastHit hit;
        for (int i = 0; i < plantParent.childCount; i++)
        {
            ray = new Ray(plantParent.GetChild(i).position + Vector3.up * 10, Vector3.down);
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(LayerName)))
            {
                DestroyImmediate(plantParent.GetChild(i).gameObject);
            }
        }
    }

}
