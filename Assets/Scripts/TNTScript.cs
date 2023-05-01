using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TNTScript : IPullable
{
    public PullScript Pull;
    public Transform Neck;
    public GameObject Explodable;
    public VisualEffect explosion_effect;
    public VisualEffect destruction_effect;  

    override public bool CanBePulled()
    {
        return true;
    }

    override public TryPullResult GetPulled(LassoScript lasso, PlayerController player)
    {
        if (Explodable != null)
        {
            destruction_effect.Play();
            Destroy(Explodable);
        }
        explosion_effect.Play();
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
