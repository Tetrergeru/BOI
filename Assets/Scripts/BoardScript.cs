using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    public List<GameObject> Bottles;
    public PlayerTipScript Player;
    public TMPro.TextMeshPro Text;
    public GameObject Bang;

    void Start()
    {
        ClearScore();
        foreach (var bottle in Bottles)
        {
            bottle.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        var playerPos = Player.transform.position;
        var r = Quaternion.LookRotation(this.transform.position - playerPos).eulerAngles;
        Text.transform.rotation = Quaternion.Euler(0, r.y, 0);
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

    private void RenderScore()
    {
        var text = $"Cows: {Player.Animals[AnimalType.Cow]}\n";
        text += $"Bisons: {Player.Animals[AnimalType.Bison]}\n";
        text += $"Geese: {Player.Animals[AnimalType.Goose]}\n";
        text += $"Horses: {Player.Animals[AnimalType.Horse]}\n";
        text += $"Bottles: {Player.Bottles}\n";
        text += $"{(Player.Dynamite == 0 ? "???" : "TNT")}: {Player.Dynamite}";
        Text.text = text;
    }

    private void ClearScore()
    {
        Text.text = "";
    }
}
