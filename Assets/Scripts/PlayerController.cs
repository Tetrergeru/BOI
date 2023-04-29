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

    private bool _lassoThrown = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();
        Lasso();
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
            collisionFlag = true;
            animal = a;
            if (a != null)
                Debug.Log(a.Name);
        };

        lassoScript.StartPoint = LassoMountPoint.position;
        var start = LassoMountPoint.position;
        var forward = CameraMountPoint.forward;
        var amount = 2.0f;
        lassoScript.EntPoint = start + forward * amount;

        lassoScript.Update();

        yield return new WaitForFixedUpdate();

        while (!collisionFlag && amount < 5.0f)
        {
            lassoScript.StartPoint = LassoMountPoint.position;
            lassoScript.LoopSize = amount / 2;
            amount += 0.1f;
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
            Destroy(animal.gameObject);
        }
        Destroy(lassoScript.gameObject);
        _lassoThrown = false;
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

        Body.velocity = (transform.forward * Input.GetAxis("Vertical") +
            transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * 400;
    }
}
