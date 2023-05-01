using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullScript : MonoBehaviour
{
    public Transform NeckPoint;

    public Transform LassoLoop;
    public Transform LassoMountPoint;
    public bool Enabled;

    public void Update()
    {
        if (!Enabled) return;

        var neck = NeckPoint.position;
        var it = this.transform.position;

        var horDist = Vector2.Distance(new Vector2(neck.x, neck.z), new Vector2(it.x, it.z));

        var vec = (LassoLoop.position - LassoMountPoint.position).normalized;
        vec.y = 0;

        var neckHeight = NeckPoint.position.y - this.transform.position.y;

        // Debug.Log($"neckHeight: {neckHeight}, NeckPoint.position.y: {NeckPoint.position.y}, this.transform.position.y: {this.transform.position.y}");
        // Debug.Log($"LassoLoop.position: {LassoLoop.position}");

        this.transform.rotation = Quaternion.LookRotation(-vec, Vector3.up);
        this.transform.position = LassoLoop.position
            + vec * horDist
            + Vector3.down * neckHeight;
    }
}
