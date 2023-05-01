using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    public Rigidbody Body;
    public Animator Animator;
    public VisualEffect Dust;

    public Transform CameraMountPoint;
    public Transform Camera;
    public CameraShake CameraShake;

    public GameObject LassoPrefab;
    public GameObject LassoPrefab2;
    public Transform LassoMountPoint;

    public Transform BoiTransform;
    public List<AnimalScript> Tower;

    private bool _lassoThrown = false;
    private Vector3 _cameraVector;
    private float _cameraDistance;
    private float _towerHeight;
    private Vector2 _towerAngle;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _cameraVector = Camera.localPosition.normalized;
        _cameraDistance = Camera.localPosition.magnitude;
    }

    void FixedUpdate()
    {
        _towerAngle *= 0.9f;

        Move();
        Lasso();
        TowerControls();

        UpdateTowerAngle();
        UpdateAnimationSpeed();
    }

    void LateUpdate()
    {
        // Camera.localPosition = _cameraVector * (_cameraDistance * (Tower.Count / 2f + 1));
        var camRot = Camera.transform.rotation.eulerAngles;
        this.transform.rotation = Quaternion.Euler(0, camRot.y, 0);
    }

    void Lasso()
    {
        if (Input.GetKey(KeyCode.E) && !_lassoThrown)
        {
            _lassoThrown = true;
            var lasso = Instantiate(LassoPrefab);
            var lassoScript = lasso.GetComponent<LassoScript>();
            StartCoroutine(ThrowLasso(lassoScript));
        }
        if (Input.GetKey(KeyCode.R) && !_lassoThrown)
        {
            _lassoThrown = true;
            var lasso = Instantiate(LassoPrefab2);
            var lassoScript = lasso.GetComponent<LassoScript>();
            StartCoroutine(ThrowLasso(lassoScript));
        }
    }

    IEnumerator ThrowLasso(LassoScript lassoScript)
    {
        var collisionFlag = false;
        AnimalScript animal = null;
        lassoScript.CollisionCallback = (a) =>
        {
            if (collisionFlag) return;
            if (a == null || !a.CanBePulled()) return;
            collisionFlag = true;
            animal = a;
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

        if (animal != null)
        {
            pullingSpeed = 0.07f;
            end = animal.NeckPoint.position;

            CameraShake.Shake(0.7f);
            animal.GetPulled(lassoScript, this);
        }

        while (amount > 1.0)
        {
            lassoScript.StartPoint = LassoMountPoint.position;
            amount -= pullingSpeed;
            lassoScript.EntPoint = LassoMountPoint.position + (end - LassoMountPoint.position).normalized * amount;
            yield return new WaitForFixedUpdate();
        }

        if (animal != null)
        {
            AddAnimalToTower(animal);
        }
        Destroy(lassoScript.gameObject);
        _lassoThrown = false;
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
        Animator.SetFloat("Speed", Body.velocity.magnitude);
        Dust.SetInt("SpawnRate", (int)(Body.velocity.magnitude * 5));
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

        Body.AddForce(new Vector3(0, 700, 0));

        BoiTransform.transform.parent = this.transform;
        this.transform.position = BoiTransform.transform.position;
        BoiTransform.transform.localPosition = Vector3.zero;
    }

    void Move()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = -Input.GetAxis("Mouse Y");

        _towerAngle.x += Input.GetAxis("Vertical") * Time.deltaTime * 30;
        _towerAngle.y += Input.GetAxis("Horizontal") * Time.deltaTime * 30;

        Body.velocity = (transform.forward * Input.GetAxis("Vertical")
            + transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * 400
            + transform.up * Body.velocity.y;
    }
}
