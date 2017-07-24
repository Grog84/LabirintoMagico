using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour {
    public GameObject loadText;
    
    IEnumerator loading ()
    {
        loadText.GetComponent<Text>().text = "LOADING";
        yield return new WaitForSeconds(1);
        loadText.GetComponent<Text>().text = "LOADING.";
        yield return new WaitForSeconds(1);
        loadText.GetComponent<Text>().text = "LOADING..";
        yield return new WaitForSeconds(1);
        loadText.GetComponent<Text>().text = "LOADING...";
        yield return new WaitForSeconds(1);
    }

    // Use this for initialization
    void Start () {
        SceneManager.LoadScene("_Scenes/scenaprova");
    }
	
	// Update is called once per frame
	void Update ()
    {
        StartCoroutine(loading());
	}
}
