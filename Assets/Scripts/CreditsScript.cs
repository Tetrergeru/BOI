using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    public GameObject Camera;
    public PlayerTipScript Player;
    public TMPro.TextMeshProUGUI Text;

    private bool _moveCredits;

    public void StartCredits()
    {
        _moveCredits = true;
        Player.StopRendering();
        Player.gameObject.SetActive(false);
        Camera.gameObject.SetActive(true);
        Text.text = "Press ESC to return to the game";
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
            return;
        }

        var p = this.transform.position;
        this.transform.position = new Vector3(p.x, p.y + 0.01f, p.z);
    }
}
