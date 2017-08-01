using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blackHole : MonoBehaviour {

    private Tile myTile;

    public void AssignTile(Tile thisTile)
    {
        myTile = thisTile;
    }

    public void endAttack()
    {
        myTile.SetBlackHolePlaying(false);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
