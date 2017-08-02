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
    private float destination;
    public AudioClip[] SFX;
    private AudioSource audio;

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
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (transform.rotation.eulerAngles.z > (destination - 0.1f) && transform.rotation.eulerAngles.z < (destination + 0.1f) && moving)
        {
            Vector3 newPos = transform.rotation.eulerAngles;
            newPos.z = destination;
            transform.rotation.eulerAngles.Set(newPos.x, newPos.y, newPos.z);
            moving = false;
        }    
        

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetAxis("VerticalJoy") == 1) && !moving)
        {
            moving = true;
            audio.clip = SFX[0];
            audio.Play();
            destination = (transform.rotation.eulerAngles.z + 90)%360;
            transform.DOLocalRotate(new Vector3(0, 0, 90), 1, RotateMode.LocalAxisAdd);
            camera.transform.DOShakePosition(0.6F, 0.7f);
            if (selezione < 4) selezione++;
            else selezione = 1;

            Debug.Log(selezione);
            //StartCoroutine(MoveRight());
        }
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetAxis("VerticalJoy") == -1) && !moving)
        {
            moving = true;
            audio.clip = SFX[0];
            audio.Play();
            destination = (360+transform.rotation.eulerAngles.z - 90)%360;
            transform.DOLocalRotate(new Vector3(0, 0, -90), 1, RotateMode.LocalAxisAdd);
            camera.transform.DOShakePosition(0.6F, 0.7f);
            if (selezione > 1) selezione--;
            else selezione = 4;

            Debug.Log(selezione);
            //StartCoroutine(MoveLeft());
        }
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1joy")) && !moving)
        {
            audio.clip = SFX[1];
            audio.Play();
            StartCoroutine(Selection());
        }

        //if (destination > 360) 
    }
}