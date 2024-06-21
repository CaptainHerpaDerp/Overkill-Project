using GaiaElements;
using System;
using System.Collections.Generic;
using TeamColors;
using UnityEngine;

namespace Creatures
{
    public abstract class Creature : MonoBehaviour
    {
        public event Action<ColorEnum.TEAMCOLOR> ONOwnColorChanged;
        public event Action<Plant, ColorEnum.TEAMCOLOR> ONPlantColorChanged;
        public event Action<Vector3> ONTargetChanged;

        [SerializeField] protected SurroundingPlant plantLocator;
        [SerializeField] protected float weightCoefficient;

       // public Transform plantsHierarchyParent;

        public Transform plantTarget { get; protected set; }

        protected virtual void ChooseClosestOpponentPlant()
        {
            if (plantLocator == null)
            {
                Debug.LogWarning("Plant locator not found, please assign!");
                return;
            }

            List<Plant> opponentPlants = plantLocator.GetSurroundingOpponentPlantsList();

            float smallestWeight = float.MaxValue;
            Plant targetPlant = null;

            foreach (Plant plant in opponentPlants)
            {
                if (plant == null)
                {
                    Debug.LogError("Plant component not found");
                    continue;
                }

                // Get the forward vector of the current object
                Vector3 forward = transform.forward;

                // Calculate the direction vector from the object to the target
                Vector3 toTarget = (plant.transform.position - transform.position).normalized;

                // Calculate the angle between the forward vector and the direction to the target
                float givenAngle = Vector3.Angle(forward, toTarget);
                float distance = Vector3.Distance(transform.position, plant.transform.position);

                if (givenAngle > 180)
                {
                    givenAngle = 360 - givenAngle;
                }

                givenAngle = Mathf.Clamp(givenAngle, 0, weightCoefficient);

                float weight = givenAngle * distance;
                if (weight < smallestWeight)
                {
                    smallestWeight = weight;
                    targetPlant = plant;
                }
            }

            if (targetPlant != null)
            {
                print("found plant target");
                plantTarget = targetPlant.transform;
                TriggerTargetChange(plantTarget.position);
            }
        }

        protected virtual void ChooseClosestNeutralPlant()
        {
            if (plantLocator == null)
            {
                Debug.LogWarning("Plant locator not found, please assign!");
                return;
            }

            List<Plant> opponentPlants = plantLocator.GetSurroundingNeutralPlantsList();

            float smallestWeight = float.MaxValue;
            Plant targetPlant = null;

            foreach (Plant plant in opponentPlants)
            {
                if (plant == null)
                {
                    Debug.LogError("Plant component not found");
                    continue;
                }

                // Get the forward vector of the current object
                Vector3 forward = transform.forward;

                // Calculate the direction vector from the object to the target
                Vector3 toTarget = (plant.transform.position - transform.position).normalized;

                // Calculate the angle between the forward vector and the direction to the target
                float givenAngle = Vector3.Angle(forward, toTarget);
                float distance = Vector3.Distance(transform.position, plant.transform.position);

                if (givenAngle > 180)
                {
                    givenAngle = 360 - givenAngle;
                }

                givenAngle = Mathf.Clamp(givenAngle, 0, weightCoefficient);

                float weight = givenAngle * distance;
                if (weight < smallestWeight)
                {
                    smallestWeight = weight;
                    targetPlant = plant;
                }
            }

            if (targetPlant != null)
            {
                print("found plant target");
                plantTarget = targetPlant.transform;
                TriggerTargetChange(plantTarget.position);
            }
        }

        public virtual void OnTeleported()
        {
        }

        /// <summary>
        /// Triggered upon creature conversion, handles logic for terminating a specific colour's logic
        /// </summary>
        public virtual void StopBehaviour()
        {
        }

        protected virtual void TriggerColorChange(ColorEnum.TEAMCOLOR newColor)
        {
            ONOwnColorChanged?.Invoke(newColor);
        }

        protected void TriggerPlantColorChange(Plant plant, ColorEnum.TEAMCOLOR color)
        {
            ONPlantColorChanged?.Invoke(plant, color);
        }

        protected void TriggerTargetChange(Vector3 newTarget)
        {
            Debug.Log("nice cock");
            ONTargetChanged?.Invoke(newTarget);
        }

        public abstract void Act();
    }
}