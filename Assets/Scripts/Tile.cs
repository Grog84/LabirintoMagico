using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int type;
    public Sprite mySprite;
    public SpriteRenderer myRenderer;
    public Texture2D myTexture;
    public Coordinate myCoord;

    public bool canBeMoved = true;
    public bool[] possibleConnections, effectiveConnections;

    private BoxCollider2D myCollider;

    enum tileTypes // B - bottom, R - right, T - top, L - left, V - vertical, H - horizontal
    {
        Curve_BR, Curve_LB, Curve_RT, Curve_TL, Straight_V, Straight_H, T_B, T_L, T_T, T_R, Cross,
        Curve_BR_alt, Curve_LB_alt, Curve_RT_alt, Curve_TL_alt, T_B_alt, T_L_alt, T_T_alt, T_R_alt, Goal
    };

    public void setSprite(int type)
    {
        switch (type)
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
            case (int)tileTypes.Curve_BR_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/curva_alt");
                // TODO modificare type per diventare come quello non speciale
                break;
            case (int)tileTypes.Curve_LB_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/curva2_alt");
                break;
            case (int)tileTypes.Curve_RT_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/curva3_alt");
                break;
            case (int)tileTypes.Curve_TL_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/curva4_alt");
                break;
            case (int)tileTypes.T_B_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/t_alt");
                break;
            case (int)tileTypes.T_L_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/t2_alt");
                break;
            case (int)tileTypes.T_T_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/t3_alt");
                break;
            case (int)tileTypes.T_R_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/t4_alt");
                break;
            case (int)tileTypes.Goal:
                myTexture = (Texture2D)Resources.Load("TileProva/goal");
                break;
            default:
                break;
        }

        mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        myRenderer.sprite = mySprite;
        myCollider.size = new Vector2(myTexture.width, myTexture.height);
    }

    public void setPossibleConnections()
    {
        // uses the id to generate the connection map
    }

    public void checkConnections(Tile other)
    {
        // check the connection with another tile to update effectiveConnections
        // modifica anche la effective map dell'altro
        // se gia vero interrompe

        // se ther è null vuol dire che becca il vuoto
    }

    // Use this for initialization
    void Awake () {
        myRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
