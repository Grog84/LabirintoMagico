using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TutorialMenu : MonoBehaviour {
    public GameObject camera, videoPlayer, particle, particleInst;
    public FadeManager fade;
    public VideoClip[] tutorials;
    private int selezione = 0;
    float timer;
    private bool fadeOut, fadeIn;
    public int sortingOrder = 0;
    public float partLeft, partRight;
    public bool move;
    private float destination;

	void Start ()
    {
        StartCoroutine(fade.FadeIn());
	}
	
	void Update ()
    {
        if (transform.position.z > (destination - 0.1f) && transform.position.z < (destination + 0.1f) && move)
        {
            Vector3 newPos = transform.position;
            newPos.z = destination;
            transform.position.Set(newPos.x, newPos.y, newPos.z);
            move = false;
        }

        //if (transform.position.y % 7.5 == 0 || transform.position.y % 7.5 == 7.5) move = false;
        //else move = true;

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetAxis("VerticalJoy") == 1) && !move)
        {
            if (transform.position.y >= 0)
            {
                move = true;
                destination = transform.position.z + 7.5f;
                selezione--;
                transform.DOMoveY(transform.position.y - 7.5f, 1f);
                for (int i = 0; i < 3; i++)
                {
                    particleInst = Instantiate(particle, Vector3.zero, transform.rotation);
                    particleInst.transform.SetParent(transform);
                    particleInst.transform.localPosition = new Vector3(partRight, Random.Range(-25, +25), 0);
                }
                for (int i = 0; i < 3; i++)
                {
                    particleInst = Instantiate(particle, Vector3.zero, transform.rotation);
                    particleInst.transform.SetParent(transform);
                    particleInst.transform.localPosition = new Vector3(partLeft, Random.Range(-25, +25), 0);
                }
                
                camera.transform.DOShakePosition(0.2f, 0.6f);
                fadeOut = true;
            }

            Debug.Log(selezione);
        }

        if ((Input.GetKeyDown(KeyCode.S) || Input.GetAxis("VerticalJoy") == -1) && !move)
        {
            if (transform.position.y <= 0)
            {
                move = true;
                destination = transform.position.z - 7.5f;
                selezione++;
                transform.DOMoveY(transform.position.y + 7.5f, 1f);
                for (int i = 0; i < 3; i++)
                {
                    particleInst = Instantiate(particle, Vector3.zero, transform.rotation);
                    particleInst.transform.SetParent(transform);
                    particleInst.transform.localPosition = new Vector3(partRight, Random.Range(-25, +25), 0);
                }
                for (int i = 0; i < 3; i++)
                {
                    particleInst = Instantiate(particle, Vector3.zero, transform.rotation);
                    particleInst.transform.SetParent(transform);
                    particleInst.transform.localPosition = new Vector3(partLeft, Random.Range(-25, +25), 0);
                }

                camera.transform.DOShakePosition(0.2f, 0.6f);
                fadeOut = true;
            }

            Debug.Log(selezione);
        }

        if ((Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2joy")))
        {
            //fade.GetComponent<FadeManager>().fadeIn = false;
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
