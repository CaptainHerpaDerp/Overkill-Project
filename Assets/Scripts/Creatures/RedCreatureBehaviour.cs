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
            gameObject.SetActive(true); 
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
                plantTarget = targetPlant.transform;
                TriggerTargetChange(plantTarget.position);
            }
            else
            {
                Debug.LogWarning("No Red Targets Found");
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
            if (plantTarget == null)
            {
                ChooseEnemyPlant();
            }

            if (plantTarget == null)
            {
                Debug.Log("There are no enemies to conquer");
                return;
            }

            if (plantTarget.GetComponent<Plant>().TeamColor == ColorEnum.TEAMCOLOR.DEFAULT)
            {
                plantTarget = null;
                return;
            }

            CheckDistanceToTarget();

        }

        private void CheckDistanceToTarget()
        {
            if (Vector3.Distance(transform.position, plantTarget.position) <= TargetTurnDistance)
            {
                TriggerPlantColorChange(plantTarget.gameObject.GetComponent<Plant>(), ColorEnum.TEAMCOLOR.RED);
                plantTarget = null;
            }
        }
    }
}