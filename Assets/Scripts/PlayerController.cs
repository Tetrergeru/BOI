using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody Body;

    public Transform CameraMountPoint;
    public Transform Camera;
    public CameraShake CameraShake;

    public GameObject LassoPrefab;
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

    void Update()
    {
        _towerAngle *= 0.9f;

        Move();
        Lasso();

        UpdateTowerAngle();
        Debug.Log($"_towerAngle {_towerAngle}");
    }

    void LateUpdate()
    {
        Camera.localPosition = _cameraVector * (_cameraDistance + _towerHeight);
        // Camera.transform.LookAt(transform);
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
    }

    IEnumerator ThrowLasso(LassoScript lassoScript)
    {
        var collisionFlag = false;
        AnimalScript animal = null;
        lassoScript.CollisionCallback = (a) =>
        {
            if (collisionFlag) return;
            if (a != null && a.State != AnimalState.Chilling) return;
            collisionFlag = true;
            animal = a;

        };

        lassoScript.StartPoint = LassoMountPoint.position;
        var start = LassoMountPoint.position;
        var forward = CameraMountPoint.forward;
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

            yield return new WaitForEndOfFrame();
        }

        var steps = 20.0f;
        for (var curve = 1.0f; curve > 0; curve -= 1 / steps)
        {
            lassoScript.LoopSize -= (amount / 2 - 0.3f) / steps;
            lassoScript.CurveStrength = curve;
            yield return new WaitForEndOfFrame();
        }

        lassoScript.lassoMode = LassoMode.Straight;

        var pullingSpeed = 0.5f;
        var end = start + forward * amount;

        if (animal != null)
        {
            pullingSpeed = 0.1f;
            end = animal.NeckPoint.position;

            CameraShake.Shake(0.7f);
            animal.GetPulled(lassoScript, this);
        }

        while (amount > 1.0)
        {
            lassoScript.StartPoint = LassoMountPoint.position;
            amount -= pullingSpeed;
            lassoScript.EntPoint = LassoMountPoint.position + (end - LassoMountPoint.position).normalized * amount;
            yield return new WaitForEndOfFrame();
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
    }

    void UpdateTowerAngle()
    {
        foreach (var animal in Tower)
        {
            animal.transform.localRotation = Quaternion.Euler(_towerAngle.x, 0, _towerAngle.y);
        }
        BoiTransform.transform.localRotation = Quaternion.Euler(_towerAngle.x, 0, _towerAngle.y);
    }

    void Move()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

        this.transform.RotateAround(
            this.transform.position,
            Vector3.up,
            mouseX * Time.deltaTime * 200
        );
        CameraMountPoint.RotateAround(
            CameraMountPoint.position,
            CameraMountPoint.right,
            mouseY * Time.deltaTime * 200
        );

        _towerAngle.x += Input.GetAxis("Vertical") * Time.deltaTime * 10;
        _towerAngle.y += Input.GetAxis("Horizontal") * Time.deltaTime * 10;

        Body.velocity = (transform.forward * Input.GetAxis("Vertical") +
            transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * 400;
    }
}
