using GaiaElements;
using TeamColors;
using UnityEngine;
using Core;
using System;
using UnityEngine.AI;
using System.IO;
using System.Collections;

namespace Creatures
{
    public class RedCreatureBehaviour : Creature
    {
        [SerializeField] private float targetConversionDistance;
        public static Action OnRedEnabled;

        [Header("While moving towards the plant target, the creature may find other targets along the way")]
        [SerializeField] private float retargetTime;

        public override void Act()
        {
            gameObject.SetActive(true); 
            CheckTarget();
        }

        private void OnEnable()
        {
            StartCoroutine(FindOtherTargets());
            OnRedEnabled?.Invoke();
        }

        public override void StopBehaviour()
        {
            StopAllCoroutines();
        }

        private void CheckTarget()
        {
            if (plantTarget == null)
            {
                ChooseClosestOpponentPlant();
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
            if (Vector3.Distance(transform.position, plantTarget.position) <= targetConversionDistance)
            {
                TriggerPlantColorChange(plantTarget.gameObject.GetComponent<Plant>(), ColorEnum.TEAMCOLOR.RED);
                plantTarget = null;
            }
        }

        private IEnumerator FindOtherTargets()
        {
            while (true)
            {
                print("marker");
                ChooseClosestOpponentPlant();
                yield return new WaitForSeconds(retargetTime);
            }
        }
    }
}