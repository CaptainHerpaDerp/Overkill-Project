using GaiaElements;
using TeamColors;
using UnityEngine;
using Core;

namespace Creatures
{
    public class RedCreatureBehaviour : Creature
    {
        [SerializeField]
        private float TargetTurnDistance;

        // Copies the ScoreManager array using observed values
        private int[] playerTeamScore = new int[5];
         
        public override void Act()
        {
            CheckTarget();
        }

        private void OnEnable()
        {
            ScoreReceptionManager.OnValueChanged += UpdateScore;
        }

        private void OnDisable()
        {
            ScoreReceptionManager.OnValueChanged -= UpdateScore;
        }

        private void UpdateScore(int playerIndex, int newScore)
        {
            playerTeamScore[playerIndex] = newScore;
        }

        private void ChooseEnemyPlant()
        {
            float distToPlant = 1000f;
            Plant targetPlant = null;

            ColorEnum.TEAMCOLOR highestOtherPlayerScore = FindHighestScoreNotRed();

            foreach (Transform plant in plantsHierarchyParent)
            {
                Plant plantScript = plant.GetComponent<Plant>();
                if (plantScript == null)
                {
                    Debug.LogError("Plant component not found");
                    continue;
                }

                if (plantScript.TeamColor != highestOtherPlayerScore)
                {
                    continue;
                }

                float TMPDist = Vector3.Distance(transform.position, plant.transform.position);
                if (!(TMPDist < distToPlant)) continue;
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
                Debug.LogError("Red target is null");
            }

        }

        private ColorEnum.TEAMCOLOR FindHighestScoreNotRed()
        {
            int blueScore = playerTeamScore[(int)ColorEnum.TEAMCOLOR.BLUE];
            int greenScore = playerTeamScore[(int)ColorEnum.TEAMCOLOR.GREEN];
            int purpleScore = playerTeamScore[(int)ColorEnum.TEAMCOLOR.PURPLE];

            if (blueScore > greenScore)
                return blueScore > purpleScore ? ColorEnum.TEAMCOLOR.BLUE : ColorEnum.TEAMCOLOR.PURPLE;
            return greenScore > purpleScore ? ColorEnum.TEAMCOLOR.GREEN : ColorEnum.TEAMCOLOR.PURPLE;
        }

        private void CheckTarget()
        {
            if (target == null)
            {
                ChooseEnemyPlant();
            }

            if (target == null)
            {
                Debug.Log("There are no enemies to conquer");
                return;
            }

            if (target.GetComponent<Plant>().TeamColor == ColorEnum.TEAMCOLOR.DEFAULT)
            {
                target = null;
                return;
            }

            CheckDistanceToTarget();

        }

        private void CheckDistanceToTarget()
        {
            if (Vector3.Distance(transform.position, target.position) <= TargetTurnDistance)
            {
                TriggerPlantColorChange(target.gameObject.GetComponent<Plant>(), ColorEnum.TEAMCOLOR.RED);
                target = null;
            }
        }

    }
}