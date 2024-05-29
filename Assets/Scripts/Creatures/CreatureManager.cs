using GaiaElements;
using System.Collections;
using System.Collections.Generic;
using TeamColors;
using UnityEngine;
using UnityEngine.AI;

namespace Creatures
{
    using ParticleSystems;

    public class CreatureManager : MonoBehaviour
    {
        [SerializeField]
        private NavMeshAgent agent;

        public List<Creature> CreatureColorScripts;
        private ColorEnum.TEAMCOLOR creatureColor = ColorEnum.TEAMCOLOR.DEFAULT;

        // Conversion Variables
        private float conversionProgress;

        [SerializeField] float conversionThreshold = 100f, baseConversionSpeed, conversionRevertSpeed;

        [Header("Assuming all of the surrounding plants are owned by the creature owner, conversion speed of this creature will be multiplied by this value")]
        [SerializeField] float ratioPenalty = 0.5f;

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float colorShiftMod;

        // If false, the creature will stop moving when converting
        [SerializeField] private bool canMoveOnConvert = true;

        private bool isConverting;

        // A list to track every player attempting to convert this creature
        private List<TeamColors.ColorEnum.TEAMCOLOR> converters = new();

        // The script that determines the ratio of nearby influence
        [SerializeField] private SurroundingPlant surroundingPlant;

        private Color targetColor;
        private Color initialColor;

        // The script that handles the particle system for the conversion
        [SerializeField] ConversionParticles conversionParticles;

        public bool isRedConversion;

        public bool IsHighlighted;
        [SerializeField] private Outline outlineEffect;

        private void Start()
        {
            Init();
            ChangeThisCreatureColor(creatureColor);
            StartCoroutine(DoConversion());
        }

        private void Init()
        {
            AnimalLocator.Instance.RegisterAnimalOfTeam(transform, creatureColor);

            CreatureColorScripts.Add(transform.Find("RedLogic").GetComponent<Creature>());
            CreatureColorScripts.Add(transform.Find("GreenLogic").GetComponent<Creature>());
            CreatureColorScripts.Add(transform.Find("BlueLogic").GetComponent<Creature>());
            CreatureColorScripts.Add(transform.Find("PurpleLogic").GetComponent<Creature>());
            CreatureColorScripts.Add(transform.Find("DefaultLogic").GetComponent<Creature>());

            foreach (Creature creature in CreatureColorScripts)
            {
                creature.ONOwnColorChanged += ChangeThisCreatureColor;
                creature.ONPlantColorChanged += ChangePlantColor;
                creature.ONTargetChanged += SetNewTarget;
            }

            surroundingPlant.teamColour = creatureColor;
            initialColor = ColorEnum.GetColor(ColorEnum.TEAMCOLOR.DEFAULT);
            meshRenderer.material.color = initialColor;
            conversionParticles.gameObject.SetActive(false);
        }


        public void Convert(ColorEnum.TEAMCOLOR newColor)
        {
            // If a red player is convering
            if (newColor == ColorEnum.TEAMCOLOR.RED)
            {
                isRedConversion = true;
            }
            else
            {
                isRedConversion = false;
            }

            isConverting = true;
            targetColor = ColorEnum.GetColor(newColor);

            if (!converters.Contains(newColor))
            {
                converters.Add(newColor);
            }

            if (newColor == creatureColor)
            {
                conversionProgress = 0;
                return;
            }

            if (conversionProgress >= conversionThreshold - baseConversionSpeed)
            {
                print("Converted");
                ChangeThisCreatureColor(newColor);

                if (newColor == ColorEnum.TEAMCOLOR.GREEN)
                {
                    agent.velocity = Vector3.zero;
                    agent.isStopped = true;
                }

                conversionProgress = 0;
                initialColor = targetColor;
                converters.Clear();
            }
        }

        /// <summary>
        /// Lerp from the current color to the target color by the factor of the conversion progress
        /// </summary>
        /// <param name="targetColor"></param>
        private void LerpColor(Color targetColor)
        {
            Color color = meshRenderer.material.color;

            meshRenderer.material.color = Color.Lerp(color, targetColor, conversionProgress / conversionThreshold);
        }

