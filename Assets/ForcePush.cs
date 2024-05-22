using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ForcePush : MonoBehaviour
{
    [Header("Maximum possible force")]
    public float force = 10f;

    public ForceMode forceMode = ForceMode.Force;

    [SerializeField] private float pushAngle;

    private float totalForce;

    [SerializeField] private HashSet<Rigidbody> rigidbodies = new();

    // Reference the parent script
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private SurroundingPlant surroundingPlant;

    private void Start()
    {
        if (playerMovement == null)
        {
            playerMovement = GetComponentInParent<PlayerMovement>();
        }

        StartCoroutine(PushOpponent());
        StartCoroutine(UpdateForceValue());
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb;

        if (other.transform == this.transform.parent)
        {
            print("Ignoring parent");
            return;
        }

        if (!other.TryGetComponent(out rb))
            return;

        if (rigidbodies.Contains(rb))
        {
            return;
        }

        rigidbodies.Add(rb);
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb;

        if (!other.TryGetComponent(out rb))
            return;

        if (rigidbodies.Contains(rb))
        {
            rigidbodies.Remove(rb);
        }
    }

    /// <summary>
    /// Updates the force value based on the surrounding plants around the player
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateForceValue()
    {
        while (true)
        {
            float surroundingPlants = surroundingPlant.GetSurroundingPlants();

            yield return new WaitForSeconds(0.33f);

            //Debug.Log("Surrounding Plants: " + surroundingPlants);

            totalForce = force * surroundingPlants;

            yield return new WaitForSeconds(0.33f);
        }
    }

    private IEnumerator PushOpponent()
    {
        while (true)
        {
            // Only apply force if the player is pressing the push button
            if (playerMovement.IsPushing)
                foreach (var item in rigidbodies)
                {
                    Vector3 diff = (item.transform.position - transform.position).normalized;

                    // Calculate the angle between the player and the opponent
                    float angle = Vector3.Angle(transform.forward, diff);

                    // Only apply force if the opponent is within the push angle
                    if (angle < pushAngle)
                    {
                        Debug.Log("Total force: " + totalForce);

                        item.AddForce(diff * (totalForce), forceMode);
                    }
                }

            yield return new WaitForFixedUpdate();
        }
    }
}
