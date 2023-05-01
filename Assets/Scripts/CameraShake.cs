using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform Transform;
    public AnimationCurve Amplitude;

    private Vector3 _startPosition;
    private float _timeLeft;

    public void Shake(float time = 1.0f)
    {
        _timeLeft = Mathf.Max(_timeLeft, time);
    }

    void Start()
    {
        _startPosition = Transform.localPosition;
    }

    void Update()
    {
        if (Transform == null) return;

        if (Mathf.Abs(_timeLeft) < 0.00001f)
        {
            Transform.localPosition = _startPosition;
            return;
        };

        _timeLeft = Mathf.Max(_timeLeft - Time.deltaTime, 0.0f);

        var amplitude = Amplitude.Evaluate(_timeLeft);

        Transform.localPosition = _startPosition + new Vector3(
            Random.Range(-amplitude, amplitude),
            0,
            Random.Range(-amplitude, amplitude)
        );
    }
}