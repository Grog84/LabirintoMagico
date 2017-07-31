using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour {

    public FadeManager fade;

    private void Start()
    {
        StartCoroutine(fade.FadeIn());
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2joy")))
        {
            StartCoroutine(fade.FadeOut("_Scenes/MenuIniziale"));
        }
    }
}
