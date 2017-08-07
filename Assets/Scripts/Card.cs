using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {

    enum tileTypes // B - bottom, R - right, T - top, L - left, V - vertical, H - horizontal
    {
        Curve_BR, Curve_LB, Curve_RT, Curve_TL, Straight_V, Straight_H, T_B, T_L, T_T, T_R, Cross
    };

    public int tileType;
    private Image myImage;
    private Sprite mySprite;
    private bool isTrapped;
    public GameObject trapMarker;
    private RectTransform trapMarkerTransform;
   
    public void SetIsTrapped(bool isTrapped)
    {
        this.isTrapped = isTrapped;
    }

    public int GetTileType()
    {
        return tileType;
    }

    public void AssignType(int type, bool trapStatus)
    {
        tileType = type;
        isTrapped = trapStatus;
        if (isTrapped)
            trapMarkerTransform.localPosition = new Vector2(0f, 5f);
        else
            trapMarkerTransform.localPosition = new Vector2(0f, -2000f);
        Texture2D myTexture = null;

        switch (tileType)
        {
            case (int)tileTypes.Curve_BR:
                myTexture = (Texture2D)Resources.Load("MenuUI/curva");
                break;
            case (int)tileTypes.Curve_LB:
                myTexture = (Texture2D)Resources.Load("MenuUI/curva2");
                break;
            case (int)tileTypes.Curve_RT:
                myTexture = (Texture2D)Resources.Load("MenuUI/curva3");
                break;
            case (int)tileTypes.Curve_TL:
                myTexture = (Texture2D)Resources.Load("MenuUI/curva4");
                break;
            case (int)tileTypes.Straight_V:
                myTexture = (Texture2D)Resources.Load("MenuUI/Straight");
                break;
            case (int)tileTypes.Straight_H:
                myTexture = (Texture2D)Resources.Load("MenuUI/Straight2");
                break;
            case (int)tileTypes.T_B:
                myTexture = (Texture2D)Resources.Load("MenuUI/t");
                break;
            case (int)tileTypes.T_L:
                myTexture = (Texture2D)Resources.Load("MenuUI/t2");
                break;
            case (int)tileTypes.T_T:
                myTexture = (Texture2D)Resources.Load("MenuUI/t3");
                break;
            case (int)tileTypes.T_R:
                myTexture = (Texture2D)Resources.Load("MenuUI/t4");
                break;
            case (int)tileTypes.Cross:
                myTexture = (Texture2D)Resources.Load("MenuUI/cross");
                break;

            default:
                break;
        }

        mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        myImage.sprite = mySprite;
    }

    public bool GetTrappedStatus()
    {
        return isTrapped;
    }

    void AssignStartingRandomType()
    {
        int randType = Random.Range(0, 11);
        AssignType(randType, true);
        isTrapped = true;
    }

    // Use this for initialization
    void Start () {

        myImage = GetComponent<Image>();
        trapMarkerTransform = trapMarker.GetComponent<RectTransform>();
        AssignStartingRandomType();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
