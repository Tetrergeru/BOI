using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody Body;
    public Transform CameraMountPoint;
    public Transform Camera;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
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
