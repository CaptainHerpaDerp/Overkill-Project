using GaiaElements;
using TeamColors;
using UnityEngine;
using UnityEngine.AI;

namespace Creatures
{
    public class BlueCreatureBehaviour : Creature
    {
        [SerializeField] private float targetTurnDistance;

        public override void Act()
        {
            CheckTarget();
        }

        private void CheckTarget()
        {
            if (plantTarget == null)
            {
                ChooseClosestNeutralPlant();
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