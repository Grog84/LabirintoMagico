using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    private int type;
    private Sprite sprite;
    private SpriteRenderer renderer;

    enum tileTypes // B - bottom, R - right, T - top, L - left, V - vertical, H - horizontal
    {
        Curve_BR, Curve_LB, Curve_RT, Curve_TL, Straight_V, Straight_H, T_B, T_L, T_T, T_R, Cross,
        Curve_BR_alt, Curve_LB_alt, Curve_RT_alt, Curve_TL_alt, T_B_alt, T_L_alt, T_T_alt, T_R_alt
    };

    public Tile(int type)
    {
        this.type = type;
        Texture2D myTexture = getTexture();
        renderer = GetComponent<SpriteRenderer>();
        sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        renderer.sprite = sprite;
    }

    private Texture2D getTexture()
    {
        Texture2D myTexture = null;
        switch (type)
        {
            case (int)tileTypes.Curve_BR:
                myTexture = (Texture2D)Resources.Load("TileProva/curve");
                break;
            case (int)tileTypes.Curve_LB:
                myTexture = (Texture2D)Resources.Load("TileProva/curve2");
                break;
            case (int)tileTypes.Curve_RT:
                myTexture = (Texture2D)Resources.Load("TileProva/curve3");
                break;
            case (int)tileTypes.Curve_TL:
                myTexture = (Texture2D)Resources.Load("TileProva/curve4");
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
                myTexture = (Texture2D)Resources.Load("TileProva/curve_alt");
                break;
            case (int)tileTypes.Curve_LB_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/curve2_alt");
                break;
            case (int)tileTypes.Curve_RT_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/curve3_alt");
                break;
            case (int)tileTypes.Curve_TL_alt:
                myTexture = (Texture2D)Resources.Load("TileProva/curve4_alt");
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
            default:
                break;
        }
        return myTexture;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
