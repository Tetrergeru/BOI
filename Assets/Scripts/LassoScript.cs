using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LassoMode
{
    Curved,
    Straight,
}

public class LassoScript : MonoBehaviour
{
    public List<Transform> Bones;
    public Transform LoopBone;
    public LassoMode lassoMode;

    public Vector3 StartPoint = Vector3.zero;
    public Vector3 EntPoint = Vector3.zero;
    public Action CollisionCallback = () => { };

    void Update()
    {
        if (lassoMode == LassoMode.Straight)
        {
            DrawStraight();
        }

    }

    [ContextMenu("Draw Straight")]
    void DrawStraight()
    {
        this.transform.position = StartPoint;
        this.transform.rotation = Quaternion.LookRotation(EntPoint - StartPoint);

        var distance = Vector3.Distance(EntPoint, StartPoint);
        var scale = this.transform.lossyScale.x;
        distance /= scale;
        distance -= 0.015f;

        for (var i = 1; i < Bones.Count; i++)
        {
            Bones[i].localPosition = new Vector3(0, distance / 3, 0);
        }

    }

    public void LoopCollision()
    {
        CollisionCallback();
    }
}
