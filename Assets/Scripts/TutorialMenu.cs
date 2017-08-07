using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TutorialMenu : MonoBehaviour {
    public GameObject camera, videoPlayer, particle, particleInst, text;
    public FadeManager fade;
    public VideoClip[] tutorials;
    public int selezione = 0;
    float timer;
    private bool fadeOut, fadeIn;
    public int sortingOrder = 0;
    public float partLeft, partRight;
    public bool move;
    private float destination;
    public Sprite[] texts;
    public AudioClip[] SFX;

	void Start ()
    {
        StartCoroutine(fade.FadeIn());
    }
	
	void Update ()
    {
        text.GetComponent<SpriteRenderer>().sprite = texts[selezione];

        if (transform.position.y > (destination - 0.15f) && transform.position.y < (destination + 0.15f) && move)
        {
            Vector3 newPos = transform.position;
            newPos.y = destination;
            transform.position.Set(newPos.x, newPos.y, newPos.z);
            //move = false;
        }

        if (transform.position.y == destination) move = false;

        //if (transform.position.y % 7.5 == 0 || transform.position.y % 7.5 == 7.5) move = false;
        //else move = true;

            if ((Input.GetKeyDown(KeyCode.W) || Input.GetAxis("VerticalJoy") == 1 || Input.GetAxis("VerticalAnalog") >= 0.9f) && !move)
            {
                if (transform.position.y > -4)
                {
                move = true;
                destination = transform.position.y - 1.33f;
                GetComponent<AudioSource>().clip = SFX[0];
                GetComponent<AudioSource>().Play();
                selezione--;
                transform.DOMoveY(transform.position.y - 1.33f, 1f);
                for (int i = 0; i < 3; i++)
                {
                    particleInst = Instantiate(particle, Vector3.zero, transform.rotation);
                    particleInst.transform.SetParent(transform);
                    particleInst.transform.localPosition = new Vector3(partRight, Random.Range(-10, +10), 0);
                }
                for (int i = 0; i < 3; i++)
                {
                    particleInst = Instantiate(particle, Vector3.zero, transform.rotation);
                    particleInst.transform.SetParent(transform);
                    particleInst.transform.localPosition = new Vector3(partLeft, Random.Range(-10, +10), 0);
                }
                
                camera.transform.DOShakePosition(0.2f, 0.6f);
                fadeOut = true;
                }

            Debug.Log(selezione);
            }

        if ((Input.GetKeyDown(KeyCode.S) || Input.GetAxis("VerticalJoy") == -1 || Input.GetAxis("VerticalAnalog") <= -0.9f) && !move)
        {
            if (transform.position.y < 4)
            {
                move = true;
                destination = transform.position.y + 1.33f;
                GetComponent<AudioSource>().clip = SFX[0];
                GetComponent<AudioSource>().Play();
                selezione++;
                transform.DOMoveY(transform.position.y + 1.33f, 1f);
                for (int i = 0; i < 3; i++)
                {
                    particleInst = Instantiate(particle, Vector3.zero, transform.rotation);
                    particleInst.transform.SetParent(transform);
                    particleInst.transform.localPosition = new Vector3(partRight, Random.Range(-10, +10), 0);
                }
                for (int i = 0; i < 3; i++)
                {
                    particleInst = Instantiate(particle, Vector3.zero, transform.rotation);
                    particleInst.transform.SetParent(transform);
                    particleInst.transform.localPosition = new Vector3(partLeft, Random.Range(-10, +10), 0);
                }

                camera.transform.DOShakePosition(0.2f, 0.6f);
                fadeOut = true;
            }

            Debug.Log(selezione);
        }

        if ((Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2joy")) && !fade.fading)
        {
            GetComponent<AudioSource>().clip = SFX[1];
            GetComponent<AudioSource>().Play();
            StartCoroutine(fade.FadeOut("_Scenes/MenuIniziale"));
        }


        if (fadeOut)
        {
            videoPlayer.GetComponent<Renderer>().material.color -= new Color(0, 0, 0, .015f);
        }

        if (videoPlayer.GetComponent<Renderer>().material.color.a <= 0)
        {
            fadeOut = false;
            videoPlayer.GetComponent<VideoPlayer>().clip = tutorials[selezione];
            videoPlayer.GetComponent<Renderer>().material.color += new Color(0, 0, 0, 1);
        }
        
    }

    
}
