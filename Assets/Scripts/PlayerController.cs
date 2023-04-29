using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody Body;
    public Transform CameraMountPoint;
    public Transform Camera;

    public GameObject LassoPrefab;
    public Transform LassoMountPoint;

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
        if (Input.GetKey(KeyCode.E))
        {
            var lasso = Instantiate(LassoPrefab);
            var lassoScript = lasso.GetComponent<LassoScript>();
            StartCoroutine(ThrowLasso(lassoScript));
        }
    }

    IEnumerator ThrowLasso(LassoScript lassoScript)
    {
        var collisionFlag = false;
        lassoScript.CollisionCallback = () =>
        {
            collisionFlag = true;
        };

        lassoScript.StartPoint = LassoMountPoint.position;
        var start = LassoMountPoint.position;
        var forward = this.transform.forward;
        var amount = 1.0f;
        lassoScript.EntPoint = start + forward * amount;

        yield return new WaitForFixedUpdate();

        while (!collisionFlag && amount < 20.0f)
        {
            lassoScript.StartPoint = LassoMountPoint.position;
            amount += 0.1f;
            lassoScript.EntPoint = start + forward * amount;

            yield return new WaitForFixedUpdate();
        }
        Destroy(lassoScript.gameObject);
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
        Camera.RotateAround(
            CameraMountPoint.position,
            CameraMountPoint.right,
            mouseY * Time.deltaTime * 200
        );

        Body.velocity = (transform.forward * Input.GetAxis("Vertical") +
            transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * 400;
    }
}
