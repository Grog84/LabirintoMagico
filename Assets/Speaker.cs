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

    public float getClipDuration()
    {
        return audioComponent.clip.length;
    }

    void Awake () {
        audioComponent = this.GetComponent<AudioSource>();
        theme = music.GetComponent<AudioSource>();
	}
	
	void Update ()
    {
        if (audioComponent.isPlaying)
        {
            theme.volume = 0.07f;
        }
        else theme.volume = 0.15f;
    }

    public void PlayBegin()
    {
        audioComponent.clip = begin.PlayClip();
        audioComponent.Play();
    }

    public void PlayIntros(int player) 
    {
        StartCoroutine(Intros(player));
    }

    IEnumerator Intros (int player)
    {
        yield return new WaitForSeconds(2.5f);
        if (player == 1)
            audioComponent.volume = 0.15f;
        else audioComponent.volume = 0.3f;
        audioComponent.clip = intros.PlayClip(player);
        audioComponent.Play();
        yield return null;
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

    public void PlayPvP(int player, int otherPlayer)
    {
        switch (player)
        {
            case 0:
                if (otherPlayer == 3) audioComponent.clip = pvp.PlayClip(0);
                else audioComponent.clip = pvp.PlayClip(1);
                break;
            case 1:
                if (otherPlayer == 2) audioComponent.clip = pvp.PlayClip(2);
                else audioComponent.clip = pvp.PlayClip(3);
                break;
            case 2:
                if (otherPlayer == 1) audioComponent.clip = pvp.PlayClip(4);
                else audioComponent.clip = pvp.PlayClip(5);
                break;
            case 3:
                if (otherPlayer == 0) audioComponent.clip = pvp.PlayClip(6);
                else audioComponent.clip = pvp.PlayClip(7);
                break;
        }
        audioComponent.Play();
    }
}


