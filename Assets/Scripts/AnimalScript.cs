using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalState
{
    Chilling,
    Pulled,
    Rided,
}

public class AnimalScript : MonoBehaviour
{
    public Rigidbody Body;
    public Animator Animator;

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
        if (State == AnimalState.Chilling)
        {
            SetSpeed(0);
        }
        else if (State == AnimalState.Pulled)
        {
            var neck = NeckPoint.position;
            var it = this.transform.position;

            var horDist = Vector2.Distance(new Vector2(neck.x, neck.z), new Vector2(it.x, it.z));

            var vec = (_lassoLoop.position - _lassoMountPoint.position).normalized;
            vec.y = 0;

            var neckHeight = NeckPoint.position.y - this.transform.position.y;

            this.transform.rotation = Quaternion.LookRotation(-vec, Vector3.up);
            this.transform.position = _lassoLoop.position
                + vec * horDist
                + Vector3.down * neckHeight * this.transform.localScale.x;
        }
    }

    public void GetPulled(LassoScript lasso, PlayerController player)
    {
        _lassoLoop = lasso.LoopBone;
        _lassoMountPoint = player.LassoMountPoint;
        State = AnimalState.Pulled;

        if (Animator == null || !Animator.isActiveAndEnabled) return;

        Animator.SetBool("Resist", true);
    }

    public void GetRided()
    {
        Destroy(Body);
        State = AnimalState.Rided;
        _lassoLoop = null;
        _lassoMountPoint = null;

        if (Animator == null || !Animator.isActiveAndEnabled) return;

        Animator.SetBool("Resist", false);
    }

    public void RideSpeed(float speed)
    {
        if (State != AnimalState.Rided) return;

        SetSpeed(speed);
    }

    private void SetSpeed(float speed)
    {
        if (Animator == null || !Animator.isActiveAndEnabled) return;

        Animator.SetFloat("Speed", speed);
    }
}
