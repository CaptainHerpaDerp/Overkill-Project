using Creatures;
using Players;
using System.Collections;
using UnityEngine;

public class GreenSpecialBehaviour : SpecialBehaviour
{
    [SerializeField] private TrajectoryDrawer trajectoryDrawer;
    [SerializeField] private Transform orientation;

    // Use to see if the player is looking at a creature
    [SerializeField] private CreatureSelector creatureSelector;

    [SerializeField] private Player parentPlayer;

    private float x;

    private Coroutine TrajectoryCoroutine;

    private CreatureManager selectedCreature;

    public override void Activate()
    {
        if (onCooldown)
        {
            return;
        }

        if (creatureSelector.selectedCreature != null)
        {
            selectedCreature = creatureSelector.selectedCreature;
            TrajectoryCoroutine ??= StartCoroutine(DoTrajectory());
        }
    }

    private IEnumerator DoTrajectory()
    {
        while (true)
        {
            // Set x to the orientation's y rotation in Vector form
            x = Quaternion.Euler(0, orientation.localEulerAngles.y, 0).eulerAngles.y;

            trajectoryDrawer.DrawTrajectory(x, transform);

            // Check if the player has let got of the trigger
            if (!parentPlayer.IsSpecial)
            {
                print("released");

                // Move the selected creature to the landing point
                if (trajectoryDrawer.IsOnGround())
                {
                    selectedCreature.TeleportTo(trajectoryDrawer.GetLandingPosition());

                    DoCooldown();
                }

                TrajectoryCoroutine = null;

                trajectoryDrawer.HideTrajectory();

                yield break;
            }

            // Check to see if the selected creature is still owned by the player
            if (selectedCreature.CreatureColor != parentPlayer.TeamColor)
            {
                print("lost ownership");

                trajectoryDrawer.HideTrajectory();

                TrajectoryCoroutine = null;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
