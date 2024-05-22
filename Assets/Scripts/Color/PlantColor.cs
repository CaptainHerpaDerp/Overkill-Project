using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantColor : MonoBehaviour {
    [SerializeField]
    private ColorEnum.PLANTCOLOR plantColor;

    [SerializeField]
    private float ChangeColorCooldown = 7f;

    private float lastTimeChangedColor = - 10f;

    private void Start() {
        ChangeThisPlantColor(ColorEnum.PLANTCOLOR.DEFAULT);
        lastTimeChangedColor = -10f;
    }

    public void ChangeThisPlantColor(ColorEnum.PLANTCOLOR newColor) {
        if (lastTimeChangedColor + ChangeColorCooldown > Time.time) {
            return;
        }

        plantColor = newColor;
        gameObject.GetComponent<Renderer>().material.color = ColorEnum.GetColor(newColor);
        lastTimeChangedColor = Time.time;
    }

    public ColorEnum.PLANTCOLOR GetThisPlantColor() {
        return plantColor;
    }

    public bool GetColorChangeState() {
        return !(lastTimeChangedColor + ChangeColorCooldown > Time.time);
    }
}
