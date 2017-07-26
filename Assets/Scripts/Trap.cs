﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    private int playerDropping;
    private SpriteRenderer myRenderer;
    private Sprite mySprite;
    private Texture2D myTexture;
    private Coordinate myCoord;
    public bool isActive;

    public void SetCoordiantes(Coordinate coord)
    {
        myCoord = coord;
    }

    public Coordinate GetCoordiantes()
    {
        return myCoord;
    }

    void SetInvisible()
    {
        myRenderer.color = Color.clear;
    }

    void SetVisible()
    {
        myRenderer.color = Color.white;
    }

    public void Activate()
    {
        isActive = true;
        transform.parent.gameObject.GetComponent<Tile>().SetIsTrapped(true);
        SetInvisible();  // could become a coroutine
    }

    public void Trigger(Player thisPlayer)
    {
        isActive = false;
        thisPlayer.ResetToStartingPosition();
        SetVisible();
    }

    public void SetPlayerDropping(int player)
    {
        playerDropping = player;
        SetSprite();
    }

    public void SetSprite()
    {
        switch (playerDropping)
        {
            case 1:
                myTexture = (Texture2D)Resources.Load("TrapProva/trap_red");
                break;
            case 2:
                myTexture = (Texture2D)Resources.Load("TrapProva/trap_blue");
                break;
            case 3:
                myTexture = (Texture2D)Resources.Load("TrapProva/trap_green");
                break;
            case 4:
                myTexture = (Texture2D)Resources.Load("TrapProva/trap_yellow");
                break;
            default:
                break;
        }

        mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        myRenderer.sprite = mySprite;
    }

    public bool GetIsActive()
    {
        return isActive;
    }

	// Use this for initialization
	void Awake () {
        myRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
