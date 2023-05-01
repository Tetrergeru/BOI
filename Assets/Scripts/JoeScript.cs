using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum JoeState
{
    GameStarted,
    QuestGiven,
    BroughtOneCow,
    BroughtThreeCows,
    BroughtMoreCows,
    BroughtNineCows,
    BroughtAllCows,
    AfterTheQuest,
}

public class JoeScript : MonoBehaviour
{
    public GameObject Bang;
    public TMPro.TextMeshPro SpeachText;
    public AudioSource Audio;
    public Animator Animator;

    public PlayerController Player;

    private JoeState State = JoeState.GameStarted;
    private PlayerTipScript _playerTipScript;

    private void Start()
    {
        SetUnSpeak();
        _playerTipScript = Player.GetComponent<PlayerTipScript>();
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
                UpdateCowCount();
                Speak(
                    "Oh no! The flood have scattered my dear animals! Please, friend, help me! Bring my precious little cows back!",
                    15f
                );
                State = JoeState.QuestGiven;
                break;

            case JoeState.QuestGiven:
                Speak(
                    "Oh please, please, friend! Bring my precious little cows back!",
                    5f
                );
                UpdateCowCount();
                break;

            case JoeState.BroughtOneCow:
                Speak(
                    "This was my favourite cow! Her name is margareth. I am so grateful to you!",
                    5f
                );
                UpdateCowCount();
                break;

            case JoeState.BroughtThreeCows:
                Speak(
                    "Thank you sincerely! I am afraid other cows would be far more difficult to find.",
                    5f
                );
                UpdateCowCount();
                break;

            case JoeState.BroughtMoreCows:
                Speak(
                    "Oh, golly. My little beatiful cows. Soon I will be the happiest man in the wild west again!",
                    5f
                );
                UpdateCowCount();
                break;

            case JoeState.BroughtNineCows:
                Speak(
                    "Oh, golly. My little beatiful cows. One more, and I will be the happiest man in the wild west again!",
                    5f
                );
                UpdateCowCount();
                break;

            case JoeState.BroughtAllCows:
                Speak(
                    "Horray! All of my cows! Horray! Horray! Horray!",
                    10f,
                    true
                );
                break;

            case JoeState.AfterTheQuest:
                Speak(
                    "I am in your debt, Boy!",
                    5f
                );
                break;
        }
    }

    void UpdateCowCount()
    {
        var cows = _playerTipScript.Animals[AnimalType.Cow];
        if (cows >= 10)
            State = JoeState.BroughtAllCows;
        else if (cows >= 9)
            State = JoeState.BroughtNineCows;
        else if (cows > 6)
            State = JoeState.BroughtMoreCows;
        else if (cows > 2)
            State = JoeState.BroughtThreeCows;
        else if (cows > 0)
            State = JoeState.BroughtOneCow;
    }

    void Speak(string text, float time, bool finish = false)
    {
        Audio.volume = 1;
        SpeachText.text = text;
        Audio.enabled = true;
        Animator.SetBool("Talk", true);
        Bang.SetActive(false);

        StartCoroutine(UnSpeak(time, finish));
    }

    IEnumerator UnSpeak(float time, bool finish)
    {
        yield return new WaitForSeconds(time - 1);
        for (var i = 20; i > 0; i--)
        {
            Audio.volume = i / 20f;
            yield return new WaitForSeconds(1 / 20f);
        }
        if (finish)
        {
            State = JoeState.AfterTheQuest;
            Player.Credits.StartCredits();
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
