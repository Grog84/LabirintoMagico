using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker : MonoBehaviour {

    public DialogueManager begin;
    public DialogueManager crystalGrab;
    public DialogueManager elfTrap;
    public DialogueManager intros;
    public DialogueManager monkTrap;
    public DialogueManager necroTrap;
    public DialogueManager pvp;
    public DialogueManager pyroTrap;
    public DialogueManager someoneFall;
    public DialogueManager stasys;
    public DialogueManager trapActivated;
    public DialogueManager victory;

    public GameObject music;
    private AudioSource theme;

    private AudioSource audioComponent;

    // Use this for initialization
    void Awake () {
        audioComponent = this.GetComponent<AudioSource>();
        theme = music.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (audioComponent.isPlaying)
        {
            theme.volume = 0.07f;
        }
        else theme.volume = 0.15f;
    }

    public void PlayIntros(int player) 
    {
        if (player == 1)
            audioComponent.volume = 0.15f;
        else audioComponent.volume = 0.3f;
        audioComponent.clip = intros.PlayClip(player);
        audioComponent.Play();
    }

    public void PlayVictory(int player)
    {
        audioComponent.clip = victory.PlayClip(player);
        audioComponent.Play();
    }

    public void PlayCrystalGrab(int player)
    {
        audioComponent.clip = crystalGrab.PlayClip(player);
        audioComponent.Play();
    }

    public void PlayStasys(int index)
    {
        audioComponent.clip = stasys.PlayClip(index);
        audioComponent.Play();
    }

    public void PlaySomeoneFall()
    {
        audioComponent.clip = someoneFall.PlayClip();
        audioComponent.Play();
    }

    public void PlayTrapActivated(int player)
    {
        audioComponent.clip = trapActivated.PlayClip(player);
        audioComponent.Play();
    }
}

