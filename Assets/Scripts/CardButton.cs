using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    enum tileTypes // B - bottom, R - right, T - top, L - left, V - vertical, H - horizontal
    {
        Curve_BR, Curve_LB, Curve_RT, Curve_TL, Straight_V, Straight_H, T_B, T_L, T_T, T_R, Cross
    };

    private Image myImage;
    private int tileType, rotation;
    private Sprite mySprite;
    private int[] curveTypesInClockwiseRotation = new int[4] { 0 , 1, 3, 2 };

    public void setTileType(int type)
    {
        Texture2D myTexture = null;
        tileType = type;
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

    public void RotateTile(int rotation)  // 1 is clockwise, -1 is counterclockwise
    {
        myImage.transform.Rotate(Vector3.forward * rotation * 90);
        this.rotation -= rotation;
    }

    public int GetTileType()
    {
        int myType = -1;

        if (rotation % 4 == 0) // no rotation
        {
            myType = tileType;
        }

        else
        {
            if (tileType <= 3) // curve
            {
                int myRotationValue = rotation % 4;
                int tileIDX = GeneralMethods.FindElementIdx(curveTypesInClockwiseRotation, tileType);
                if (myRotationValue > 0)
                {
                    tileIDX = (tileIDX + myRotationValue) % 4;
                }
                else
                {
                    tileIDX = (tileIDX + (4 + myRotationValue) ) % 4;
                }
                myType = curveTypesInClockwiseRotation[tileIDX];
                Debug.Log(tileType.ToString());
                Debug.Log(rotation.ToString());
                Debug.Log(myRotationValue.ToString());
                Debug.Log(myType.ToString());
            }
            else if (tileType == (int)tileTypes.Straight_H || tileType == (int)tileTypes.Straight_H)
            {
                int myRotationValue = rotation % 2;

                if(myRotationValue != 0)
                {
                    if (tileType == (int)tileTypes.Straight_H)
                        myType = (int)tileTypes.Straight_V;
                    else
                        myType = (int)tileTypes.Straight_H;
                }
                Debug.Log(tileType.ToString());
                Debug.Log(rotation.ToString());
                Debug.Log(myRotationValue.ToString());
                Debug.Log(myType.ToString());
            }
            else if (tileType >= (int)tileTypes.T_B && tileType <= (int)tileTypes.T_R)
            {
                int myRotationValue = rotation % 4;

                if (myRotationValue < 0)
                {
                    myRotationValue = (4 + myRotationValue);
                }

                myType = 6 + ((tileType-6) + myRotationValue) % 4;
                Debug.Log(tileType.ToString());
                Debug.Log(rotation.ToString());
                Debug.Log(myRotationValue.ToString());
                Debug.Log(myType.ToString());
            }
            else
                myType = tileType;
        }

        tileType = myType;
        rotation = 0;
        return myType;
    }

    // Use this for initialization
    void Start()
    {
        myImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
