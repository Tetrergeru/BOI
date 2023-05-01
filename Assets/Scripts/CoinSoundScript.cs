using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSoundScript : MonoBehaviour
{
    public AudioSource Source;

    public void Update()
    {
        if (!Source.isPlaying)
            Destroy(this.gameObject);
    }
}
