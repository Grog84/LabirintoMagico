using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    public GameObject menuSprite;
    public GameObject camera;
    private int selezione = 1;
    private bool moving = false;
    public FadeManager fading;

    IEnumerator Selection()
    {
        switch (selezione)
        {
            case 1:
                {
                    StartCoroutine(fading.FadeOut("_Scenes/Connection"));
                    break;
                }
            case 2:
                {
                    StartCoroutine(fading.FadeOut("_Scenes/Controls"));
                    break;
                }
            case 3:
                {
                    StartCoroutine(fading.FadeOut("_Scenes/Tutorial"));
                    break;
                }
            case 4:
                {
                    StartCoroutine(fading.FadeOut("_Scenes/Options"));
                    break;

                }
            default: break;

        }
        yield return null;
    }

    void Start()
    {
        StartCoroutine(fading.FadeIn());
        //fading.FadeIn();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetAxis("VerticalJoy") > 0.5) && !moving)
        {
            transform.DOLocalRotate(new Vector3(0, 0, 90), 1, RotateMode.LocalAxisAdd);
            camera.transform.DOShakePosition(0.8F, 0.7f);
            if (selezione < 4) selezione++;
            else selezione = 1;

            Debug.Log(selezione);
            //StartCoroutine(MoveRight());
        }
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetAxis("VerticalJoy") < -0.5) && !moving)
        {
            transform.DOLocalRotate(new Vector3(0, 0, -90), 1, RotateMode.LocalAxisAdd);
            camera.transform.DOShakePosition(0.8F, 0.7f);
            if (selezione > 1) selezione--;
            else selezione = 4;

            Debug.Log(selezione);
            //StartCoroutine(MoveLeft());
        }
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1joy")) && !moving)
        {
            StartCoroutine(Selection());
        }

        if (transform.rotation.eulerAngles.z % 90 > 44.9f && transform.rotation.eulerAngles.z % 90 < 45.1f) moving = false;
        else moving = true;
    }
}