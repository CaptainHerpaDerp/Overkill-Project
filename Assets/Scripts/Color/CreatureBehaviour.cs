using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder.MeshOperations;
using Color = System.Drawing.Color;

public class CreatureBehaviour : MonoBehaviour {
    [SerializeField]
    private GameObject plantsHierarchyParent;

    [Header("Green")] 
    [SerializeField] 
    private float growSpeed = 2;
    
    [SerializeField] 
    private SphereCollider growCollider;
    
    private LayerMask plantLayer;
    private GameObject[] plants;
    private ColorEnum.PLANTCOLOR creatureColor;
    private NavMeshAgent agent;
    private Transform target;
    private float currentGrowRadius = 1;
    
    private void Start() {
        plantLayer = (1 << 3); // 11 is number of Plant layer
        agent = gameObject.GetComponent<NavMeshAgent>();
        plants = plantsHierarchyParent.GetChildren(false);
    }

    private void Update() {
        if (creatureColor == ColorEnum.PLANTCOLOR.RED) {
            RedMoveToTargetPlant();
        }
        else if (creatureColor == ColorEnum.PLANTCOLOR.GREEN) {
            IncreaseGrowCollider();
            CheckPlantsInGrow();
        }
        else if (creatureColor == ColorEnum.PLANTCOLOR.BLUE) {
            BlueMoveToTargetPlant();
        }
    }
    
    public void SetCreatureColor(ColorEnum.PLANTCOLOR newColor) {
        creatureColor = newColor;
    }

    #region Red
    private Transform ChooseEnemyPlant() {
        float distToPlant = 1000f;
        
        foreach (GameObject plant in plants) {
            PlantColor plantColorScript = plant.GetComponent<PlantColor>();
            if (plantColorScript.GetThisPlantColor() == ColorEnum.PLANTCOLOR.RED || plantColorScript.GetThisPlantColor() == ColorEnum.PLANTCOLOR.DEFAULT) {
                continue;
            }

            if (!plantColorScript.GetColorChangeState())
                continue;

            float TMPDist = Vector3.Distance(transform.position, plant.transform.position);
            if (TMPDist < distToPlant) {
                distToPlant = TMPDist;
                target = plant.transform;
            }
        }

        return target;
    }

    private void RedMoveToTargetPlant() {
        if (target == null) {
            ChooseEnemyPlant();
        }

        if (target == null)
            return;
        
        ColorEnum.PLANTCOLOR plantColor = target.gameObject.GetComponent<PlantColor>().GetThisPlantColor();
        if (plantColor == ColorEnum.PLANTCOLOR.RED || plantColor == ColorEnum.PLANTCOLOR.DEFAULT) {
            target = null;
            return;
        }

        agent.SetDestination(target.position);
    }
    
    #endregion

    #region Green

    private void IncreaseGrowCollider() {
        currentGrowRadius += growSpeed * Time.deltaTime;
        growCollider.radius = currentGrowRadius;
    }

    private void CheckPlantsInGrow() {
        Collider[] plantsInside = Physics.OverlapSphere(transform.position, currentGrowRadius, 
            plantLayer, QueryTriggerInteraction.Collide);
        
        foreach (Collider plantCollider in plantsInside) {
            PlantColor plantColorScript = plantCollider.gameObject.GetComponent<PlantColor>();
            if (plantColorScript.GetThisPlantColor() == ColorEnum.PLANTCOLOR.GREEN) {
                continue;
            }
            
            plantColorScript.ChangeThisPlantColor(ColorEnum.PLANTCOLOR.GREEN);
        }
    }

    #endregion

    #region Blue

    private Transform ChooseDefaultPlant() {
        float distToPlant = 1000f;
        
        foreach (GameObject plant in plants) {
            PlantColor plantColorScript = plant.GetComponent<PlantColor>();
            if (plantColorScript.GetThisPlantColor() != ColorEnum.PLANTCOLOR.DEFAULT) {
                continue;
            }

            if (!plantColorScript.GetColorChangeState())
                continue;

            float TMPDist = Vector3.Distance(transform.position, plant.transform.position);
            if (TMPDist < distToPlant) {
                distToPlant = TMPDist;
                target = plant.transform;
            }
        }

        return target;
    }

    private void BlueMoveToTargetPlant() {
        if (target == null) {
            ChooseDefaultPlant();
        }

        if (target == null)
            return;
        
        ColorEnum.PLANTCOLOR plantColor = target.gameObject.GetComponent<PlantColor>().GetThisPlantColor();
        if (plantColor == ColorEnum.PLANTCOLOR.BLUE) {
            target = null;
            return;
        }

        agent.SetDestination(target.position);
    }

    #endregion
    
}
