using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PauseMenuScript : MonoBehaviour {

    private GameObject cursor;
    public GameObject turnManager;
    public float cursorMovement = 25.0f;
    private int selezione = 1;
    private bool move;
    private float destination;
    public FadeManager fade;

    IEnumerator Selection()
    {
        switch (selezione)
        {
            case 1:
                {
                    break;
                }
            case 2:
                {
                    StartCoroutine(fade.FadeOut("_Scenes/Controls"));
                    break;
                }
            case 3:
                {
                    StartCoroutine(fade.FadeOut("_Scenes/MenuIniziale"));
                    break;
                }
            default: break;

        }
        yield return null;
    }

    void Start () {
        cursor = transform.GetChild(0).gameObject;
        cursor.transform.localPosition.Set(0, cursorMovement, 0);
        selezione = 1;
	}
	
	void Update ()
    {
        if (turnManager.GetComponent<TurnManager>().isInPause == true)
        {
            if (cursor.transform.localPosition.y > (destination - 0.2f) && cursor.transform.localPosition.y < (destination + 0.2f) && move)
            {
                Vector3 newPos = cursor.transform.localPosition;
                newPos.y = destination;
                cursor.transform.localPosition.Set(newPos.x, newPos.y, newPos.z);
            }

            if (cursor.transform.localPosition.y == destination) move = false;

            if ((Input.GetKeyDown(KeyCode.W) || Input.GetAxis("VerticalJoy") == 1 || Input.GetAxis("VerticalAnalog") >= 0.9f) && !move)
            {
                if (cursor.transform.localPosition.y < 3)
                {
                    move = true;
                    destination = cursor.transform.localPosition.y + cursorMovement;
                    selezione--;
                    cursor.transform.DOLocalMoveY(cursor.transform.localPosition.y + cursorMovement, 1f);

                    //camera.transform.DOShakePosition(0.2f, 0.6f);
                    //fadeOut = true;
                }
            }

            if ((Input.GetKeyDown(KeyCode.S) || Input.GetAxis("VerticalJoy") == -1 || Input.GetAxis("VerticalAnalog") <= -0.9f) && !move)
            {
                if (cursor.transform.localPosition.y > -3)
                {
                    move = true;
                    destination = cursor.transform.localPosition.y - cursorMovement;
                    selezione++;
                    cursor.transform.DOLocalMoveY(cursor.transform.localPosition.y - cursorMovement, 1f);

                    //camera.transform.DOShakePosition(0.2f, 0.6f);
                    //fadeOut = true;
                }
            }

            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1joy")) && !move && !fade.fading)
            {
                StartCoroutine(Selection());
            }

        }
    }
}
