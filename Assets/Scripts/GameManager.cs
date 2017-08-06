using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public TurnManager turnManagerComponent;

    private GameObject mapManager;
    public GameObject dialogueManager;
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
        StartCoroutine(initialPause());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator initialPause ()
    {
        dialogueManager.GetComponent<Speaker>().PlayBegin();
        yield return new WaitForSeconds(5);
        turnManagerComponent.ActivatePlayer(0);
        turnManagerComponent.PassTurn();
        yield return null;
    }
}
