using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CubeAnimator : MonoBehaviour {

    private float timer;
    private float videoFinish;


	void Awake ()
    {
        videoFinish = (float)GetComponent<VideoPlayer>().clip.length * 1.25f;
	}
	
	void Update ()
    {
        timer += Time.deltaTime;

        if (timer >= videoFinish) SceneManager.LoadScene("_Scenes/MenuIniziale");
	}
}
