using Players;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PurpleSpecialBehaviour : SpecialBehaviour
{
    public Transform laserOrigin;
    public float laserRange = 100f;
    public LayerMask collisionMask; // Layer mask for detecting collisions

    private LineRenderer lineRenderer;

    [SerializeField] private float laserTime;
    [SerializeField] private float laserPushback;

    public override void Activate()
    {
        print("purp active");

        if (onCooldown)
        {
            return;
        }

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        StartCoroutine(FireLaserCoroutine());

        DoCooldown();
    }

    private IEnumerator FireLaserCoroutine()
    {
        lineRenderer.enabled = true;

        float timer = 0;
         
        while (timer < laserTime)
        {
            // Set the start point of the laser to the laser origin
            lineRenderer.SetPosition(0, laserOrigin.position);

            // Cast a ray forward from the laser origin
            Ray ray = new Ray(laserOrigin.position, laserOrigin.forward);
            RaycastHit hit;

            // Check if the ray hits a gameObject with a player script

            if (Physics.Raycast(ray, out hit, laserRange, collisionMask))
            {
                // Set the end point of the laser to the point where the ray hit
                lineRenderer.SetPosition(1, hit.point);

                //Get the player component of the object hit, and add a force to it
                hit.collider.GetComponent<Player>().GetComponent<Rigidbody>().AddForce(ray.direction * laserPushback);
            }
            else
            {
                // Set the end point of the laser to the point at the end of the laser range
                lineRenderer.SetPosition(1, ray.GetPoint(laserRange));
            }

            timer += Time.deltaTime;
        }

        yield return new WaitForSeconds(laserTime);

        // Remove the line renderer
        lineRenderer.enabled = false;

        yield return null;
    }
}
