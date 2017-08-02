using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneFade", menuName = "Personal Tools/Fade Between Scenes", order = 2)]
public class FadeManager : ScriptableObject
{
    public GameObject mask;
    private GameObject maskInstance;

    [Header("FadeIn Speed")]
    [Tooltip ("FadeIn Speed value between 1 and 20.")]
    [Range(1, 20)]
    public int inSpeed = 1;

    [Header("FadeOut Speed")]
    [Tooltip("FadeOut Speed value between 1 and 20.")]
    [Range(1, 20)]
    public int outSpeed = 1;

    private float fadeInEffective, fadeOutEffective;


    public void createFadeMask ()
    {
        //mask = Resources.Load("Assets/Prefabs/FadeMask");
        fadeInEffective = ((float)inSpeed / 1000) * 5;
        fadeOutEffective = ((float)outSpeed / 1000) * 5;
        maskInstance = Instantiate(mask, new Vector3 (0, 0, -2), Quaternion.identity);
    }

    public IEnumerator FadeOut (string destination)
    {
        while (maskInstance.GetComponent<Renderer>().material.color.a < 1)
        {
            maskInstance.GetComponent<Renderer>().material.color += new Color(0, 0, 0, fadeOutEffective);
            yield return null;
        }
        SceneManager.LoadScene(destination);
        yield return null;
    }

    public IEnumerator FadeIn()
    {
        createFadeMask();
        while (maskInstance.GetComponent<Renderer>().material.color.a > 0)
        {
            maskInstance.GetComponent<Renderer>().material.color -= new Color(0, 0, 0, fadeInEffective);
            yield return null;
        }

    }

    
}