        private void Update()
        {
            CreatureColorScripts[(int)creatureColor].Act();

            if (IsHighlighted)
            {
                outlineEffect.enabled = true;
            }
            else
            {
                outlineEffect.enabled = false;
            }
        }


        private IEnumerator DoConversion()
        {
            while (true)
            {
                if (isConverting)
                {
                    if (!canMoveOnConvert)
                    {
                        agent.isStopped = true;
                    }

                    // If there are more than 1 converters, there will be no conversion
                    if (converters.Count > 1)
                    {
                        yield return new WaitForSeconds(0.3f);

                        // Clear the converters list
                        converters.Clear();

                        continue;
                    }

                    /* 
                     * This ratio determines the influence of the surrounding plants between it's current owner and the other owner.
                     * If the ratio of the surrounding plants is 1, it means that all the surrounding plants are owned by the same player.
                     * !Unplanted plants still make up this ratio, so by default the ratio will be 1, meaning the player should plant around the creature to capture it quicker!                    
                    */

                    float ratio = surroundingPlant.GetSurroundingPlantsClamped();

                    // If the ratio is 1, the hinderance should be ratioPenalty, meaning the conversion speed will be at its slowest
                    float hinderance = 1;

                    if (!isRedConversion)
                    {
                        hinderance = 1 - ratio * ratioPenalty;
                    }

                    conversionProgress += baseConversionSpeed * hinderance;
                    conversionProgress = Mathf.Clamp(conversionProgress, 0, conversionThreshold);

                    float t = conversionProgress / conversionThreshold;
                    meshRenderer.material.color = Color.Lerp(initialColor, targetColor, t);

                    //print($"conversion: {conversionProgress} / {conversionThreshold} | ratio: {surroundingPlant.GetSurroundingPlantsClamped()} || hinderance: {hinderance}");
                }
                else
                {
                    if (conversionProgress > 0)
                    {
                        conversionProgress -= conversionRevertSpeed;
                        conversionProgress = Mathf.Clamp(conversionProgress, 0, conversionThreshold);

                        float t = conversionProgress / conversionThreshold;
                        meshRenderer.material.color = Color.Lerp(initialColor, targetColor, t);
                    }
                }

                isConverting = false;
                converters.Clear();
                agent.isStopped = false;
                yield return new WaitForEndOfFrame();
            }
        }

        private void ChangeThisCreatureColor(ColorEnum.TEAMCOLOR newColor)
        {
            creatureColor = newColor;

            AnimalLocator.Instance.ChangeTeamOfAnimal(transform, creatureColor);

            // Only play particle effects if the color is not the default color
            if (newColor != ColorEnum.TEAMCOLOR.DEFAULT)
            {
                conversionParticles.ActivateWithColor(ColorEnum.GetColor(creatureColor));
            }

            for (int i = 0; i < CreatureColorScripts.Count; i++)
            {
                if ((int)newColor == i)
                {
                    CreatureColorScripts[i].gameObject.SetActive(true);
                    continue;
                }

                CreatureColorScripts[i].StopBehaviour();
                CreatureColorScripts[i].gameObject.SetActive(false);
            }

            // Set the surrounding plant color to the new color
            surroundingPlant.teamColour = creatureColor;

            // Change the color of the creature
            gameObject.GetComponent<Renderer>().material.color = ColorEnum.GetColor(newColor);
        }

        private void ChangePlantColor(Plant plant, ColorEnum.TEAMCOLOR newColor)
        {
            //TEMP
            if (creatureColor == ColorEnum.TEAMCOLOR.BLUE)
            {
                plant.PlantSpreadCreep = true;
                plant.PlayerParentTransform = PlayerLocator.Instance.GetTransformOfTeam(ColorEnum.TEAMCOLOR.BLUE);
            }

            if (plant.plantRenderer.enabled == false)
            {
                plant.Activate(newColor);
                return;
            }

            plant.TeamColor = newColor;
        }

        private void SetNewTarget(Vector3 newTarget)
        {
            agent.SetDestination(newTarget);
        }
    }
}