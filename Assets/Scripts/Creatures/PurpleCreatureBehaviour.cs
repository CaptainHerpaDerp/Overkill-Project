using Core;
using Creatures;
using GaiaElements;
using System.Collections;
using TeamColors;
using UnityEngine;
using UnityEngine.AI;

public class PurpleCreatureBehaviour : Creature
{
    private enum CreatureState
    {
        FOLLOWING_PLAYER,
        PLANTING
    }

    private NavMeshAgent agent;
    private PlayerLocator playerLocator;

    private Coroutine playerFollowCoroutine;

    private int[] playerTeamScore = new int[5];
    [SerializeField] private float TargetTurnDistance;

    [Header("The creature will plant while it is within this radius of the player, otherwise it will move to the player")]
    [SerializeField] private float maxDistanceFromPlayer = 10f;

    [Header("While moving to the player, this is the distance where the creature will stop following and start planting")]
    [SerializeField] private float playerStopDistance;

    private CreatureState state = CreatureState.FOLLOWING_PLAYER;

    private float playerDistance
    {
        get => Vector3.Distance(transform.position, playerLocator.GetPositionOfPlayer(ColorEnum.TEAMCOLOR.PURPLE));
    }

    private void OnEnable()
    {
        agent = transform.parent.GetComponent<NavMeshAgent>();
        playerLocator = PlayerLocator.Instance;
        ScoreReceptionManager.OnValueChanged += UpdateScore;
    }

    private void OnDisable()
    {
        ScoreReceptionManager.OnValueChanged -= UpdateScore;
    }

    private void UpdateScore(int playerIndex, int newScore)
    {
        playerTeamScore[playerIndex] = newScore;
    }

    public override void Act() 
    {
        gameObject.SetActive(true);
        CheckTarget();
        playerFollowCoroutine ??= StartCoroutine(FollowPlayer());
    }

    private IEnumerator FollowPlayer()
    {
        while (true)
        {
            if (state == CreatureState.PLANTING)
            {
                // While planting, check if the player is out of range
                if (playerDistance > maxDistanceFromPlayer)
                {
                    print("Following player");

                    state = CreatureState.FOLLOWING_PLAYER;
                }
                else
                {
                    if (plantTarget != null)
                    agent.SetDestination(plantTarget.position);
                }
            }

            if (state == CreatureState.FOLLOWING_PLAYER)
            {
                // If the player is within the stopping distance, stop following and start planting
                if (playerDistance < playerStopDistance)
                {
                    print("Stopping follow");
                    state = CreatureState.PLANTING;
                }
                else
                {
                    // Follow the purple player
                    agent.SetDestination(playerLocator.GetPositionOfPlayer(ColorEnum.TEAMCOLOR.PURPLE));
                    yield return new WaitForSeconds(0.5f);  
                }

            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void ChooseClosestPlant()
    {
        if (plantsHierarchyParent == null)
        {
            plantsHierarchyParent = GameObject.Find("Plants").transform;
        }

        float distToPlant = maxDistanceFromPlayer;
        Plant targetPlant = null;

        foreach (Transform plant in plantsHierarchyParent)
        {
            Plant plantScript = plant.GetComponent<Plant>();
            if (plantScript == null)
            {
                Debug.LogError("Plant component not found");
                continue;
            }

            if (plantScript.TeamColor == ColorEnum.TEAMCOLOR.PURPLE)
                continue;

            float TMPDist = Vector3.Distance(transform.position, plant.transform.position);
            if (TMPDist > distToPlant) continue;
            distToPlant = TMPDist;
            targetPlant = plantScript;
        }

        if (targetPlant != null)
        {
            plantTarget = targetPlant.transform;
            //TriggerTargetChange(plantTarget.position);
        }
        else
        {
            Debug.LogError("Blue plantTarget is null");
        }
    }

    private void CheckTarget()
    {
        if (plantTarget == null)
        {
            ChooseClosestPlant();
        }

        if (plantTarget == null)
        {
            Debug.Log("There are no enemies to conquer");
            return;
        }

        if (plantTarget.GetComponent<Plant>().TeamColor == ColorEnum.TEAMCOLOR.PURPLE)
        {
            plantTarget = null;
            return;
        }

        CheckDistanceToTarget();
    }

    private void CheckDistanceToTarget()
    {
        if (Vector3.Distance(transform.position, plantTarget.position) <= TargetTurnDistance)
        {
            TriggerPlantColorChange(plantTarget.gameObject.GetComponent<Plant>(), ColorEnum.TEAMCOLOR.PURPLE);
            plantTarget = null;
        }
    }
}
