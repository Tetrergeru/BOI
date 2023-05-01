using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    public Rigidbody Body;
    public Animator Animator;
    public VisualEffect Dust;
    public PlayerTipScript Tip;

    public Transform CameraMountPoint;
    public Transform Camera;
    public CameraShake CameraShake;

    public GameObject LassoPrefab;
    public Transform LassoMountPoint;

    public AudioSource WalkAudio;
    public GameObject WhooshSound;

    public Transform BoiTransform;
    public List<AnimalScript> Tower;

    public CreditsScript Credits;

    private bool _lassoThrown = false;
    private Vector3 _cameraVector;
    private float _cameraDistance;
    private float _towerHeight;
    private Vector2 _towerAngle;

    private float _speedModifier = 1;
    private float _stunTime = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _cameraVector = Camera.localPosition.normalized;
        _cameraDistance = Camera.localPosition.magnitude;
    }

    void FixedUpdate()
    {
        _towerAngle *= 0.9f;

        if (Input.GetKeyDown(KeyCode.P))
        {
            Credits.StartCredits();
        }

        Move();
        Lasso();
        TowerControls();

        UpdateTowerAngle();
        UpdateAnimationSpeed();
    }

    void Lasso()
    {
        if (Input.GetMouseButton(0) && !_lassoThrown)
        {
            Instantiate(WhooshSound);
            _lassoThrown = true;
            var lasso = Instantiate(LassoPrefab);
            var lassoScript = lasso.GetComponent<LassoScript>();
            StartCoroutine(ThrowLasso(lassoScript));
        }
    }

    IEnumerator ThrowLasso(LassoScript lassoScript)
    {
        var collisionFlag = false;
        IPullable pullable = null;
        lassoScript.CollisionCallback = (a) =>
        {
            if (collisionFlag) return;
            if (a == null || !a.CanBePulled()) return;

            collisionFlag = true;
            pullable = a;
        };

        lassoScript.StartPoint = LassoMountPoint.position;
        var start = LassoMountPoint.position;
        var forward = Camera.forward;
        var amount = 2.0f;
        lassoScript.EntPoint = start + forward * amount;

        lassoScript.Update();

        yield return new WaitForFixedUpdate();

        while (!collisionFlag && amount < 10.0f)
        {
            lassoScript.StartPoint = LassoMountPoint.position;
            lassoScript.LoopSize = amount / 2;
            amount += 0.5f;
            lassoScript.EntPoint = start + forward * amount;

            yield return new WaitForFixedUpdate();
        }

        var steps = 20.0f;
        for (var curve = 1.0f; curve > 0; curve -= 1 / steps)
        {
            lassoScript.LoopSize -= (amount / 2 - 0.3f) / steps;
            lassoScript.CurveStrength = curve;
            yield return new WaitForFixedUpdate();
        }

        lassoScript.lassoMode = LassoMode.Straight;

        var pullingSpeed = 0.7f;
        var end = start + forward * amount;

        if (pullable != null)
        {
            var tryPullResult = pullable.GetPulled(lassoScript, this);
            if (tryPullResult == TryPullResult.StartPulling)
            {
                pullingSpeed = 0.07f;
                end = pullable.NeckPosition();
                lassoScript.EntPoint = end;

                CameraShake.Shake(0.7f);
            }
            else
            {
                pullable = null;
            }
        }

        while (amount > 1.0)
        {
            lassoScript.StartPoint = LassoMountPoint.position;
            amount -= pullingSpeed;
            lassoScript.EntPoint = LassoMountPoint.position + (end - LassoMountPoint.position).normalized * amount;
            yield return new WaitForFixedUpdate();
        }

        if (pullable != null)
        {
            OnPulled(pullable);
        }
        Destroy(lassoScript.gameObject);
        _lassoThrown = false;
    }

    void OnPulled(IPullable pullable)
    {
        switch (pullable)
        {
            case AnimalScript animal:
                AddAnimalToTower(animal);
                break;
            case BottleScript bottle:
                Destroy(bottle.gameObject);
                Tip.AddScore(50);
                Tip.AddBottles(1);
                break;
            case VultureScript vulture:
                vulture.StartFleing();
                break;
        }
    }

    void AddAnimalToTower(AnimalScript animal)
    {
        animal.GetRided();
        animal.transform.parent = this.transform;

        animal.transform.localRotation = new Quaternion();

        Tower.Add(animal);

        RecalculateTower();
    }

    void RecalculateTower()
    {
        var height = 0f;
        var parent = this.transform;
        foreach (var animal in Tower)
        {
            var animalHeight = animal.transform.localScale.x * animal.MountPoint.localPosition.y;
            height += animalHeight;

            animal.transform.parent = parent;
            animal.transform.localPosition = Vector3.zero;

            parent = animal.MountPoint;
        }
        _towerHeight = height;

        if (Tower.Count > 0)
        {
            _speedModifier = Tower[0].SpeedModifier;
        }

        BoiTransform.parent = parent;
        BoiTransform.transform.localPosition = Vector3.zero;

        foreach (var child in this.transform.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    void UpdateAnimationSpeed()
    {
        if (Tower.Count > 0)
        {
            Animator.SetBool("Ride", true);
            Tower[0].RideSpeed(Body.velocity.magnitude);
        }
        else
        {
            Animator.SetBool("Ride", false);
        }
        WalkAudio.pitch = Body.velocity.magnitude / 10f;

        Animator.SetFloat("Speed", Body.velocity.magnitude);
        Dust.SetInt("SpawnRate", (int)(Mathf.Pow(Body.velocity.magnitude, 2) * 4));
    }

    void UpdateTowerAngle()
    {
        foreach (var animal in Tower)
        {
            animal.transform.localRotation = Quaternion.Euler(_towerAngle.x, 0, _towerAngle.y);
        }
        BoiTransform.transform.localRotation = Quaternion.Euler(_towerAngle.x, 0, _towerAngle.y);
    }

    void TowerControls()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Tower.Count != 0)
        {
            BlowUpTower();
        }
    }

    void BlowUpTower()
    {
        foreach (var animal in Tower)
        {
            animal.transform.parent = this.transform.parent;
            animal.StopBeingRided();
            animal.Body.velocity = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 20;
        }
        var towerLen = Tower.Count;
        Tower = new List<AnimalScript>();
        _speedModifier = 1;

        Body.AddForce(new Vector3(0, 700, 0));

        BoiTransform.transform.parent = this.transform;
        this.transform.position = BoiTransform.transform.position;
        BoiTransform.transform.localPosition = Vector3.zero;
    }

    void Move()
    {
        _stunTime = Mathf.Max(_stunTime - Time.deltaTime, 0);
        if (_stunTime > 0.01) return;

        _towerAngle.x += Input.GetAxis("Vertical") * Time.deltaTime * 30;
        _towerAngle.y += Input.GetAxis("Horizontal") * Time.deltaTime * 30;

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f)
        {
            var camRot = Camera.transform.rotation.eulerAngles;
            this.transform.rotation = Quaternion.Euler(0, camRot.y, 0);
        }

        var direction = (
            transform.forward * Input.GetAxis("Vertical")
            + transform.right * Input.GetAxis("Horizontal")
        ).normalized;

        Body.velocity = direction * Time.deltaTime * 400 * _speedModifier
            + transform.up * Body.velocity.y;
    }

    public void GetAttacked(Vector3 attackerPosition)
    {
        Body.AddForce((this.transform.position - attackerPosition).normalized * 800);
        _stunTime += 0.2f;
    }
}
