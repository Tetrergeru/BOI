using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum JoeState
{
    GameStarted,
    Quest1,
}

public class JoeScript : MonoBehaviour
{
    public GameObject Bang;
    public TMPro.TextMeshPro SpeachText;
    public AudioSource Audio;
    public Animator Animator;

    public PlayerController Player;

    private JoeState State = JoeState.GameStarted;

    private void Start()
    {
        SetUnSpeak();
    }

    private void LateUpdate()
    {
        var playerPos = Player.transform.position;
        var r = Quaternion.LookRotation(this.transform.position - playerPos).eulerAngles;
        SpeachText.transform.rotation = Quaternion.Euler(0, r.y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player == null)
            return;

        switch (State)
        {
            case JoeState.GameStarted:
                Speak(
                    "Oh no! The flood have scattered my dear animals! Please, friend, help me! Bring my precious little cows back!",
                    15f
                );
                State = JoeState.Quest1;
                break;

            case JoeState.Quest1:
                Speak(
                    "Oh please, please, friend! Bring my precious little cows back!",
                    5f
                );
                break;
        }
    }

    void Speak(string text, float time)
    {
        Audio.volume = 1;
        SpeachText.text = text;
        Audio.enabled = true;
        Animator.SetBool("Talk", true);
        Bang.SetActive(false);

        StartCoroutine(UnSpeak(time));
    }

    IEnumerator UnSpeak(float time)
    {
        yield return new WaitForSeconds(time - 1);
        for (var i = 20; i > 0; i--)
        {
            Audio.volume = i / 20f;
            yield return new WaitForSeconds(1 / 20f);
        }
        SetUnSpeak();
    }

    void SetUnSpeak()
    {
        SpeachText.text = "";
        Audio.enabled = false;
        Animator.SetBool("Talk", false);
    }
}
