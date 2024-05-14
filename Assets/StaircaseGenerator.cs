using System.Collections.Generic;
using UnityEngine;

public class StaircaseGenerator : MonoBehaviour
{
    [SerializeField] private GameObject stairPrefab;

    [SerializeField] private int height;
    [SerializeField] private float rotation;
    [SerializeField] private float stepHeight;
    [SerializeField] private float outwardOffset;
    [SerializeField] private float stepWidth;


    public void Generate()
    {
        // Delete all children
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject); 
        }

        for (int i = 0; i < height; ++i)
        {
            var stair = Instantiate(stairPrefab, transform);
            stair.transform.position = new Vector3(transform.position.x, i * stepHeight, transform.position.z);
            stair.transform.rotation = Quaternion.Euler(0, rotation * i, 0);

            // Move the stair right by 3 units
            stair.transform.position += stair.transform.right * outwardOffset;

            stair.transform.localScale = new Vector3(stepWidth, stepHeight, 1);
        }
    }
}
