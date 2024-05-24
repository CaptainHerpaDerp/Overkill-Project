
using UnityEngine;
using Random = UnityEngine.Random;
using GaiaElements;
using TeamColors;
namespace Creatures
{
    public class CreatureColorChange : MonoBehaviour
    {
        [SerializeField]
        private SphereCollider plantsAroundCollider;

        [SerializeField]
        private float ChangeColorCooldown = 7f;

        [SerializeField] private LayerMask plantLayer;
        private ColorEnum.TEAMCOLOR creatureColor = ColorEnum.TEAMCOLOR.DEFAULT;
        private CreatureBehaviour creatureBehaviourScript;
        private CreatureMovingColorChange movingColorChangeScript;
        private float lastTimeChangedColor = -10f;
        private Collider[] plantCollisionResults = new Collider[10];
        //10 is a random number big enough

        private void Start()
        {
            movingColorChangeScript = gameObject.GetComponent<CreatureMovingColorChange>();
            creatureBehaviourScript = gameObject.GetComponent<CreatureBehaviour>();
            ChangeThisCreatureColor(creatureColor);
            lastTimeChangedColor = -10f;
        }

        private void Update()
        {
            ColorEnum.TEAMCOLOR aroundPlantsColor = CheckPlantSurroundings();
            if (aroundPlantsColor != ColorEnum.TEAMCOLOR.DEFAULT && creatureColor != ColorEnum.TEAMCOLOR.RED
                                                                  && creatureColor != ColorEnum.TEAMCOLOR.BLUE
                                                                  && creatureColor != ColorEnum.TEAMCOLOR.GREEN)
            {
                ChangeThisCreatureColor(aroundPlantsColor);
            }
        }

        public void ChangeThisCreatureColor(ColorEnum.TEAMCOLOR newColor)
        {
            //if (lastTimeChangedColor + ChangeColorCooldown > Time.time) {
            //    return;
            //}
            UpdateCreatureColor(newColor);
            gameObject.GetComponent<Renderer>().material.color = ColorEnum.GetColor(newColor);
            lastTimeChangedColor = Time.time;
        }

        private void UpdateCreatureColor(ColorEnum.TEAMCOLOR newColor)
        {
            creatureColor = newColor;
            creatureBehaviourScript.SetCreatureColor(newColor);
            movingColorChangeScript.SetCreatureColor(newColor);
        }

        public ColorEnum.TEAMCOLOR GetThisCreatureColor()
        {
            return creatureColor;
        }

        private ColorEnum.TEAMCOLOR CheckPlantSurroundings()
        {
            int numPlants = Physics.OverlapSphereNonAlloc(transform.position, plantsAroundCollider.radius,
                plantCollisionResults, plantLayer, QueryTriggerInteraction.Collide);

            if (numPlants < 1) return ColorEnum.TEAMCOLOR.DEFAULT;

            ColorEnum.TEAMCOLOR TMPColor = plantCollisionResults[0].gameObject.GetComponent<Plant>().TeamColor;
            for (int i = 1; i < numPlants; i++)
            {
                if (plantCollisionResults[i].gameObject.GetComponent<Plant>().TeamColor != TMPColor)
                {
                    return ColorEnum.TEAMCOLOR.DEFAULT;
                }
            }
            return TMPColor;
        }

    }
}