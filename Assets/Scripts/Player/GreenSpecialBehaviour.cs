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

    private Coroutine OnActivate; 

    public override void Activate()
    {
        if (creatureSelector.selectedCreature != null)
        {
            OnActivate ??= StartCoroutine(DoTrajectory());
        }
    }


    private IEnumerator DoTrajectory()
    {
        while (true)
        {
            // Set x to the orientation's y rotation in Vector form
            x = Quaternion.Euler(0, orientation.localEulerAngles.y, 0).eulerAngles.y;

            trajectoryDrawer.DrawTrajectory(x, transform);

            yield return new WaitForFixedUpdate();
        }
    }

}
