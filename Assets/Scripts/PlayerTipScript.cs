using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTipScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI TipText;

    public void ShowTip(string text)
    {
        TipText.text = text;
    }

    public void HideTip()
    {
        TipText.text = "";
    }
}
