using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimation : MonoBehaviour {

    Player myPlayer;

    public void attack1End()
    {
        myPlayer.SetAttack1Status(false);
    }

    public void attack2End()
    {
        myPlayer.SetAttack2Status(false);
    }

    // Use this for initialization
    void Start () {
        myPlayer = transform.parent.gameObject.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
