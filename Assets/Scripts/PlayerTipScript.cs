using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTipScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI TipText;
    public TMPro.TextMeshProUGUI ScoreText;
    public BoardScript Board;

    public GameObject CoinScript;

    private int _score = 0;
    public int Bottles = 0;
    public int Dynamite = 0;
    public Dictionary<AnimalType, int> Animals = new Dictionary<AnimalType, int>();

    void Start()
    {
        TipText.text = "";
        Animals[AnimalType.Horse] = 0;
        Animals[AnimalType.Goose] = 0;
        Animals[AnimalType.Bison] = 0;
        Animals[AnimalType.Cow] = 0;
        RenderScore();
    }

    public void AddScore(int score)
    {
        Instantiate(CoinScript);
        _score += score;
        RenderScore();
    }

    public void AddAnimals(AnimalType animal, int count)
    {
        if (!Animals.ContainsKey(animal))
            Animals[animal] = count;
        else
            Animals[animal] += count;
        RenderScore();
    }

    public void AddBottles(int count)
    {
        Bottles += count;
        Board.SetBottleCount(Bottles);
        RenderScore();
    }

    public void AddTNT(int count)
    {
        Dynamite += count;
        Board.SetTNTCount(Dynamite);
        RenderScore();
    }

    public void ShowTip(string text)
    {
        TipText.text = text;
    }

    public void HideTip()
    {
        TipText.text = "";
    }

    private void RenderScore()
    {
        ScoreText.text = $"Score: {_score}";
    }
}
