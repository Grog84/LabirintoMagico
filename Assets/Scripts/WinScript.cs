using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScript : MonoBehaviour {
    public Sprite[] winScreen;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void winner (int player)
    {
        GetComponent<Image>().sprite = winScreen[player];
    }
}
