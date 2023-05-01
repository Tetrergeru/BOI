using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTipScript : MonoBehaviour
{
    private bool Stop = false;

    public TMPro.TextMeshProUGUI TipText;
    public TMPro.TextMeshProUGUI ScoreText;
    public TMPro.TextMeshProUGUI NameText;
    public BoardScript Board;

    public GameObject CoinScript;

    public int Score = 0;
    public int Bottles = 0;
    public int Dynamite = 0;
    public int TotalBottles = 0;
    public int TotalDynamite = 0;
    public Dictionary<AnimalType, int> Animals = new Dictionary<AnimalType, int>();
    public Dictionary<AnimalType, int> TotalAnimals = new Dictionary<AnimalType, int>();

    public GameObject TipToDeleteWhenDeliverAnimal;
    public GameObject TipToDeleteWhenCatchBottle;

    void Start()
    {
        TipText.text = "";

        Animals[AnimalType.Horse] = 0;
        Animals[AnimalType.Goose] = 0;
        Animals[AnimalType.Bison] = 0;
        Animals[AnimalType.Cow] = 0;

        TotalAnimals[AnimalType.Horse] = 0;
        TotalAnimals[AnimalType.Goose] = 0;
        TotalAnimals[AnimalType.Bison] = 0;
        TotalAnimals[AnimalType.Cow] = 0;

        foreach (var animal in GameObject.FindObjectsByType<AnimalScript>(FindObjectsSortMode.None))
        {
            TotalAnimals[animal.Type] += 1;
        }
        TotalBottles = GameObject.FindObjectsByType<BottleScript>(FindObjectsSortMode.None).Length;
        TotalDynamite = GameObject.FindObjectsByType<TNTScript>(FindObjectsSortMode.None).Length;

        RenderScore();
    }

    public void AddScore(int score)
    {
        if (score != 0)
            Instantiate(CoinScript);
        Score += score;
        RenderScore();
    }

    public void AddAnimals(AnimalType animal, int count)
    {
        if (TipToDeleteWhenDeliverAnimal != null)
        {
            Destroy(TipToDeleteWhenDeliverAnimal);
            TipToDeleteWhenDeliverAnimal = null;
            TipText.text = "";
        }

        if (!Animals.ContainsKey(animal))
            Animals[animal] = count;
        else
            Animals[animal] += count;
        RenderScore();
    }

    public void AddBottles(int count)
    {
        if (TipToDeleteWhenCatchBottle != null)
        {
            Destroy(TipToDeleteWhenCatchBottle);
            TipToDeleteWhenCatchBottle = null;
            TipText.text = "";
        }
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
        if (Stop) return;
        ScoreText.text = $"Score: {Score}$";
    }

    public void StopRendering()
    {
        Stop = true;
        ScoreText.text = "";
        TipText.text = "";
        NameText.gameObject.SetActive(false);
    }

    public void StartRendering()
    {
        Stop = false;
        RenderScore();
        NameText.gameObject.SetActive(true);
        GetComponent<PlayerController>().LassoThrown = false;
    }
}
