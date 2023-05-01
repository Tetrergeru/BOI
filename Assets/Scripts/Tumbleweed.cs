using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tumbleweed : MonoBehaviour
{
    public float forceSpeed = 1;
    public float timeFrom = 3;
    public float timeTo = 5;

    float timePoint;
    float nextForceTime;
    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        UpdateForceTime();
    }

    // Update is called once per frame
    void Update()
    {
        timePoint += Time.deltaTime;
        if (timePoint < nextForceTime)
            return;
        UpdateForceTime();
        var force = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
        body.AddForce(force * forceSpeed, ForceMode.Impulse);
    }

    void UpdateForceTime() {
        nextForceTime = Random.Range(timeFrom, timeTo);
        timePoint = 0;
    }
}
