using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour {

    private GameObject cursor;
    public GameObject turnManager, controls;
    public float cursorMovement = 25.0f;
    public int selezione = 1;
    private bool move = false;
    private float destination;
    public FadeManager fade;
    private bool controlsActivated = false;

    IEnumerator Selection()
    {
        switch (selezione)
        {
            case 1:
                {
                    StartCoroutine(turnManager.GetComponent<TurnManager>().Resume());
                    break;
                }
            case 2:
                {
                    controls.SetActive(true);
                    controlsActivated = true;
                    break;
                }
            case 3:
                {
                    SceneManager.LoadScene("_Scenes/MenuIniziale");
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
        if (turnManager.GetComponent<TurnManager>().isInPause == true && !controlsActivated)
        {
            if (cursor.transform.localPosition.y > (destination - 0.3f) && cursor.transform.localPosition.y < (destination + 0.3f) && move)
            {
                Vector3 newPos = cursor.transform.localPosition;
                newPos.y = destination;
                cursor.transform.localPosition.Set(newPos.x, newPos.y, newPos.z);
            }

            if (cursor.transform.localPosition.y == destination) move = false;

            if ((Input.GetKeyDown(KeyCode.W) || Input.GetAxis("VerticalJoy") == 1 || Input.GetAxis("VerticalAnalog") >= 0.9f) && !move)
            {
                StartCoroutine(moveUp());
            }

            if ((Input.GetKeyDown(KeyCode.S) || Input.GetAxis("VerticalJoy") == -1 || Input.GetAxis("VerticalAnalog") <= -0.9f) && !move)
            {
                StartCoroutine(moveDown());
            }

            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1joy")) && !move)
            {
                StartCoroutine(Selection());
            }

        }

        if (controlsActivated && (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2joy")))
        {
            controls.SetActive(false);
            controlsActivated = false;
        }

    }

    IEnumerator moveUp ()
    {
        if (cursor.transform.localPosition.y < 3)
        {
            move = true;
            destination = cursor.transform.localPosition.y + cursorMovement;
            selezione--;
            cursor.transform.DOLocalMoveY(cursor.transform.localPosition.y + cursorMovement, 0.5f);

                //camera.transform.DOShakePosition(0.2f, 0.6f);
                //fadeOut = true;
        }
            yield return null;
    }

    IEnumerator moveDown()
    {
        if (cursor.transform.localPosition.y > -3)
        {
            move = true;
            destination = cursor.transform.localPosition.y - cursorMovement;
            selezione++;
            cursor.transform.DOLocalMoveY(cursor.transform.localPosition.y - cursorMovement, 0.5f);

            //camera.transform.DOShakePosition(0.2f, 0.6f);
            //fadeOut = true;
        }
        yield return null;
    }
}
