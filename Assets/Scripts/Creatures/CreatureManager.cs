using Creatures;
using GaiaElements;
using System.Collections;
using System.Collections.Generic;
using TeamColors;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class CreatureManager : MonoBehaviour {

    [SerializeField] 
    private NavMeshAgent agent;
    
    public List<Creature> CreatureColorScripts;
    private ColorEnum.TEAMCOLOR creatureColor = ColorEnum.TEAMCOLOR.DEFAULT;

    private float conversionProgress;
    [SerializeField] float  conversionThreshold = 100f, baseConversionSpeed, conversionRevertSpeed;

    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private GameObject conversionRing;
    [SerializeField] private float conversionRingMinY, conversionRingMaxY;
    [SerializeField] private float colorShiftMod;
    private bool isConverting;

    private Color targetColor;
    private Color initialColor;

    private void Start() {
        Init();
        ChangeThisCreatureColor(creatureColor);
        StartCoroutine(DoConversion());
    }

    private void Init() {
        CreatureColorScripts.Add(transform.Find("RedLogic").GetComponent<Creature>());
        CreatureColorScripts.Add(transform.Find("GreenLogic").GetComponent<Creature>());
        CreatureColorScripts.Add(transform.Find("BlueLogic").GetComponent<Creature>());
        CreatureColorScripts.Add(transform.Find("PurpleLogic").GetComponent<Creature>());
        CreatureColorScripts.Add(transform.Find("DefaultLogic").GetComponent<Creature>());

        foreach (Creature creature in CreatureColorScripts) {
            creature.ONOwnColorChanged += ChangeThisCreatureColor;
            creature.ONPlantColorChanged += ChangePlantColor;
            creature.ONTargetChanged += SetNewTarget;
        }

        initialColor = ColorEnum.GetColor(ColorEnum.TEAMCOLOR.DEFAULT);
        meshRenderer.material.color = initialColor;
    }


    public void Convert(ColorEnum.TEAMCOLOR newColor)
    {
        isConverting = true;
        targetColor = ColorEnum.GetColor(newColor);

        if (newColor == creatureColor)
        {
            conversionProgress = 0;
            return;
        }

        if (conversionProgress >= conversionThreshold - baseConversionSpeed)
        {
            print("Converted");
            ChangeThisCreatureColor(newColor);
            conversionProgress = 0;
            initialColor = targetColor;
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
    }

    private IEnumerator DoConversion()
    {
        while (true)
        {
            if (isConverting)
            {
                if (!conversionRing.activeInHierarchy)
                    conversionRing.SetActive(true);

                conversionProgress += baseConversionSpeed;
                conversionProgress = Mathf.Clamp(conversionProgress, 0, conversionThreshold);

                float t = conversionProgress / conversionThreshold;
                meshRenderer.material.color = Color.Lerp(initialColor, targetColor, t);

                print($"conversion: {conversionProgress} / {conversionThreshold}");
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

                if (conversionProgress <= 0 && conversionRing.activeInHierarchy)
                {
                    conversionRing.SetActive(false);
                }
            }

            conversionRing.transform.position = new Vector3(conversionRing.transform.position.x, Mathf.Lerp(conversionRingMinY, conversionRingMaxY, conversionProgress / conversionThreshold), conversionRing.transform.position.z);

            isConverting = false;
            yield return new WaitForEndOfFrame();
        }
    }

    private void ChangeThisCreatureColor(ColorEnum.TEAMCOLOR newColor)
    {
        creatureColor = newColor;
        for (int i = 0; i < CreatureColorScripts.Count; i++) {
            if ((int) newColor == i) {
                CreatureColorScripts[i].gameObject.SetActive(true);
                continue;
            }
            CreatureColorScripts[i].gameObject.SetActive(false);
        }
        gameObject.GetComponent<Renderer>().material.color = ColorEnum.GetColor(newColor);
    }

    private void ChangePlantColor(Plant plant, ColorEnum.TEAMCOLOR newColor) {
        if (plant.plantRenderer.enabled == false) {
            plant.Activate(newColor);
            return;
        }

        plant.TeamColor = newColor;
    }

    private void SetNewTarget(Vector3 newTarget) {
        agent.SetDestination(newTarget);
    }

}