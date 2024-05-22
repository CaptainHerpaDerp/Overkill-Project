using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMovingColorChange : MonoBehaviour
{
    [HideInInspector]
    public ColorEnum.PLANTCOLOR creatureColor;

    private Transform target = null;
    public void OnColorTriggerEnter(Collider other) {
        Debug.Log(target);
        if(other.gameObject.transform == target)
            other.gameObject.GetComponent<PlantColor>().ChangeThisPlantColor(creatureColor);
    }
    
    public void SetCreatureColor(ColorEnum.PLANTCOLOR newColor) {
        creatureColor = newColor;
    }
    
    public void SetCreatureTarget(Transform newTarget) {
        target = newTarget;
    }

}
