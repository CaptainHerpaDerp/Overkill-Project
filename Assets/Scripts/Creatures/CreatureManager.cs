using Creatures;
using GaiaElements;
using System.Collections.Generic;
using TeamColors;
using UnityEngine;
using UnityEngine.AI;

public class CreatureManager : MonoBehaviour {

    [SerializeField] 
    private NavMeshAgent agent;
    
    public List<Creature> CreatureColorScripts;
    private ColorEnum.TEAMCOLOR creatureColor = ColorEnum.TEAMCOLOR.DEFAULT;

    private void Start() {
        Init();
        ChangeThisCreatureColor(creatureColor);
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
    }

    private void Update()
    {
        CreatureColorScripts[(int)creatureColor].Act();
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