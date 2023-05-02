using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VultureState
{
    Chilling,
    InCombat,
    FlyingHome,
    IsPulled,
    Fleeing
}

public class VultureScript : IPullable
{
    public Rigidbody Body;
    public VultureState State = VultureState.Chilling;
    public Animator Animator;
    public float ViewDistance = 15;
    public Transform Eye;
    public SphereCollider EyeTrigger;
    public Transform FlyingPart;

    public Transform Neck;
    public PullScript Pull;
    public AudioSource Sound;

    private Vector3 _startPosition;
    private float _startDirection;

    private PlayerController _player;
    private bool _see = false;
    private float _timeNotSeeing;
    private float _coolDown;
    private Vector3 _fleingPosition;
    private float _fleingTime = 5f;

    void Start()
    {
        _startPosition = this.transform.position;
        _startDirection = this.transform.rotation.eulerAngles.y;
        EyeTrigger.radius = ViewDistance;
    }

    void FixedUpdate()
    {
        _coolDown = Mathf.Max(0, _coolDown - Time.deltaTime);
        switch (State)
        {
            case VultureState.InCombat:
                if (!ISee(_player.transform))
                {
                    if (!_see)
                        _timeNotSeeing -= Time.deltaTime;
                    if (_timeNotSeeing > 5)
                        StopCombat();

                    _see = false;
                }
                else
                {
                    _see = true;
                    _timeNotSeeing = 0;
                }

                if (Vector3.Distance(this.transform.position, _player.transform.position) < 3f)
                {
                    if (_coolDown < 0.001)
                    {
                        Sound.pitch = Random.Range(0.7f, 1.3f);
                        Sound.Play();
                        Animator.SetTrigger("Attack");
                        _player.GetAttacked(this.transform.position);
                        _coolDown += 40f / 30;
                    }
                }
                else
                {
                    FlyTo(_player.transform.position);
                }

                break;
            case VultureState.FlyingHome:
                FlyTo(_startPosition);
                if (Vector3.Distance(this.transform.position, _startPosition) < 2)
                {
                    SetHeight(0);
                    this.transform.position = _startPosition;
                    this.transform.rotation = Quaternion.Euler(0, _startDirection, 0);
                    State = VultureState.Chilling;
                    Body.velocity = Vector3.zero;
                    Animator.SetBool("Flying", false);
                }
                break;
            case VultureState.Fleeing:
                _fleingTime -= Time.deltaTime;
                if (_fleingTime < 0)
                {
                    Destroy(this.gameObject);
                }
                Sound.pitch += 0.003f;
                FlyTo(_fleingPosition);
                AddHeight(0.1f);
                this.FlyingPart.localScale = Vector3.one * _fleingTime / 4;
                break;
        }
    }

    private void FlyTo(Vector3 position)
    {
        var look = Quaternion.LookRotation(position - this.transform.position).eulerAngles.y;
        this.transform.rotation = Quaternion.Euler(0, look, 0);
        var v = this.transform.forward * 5;
        Body.velocity = new Vector3(v.x, Body.velocity.y, v.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (State != VultureState.Chilling) return;

        var player = other.GetComponent<PlayerController>();
        if (player == null)
            return;

        StartCombat(player);
    }

    private void StartCombat(PlayerController player)
    {
        Sound.Play();
        SetHeight(1.5f);
        _player = player;
        State = VultureState.InCombat;
        Animator.SetBool("Flying", true);
    }

    public void StartFleing(PlayerController player)
    {
        _player = player;
        _player.GetComponent<PlayerTipScript>().AddScore(100);
        Sound.loop = true;
        Sound.pitch = 1.5f;
        Sound.Play();
        var p = this.transform.position;
        this.transform.position = new Vector3(p.x, p.y + 0.5f, p.z);
        this.SetHeight(0);

        Pull.Enabled = false;
        State = VultureState.Fleeing;
        Animator.SetBool("Flying", true);
        _fleingPosition = this.transform.position
            + (this.transform.position - _player.transform.position).normalized * 1000f;
    }

    private void StopCombat()
    {
        _player = null;
        State = VultureState.FlyingHome;
    }

    void SetHeight(float height)
    {
        var pos = this.transform.position;
        FlyingPart.position = new Vector3(pos.x, pos.y + height, pos.z);
    }

    void AddHeight(float height)
    {
        var pos = FlyingPart.position;
        FlyingPart.position = new Vector3(pos.x, pos.y + height, pos.z);
    }

    private bool ISee(Transform player)
    {
        var eyePos = Eye.position;
        var playerPos = player.position;
        var direction = playerPos - eyePos;

        // Debug.DrawLine(eyePos, eyePos + direction, Color.red, 0.1f);
        // Debug.DrawLine(eyePos, eyePos + transform.forward * ViewDistance, Color.blue, 0.1f);

        var ray = new Ray(eyePos, direction);
        var hit = Physics.Raycast(ray, out var obj, ViewDistance);
        if (!hit)
        {
            return false;
        }

        return obj.transform.IsChildOf(player) || obj.transform == player;
    }

    override public bool CanBePulled()
    {
        return State != VultureState.Fleeing;
    }

    public override float SpeedReduction()
    {
        return 10;
    }

    public override TryPullResult GetPulled(LassoScript lasso, PlayerController player)
    {
        State = VultureState.IsPulled;
        Pull.LassoLoop = lasso.LoopBone;
        Pull.LassoMountPoint = player.LassoMountPoint;
        Pull.Enabled = true;
        return TryPullResult.StartPulling;
    }

    public override Vector3 NeckPosition()
    {
        return Neck.position;
    }
}
