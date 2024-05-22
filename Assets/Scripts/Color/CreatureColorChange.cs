using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreatureColorChange : MonoBehaviour
{
    [SerializeField] 
    private SphereCollider plantsAroundCollider;
    
    [SerializeField]
    private float ChangeColorCooldown = 7f;

    private ColorEnum.PLANTCOLOR creatureColor = ColorEnum.PLANTCOLOR.DEFAULT;
    private LayerMask plantLayer;
    private CreatureBehaviour creatureBehaviourScript;
    private CreatureMovingColorChange movingColorChangeScript;
    private float lastTimeChangedColor = - 10f;
    private Collider[] plantCollisionResults = new Collider[10];
    //10 is a random number big enough
    
    private void Start() {
        plantLayer = (1 << 3); // 11 is number of Plant layer
        movingColorChangeScript = gameObject.GetComponent<CreatureMovingColorChange>();
        creatureBehaviourScript = gameObject.GetComponent<CreatureBehaviour>();
        ChangeThisCreatureColor(creatureColor);
        lastTimeChangedColor = -10f;
    }

    private void Update() {
        ColorEnum.PLANTCOLOR aroundPlantsColor = CheckPlantSurroundings();
        if (aroundPlantsColor != ColorEnum.PLANTCOLOR.DEFAULT && creatureColor != ColorEnum.PLANTCOLOR.RED 
                                                              && creatureColor != ColorEnum.PLANTCOLOR.BLUE
                                                              && creatureColor != ColorEnum.PLANTCOLOR.GREEN) {
                ChangeThisCreatureColor(aroundPlantsColor);
        }
    }

    public void ChangeThisCreatureColor(ColorEnum.PLANTCOLOR newColor) {
        //if (lastTimeChangedColor + ChangeColorCooldown > Time.time) {
        //    return;
        //}
        UpdateCreatureColor(newColor);
        gameObject.GetComponent<Renderer>().material.color = ColorEnum.GetColor(newColor);
        lastTimeChangedColor = Time.time;
    }

    private void UpdateCreatureColor(ColorEnum.PLANTCOLOR newColor) {
        creatureColor = newColor;
        creatureBehaviourScript.SetCreatureColor(newColor);
        movingColorChangeScript.SetCreatureColor(newColor);
    }

    public ColorEnum.PLANTCOLOR GetThisCreatureColor() {
        return creatureColor;
    }
    
    private ColorEnum.PLANTCOLOR CheckPlantSurroundings() {
        int numPlants = Physics.OverlapSphereNonAlloc(transform.position, plantsAroundCollider.radius, 
            plantCollisionResults, plantLayer, QueryTriggerInteraction.Collide);

        if(numPlants < 1) return ColorEnum.PLANTCOLOR.DEFAULT;
        
        ColorEnum.PLANTCOLOR TMPColor = plantCollisionResults[0].gameObject.GetComponent<PlantColor>().GetThisPlantColor();
        for (int i = 1; i < numPlants; i++) {
            if (plantCollisionResults[i].gameObject.GetComponent<PlantColor>().GetThisPlantColor() != TMPColor) {
                return ColorEnum.PLANTCOLOR.DEFAULT;
            }
        }
        return TMPColor;
    }
    
}
