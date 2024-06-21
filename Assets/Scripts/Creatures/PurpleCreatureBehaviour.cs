using Core;
using Creatures;
using GaiaElements;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private NavMeshAgent agent;
    private PlayerLocator playerLocator;
    private Coroutine playerFollowCoroutine;

    [SerializeField] private float TargetTurnDistance;

    [Header("The creature will plant while it is within this radius of the player, otherwise it will move to the player")]
    [SerializeField] private float maxDistanceFromPlayer = 10f;

    [Header("While moving to the player, this is the distance where the creature will stop following and start planting")]
    [SerializeField] private float playerStopDistance;

    private CreatureState state = CreatureState.FOLLOWING_PLAYER;

    [SerializeField] private float followPlayerSpeed = 5f;

    private float playerDistance
    {
        get => Vector3.Distance(transform.position, playerLocator.GetPositionOfPlayer(ColorEnum.TEAMCOLOR.PURPLE));
    }

    private Vector3 playerPosition => playerLocator.GetPositionOfPlayer(ColorEnum.TEAMCOLOR.PURPLE);
    private Transform playerTransform => playerLocator.GetTransformOfTeam(ColorEnum.TEAMCOLOR.PURPLE);

    private void OnEnable()
    {
        playerLocator = PlayerLocator.Instance;
    }

    public override void StopBehaviour()
    {
        base.StopBehaviour();
    }

    public override void Act()
    {
        gameObject.SetActive(true);
        CheckTarget();
        //playerFollowCoroutine ??= StartCoroutine(FollowPlayer());
    }

    //private IEnumerator FollowPlayer()
    //{
    //    while (true)
    //    {
    //        if (state == CreatureState.PLANTING)
    //        {
    //            agent.speed = baseSpeed;

    //            // While planting, check if the player is out of range
    //            if (playerDistance > maxDistanceFromPlayer)
    //            {
    //                print("Following player");
    //                state = CreatureState.FOLLOWING_PLAYER;
    //            }
    //            else
    //            {
    //                if (plantTarget != null)
    //                    agent.SetDestination(plantTarget.position);
    //            }
    //        }

    //        if (state == CreatureState.FOLLOWING_PLAYER)
    //        {
    //            agent.speed = followPlayerSpeed;

    //            // If the player is within the stopping distance, stop following and start planting
    //            if (playerDistance < playerStopDistance)
    //            {
    //                print("Stopping follow");
    //                state = CreatureState.PLANTING;
    //            }
    //            else
    //            {
    //                // Follow the purple player
    //                agent.SetDestination(playerLocator.GetPositionOfPlayer(ColorEnum.TEAMCOLOR.PURPLE));
    //                yield return new WaitForSeconds(0.5f);
    //            }

    //        }

    //        yield return new WaitForFixedUpdate();
    //    }
    //}

    private void CheckTarget()
    {
        if (plantTarget == null)
        {
            ChooseClosestOpponentPlant();
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

    protected override void ChooseClosestOpponentPlant()
    {
        if (plantLocator == null)
        {
            Debug.LogWarning("Plant locator not found, please assign!");
            return;
        }

        List<Plant> opponentPlants = plantLocator.GetSurroundingOpponentPlantsList(playerTransform);

        float smallestWeight = float.MaxValue;
        Plant targetPlant = null;

        foreach (Plant plant in opponentPlants)
        {
            if (plant == null)
            {
                Debug.LogError("Plant component not found");
                continue;
            }

            // Get the forward vector of the current object
            Vector3 forward = transform.forward;

            // Calculate the direction vector from the object to the target
            Vector3 toTarget = (plant.transform.position - transform.position).normalized;

            // Calculate the angle between the forward vector and the direction to the target
            float givenAngle = Vector3.Angle(forward, toTarget);
            float distance = Vector3.Distance(transform.position, plant.transform.position);

            if (givenAngle > 180)
            {
                givenAngle = 360 - givenAngle;
            }

            givenAngle = Mathf.Clamp(givenAngle, 0, weightCoefficient);

            float weight = givenAngle * distance;
            if (weight < smallestWeight)
            {
                smallestWeight = weight;
                targetPlant = plant;
            }
        }

        if (targetPlant != null)
        {
            print("found plant target");
            plantTarget = targetPlant.transform;
            TriggerTargetChange(plantTarget.position);
        }
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
