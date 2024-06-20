using GaiaElements;
using TeamColors;
using UnityEngine;
using Core;
using System;
using UnityEngine.AI;
using System.IO;

namespace Creatures
{
    public class RedCreatureBehaviour : Creature
    {
        [SerializeField]
        private float TargetTurnDistance;

        // Copies the ScoreManager array using observed values
        private int[] playerTeamScore = new int[5];

        public static Action OnRedEnabled;

        public override void Act()
        {
            gameObject.SetActive(true); 
            CheckTarget();
        }

        private void OnEnable()
        {
            ScoreReceptionManager.OnValueChanged += UpdateScore;
            OnRedEnabled?.Invoke();
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
            float distToPlant = 10000f;
            Plant targetPlant = null;

            ColorEnum.TEAMCOLOR highestOtherPlayerScore = FindHighestScoreNotRed();
            Debug.Log("highest other score = " + highestOtherPlayerScore);

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

                float tmpDistance = Vector3.Distance(transform.position, plant.transform.position);
                /*NavMeshPath tmpPath= new NavMeshPath();
                if (!NavMesh.CalculatePath(transform.position, plant.transform.position, NavMesh.AllAreas, tmpPath)) {
                    Debug.Log("Red Path cannot be calculated");
                    continue;
                }

                if (tmpPath.status != NavMeshPathStatus.PathComplete) {
                    Debug.Log("Red Path is not complete");
                    continue;
                }
                    
                float tmpDistance = 0f;
                Vector3 previousCorner = tmpPath.corners[0];
                for (int i = 1; i < tmpPath.corners.Length; i++) { 
                    Vector3 currentCorner = tmpPath.corners[i];
                    Vector3 cornerDist = currentCorner - previousCorner;
                    tmpDistance += cornerDist.sqrMagnitude;
                    previousCorner = currentCorner;

                }
                */
                if (!(tmpDistance < distToPlant)) continue;
                distToPlant = tmpDistance;
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

            Debug.Log($"blue score = {blueScore}, green score = {greenScore}, purple score = {purpleScore}");

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