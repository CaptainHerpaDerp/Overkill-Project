using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingColorChange : MonoBehaviour {
    
    public ColorEnum.PLANTCOLOR characterColor;

    public void OnColorTriggerEnter(Collider other) {
        other.gameObject.GetComponent<PlantColor>().ChangeThisPlantColor(characterColor);
    }

    public void SetCharacterColor(ColorEnum.PLANTCOLOR newColor) {
        characterColor = newColor;
    }
}
