﻿using UnityEngine;
using System.Collections;
using BennyBroseph;


public class GameController : MonoSingleton<GameController>
{

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        Publisher.self.Update();
    }
}
