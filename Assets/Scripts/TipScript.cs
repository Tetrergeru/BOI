using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipScript : MonoBehaviour
{
    public string TipText = "";

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerTipScript>();
        if (player == null) return;

        player.ShowTip(TipText);
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerTipScript>();
        if (player == null) return;

        player.HideTip();
    }
}
