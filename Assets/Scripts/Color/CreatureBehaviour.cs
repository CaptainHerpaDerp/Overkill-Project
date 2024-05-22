using UnityEngine;
using UnityEngine.AI;

public class CreatureBehaviour : MonoBehaviour {
    [SerializeField]
    private Transform plantsHierarchyParent;

    [Header("Green")] 
    [SerializeField] 
    private float growSpeed = 2;
    
    [SerializeField] 
    private SphereCollider growCollider;
    
    
    [SerializeField] private LayerMask plantLayer;
    //  private GameObject[] plants;
    [SerializeField] private ColorEnum.TEAMCOLOR creatureColor;
    private NavMeshAgent agent;
    private CreatureMovingColorChange creatureMovingChangeScript;
    private GreenExpandingSphere greenExpandingSphere;  
    private Transform target = null;
    private float currentGrowRadius = 1;

    [SerializeField] Collider[] plantsInside;

    private void Start()
    {
        creatureMovingChangeScript = gameObject.GetComponent<CreatureMovingColorChange>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        //  plants = plantsHierarchyParent.GetChildren(false);
        greenExpandingSphere = gameObject.GetComponentInChildren<GreenExpandingSphere>();

        greenExpandingSphere.PlantTriggered += (plant) =>
        {
            if (plant.TeamColor == ColorEnum.TEAMCOLOR.DEFAULT)
            {
                plant.Activate(creatureColor);
            }
            else
            {
                plant.TeamColor = creatureColor;
            }
        };
    }

    private void Update() {
        if (creatureColor == ColorEnum.TEAMCOLOR.RED) {
            RedMoveToTargetPlant();
        }
        else if (creatureColor == ColorEnum.TEAMCOLOR.GREEN) {
            greenExpandingSphere.StartPulse();

            //IncreaseGrowCollider();
            //CheckPlantsInGrow();
        }
        else if (creatureColor == ColorEnum.TEAMCOLOR.BLUE) {
            BlueMoveToTargetPlant();
        }
    }
    
    public void SetCreatureColor(ColorEnum.TEAMCOLOR newColor) {
        creatureColor = newColor;
    }

    private void UpdateCreatureTarget(Transform newTarget) {
        target = newTarget;
        creatureMovingChangeScript.SetCreatureTarget(newTarget);
    }

    #region Red
    private void ChooseEnemyPlant() {
        float distToPlant = 1000f;
        
        foreach (Transform plant in plantsHierarchyParent) {
            Plant plantScript = plant.GetComponent<Plant>();

            if (plantScript == null)
            {
                Debug.LogError("Plant component not found");
                continue;
            }

            if (plantScript.TeamColor == ColorEnum.TEAMCOLOR.RED || plantScript.TeamColor == ColorEnum.TEAMCOLOR.DEFAULT) {
                continue;
            }

            //print("marker1");

            // TODO might need?
            //if (!plantScript.GetColorChangeState())
            //    continue;

            float TMPDist = Vector3.Distance(transform.position, plant.transform.position);
            if (TMPDist < distToPlant) {
                distToPlant = TMPDist;
                UpdateCreatureTarget(plant.transform);

            //    print("got target");
            }
            else
            {
              //  print("no target");
            }
        }
    }

    private void RedMoveToTargetPlant() {
        if (target == null) {
            ChooseEnemyPlant();
        }

        if (target == null)
        {
            print("double null");
             return;
        }
        
        ColorEnum.TEAMCOLOR plantColor = target.gameObject.GetComponent<Plant>().TeamColor;

        if (plantColor == ColorEnum.TEAMCOLOR.RED || plantColor == ColorEnum.TEAMCOLOR.DEFAULT) {
            UpdateCreatureTarget(null);
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
        plantsInside = Physics.OverlapSphere(transform.position, currentGrowRadius, 
            plantLayer, QueryTriggerInteraction.Collide);
        
        foreach (Collider plantCollider in plantsInside) {

            Plant plantScript = plantCollider.gameObject.GetComponent<Plant>();

            if (plantScript == null)
            {
                Debug.LogError("Plant component not found");
                continue;
            }

            // IF the plant is on the animal's team, continue
            if (plantScript.TeamColor == ColorEnum.TEAMCOLOR.GREEN) {
                continue;
            }
            
            // Otherwise, change the plant's color to green
            plantScript.Activate(ColorEnum.TEAMCOLOR.GREEN);
        }
    }

    #endregion

    #region Blue

    private Transform ChooseDefaultPlant() {
        float distToPlant = 1000f;
        
        foreach (Transform plant in plantsHierarchyParent) {
            Plant plantScript = plant.GetComponent<Plant>();

            if (plantScript == null)
            {
                Debug.LogError("Plant component not found");
                continue;
            }

            if (plantScript.TeamColor != ColorEnum.TEAMCOLOR.DEFAULT) {
                continue;
            }

            float TMPDist = Vector3.Distance(transform.position, plant.transform.position);
            if (TMPDist < distToPlant) {
                distToPlant = TMPDist;
                UpdateCreatureTarget(plant.transform);
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
        
        ColorEnum.TEAMCOLOR plantColor = target.gameObject.GetComponent<Plant>().TeamColor;
        if (plantColor == ColorEnum.TEAMCOLOR.BLUE) {
            UpdateCreatureTarget(null);
            return;
        }


        agent.SetDestination(target.position);
    }

    #endregion
    
}
