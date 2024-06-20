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
namespace Players
{
    public class CreatureSelector : MonoBehaviour
    {
        [SerializeField] private Transform selectionPoint;
        [SerializeField] private float maximumRaycastDistance;
        [SerializeField] private float selectionRadius;
        [SerializeField] private int layerMask;

        [SerializeField] private ColorEnum.TEAMCOLOR creatureColor;

        private List<GameObject> selections = new();
        public CreatureManager selectedCreature;    

        // This keeps track of a selection crystal respective to the creature's team color
        public GameObject SelectionCrystal;

        private void OnEnable()
        {
            StartCoroutine(DoSelection()); 
        }

        private IEnumerator DoSelection()
        {
            while (true)
            {
                SelectedCreature();

                yield return new WaitForFixedUpdate();

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

                    // If the newly selected creature is not the same as the currently selected creature, deselect the current creature
                    if (selectedCreature != closestCreature || selectedCreature == null)
                    {
                        if (selectedCreature != null)
                        {
                            SelectionCrystal.SetActive(true);
                            selectedCreature.IsHighlighted = false;
                            selectedCreature = null;

                        }

                        selectedCreature = closestCreature.GetComponent<CreatureManager>();
                        selectedCreature.IsHighlighted = true;
                    }


                    SelectionCrystal.transform.parent = selectedCreature.transform.Find("CrystalPosition");
                    SelectionCrystal.transform.localPosition = Vector3.zero;
                    SelectionCrystal.transform.localScale = Vector3.one;
                                             
                }

                else
                {
                    // If the currently selected creature is not null and it is not in the selections list, deselect it
                    if (selectedCreature != null)
                    {
                        SelectionCrystal.SetActive(false);
                        selectedCreature.IsHighlighted = false;
                        selectedCreature = null;
                    }
                }

                yield return new WaitForSeconds(.1f);

                selections.Clear();

                // Write the code shorter
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
}