using Creatures;
using GaiaElements;
using TeamColors;
using UnityEngine;

public class DefaultCreatureBehaviour : Creature {
    
    [SerializeField] 
    private SphereCollider plantsAroundCollider;

    [SerializeField] 
    private LayerMask plantLayer;
    
    private Collider[] plantCollisionResults = new Collider[10];
    //10 is a random number big enough

    public override void Act() {
        CheckCreatureColor();
    }
    
    private void CheckCreatureColor(){
        ColorEnum.TEAMCOLOR aroundPlantsColor = CheckPlantSurroundings();
        if (aroundPlantsColor != ColorEnum.TEAMCOLOR.DEFAULT)
        {
            TriggerColorChange(aroundPlantsColor);
        }
    }
    
    private ColorEnum.TEAMCOLOR CheckPlantSurroundings() {
        int numPlants = Physics.OverlapSphereNonAlloc(transform.position, plantsAroundCollider.radius, 
            plantCollisionResults, plantLayer, QueryTriggerInteraction.Collide);

        if(numPlants < 1) return ColorEnum.TEAMCOLOR.DEFAULT;
        
        ColorEnum.TEAMCOLOR TMPColor = plantCollisionResults[0].gameObject.GetComponent<Plant>().TeamColor;
        for (int i = 1; i < numPlants; i++) {
            if (plantCollisionResults[i].gameObject.GetComponent<Plant>().TeamColor != TMPColor) {
                return ColorEnum.TEAMCOLOR.DEFAULT;
            }
        }
        return TMPColor;
    }

}