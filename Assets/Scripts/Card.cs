using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {

    enum tileTypes // B - bottom, R - right, T - top, L - left, V - vertical, H - horizontal
    {
        Curve_BR, Curve_LB, Curve_RT, Curve_TL, Straight_V, Straight_H, T_B, T_L, T_T, T_R, Cross
    };

    private int tileType;
    private Image myImage;

    void AssignRandomType()
    {

    }

    // Use this for initialization
    void Start () {

        myImage = GetComponent<Image>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
