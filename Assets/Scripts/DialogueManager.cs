using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "DialogueManager", menuName = "Personal Tools/Dialogue Manager", order = 1)]
public class DialogueManager : ScriptableObject
{
    public string objectName = "DialogueManager";
    public AudioClip[] audioList;
    private int toPlay;

    public AudioClip PlayClip ()
    {
        toPlay = Random.Range(0, audioList.Length);
        return audioList[toPlay];
    }

    public AudioClip PlayClip (int index)
    {
        return audioList[index];
    }
}