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

        Debug.Log("Map Created");

        turnManagerComponent.setInsertArrows(mapManagerComponent.getAllInstancedArrows());
        turnManagerComponent.ArrangePlayersInTurnOrder();
        turnManagerComponent.CameraSetRowsAndColumns();
        turnManagerComponent.ActivatePlayer(0);
        turnManagerComponent.PassTurn();
        //HERE
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
