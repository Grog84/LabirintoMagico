using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private GameObject mapManager;
    private MapManager managerComponent;
    private float[][] strartingPositions;

	// Use this for initialization
	void Start () {

        mapManager = transform.GetChild(0).gameObject;
        managerComponent = mapManager.GetComponent<MapManager>();
        managerComponent.MapSetup();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
