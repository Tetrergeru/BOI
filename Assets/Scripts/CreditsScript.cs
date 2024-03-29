using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    public GameObject Camera;
    public PlayerTipScript Player;
    public TMPro.TextMeshProUGUI Text;
    public TMPro.TextMeshPro CreditsText;

    private bool _moveCredits;

    public void StartCredits()
    {
        _moveCredits = true;
        Player.StopRendering();
        Player.gameObject.SetActive(false);
        Camera.gameObject.SetActive(true);
        Text.text = "Press Enter to return to the game";
        CreditsText.text = @$"
We are very glad that you have finished our game!

And we hope you liked it!

Thank you very much!

Your score is {Player.Score}.

You have delivered {Player.Animals[AnimalType.Cow]}/{Player.TotalAnimals[AnimalType.Cow]} cows, {Player.Animals[AnimalType.Goose]}/{Player.TotalAnimals[AnimalType.Goose]} geese, {Player.Animals[AnimalType.Bison]}/{Player.TotalAnimals[AnimalType.Bison]} bisons and {Player.Animals[AnimalType.Horse]}/{Player.TotalAnimals[AnimalType.Horse]} horses.

You have found {Player.Bottles}/{Player.TotalBottles} bottles and {Player.Dynamite}/{Player.TotalDynamite} TNTs.

CREDITS:

Programming:
Oleg Arutyunov

Level Design:
Nicole Akopdzhanova

3d models:
Sergey Duyunov

Game Design:
Oleg Arutyunov

Animations:
Sergey Duyunov

Technical Artist:
Nicole Akopdzhanova

Sound Effects:
Viacheslav Ivanchenko

Music:
Egor Afonin

Mental Support:
Viacheslav Ivanchenko
";
    }

    public void ResumeGame()
    {
        Text.text = "";
        Player.gameObject.SetActive(true);
        Camera.gameObject.SetActive(false);
        Player.StartRendering();
    }

    void FixedUpdate()
    {
        if (!_moveCredits) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ResumeGame();
            return;
        }

        var p = this.transform.position;
        this.transform.position = new Vector3(p.x, p.y + 0.01f, p.z);
    }
}
