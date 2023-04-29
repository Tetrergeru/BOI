using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalState
{
    Chilling,
    Pulled,
    Rided,
    WalkingAround,
    RunningAround,
}

public class AnimalScript : MonoBehaviour
{
    public Rigidbody Body;
    public Animator Animator;

    public AnimalState State = AnimalState.Chilling;

    public Transform MountPoint;
    public CapsuleCollider BodyCollider;

    public Transform NeckPoint;

    public string Name;

    private Transform _lassoLoop;
    private Transform _lassoMountPoint;

    private float _timeUntilWalkCheck = 0.5f;
    private Vector2 _walkAroundVector;
    public float WalkChansInHalfSecond = 0.01f;

    void Update()
    {
        if (State == AnimalState.Chilling)
        {
            SetSpeed(0);

            _timeUntilWalkCheck -= Time.deltaTime;
            if (_timeUntilWalkCheck < 0)
            {
                _timeUntilWalkCheck += 0.5f;
                if (Random.Range(0.0f, 1.0f) < WalkChansInHalfSecond)
                {
                    State = (Random.Range(0.0f, 1.0f) < 0.3f ? AnimalState.RunningAround : AnimalState.WalkingAround);
                    _walkAroundVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                }
            }
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
        else if (State == AnimalState.WalkingAround || State == AnimalState.RunningAround)
        {
            var direction = new Vector3(_walkAroundVector.x, 0, _walkAroundVector.y);

            Body.velocity = direction * Time.deltaTime * (State == AnimalState.RunningAround ? 300 : 50);
            this.transform.rotation = Quaternion.LookRotation(Body.velocity, Vector3.up);
            SetSpeed(Body.velocity.magnitude);

            _timeUntilWalkCheck -= Time.deltaTime;
            if (_timeUntilWalkCheck < 0)
            {
                _timeUntilWalkCheck += 0.5f;
                if (Random.Range(0.0f, 1.0f) < WalkChansInHalfSecond)
                {
                    State = AnimalState.Chilling;
                    Body.velocity = Vector3.zero;
                }
                else if (Random.Range(0.0f, 1.0f) < 0.2)
                {
                    _walkAroundVector = (
                        _walkAroundVector + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.5f
                    ).normalized;
                }
            }
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
