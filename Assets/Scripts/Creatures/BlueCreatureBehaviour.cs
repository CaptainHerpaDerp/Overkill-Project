using GaiaElements;
using TeamColors;
using UnityEngine;

namespace Creatures
{
    public class BlueCreatureBehaviour : Creature
    {

        [SerializeField]
        private Transform plantsHierarchyParent;

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
                target = targetPlant.transform;
                TriggerTargetChange(target.position);
            }
            else
            {
                Debug.LogError("Blue target is null");
            }

        }

        private void CheckTarget()
        {
            if (target == null)
            {
                ChooseDefaultPlant();
            }

            if (target == null)
            {
                Debug.Log("There are no default plants for blue");
                return;
            }

            if (target.GetComponent<Plant>().TeamColor != ColorEnum.TEAMCOLOR.DEFAULT)
            {
                target = null;
                return;
            }

            CheckDistanceToTarget();
        }

        private void CheckDistanceToTarget()
        {
            if (Vector3.Distance(transform.position, target.position) <= targetTurnDistance)
            {
                TriggerPlantColorChange(target.gameObject.GetComponent<Plant>(), ColorEnum.TEAMCOLOR.BLUE);
                target = null;
            }
        }

    }
}