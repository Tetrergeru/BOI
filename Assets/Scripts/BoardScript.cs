using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    public List<GameObject> Bottles;
    public List<GameObject> TNTs;
    public PlayerTipScript Player;
    public TMPro.TextMeshPro Text;
    public TMPro.TextMeshPro Text1;
    public GameObject Bang;

    void Start()
    {
        ClearScore();
        foreach (var bottle in Bottles)
        {
            bottle.SetActive(false);
        }
        foreach (var tnt in TNTs)
        {
            tnt.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        var playerPos = Player.transform.position;
        var r = Quaternion.LookRotation(this.transform.position - playerPos).eulerAngles;

        Text.transform.rotation = Quaternion.Euler(0, r.y, 0);
        Text1.transform.rotation = Quaternion.Euler(0, r.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerTipScript>();
        if (player == null) return;

        Bang.SetActive(false);
        RenderScore();
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerTipScript>();
        if (player == null) return;

        ClearScore();
    }

    public void SetBottleCount(int count)
    {
        if (count > Bottles.Count)
            count = Bottles.Count;

        for (var i = 0; i < count; i++)
            Bottles[i].SetActive(true);
    }

    public void SetTNTCount(int count)
    {
        if (count > TNTs.Count)
            count = TNTs.Count;

        for (var i = 0; i < count; i++)
            TNTs[i].SetActive(true);
    }

    private void RenderScore()
    {
        var len = 0;
        foreach (var a in Player.Animals)
            len = Mathf.Max(len, $"{a.Value}".Length);
        len = Mathf.Max(len, $"{Player.Bottles}".Length);
        len = Mathf.Max(len, $"{Player.Dynamite}".Length);

        var tl = 0;
        foreach (var a in Player.TotalAnimals)
            tl = Mathf.Max(tl, $"{a.Value}".Length);
        tl = Mathf.Max(tl, $"{Player.TotalBottles}".Length);
        tl = Mathf.Max(tl, $"{Player.TotalDynamite}".Length);

        var text = "<align=\"left\">Delivered:\n</align>";
        text += $"Cows: {PaddedNumber(Player.Animals[AnimalType.Cow], len)}/{PaddedNumber(Player.TotalAnimals[AnimalType.Cow], tl)}\n";
        text += $"Bisons: {PaddedNumber(Player.Animals[AnimalType.Bison], len)}/{PaddedNumber(Player.TotalAnimals[AnimalType.Bison], tl)}\n";
        text += $"Geese: {PaddedNumber(Player.Animals[AnimalType.Goose], len)}/{PaddedNumber(Player.TotalAnimals[AnimalType.Goose], tl)}\n";
        text += $"Horses: {PaddedNumber(Player.Animals[AnimalType.Horse], len)}/{PaddedNumber(Player.TotalAnimals[AnimalType.Horse], tl)}\n";
        Text.text = text;

        text = "<align=\"left\">Found:\n</align>";
        text += $"Bottles: {PaddedNumber(Player.Bottles, len)}/{PaddedNumber(Player.TotalBottles, tl)}\n";
        text += $"{(Player.Dynamite == 0 ? "???" : "TNT")}: {PaddedNumber(Player.Dynamite, len)}/{PaddedNumber(Player.TotalDynamite, tl)}\n";
        Text1.text = text;
    }

    private string PaddedNumber(int num, int len)
    {
        var str = $"{num}";
        for (var i = 0; i < len - str.Length; i++)
            str = ' ' + str;
        return str;
    }

    private void ClearScore()
    {
        Text.text = "";
        Text1.text = "";
    }
}
