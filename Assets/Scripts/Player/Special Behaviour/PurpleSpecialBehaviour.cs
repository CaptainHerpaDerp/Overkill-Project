

    //private IEnumerator FireLaserCoroutine()
    //{
    //    lineRenderer.enabled = true;

    //    while (true)
    //    {
    //        lineRenderer.SetPosition(0, laserOrigin.position);

    //        if (!parentPlayer.IsSpecial)
    //        {
    //            print("released");

    //            print(laserCapsule.GetAllPlayersInCollider().Count);

    //            TrajectoryCoroutine = null;

    //            lineRenderer.enabled = false;

    //            DoCooldown();

    //            yield break;
    //        }

    //        yield return new WaitForFixedUpdate();
    //    }
    //}



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
    [SerializeField] private LaserCollider laserCapsule;

    [SerializeField] private float laserTime;
    [SerializeField] private float laserPushback;

    [SerializeField] private Player parentPlayer;

    private Coroutine TrajectoryCoroutine;

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

        laserCapsule.capsuleCollider.height = laserRange;
        laserCapsule.capsuleCollider.center = new Vector3(0, laserRange / 2, 0);

        TrajectoryCoroutine ??= StartCoroutine(FireLaserCoroutine());

        // DoCooldown();
    }

    private IEnumerator FireLaserCoroutine()
    {
        lineRenderer.enabled = true;
         
        while (true)
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
            }
            else
            {
                // Set the end point of the laser to the point at the end of the laser range
                lineRenderer.SetPosition(1, ray.GetPoint(laserRange));
            }

            if (!parentPlayer.IsSpecial)
            {
                print("released");

                print(laserCapsule.PlayersInCollider);

                foreach (var player in laserCapsule.PlayersInCollider)
                {
                    player.GetComponent<Rigidbody>().AddForce(ray.direction * laserPushback);
                }

                TrajectoryCoroutine = null;

                lineRenderer.enabled = false;

                DoCooldown();

                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}


