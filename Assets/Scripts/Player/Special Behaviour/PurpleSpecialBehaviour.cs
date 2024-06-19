using Players;
using System.Collections;
using UnityEngine;

public class PurpleSpecialBehaviour : SpecialBehaviour
{
    public Transform laserOrigin;
    public float laserRange = 100f;
    public LayerMask collisionMask; // Layer mask for detecting collisions

    // The two line renderers for the laser, one for aiming and one for firing (holding and releasing trigger)
    [SerializeField] private LineRenderer laserLineRenderer;

    // The particle object at the end of the laser
    [SerializeField] private GameObject laserEndEffect;

    [SerializeField] private LaserCollider laserCapsule;

    [SerializeField] private float laserTime;
    [SerializeField] private float laserPushback;

    // The desired with of the laser at its fullest
    [SerializeField] private float laserWidth;

    [SerializeField] private Player parentPlayer;

    // The two materials for the line renderer, one to be applied when aiming and one when firing
    [SerializeField] private Material aimMat, fireMat;

    [SerializeField] AudioSource audioSource;

    private Coroutine TrajectoryCoroutine;

    public override bool Activate()
    {
        if (onCooldown)
        {
            return false;
        }

        laserCapsule.capsuleCollider.height = laserRange;
        laserCapsule.capsuleCollider.center = new Vector3(0, laserRange / 2, 0);

        TrajectoryCoroutine ??= StartCoroutine(FireLaserCoroutine());

        return true;
    }

    private IEnumerator FireLaserCoroutine()
    {
        laserLineRenderer.enabled = true;

        // Apply the aiming material to the line renderer
        laserLineRenderer.material = aimMat;

        while (true)
        {
            // Set the start point of the laser to the laser origin
            laserLineRenderer.SetPosition(0, laserOrigin.position);

            // Cast a ray forward from the laser origin
            Ray ray = new Ray(laserOrigin.position, laserOrigin.forward);
            RaycastHit hit;

            // Check if the ray hits a gameObject with a player script
            if (Physics.Raycast(ray, out hit, laserRange, collisionMask))
            {
                // Set the end point of the laser to the point where the ray hit
                laserLineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                // Set the end point of the laser to the point at the end of the laser range
                laserLineRenderer.SetPosition(1, ray.GetPoint(laserRange));
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

                audioSource.Play();

                // Apply the firing material to the line renderer
                laserLineRenderer.material = fireMat;

                // Show the laser for a short time
                StartCoroutine(ShowFireLaser(laserTime));

                // Do the cooldown
                DoCooldown();

                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    // Temporarily show the laser when the player is holding the trigger
    private IEnumerator ShowFireLaser(float showTime)
    {
        float time = 0;
        // Set the laser's width to 0 to start
        laserLineRenderer.startWidth = 0;
        laserLineRenderer.endWidth = 0;
        float width = 0;

        // Half the time so we have half the time to show the laser and half the time to hide it
        float startTime = showTime / 3;
        float endTime = showTime - startTime;


        // First, increase the width of the laser to show it
        while (time < startTime)
        {
            float t = time / startTime;

            width = Mathf.Lerp(0, laserWidth, t);
            laserLineRenderer.startWidth = width;
            laserLineRenderer.endWidth = width;

            time += Time.deltaTime;
            yield return null;
        }

        // Reset the time
        time = 0;

        // Then, decrease the width of the laser to hide it
        while (time < endTime)
        {
            float t = time / endTime;

            width = Mathf.Lerp(laserWidth, 0, t);
            laserLineRenderer.startWidth = width;
            laserLineRenderer.endWidth = width;

            time += Time.deltaTime;
            yield return null;
        }

        // Hide the line
        laserLineRenderer.enabled = false;

        // Reset the width of the laser
        laserLineRenderer.startWidth = 1;

        yield break;
    }
}


