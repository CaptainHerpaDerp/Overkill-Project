using System;
using UnityEngine;


public abstract class Creature : MonoBehaviour {

    public event Action<ColorEnum.TEAMCOLOR> ONOwnColorChanged;
    public event Action<Plant, ColorEnum.TEAMCOLOR> ONPlantColorChanged;
    public event Action<Vector3> ONTargetChanged;

    public Transform target{ get; protected set; }

    protected void TriggerColorChange(ColorEnum.TEAMCOLOR newColor) {
        ONOwnColorChanged?.Invoke(newColor);
    }
    
    protected void TriggerPlantColorChange(Plant plant, ColorEnum.TEAMCOLOR color) {
        ONPlantColorChanged?.Invoke(plant, color);
    }
    
    protected void TriggerTargetChange(Vector3 newTarget) {
        ONTargetChanged?.Invoke(newTarget);
    }

    public abstract void Act();
    
    

}