using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSpecialBehaviour : SpecialBehaviour
{
    [SerializeField] private TrajectoryDrawer trajectoryDrawer;
    [SerializeField] private Transform orientation;

    private float minRot, maxRot;

    public float x;

    public override void Activate()
    {
        throw new System.NotImplementedException();
    }


    private void Update()
    {
        // Set x to the orientation's y rotation in Vector form
        x = Quaternion.Euler(0, orientation.localEulerAngles.y, 0).eulerAngles.y;

        trajectoryDrawer.DrawTrajectory(x, transform);
    }

}
