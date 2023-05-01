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
    InPen,
}

public enum AnimalType
{
    Goose,
    Cow,
    Bison,
    Horse,
}

public class AnimalScript : IPullable
{
    public Rigidbody Body;
    public Animator Animator;

    public AnimalState State = AnimalState.Chilling;

    public Transform MountPoint;

    public Transform NeckPoint;

    public AnimalType Type;

    public PullScript Pull;

    private float _timeUntilWalkCheck = 0.5f;
    private Vector2 _walkAroundVector;
    public float WalkChansInHalfSecond = 0.01f;

    public float SpeedModifier = 1f;

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

    void LateUpdate()
    {
        if (State == AnimalState.Chilling || State == AnimalState.InPen)
        {
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
    }

    public int GetInPen()
    {
        if (State == AnimalState.InPen)
            return 0;

        SetSpeed(0);
        State = AnimalState.InPen;
        return 100;
    }

    override public bool CanBePulled()
    {
        return State == AnimalState.Chilling
            || State == AnimalState.WalkingAround
            || State == AnimalState.RunningAround;
    }

    override public TryPullResult GetPulled(LassoScript lasso, PlayerController player)
    {
        Pull.LassoLoop = lasso.LoopBone;
        Pull.LassoMountPoint = player.LassoMountPoint;
        Pull.Enabled = true;

        State = AnimalState.Pulled;

        if (Animator != null && Animator.isActiveAndEnabled) 
            Animator.SetBool("Resist", true);

        return TryPullResult.StartPulling;
    }

    public void GetRided()
    {
        Destroy(Body);

        State = AnimalState.Rided;
        Pull.LassoLoop = null;
        Pull.LassoMountPoint = null;
        Pull.Enabled = false;

        if (Animator == null || !Animator.isActiveAndEnabled) return;

        SetSpeed(0);
        Animator.SetBool("Resist", false);
    }

    public void StopBeingRided()
    {
        Body = this.gameObject.AddComponent<Rigidbody>();
        State = AnimalState.Chilling;
        foreach (var t in transform.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = LayerMask.NameToLayer("Animal");
        }
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

    override public float SpeedReduction()
    {
        return 10;
    }

    override public Vector3 NeckPosition()
    {
        return NeckPoint.position;
    }
}
