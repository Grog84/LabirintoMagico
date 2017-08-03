using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour {

    Text myText;
    // Use this for initialization
    void Start () {
		
        myText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {

        myText.text = Input.GetAxis("VerticalAnalog").ToString();
	}
}
