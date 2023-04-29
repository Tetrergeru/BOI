using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalState
{
    Chilling,
    Pulled,
}

public class AnimalScript : MonoBehaviour
{
    public AnimalState State = AnimalState.Chilling;

    public Transform MountPoint;
    public CapsuleCollider BodyCollider;

    public Transform NeckPoint;
    public SphereCollider NeckCollider;

    public string Name;

    private Transform _lassoLoop;
    private Transform _lassoMountPoint;

    void Update()
    {
        if (State == AnimalState.Pulled)
        {
            var neck = NeckPoint.position;
            var it = this.transform.position;

            var horDist = Vector2.Distance(new Vector2(neck.x, neck.z), new Vector2(it.x, it.z));

            var vec = (_lassoLoop.position - _lassoMountPoint.position).normalized;
            vec.y = 0;

            this.transform.rotation = Quaternion.LookRotation(vec, Vector3.up);
            this.transform.position = _lassoLoop.position
                + vec * horDist
                + Vector3.down * NeckPoint.localPosition.y * this.transform.localScale.x;
        }
    }

    public void GetPulled(LassoScript lasso, PlayerController player)
    {
        _lassoLoop = lasso.LoopBone;
        _lassoMountPoint = player.LassoMountPoint;
        State = AnimalState.Pulled;
    }
}
