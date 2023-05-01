using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LassoMode
{
    Curved,
    Straight,
    None
}

public class LassoScript : MonoBehaviour
{
    public List<Transform> Bones;
    public Transform LoopBone;
    public LassoMode lassoMode;
    public AnimationCurve Curve;
    public float CurveStrength = 1;

    public Vector3 StartPoint;
    public Vector3 EntPoint;
    public float LoopSize = 1;
    public Action<IPullable> CollisionCallback = (a) => { };

    public void Update()
    {
        if (Vector3.Distance(StartPoint, EntPoint) < 0.001f)
            return;

        LoopBone.localScale = new Vector3(LoopSize, LoopSize, LoopSize);

        if (lassoMode == LassoMode.Straight)
        {
            DrawStraight();
        }
        else if (lassoMode == LassoMode.Curved)
        {
            DrawCurved();
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

        Bones[0].localRotation = Quaternion.identity;
        for (var i = 1; i < Bones.Count; i++)
        {
            Bones[i].localRotation = Quaternion.identity;
            Bones[i].localPosition = new Vector3(0, distance / 3, 0);
        }
    }

    [ContextMenu("Draw Curved")]
    void DrawCurved()
    {
        this.transform.position = StartPoint;
        this.transform.rotation = Quaternion.LookRotation(EntPoint - StartPoint);

        var distance = Vector3.Distance(EntPoint, StartPoint);
        var scale = this.transform.lossyScale.x;
        distance /= scale;
        distance -= 0.015f;

        var count = (float)Bones.Count;

        var prevAngle = Derivative(0, 1 / count);
        Bones[0].localRotation = Quaternion.Euler(0, 0, prevAngle);

        for (var i = 1; i < Bones.Count; i++)
        {
            var len = LenToNextPoint(i / count, (i + 1) / count) * distance;
            Bones[i].localPosition = new Vector3(0, len, 0);

            prevAngle = prevAngle - Derivative(i / count, (i + 1) / count);
            Bones[i].localRotation = Quaternion.Euler(0, 0, -prevAngle);
        }
    }

    private float Derivative(float x, float x2)
    {
        var dx = x2 - x;
        var dy = Curve.Evaluate(x2) * CurveStrength - Curve.Evaluate(x) * CurveStrength;
        return Mathf.Atan2(dy, dx) / Mathf.PI * 180;
    }

    private float LenToNextPoint(float x, float x2)
    {
        var a = new Vector2(x, Curve.Evaluate(x) * CurveStrength);
        var b = new Vector2(x2, Curve.Evaluate(x2) * CurveStrength);
        return Vector2.Distance(a, b);
    }

    public void LoopCollision(IPullable a)
    {
        CollisionCallback(a);
    }
}
