using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneFade", menuName = "Personal Tools/Fade Between Scenes", order = 2)]
public class SceneFade : ScriptableObject
{
    public GameObject mask;
    private GameObject maskInstance;
    [Range(0, 1)]
    public float fadeInTime, fadeOutTime;

    public void createFadeMask ()
    {
        maskInstance = Instantiate(mask, new Vector3 (0, 0, -2), Quaternion.identity);
    }

    public IEnumerator FadeOut (string destination)
    {
        while (maskInstance.GetComponent<Renderer>().material.color.a < 1)
        {
            maskInstance.GetComponent<Renderer>().material.color += new Color(0, 0, 0, fadeOutTime);
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
            maskInstance.GetComponent<Renderer>().material.color -= new Color(0, 0, 0, fadeInTime);
            yield return null;
        }

    }

    
}
