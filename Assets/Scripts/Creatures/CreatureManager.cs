using GaiaElements;
using System.Collections;
using System.Collections.Generic;
using TeamColors;
using UnityEngine;
using UnityEngine.AI;

namespace Creatures
{
    using ParticleSystems;
    using System;

    public class CreatureManager : MonoBehaviour
    {

        [SerializeField] private NavMeshAgent agent;

        // The component that plays the conversion sound
        [SerializeField] private CreatureConversionSound creatureConversionSound;

        public List<Creature> creatureColorScripts;
        public List<GameObject> creatureColorMasks;
        private ColorEnum.TEAMCOLOR creatureColor = ColorEnum.TEAMCOLOR.DEFAULT;

        public ColorEnum.TEAMCOLOR CreatureColor
        {
            get => creatureColor;
        }

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

        [SerializeField] private Transform selectonCrystalParent;

        public Action OnTeleport;

        private void Start()
        {
            Init();
            ChangeThisCreatureColor(creatureColor);
            StartCoroutine(DoConversion());
        }

        /// <summary>
        ///  Used for green's special ability, invokes an action
        /// </summary>
        /// <param name="position"></param>
        public void TeleportTo(Vector3 position)
        {
            transform.position = position + new Vector3(0, 0.85f, 0);
            OnTeleport?.Invoke();
        }

        private void Init()
        {
            AnimalLocator.Instance.RegisterAnimalOfTeam(transform, creatureColor);

            creatureColorScripts.Add(transform.Find("RedLogic").GetComponent<Creature>());
            creatureColorScripts.Add(transform.Find("GreenLogic").GetComponent<Creature>());
            creatureColorScripts.Add(transform.Find("BlueLogic").GetComponent<Creature>());
            creatureColorScripts.Add(transform.Find("PurpleLogic").GetComponent<Creature>());
            creatureColorScripts.Add(transform.Find("DefaultLogic").GetComponent<Creature>());

            foreach (Creature creature in creatureColorScripts)
            {
                OnTeleport += creature.OnTeleported;
                creature.ONOwnColorChanged += ChangeThisCreatureColor;
                creature.ONPlantColorChanged += ChangePlantColor;
                creature.ONTargetChanged += SetNewTarget;
            }

            surroundingPlant.TeamColour = creatureColor;
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
                    agent.enabled = false;
                }
                else
                {
                    agent.enabled = true;
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
            creatureColorScripts[(int)creatureColor].Act();

            if (IsHighlighted)
            {
                //print("highlighted");
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

                    if (targetColor == initialColor)
                    {
                        converters.Clear();
                        creatureConversionSound.StopSound();    
                        yield return new WaitForFixedUpdate();
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

                    // Only play the conversion sound if any difference is made

                    creatureConversionSound.PlaySound(t);


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
                        creatureConversionSound.PlaySound(t);
                    }
                    else
                    {
                        creatureConversionSound.StopSound();
                    }

                }

                isConverting = false;
                converters.Clear();
                agent.isStopped = false;
                yield return new WaitForFixedUpdate();
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

            for (int i = 0; i < creatureColorScripts.Count; i++)
            {
                if ((int)newColor == i)
                {
                    creatureColorScripts[i].gameObject.SetActive(true);
                    continue;
                }

                creatureColorScripts[i].StopBehaviour();
                creatureColorScripts[i].gameObject.SetActive(false);
            }

            // Set the surrounding plant color to the new color
            surroundingPlant.TeamColour = creatureColor;

            // Change the color of the creature
            gameObject.GetComponent<Renderer>().material.color = ColorEnum.GetColor(newColor);
            for (int i = 0; i < creatureColorMasks.Count; i++) {
                if (i == (int)creatureColor) {
                    creatureColorMasks[i].gameObject.SetActive(true);
                    continue;
                }
                creatureColorMasks[i].gameObject.SetActive(false);
            }
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