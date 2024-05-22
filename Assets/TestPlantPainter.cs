using System.Collections;
using UnityEngine;

public class TestPlantPainter : MonoBehaviour
{
    [SerializeField] private Plant plantPrefab;

    private Plant plantInstance;

    [SerializeField] private Vector3 offset;

    [SerializeField] private int layerMask;

    private void Start()
    {
        plantInstance = Instantiate(plantPrefab, transform.position, Quaternion.identity);
        StartCoroutine(SpawnPlantsBelow());
    }

    /// <summary>
    /// Spawn plants on the surface of the terrain directly below the gameobject 
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnPlantsBelow()
    {
        while (true)
        {
            RaycastHit hit;

            Physics.Raycast(transform.position, Vector3.down, out hit, 20f, layerMask: this.layerMask);

            print(hit.point);

            plantInstance.transform.position = hit.point + offset;

            yield return new WaitForSeconds(0.1f);
        }
    }
}
