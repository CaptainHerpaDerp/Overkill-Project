using GaiaElements;
using TeamColors;
using UnityEngine;

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
            float distToPlant = 1000f;
            Plant targetPlant = null;

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

                float TMPDist = Vector3.Distance(transform.position, plant.transform.position);
                if (TMPDist > distToPlant) continue;
                distToPlant = TMPDist;
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