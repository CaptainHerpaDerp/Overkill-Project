using Creatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamColors;
/// <summary>
/// Selects a creature closest to the sphere collider. 
/// Sphere collider's position is raycast onto the player's look position with a maximum distance. 
/// This allow's good accessibility for the player to select a creature.
/// </summary>
public class CreatureSelector : MonoBehaviour
{
    [SerializeField] private Transform selectionPoint;
    [SerializeField] private float maximumRaycastDistance;
    [SerializeField] private float selectionRadius;
    [SerializeField] private int layerMask;

    [SerializeField] private ColorEnum.TEAMCOLOR creatureColor;

    private List<GameObject> selections = new List<GameObject>();
    private CreatureManager selectedCreature;

    private void Update()
    {
        selectedCreature = null;
        selections.Clear();

        SelectedCreature();

        // Get the closest creature to the selection point
        if (selections.Count > 0)
        {
            GameObject closestCreature = selections[0];
            float closestDistance = Vector3.Distance(selectionPoint.position, closestCreature.transform.position);

            foreach (var creature in selections)
            {
                float distance = Vector3.Distance(selectionPoint.position, creature.transform.position);

                if (distance < closestDistance)
                {
                    closestCreature = creature;
                    closestDistance = distance;
                }
            }

            selectedCreature = closestCreature.GetComponent<CreatureManager>();
            selectedCreature.IsHighlighted = true;
        }
    }

    public void SelectedCreature()
    {
        // Raycast from the player's look position with a maximum distance
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maximumRaycastDistance, layerMask: this.layerMask))
        {
            selectionPoint.position = hit.point;
        }
        else
        {
            selectionPoint.position = transform.position + transform.forward * maximumRaycastDistance;
        }

        Collider[] colliders = Physics.OverlapSphere(selectionPoint.position, selectionRadius);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out CreatureManager creature))
            {
                selections.Add(collider.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(selectionPoint.position, selectionRadius);
    }
}

