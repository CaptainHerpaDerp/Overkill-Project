using GaiaElements;
using TeamColors;
using UnityEngine;
using UnityEngine.AI;

namespace Creatures
{
    public class BlueCreatureBehaviour : Creature
    {

        [SerializeField]
        private float targetTurnDistance;

        public override void Act()
        {
            CheckTarget();
        }

        private void ChooseDefaultPlant()
        {
            float distToPlant = 10000f;
            Plant targetPlant = null;

            if (plantsHierarchyParent == null)
            {
                plantsHierarchyParent = GameObject.Find("Plants").transform;
            }

            foreach (Transform plant in plantsHierarchyParent)
            {
                Plant plantScript = plant.GetComponent<Plant>();
                if (plantScript == null)
                {
                    Debug.LogError("Plant component not found");
                    continue;
                }

                if (plantScript.TeamColor != ColorEnum.TEAMCOLOR.DEFAULT)
                    continue;
                Vector3 tmpDist = plant.transform.position - transform.position;

                float TMPDistance = tmpDist.sqrMagnitude;
                /*
                NavMeshPath tmpPath = new NavMeshPath();
                if (!NavMesh.CalculatePath(transform.position, plant.transform.position, NavMesh.AllAreas, tmpPath))
                {
                    Debug.Log("Blue Path cannot be calculated");
                    continue;
                }

                if (tmpPath.status != NavMeshPathStatus.PathComplete)
                {
                    Debug.Log("Blue Path is not complete");
                    continue;
                }

                float tmpDistance = 0f;
                Vector3 previousCorner = tmpPath.corners[0];
                for (int i = 1; i < tmpPath.corners.Length; i++)
                {
                    Vector3 currentCorner = tmpPath.corners[i];
                    Vector3 cornerDist = currentCorner - previousCorner;
                    tmpDistance += cornerDist.sqrMagnitude;
                    previousCorner = currentCorner;

                }
                */
                if (!(TMPDistance < distToPlant)) continue;
                distToPlant = TMPDistance;
                targetPlant = plantScript;
            }

            if (targetPlant != null)
            {
                plantTarget = targetPlant.transform;
                TriggerTargetChange(plantTarget.position);
            }
            else
            {
                Debug.LogError("Blue plantTarget is null");
            }

        }

        private void CheckTarget()
        {
            if (plantTarget == null)
            {
                ChooseDefaultPlant();
            }

            if (plantTarget == null)
            {
                Debug.Log("There are no default plants for blue");
                return;
            }

            if (plantTarget.GetComponent<Plant>().TeamColor != ColorEnum.TEAMCOLOR.DEFAULT)
            {
                plantTarget = null;
                return;
            }

            CheckDistanceToTarget();
        }

        private void CheckDistanceToTarget()
        {
            if (Vector3.Distance(transform.position, plantTarget.position) <= targetTurnDistance)
            {
                TriggerPlantColorChange(plantTarget.gameObject.GetComponent<Plant>(), ColorEnum.TEAMCOLOR.BLUE);
                plantTarget = null;
            }
        }

    }
}