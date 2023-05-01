using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTScript : IPullable
{
    public PullScript Pull;
    public Transform Neck;
    public GameObject Explodable;

    override public bool CanBePulled()
    {
        return true;
    }

    override public TryPullResult GetPulled(LassoScript lasso, PlayerController player)
    {
        if (Explodable != null)
        {
            Destroy(Explodable);
        }
        player.GetComponent<PlayerTipScript>().AddTNT(1);
        Destroy(this.gameObject);
        return TryPullResult.Fail;
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
