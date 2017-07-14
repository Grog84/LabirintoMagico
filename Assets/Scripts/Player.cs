using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int playerNbr;

    SpriteRenderer myRenderer;
    Texture2D myTexture;
    Sprite mySprite;
    BoxCollider2D myCollider;

    public void setPlayerSprite()
    {
        switch (playerNbr)
        {
            case 1:
                myTexture = (Texture2D)Resources.Load("PlayerProva/p1");
                break;
            case 2:
                myTexture = (Texture2D)Resources.Load("PlayerProva/p2");
                break;
            case 3:
                myTexture = (Texture2D)Resources.Load("PlayerProva/p3");
                break;
            case 4:
                myTexture = (Texture2D)Resources.Load("PlayerProva/p4");
                break;
            default:
                break;
        }

        mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        myRenderer.sprite = mySprite;
        myCollider.size = new Vector2(myTexture.width, myTexture.height);

    }

	// Use this for initialization
	void Awake () {

        myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
