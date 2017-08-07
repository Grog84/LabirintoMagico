using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

    public FadeManager fade;
    public AudioClip[] SFX;

    private void Start()
    {
        StartCoroutine(fade.FadeIn());    
    }

    void Update () {
		if ((Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2joy")) && !fade.fading)
        {
            GetComponent<AudioSource>().clip = SFX[0];
            GetComponent<AudioSource>().Play();
            StartCoroutine(fade.FadeOut("_Scenes/MenuIniziale"));
        }
    }

            
            
}
