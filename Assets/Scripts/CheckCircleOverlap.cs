using UnityEngine;

public class CircleOverlap : MonoBehaviour
{
    [SerializeField] private SphereCollider sphereCollider1, sphereCollider2;

    private void Update()
    {
        // Distance between centers
        float distance = Vector3.Distance(sphereCollider1.transform.position, sphereCollider2.transform.position);

        float radius1 = sphereCollider1.radius;
        float radius2 = sphereCollider2.radius;

        // Check if circles overlap
        if (distance < radius1 + radius2)
        {
            // Calculate overlapping area
            float d = Mathf.Min(radius1 + radius2 - distance, Mathf.Max(distance - Mathf.Abs(radius1 - radius2), 0));
            float overlappingArea = Mathf.PI * (Mathf.Pow(radius1, 2) - Mathf.Pow(radius1 - d, 2)); // Area of circular segment
            Debug.Log("Overlapping Area: " + overlappingArea);
        }
        else
        {
            Debug.Log("No overlap");
        }
    }
}
