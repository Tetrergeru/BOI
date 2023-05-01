using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenScript : MonoBehaviour
{
    public PlayerTipScript Player;

    private void OnTriggerEnter(Collider other)
    {
        var animal = other.GetComponent<AnimalScript>();
        if (animal == null || animal.State == AnimalState.Rided || animal.State == AnimalState.InPen)
            return;

        Player.AddScore(animal.GetInPen());
        Player.AddAnimals(animal.Type, 1);
    }
}
