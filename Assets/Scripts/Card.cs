﻿using System.Collections;
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
    private Sprite mySprite;

    public int getTileType()
    {
        return tileType;
    }

    void AssignRandomType()
    {
        tileType = Random.Range(0, 11);
        Texture2D myTexture = null;

        switch (tileType)
        {
            case (int)tileTypes.Curve_BR:
                myTexture = (Texture2D)Resources.Load("TileProva/curva");
                break;
            case (int)tileTypes.Curve_LB:
                myTexture = (Texture2D)Resources.Load("TileProva/curva2");
                break;
            case (int)tileTypes.Curve_RT:
                myTexture = (Texture2D)Resources.Load("TileProva/curva3");
                break;
            case (int)tileTypes.Curve_TL:
                myTexture = (Texture2D)Resources.Load("TileProva/curva4");
                break;
            case (int)tileTypes.Straight_V:
                myTexture = (Texture2D)Resources.Load("TileProva/Straight");
                break;
            case (int)tileTypes.Straight_H:
                myTexture = (Texture2D)Resources.Load("TileProva/Straight2");
                break;
            case (int)tileTypes.T_B:
                myTexture = (Texture2D)Resources.Load("TileProva/t");
                break;
            case (int)tileTypes.T_L:
                myTexture = (Texture2D)Resources.Load("TileProva/t2");
                break;
            case (int)tileTypes.T_T:
                myTexture = (Texture2D)Resources.Load("TileProva/t3");
                break;
            case (int)tileTypes.T_R:
                myTexture = (Texture2D)Resources.Load("TileProva/t4");
                break;
            case (int)tileTypes.Cross:
                myTexture = (Texture2D)Resources.Load("TileProva/cross");
                break;
            
            default:
                break;
        }

        mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        myImage.sprite = mySprite;
    }

    // Use this for initialization
    void Start () {

        myImage = GetComponent<Image>();
        AssignRandomType();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}