using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "DialogueManager", menuName = "Personal Tools/Dialogue Manager", order = 1)]
public class DialogueManager : ScriptableObject
{
    public string objectName = "DialogueManager";
    public AudioClip[] audioList;
    private int toPlay;

    AudioClip PlayClip ()
    {
        toPlay = Random.Range(0, audioList.Length);
        return audioList[toPlay];
    }

    AudioClip PlayClip (int index)
    {
        toPlay = index;
        return audioList[toPlay];
    }
}