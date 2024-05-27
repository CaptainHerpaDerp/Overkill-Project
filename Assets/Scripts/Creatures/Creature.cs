using GaiaElements;
using System;
using TeamColors;
using UnityEngine;

namespace Creatures
{
    public abstract class Creature : MonoBehaviour
    {
        public event Action<ColorEnum.TEAMCOLOR> ONOwnColorChanged;
        public event Action<Plant, ColorEnum.TEAMCOLOR> ONPlantColorChanged;
        public event Action<Vector3> ONTargetChanged;

        public Transform plantsHierarchyParent;

        public Transform target { get; protected set; }

        public virtual void Start()
        {
            if (plantsHierarchyParent == null)
            plantsHierarchyParent = GameObject.FindGameObjectWithTag("PlantsParent").transform;

            if (plantsHierarchyParent == null)
            {
                Debug.LogError("Plants parent not found");
            }
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
            ONTargetChanged?.Invoke(newTarget);
        }

        public abstract void Act();
    }
}