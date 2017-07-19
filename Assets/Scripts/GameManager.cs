using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public TurnManager turnManagerComponent;

    private GameObject mapManager;
    private MapManager mapManagerComponent;
    private float[][] strartingPositions;

	// Use this for initialization
	void Start () {

        mapManager = transform.GetChild(0).gameObject;
        mapManagerComponent = mapManager.GetComponent<MapManager>();
        mapManagerComponent.MapSetup();

        turnManagerComponent.setInsertArrows(mapManagerComponent.getAllInstancedArrows());
        turnManagerComponent.ArrangePlayersInTurnOrder();
        turnManagerComponent.ActivatePlayer(0);
        turnManagerComponent.PassTurn();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
