using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float throwSpeed;
    public float timeStep = 0.1f;
    public int maxSteps = 100;
    public LayerMask collisionMask; // To detect collision with ground or other objects
    public GameObject landingMarkerPrefab; // Prefab for the landing marker

    private GameObject landingMarker;

    // Function to be called to draw trajectory
    public void DrawTrajectory(float xRotation, Transform yRotationTransform)
    {
        Vector3 initialVelocity = CalculateInitialVelocity(xRotation, yRotationTransform);
        DrawTrajectory(transform.position, initialVelocity);
    }

    Vector3 CalculateInitialVelocity(float xRotation, Transform yRotationTransform)
    {
        // Apply y rotation from the reference transform
        float yRotation = yRotationTransform.eulerAngles.y;
        Vector3 direction = Quaternion.Euler(xRotation, yRotation, 0) * Vector3.forward;
        return direction * throwSpeed;
    }

    void DrawTrajectory(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3[] trajectoryPoints = new Vector3[maxSteps];
        Vector3 position = startPosition;
        Vector3 velocity = initialVelocity;
        lineRenderer.positionCount = maxSteps;

        bool collisionDetected = false;
        Vector3 collisionPoint = Vector3.zero;

        for (int i = 0; i < maxSteps; i++)
        {
            trajectoryPoints[i] = position;

            // Check for collision
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, velocity.magnitude * timeStep, collisionMask))
            {
                // If collision detected, truncate the trajectory and end
                lineRenderer.positionCount = i + 1;
                trajectoryPoints[i] = hit.point;
                collisionDetected = true;
                collisionPoint = hit.point;
                break;
            }

            position += velocity * timeStep;
            velocity += Physics.gravity * timeStep; // Gravity affects the velocity
        }

        lineRenderer.SetPositions(trajectoryPoints);

        // Set the landing marker at the collision point
        if (landingMarker == null && landingMarkerPrefab != null)
        {
            landingMarker = Instantiate(landingMarkerPrefab);
        }

        if (landingMarker != null)
        {
            landingMarker.transform.position = collisionDetected ? collisionPoint : position;
        }
    }
}
