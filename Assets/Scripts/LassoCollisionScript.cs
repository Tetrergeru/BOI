using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoCollisionScript : MonoBehaviour
{
    public LassoScript LassoScript;

    private bool _collision = false;
    private AnimalScript _animal = null;

    void LateUpdate()
    {
        if (_collision)
        {
            LassoScript.LoopCollision(_animal);
        }
        _animal = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        _collision = true;

        var neck = other.GetComponent<NeckScript>();
        if (neck != null)
        {
            _animal = neck.Animal;
        }

        var animal = other.GetComponent<AnimalScript>();
        if (animal != null)
        {
            _animal = animal;
        }
    }
}
