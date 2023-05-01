using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneScript : MonoBehaviour
{
    public PlayerController Player;
    public AnimalScript Animal;

    void Start()
    {
        Player.AddAnimalToTower(Animal);
    }

}
