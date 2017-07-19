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
        myCollider.size = new Vector2(myTexture.width/100f, myTexture.height/100f);
    }

    public void setPossibleConnections(int type)
    {
        switch (type)
        {
            case (int)tileTypes.Curve_BR:
            case (int)tileTypes.Curve_BR_alt:
                possibleConnections[0] = false;
                possibleConnections[1] = true;
                possibleConnections[2] = true;
                possibleConnections[3] = false;
                break;
            case (int)tileTypes.Curve_LB:
            case (int)tileTypes.Curve_LB_alt:
                possibleConnections[0] = false;
                possibleConnections[1] = false;
                possibleConnections[2] = true;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.Curve_RT:
            case (int)tileTypes.Curve_RT_alt:
                possibleConnections[0] = true;
                possibleConnections[1] = true;
                possibleConnections[2] = false;
                possibleConnections[3] = false;
                break;
            case (int)tileTypes.Curve_TL:
            case (int)tileTypes.Curve_TL_alt:
                possibleConnections[0] = true;
                possibleConnections[1] = false;
                possibleConnections[2] = false;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.Straight_V:
                possibleConnections[0] = true;
                possibleConnections[1] = false;
                possibleConnections[2] = true;
                possibleConnections[3] = false;
                break;
            case (int)tileTypes.Straight_H:
                possibleConnections[0] = false;
                possibleConnections[1] = true;
                possibleConnections[2] = false;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.T_B:
            case (int)tileTypes.T_B_alt:
                possibleConnections[0] = false;
                possibleConnections[1] = true;
                possibleConnections[2] = true;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.T_L:
            case (int)tileTypes.T_L_alt:
                possibleConnections[0] = true;
                possibleConnections[1] = false;
                possibleConnections[2] = true;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.T_T:
            case (int)tileTypes.T_T_alt:
                possibleConnections[0] = true;
                possibleConnections[1] = true;
                possibleConnections[2] = false;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.T_R:
            case (int)tileTypes.T_R_alt:
                possibleConnections[0] = true;
                possibleConnections[1] = true;
                possibleConnections[2] = true;
                possibleConnections[3] = false;
                break;
            case (int)tileTypes.Cross:
            case (int)tileTypes.Goal:
                possibleConnections[0] = true;
                possibleConnections[1] = true;
                possibleConnections[2] = true;
                possibleConnections[3] = true;
                break;
                       
            default:
                break;
        }
    }

    public void checkConnections(Tile other, int lato)
    {
        if (other != null)
        {
                switch (lato)
                {
                    case 0:
                        if (possibleConnections[lato] && other.possibleConnections[2])
                        {
                            effectiveConnections[lato] = true;
                            other.effectiveConnections[2] = true;
                        }
                        break;
                    case 1:
                        if (possibleConnections[lato] && other.possibleConnections[3])
                        {
                            effectiveConnections[lato] = true;
                            other.effectiveConnections[3] = true;
                        }
                        break;
                    case 2:
                        if (possibleConnections[lato] && other.possibleConnections[0])
                        {
                            effectiveConnections[lato] = true;
                            other.effectiveConnections[0] = true;
                        }
                        break;
                    case 3:
                        if (possibleConnections[lato] && other.possibleConnections[1])
                        {
                            effectiveConnections[lato] = true;
                            other.effectiveConnections[1] = true;
                        }
                        break;
                    default:
                        break;

                }
            
        }
    }

    public IEnumerator MoveToPosition(Vector2 movement, float animTime)
    {
        // Might there be a child unchild issue?

        float elapsedTime = 0;

        Vector3 destination = new Vector3(transform.position.x + movement[0], transform.position.y + movement[1]);

        while (elapsedTime < animTime)
        {
            this.transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    void Awake () {
        myRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
        possibleConnections = new bool[4];
        effectiveConnections = new bool[4];
        //Debug.Log(possibleConnections.Length);
    }
	
	void Update () {
		
	}
}
