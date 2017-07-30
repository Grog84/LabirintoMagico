using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour {
    public bool fadeIn, fadeOut;

	// Use this for initialization
	void Start () {
        fadeIn = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (fadeIn) GetComponent<Renderer>().material.color -= new Color(0, 0, 0, .02f);

        if (fadeOut) GetComponent<Renderer>().material.color += new Color(0, 0, 0, .02f);

        if (GetComponent<Renderer>().material.color.a <= 0)
        {
            fadeIn = false;
        }

        if (GetComponent<Renderer>().material.color.a >= 1)
        {
            SceneManager.LoadScene("_Scenes/MenuIniziale");
        }
    }
}
