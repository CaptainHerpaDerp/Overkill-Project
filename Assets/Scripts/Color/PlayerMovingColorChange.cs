using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovingColorChange : MonoBehaviour {
    
    public ColorEnum.PLANTCOLOR playerColor;

    public void OnColorTriggerEnter(Collider other) {
        other.gameObject.GetComponent<PlantColor>().ChangeThisPlantColor(playerColor);
    }

    public void SetCharacterColor(ColorEnum.PLANTCOLOR newColor) {
        playerColor = newColor;
    }
}
