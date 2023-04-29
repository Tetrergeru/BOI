using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoCollisionScript : MonoBehaviour
{
    public LassoScript LassoScript;

    private void OnTriggerEnter(Collider other)
    {
        LassoScript.LoopCollision();
    }

}
