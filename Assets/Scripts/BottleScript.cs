using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleScript : IPullable
{
    public PullScript Pull;
    public Transform Neck;

    override public bool CanBePulled()
    {
        return true;
    }

    override public void GetPulled(LassoScript lasso, PlayerController player)
    {
        Pull.LassoLoop = lasso.LoopBone;
        Pull.LassoMountPoint = player.LassoMountPoint;
        Pull.Enabled = true;
    }

    override public Vector3 NeckPosition()
    {
        return Neck.position;
    }

    override public float SpeedReduction()
    {
        return 1;
    }
}
