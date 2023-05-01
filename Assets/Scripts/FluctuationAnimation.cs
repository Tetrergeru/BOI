using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluctuationAnimation : MonoBehaviour
{
    public Transform Object;

    public Animator AnimationX;
    [UnityEngine.Serialization.FormerlySerializedAs("AnimationCurve")]
    public Animator AnimationY;
    public Animator AnimationZ;

    private Vector3 _startPosition;

    [System.Serializable]
    public class Animator
    {
        public AnimationCurve AnimationCurve;

        private bool NoAnimation => AnimationCurve == null || AnimationCurve.length == 0;

        private float AnimationLength;
        private float Time;

        public void Init()
        {
            if (!NoAnimation)
                AnimationLength = AnimationCurve[AnimationCurve.length - 1].time;
            Time = 0;
        }

        public float Tick(float deltaTime)
        {
            if (NoAnimation)
                return 0;

            Time += deltaTime;

            while (Time > AnimationLength)
                Time -= AnimationLength;

            return AnimationCurve.Evaluate(Time);
        }
    }

    void Start()
    {
        _startPosition = Object.localPosition;
        AnimationX.Init();
        AnimationY.Init();
        AnimationZ.Init();
        StartCoroutine(LegFloat());
    }

    IEnumerator LegFloat()
    {
        while (true)
        {
            var dt = Time.deltaTime;
            var dx = AnimationX.Tick(dt);
            var dy = AnimationY.Tick(dt);
            var dz = AnimationZ.Tick(dt);
            Object.localPosition = _startPosition + new Vector3(dx, dy, dz);

            yield return new WaitForEndOfFrame();
        }
    }
}