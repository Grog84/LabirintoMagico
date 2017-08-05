using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Connection : MonoBehaviour {
    private float searching;
    public FadeManager fade;
    public GameObject text;

    void Start ()
    {
        StartCoroutine(fade.FadeIn());	
	}
	
	void Update () {
        searching += Time.deltaTime;
        if (searching < 5f)
        {
        //transform.DOLocalRotate(new Vector3(0, 0, 90), 1, RotateMode.LocalAxisAdd);
        }

        if (searching >= 5f)
        {
            //transform.DOKill();
            text.GetComponent<Text>().text = "Connection Failed. Press A to Start.";

        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetButtonDown("Fire1joy")) && searching >= 5)
        {
            text.SetActive(false);
            StartCoroutine(fade.FadeOut("_Scenes/scenaprova"));
        }
    }
}
