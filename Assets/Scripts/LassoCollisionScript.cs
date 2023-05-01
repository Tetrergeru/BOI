using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoCollisionScript : MonoBehaviour
{
    public LassoScript LassoScript;

    private bool _collision = false;
    private IPullable _pullable = null;

    void LateUpdate()
    {
        if (_collision)
        {
            LassoScript.LoopCollision(_pullable);
        }
        _pullable = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        _collision = true;

        var neck = other.GetComponent<NeckScript>();
        if (neck != null)
        {
            _pullable = neck.Pullable;
        }

        var animal = other.GetComponent<AnimalScript>();
        if (animal != null)
        {
            _pullable = animal;
        }
    }
}
