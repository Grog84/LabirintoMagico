using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoTransition : MonoBehaviour {

    public FadeManager fade;
    float timer;
    bool timerGo;

    private void Awake()
    {
        fade.createFadeMaskAZ(-5);
        timerGo = true;
    }

    void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (timerGo) timer += Time.deltaTime;

        if (timer >= 3) timerGo = false;
        
        if(!timerGo) StartCoroutine(fade.FadeOut("_Scenes/CubeScene"));
    }
}
