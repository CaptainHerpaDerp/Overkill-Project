using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Players;
using Creatures;

public class ForcePush : MonoBehaviour
{
    [Header("Maximum possible force")]
    public float force = 10f;

    public ForceMode forceMode = ForceMode.Force;

    [SerializeField] private float pushAngle;
    [SerializeField] private float pushDistance;

    [SerializeField] SphereCollider sphereCollider;

    private float totalForce;

    [SerializeField] private HashSet<Rigidbody> rigidbodies = new();

    // Reference the parent script
    [SerializeField] private Player Player;

    [SerializeField] private SurroundingPlant surroundingPlant;

    [SerializeField] private CreatureSelector creatureSelector;

    private void Start()
    {
        if (Player == null)
        {
            Player = GetComponentInParent<Player>();
        }

        if (creatureSelector == null)
        {
            creatureSelector = transform.parent.GetComponentInChildren<CreatureSelector>();
        }

        Player.OnPlayerStart += Initialize;

        StartCoroutine(PushOpponent());
        StartCoroutine(UpdateForceValue());

        // Set the sphere collider radius to the push distance
        sphereCollider.radius = pushDistance;

    }

    private void Initialize()
    {
        surroundingPlant.TeamColour = Player.TeamColor;
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
            if (Player.IsPushing)
            {

                if (creatureSelector.selectedCreature != null)
                creatureSelector.selectedCreature.Convert(Player.TeamColor);

                foreach (var item in rigidbodies)
                {
                    //if (item.TryGetComponent(out CreatureManager creatureManager))
                    //{
                    //    if (creatureSelector == null)
                    //    {
                    //        print("Creature selector is null");
                    //        yield return new WaitForFixedUpdate();
                    //        continue;
                    //    }

                    //    if (creatureManager == creatureSelector.selectedCreature)
                    //        creatureManager.Convert(Player.TeamColor);
                    //}

                    if (!item.TryGetComponent<Player>(out _))
                        continue;

                    Vector3 diff = (item.transform.position - transform.position).normalized;

                    // Calculate the angle between the player and the opponent
                    float angle = Vector3.Angle(transform.forward, diff);

                    // Only apply force if the opponent is within the push angle
                    if (angle < pushAngle)
                    {
                        Debug.Log("Total force: " + totalForce);

                        item.AddForce(diff * (totalForce / Vector3.Distance(transform.position, item.transform.position)), forceMode);
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
