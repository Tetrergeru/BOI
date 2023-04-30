using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTipScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI TipText;
    public TMPro.TextMeshProUGUI ScoreText;
    private int _score = 0;

    void Start()
    {
        TipText.text = "";
        ScoreText.text = $"Score: {_score}";
    }

    public void AddScore(int score)
    {
        _score += score;
        ScoreText.text = $"Score: {_score}";
    }

    public void ShowTip(string text)
    {
        TipText.text = text;
    }

    public void HideTip()
    {
        TipText.text = "";
    }
}
