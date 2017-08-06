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

    private AudioSource audioComponent;

    // Use this for initialization
    void Awake () {
        audioComponent = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayIntros(int player) 
    {
        if (player == 1)
            audioComponent.volume = 0.15f;
        else audioComponent.volume = 0.3f;
        audioComponent.clip = intros.PlayClip(player);
        audioComponent.Play();
    }
}
