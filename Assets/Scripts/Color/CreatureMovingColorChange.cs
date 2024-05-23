using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ColorEnum;

public class CreatureMovingColorChange : MonoBehaviour
{
    [HideInInspector]
    public ColorEnum.TEAMCOLOR creatureColor;

    private Transform target = null;

    public void OnColorTriggerEnter(Collider other)
    {
        Debug.Log(target);
        if (other.gameObject.transform == target)
        {
            other.TryGetComponent(out Plant plantColor);

            if (plantColor != null)
            {

                if (creatureColor == TEAMCOLOR.RED)
                    plantColor.TeamColor = creatureColor;

                if (creatureColor == TEAMCOLOR.BLUE)
                    plantColor.Activate(TEAMCOLOR.BLUE);
            }
            else
            {
                Debug.LogError("Plant component not found");
            }
        }
    }

    public void SetCreatureColor(ColorEnum.TEAMCOLOR newColor)
    {
        creatureColor = newColor;
    }

    public void SetCreatureTarget(Transform newTarget)
    {
        target = newTarget;
    }

}


/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantColor : MonoBehaviour {
    [SerializeField]
    private ColorEnum.TEAMCOLOR plantColor;

    [SerializeField]
    private float ChangeColorCooldown = 7f;

    private float lastTimeChangedColor = - 10f;

    private void Start() {
        ChangeThisPlantColor(ColorEnum.TEAMCOLOR.DEFAULT);
        lastTimeChangedColor = -10f;
    }

    public void ChangeThisPlantColor(ColorEnum.TEAMCOLOR newColor) {
        if (lastTimeChangedColor + ChangeColorCooldown > Time.time) {
            return;
        }

        plantColor = newColor;
        gameObject.GetComponent<Renderer>().material.color = ColorEnum.GetColor(newColor);
        lastTimeChangedColor = Time.time;
    }

    public ColorEnum.TEAMCOLOR TeamColor {
        return plantColor;
    }

    public bool GetColorChangeState() {
        return !(lastTimeChangedColor + ChangeColorCooldown > Time.time);
    }
}
*/