﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

    public FadeManager fade;

    private void Start()
    {
        StartCoroutine(fade.FadeIn());    
    }

    void Update () {
		if ((Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2joy")) && !fade.fading)
        {
            StartCoroutine(fade.FadeOut("_Scenes/MenuIniziale"));
        }
    }

            
            
}
