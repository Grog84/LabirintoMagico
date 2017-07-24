﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int playerNbr;
    public int isPlayerTurn; // number corresponding to the player playing

    SpriteRenderer myRenderer;
    Texture2D myTexture;
    Sprite mySprite;
    BoxCollider2D myCollider;
    private bool moving = false;
    public Coordinate coordinate;
    public GameObject mapManager, turnManager;
    public MapManager mapManagerComponent;
    public List<Tile> toBright;
    private bool hasDiamond = false;
    private Coordinate startingPoint;

    // Accessing Variable

    public Coordinate GetCoordinates()
    {
        return coordinate;
    }

    // Animation

    public void setPlayerSprite()
    {
        switch (playerNbr)
        {
            case 1:
                myTexture = (Texture2D)Resources.Load("PlayerProva/p1");
                break;
            case 2:
                myTexture = (Texture2D)Resources.Load("PlayerProva/p2");
                break;
            case 3:
                myTexture = (Texture2D)Resources.Load("PlayerProva/p3");
                break;
            case 4:
                myTexture = (Texture2D)Resources.Load("PlayerProva/p4");
                break;
            default:
                break;
        }

        mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
        myRenderer.sprite = mySprite;
        myCollider.size = new Vector2(myTexture.width/100f, myTexture.height/100f);

    }

    // Walking Path

    public void BrightPossibleTiles()
    {
        //Debug.Log("passaaa");
        Tile nextToAdd;
        Tile currentPosition = mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>();

        toBright.Add(currentPosition);
        for (int i = 0; i < toBright.Count; i++)
        {
            if (toBright[i].effectiveConnections[0])
            {
                nextToAdd = mapManagerComponent.myMap[toBright[i].myCoord.getX(), toBright[i].myCoord.getY() + 1].GetComponent<Tile>();
                if (!toBright.Contains(nextToAdd))
                {
                    toBright.Add(nextToAdd);
                }
            }

            if (toBright[i].effectiveConnections[1])
            {
                nextToAdd = mapManagerComponent.myMap[toBright[i].myCoord.getX() + 1, toBright[i].myCoord.getY()].GetComponent<Tile>();
                if (!toBright.Contains(nextToAdd))
                {
                    toBright.Add(nextToAdd);
                }
            }

            if (toBright[i].effectiveConnections[2])
            {
                nextToAdd = mapManagerComponent.myMap[toBright[i].myCoord.getX(), toBright[i].myCoord.getY() - 1].GetComponent<Tile>();
                if (!toBright.Contains(nextToAdd))
                {
                    toBright.Add(nextToAdd);
                }
            }

            if (toBright[i].effectiveConnections[3])
            {
                nextToAdd = mapManagerComponent.myMap[toBright[i].myCoord.getX() - 1, toBright[i].myCoord.getY()].GetComponent<Tile>();
                if (!toBright.Contains(nextToAdd))
                {
                    toBright.Add(nextToAdd);
                }
            }
        }

        for (int i = 0; i < toBright.Count; i++)
        {
            toBright[i].GetComponent<SpriteRenderer>().color = Color.green;
            //Debug.Log("passaaa");
        }
    }

    public void SwitchOffTiles()
    {
        for (int i = 0; i < toBright.Count; i++)
        {
            toBright[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
        toBright.Clear();
    }

    // Player position update

    public void UpdatePlayerPosition(Tile myTile)
    {
        coordinate = myTile.GetCoordinatesCopy();
    }

    public void UpdatePlayerPosition()
    {
        Tile myTile = transform.parent.GetComponent<Tile>();
        coordinate = myTile.GetCoordinatesCopy();
    }

    public void ResetToStartingPosition()
    {
        TeleportAtCoordinates(startingPoint);
    }

    public void SetStartingPoint()
    {
        switch (playerNbr)
        {

            case 1:
                startingPoint = new Coordinate(0, mapManagerComponent.rows - 1);
                break;
            case 2:
                startingPoint = new Coordinate(mapManagerComponent.columns - 1, mapManagerComponent.rows - 1);
                break;
            case 3:
                startingPoint = new Coordinate(0, 0);
                break;
            case 4:
                startingPoint = new Coordinate(mapManagerComponent.columns - 1, 0);
                break;
            default:
                break;
        }
    }

    public void TeleportAtCoordinates(Coordinate coord)
    {
        GameObject targetTile;

        targetTile = mapManagerComponent.PickTileObject(coord);
        transform.position = new Vector3(targetTile.transform.position.x, targetTile.transform.position.y, -5);
        transform.SetParent(targetTile.transform);
        targetTile.GetComponent<Tile>().SetPlayerChild(playerNbr);

    }

    public void TeleportOffScreen()
    {
        transform.position = new Vector3(-100, 0, -5);
    }

    public void CheckForTraps(Tile tile)
    {
        if (tile.GetIsTrapped())
        {
            tile.myTrapComponent.Trigger(this);
            turnManager.GetComponent<TurnManager>().SetTrapHasTriggered(true);
        }
    }

    // Dismond

    public void SetHasDiamond(bool hasDiamond)
    {
        this.hasDiamond = hasDiamond;
    }

    // Movement Coroutines

    public IEnumerator MoveRight()
    {
        //Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[1]);
        //Debug.Log("effective: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[1]);
        moving = true;

        if (mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[1] == true)
        {
            float elapsedTime = 0;
            float animTime = 0.2f;

            GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX() + 1, coordinate.getY()];
            Vector3 destination = destinationTile.GetComponent<Transform>().position;
            destination.z--;

            while (elapsedTime < animTime)
            {
                transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            UpdatePlayerPosition(destinationTileComponent);

            CheckForTraps(destinationTileComponent);

            yield return null;

            //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
            //coordinate.setCoordinate(coordinate.getX() + 1, coordinate.getY());
        }

        //yield return new WaitForSeconds(0.1f);

        moving = false;
    }

    public IEnumerator MoveUp()
    {
        //Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[0]);
        moving = true;

        if (mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[0] == true)
        {

        float elapsedTime = 0;
        float animTime = 0.2f;

        GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX(), coordinate.getY() + 1];
        Vector3 destination = destinationTile.GetComponent<Transform>().position;
        destination.z--;

        while (elapsedTime < animTime)
        {
            transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            UpdatePlayerPosition(destinationTileComponent);

            CheckForTraps(destinationTileComponent);

            //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
            //coordinate.setCoordinate(coordinate.getX(), coordinate.getY() + 1);
        }

        //yield return new WaitForSeconds(0.1f);

        moving = false;
    }

    public IEnumerator MoveDown()
    {
        //Debug.Log("effective:" + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[2]);
        //Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[2]);
        moving = true;

        if (mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[2] == true)
        {
            float elapsedTime = 0;
            float animTime = 0.2f;

            GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX(), coordinate.getY() - 1];
            Vector3 destination = destinationTile.GetComponent<Transform>().position;
            destination.z--;

            while (elapsedTime < animTime)
            {
                transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            UpdatePlayerPosition(destinationTileComponent);

            CheckForTraps(destinationTileComponent);

            //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
            //coordinate.setCoordinate(coordinate.getX(), coordinate.getY() - 1);
        }

        //yield return new WaitForSeconds(0.1f);

        moving = false;
    }

    public IEnumerator MoveLeft()
    {
        //Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[3]);
        moving = true;

        if (mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[3] == true)
        {
            float elapsedTime = 0;
            float animTime = 0.2f;

            GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX() - 1, coordinate.getY()];
            Vector3 destination = destinationTile.GetComponent<Transform>().position;
            destination.z--;

            while (elapsedTime < animTime)
            {
                transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            UpdatePlayerPosition(destinationTileComponent);

            CheckForTraps(destinationTileComponent);

            //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
            //coordinate.setCoordinate(coordinate.getX() - 1, coordinate.getY());
        }

        //yield return new WaitForSeconds(0.1f);

        moving = false;
    }

    // Unity Specific methods

    void Awake()
    {
        myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
        mapManager = GameObject.FindGameObjectWithTag("MapManager");
        turnManager = GameObject.FindGameObjectWithTag("TurnManager");
        toBright = new List<Tile>();
        mapManagerComponent = mapManager.GetComponent<MapManager>();
    }

    void Update()
    {
        if (!moving && playerNbr == isPlayerTurn)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetAxis("HorizontalJoy") == 1)
            {
                StartCoroutine(MoveRight());
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetAxis("HorizontalJoy") == -1)
            {
                StartCoroutine(MoveLeft());
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetAxis("VerticalJoy") == -1)
            {
                StartCoroutine(MoveUp());
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetAxis("VerticalJoy") == 1)
            {
                StartCoroutine(MoveDown());
            }
        }
    }

    // deprecated
    private void OnTriggerEnter2D(Collider2D collision)  // This does not work since no physics is involved...
    {
        if (collision.gameObject.tag == "Tile")
        {
            Tile myTile = collision.gameObject.GetComponent<Tile>();
            UpdatePlayerPosition(myTile);

            if (myTile.GetIsTrapped())
            {
                myTile.myTrapComponent.Trigger(this);
                turnManager.GetComponent<TurnManager>().SetTrapHasTriggered(true);
            }

        }
    }
}


