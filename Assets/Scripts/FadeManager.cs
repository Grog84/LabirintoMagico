﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SceneFade", menuName = "Personal Tools/Fade Between Scenes", order = 2)]
public class FadeManager : ScriptableObject
{
    public GameObject mask;
    public GameObject myCanvas;
    public Image maskUI, maskUIInstance;
    public GameObject maskInstance;
    public bool fading;

    [Header("FadeIn Speed")]
    [Tooltip ("FadeIn Speed value between 1 and 20.")]
    [Range(1, 20)]
    public int inSpeed = 1;

    [Header("FadeOut Speed")]
    [Tooltip("FadeOut Speed value between 1 and 20.")]
    [Range(1, 20)]
    public int outSpeed = 1;

    private float fadeInEffective, fadeOutEffective;

    public void createFadeMaskAZ(int zOrder)
    {
        //mask = Resources.Load("Assets/Prefabs/FadeMask");
        fadeInEffective = ((float)inSpeed / 1000) * 5;
        fadeOutEffective = ((float)outSpeed / 1000) * 5;
        maskInstance = Instantiate(mask, new Vector3(0, 0, zOrder), Quaternion.identity);
        maskInstance.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
    }

    public void createFadeMask ()
    {
        //mask = Resources.Load("Assets/Prefabs/FadeMask");
        fadeInEffective = ((float)inSpeed / 1000) * 5;
        fadeOutEffective = ((float)outSpeed / 1000) * 5;
        maskInstance = Instantiate(mask, new Vector3 (0, 0, -2), Quaternion.identity);
    }

    public void createFadeMaskUI()
    {
        //mask = Resources.Load("Assets/Prefabs/FadeMask");
        fadeInEffective = ((float)inSpeed / 1000) * 5;
        fadeOutEffective = ((float)outSpeed / 1000) * 5;
        myCanvas = GameObject.FindGameObjectWithTag("Canvas");
        maskUIInstance = Instantiate(maskUI, myCanvas.transform);
    }

    public void createFadeMask(int zOrder)
    {
        //mask = Resources.Load("Assets/Prefabs/FadeMask");
        fadeInEffective = ((float)inSpeed / 1000) * 5;
        fadeOutEffective = ((float)outSpeed / 1000) * 5;
        maskInstance = Instantiate(mask, new Vector3(0, 0, zOrder), Quaternion.identity);
    }

    public IEnumerator FadeOut (string destination)
    {
        fading = true;
        while (maskInstance.GetComponent<Renderer>().material.color.a < 1)
        {
            maskInstance.GetComponent<Renderer>().material.color += new Color(0, 0, 0, fadeOutEffective);
            yield return null;
        }
        fading = false;
        SceneManager.LoadScene(destination);
        yield return null;
    }

    public IEnumerator FadeOutUI(string destination)
    {
        fading = true;
        while (maskUIInstance.GetComponent<Image>().color.a < 1)
        {
            maskUIInstance.GetComponent<Image>().color += new Color(0, 0, 0, fadeOutEffective);
            yield return null;
        }
        fading = false;
        SceneManager.LoadScene(destination);
        yield return null;
    }

    public IEnumerator FadeIn()
    {
        fading = true;
        createFadeMask();
        while (maskInstance.GetComponent<Renderer>().material.color.a > 0)
        {
            maskInstance.GetComponent<Renderer>().material.color -= new Color(0, 0, 0, fadeInEffective);
            yield return null;
        }
        fading = false;
    }

    public IEnumerator FadeIn(int zOrder)
    {
        fading = true;
        createFadeMask(zOrder);
        while (maskInstance.GetComponent<Renderer>().material.color.a > 0)
        {
            maskInstance.GetComponent<Renderer>().material.color -= new Color(0, 0, 0, fadeInEffective);
            yield return null;
        }
        fading = false;
    }

    public IEnumerator FadeInUI()
    {
        fading = true;
        createFadeMaskUI();
        while (maskUIInstance.GetComponent<Image>().color.a > 0)
        {
            maskUIInstance.GetComponent<Image>().color -= new Color(0, 0, 0, fadeInEffective);
            yield return null;
        }
        fading = false;
    }


}
