using System.Collections;
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
    public bool moving = false;
    public Coordinate coordinate;
    public GameObject mapManager, turnManager;
    public MapManager mapManagerComponent;
    public List<Tile> toBright;
    public bool hasDiamond = false;
    private Coordinate startingPoint;
    private bool checkingCombat = false;
    private bool isStasisActive = false, canActivateStasis = false;
    private int turnsBeforeStasisCounter = 0, turnsBeforeStasisIsActive = 3;

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

    // Initial Assignements

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

    // Walking Path

    public void BrightPossibleTiles()
    {
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
        if (!isStasisActive)
        {
            Tile myTile = transform.parent.GetComponent<Tile>();
            coordinate = myTile.GetCoordinatesCopy();
        }
    }

    public void ResetToStartingPosition()
    {
        TeleportAtCoordinates(startingPoint);
    }

    public void TeleportAtCoordinates(Coordinate coord)
    {
        GameObject targetTile;

        transform.parent = null;
        this.coordinate = coord;
        targetTile = mapManagerComponent.PickTileObject(coord);
        transform.position = new Vector3(targetTile.transform.position.x, targetTile.transform.position.y, -5);
        transform.SetParent(targetTile.transform);
        targetTile.GetComponent<Tile>().SetPlayerChild(playerNbr);

    }

    public void TeleportOffScreen()
    {
        transform.position = new Vector3(-1000, 0, -5);
    }

    public void CheckForTraps(Tile tile)
    {
        if (tile.GetIsTrapped() && tile.GetTrap().GetIsActive())
        {
            if (hasDiamond)
            {
                turnManager.GetComponent<TurnManager>().DropDiamond(this);
            }
            tile.myTrapComponent.Trigger(this);
            turnManager.GetComponent<TurnManager>().SetTrapHasTriggered(true);
        }
    }

    public bool CheckForVictory()
    {
        if (hasDiamond && coordinate.isEqual(startingPoint))
            return true;
        else
            return false;
    }

    public void CheckForOtherPlayer(Tile tile)
    {
        int other = tile.GetPlayerChild();
        if (other != -1)
        {
            checkingCombat = true;
            StartCoroutine(AttackPlayerOnTile(tile));
        }
    }
 
    public IEnumerator AttackPlayerOnTile(Tile tile)
    {
        Player otherPlayer = tile.gameObject.transform.GetComponentInChildren<Player>();
        turnManager.GetComponent<TurnManager>().DropDiamond(otherPlayer);
        otherPlayer.ResetToStartingPosition();

        float elapsedTime = 0;
        float animTime = 1f;

        Vector3 destination = tile.GetComponent<Transform>().position;
        destination.z--;

        while (elapsedTime < animTime)
        {
            transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        turnManager.GetComponent<TurnManager>().CollectDiamond(this);
        turnManager.GetComponent<TurnManager>().SetAttackHasHappened(true);
        checkingCombat = false;
        yield return null;
    }

    public void UnchildFromTile()
    {
        if(!isStasisActive)
        {
            GameObject parentTile = transform.parent.gameObject;
            parentTile.GetComponent<Tile>().SetPlayerChild(-1);
            transform.parent = null;
        }
    }

    // Diamond

    public void SetHasDiamond(bool hasDiamond)
    {
        this.hasDiamond = hasDiamond;
    }

    public bool GetHasDiamond()
    {
        return hasDiamond;
    }

    public bool GetStasisStatus()
    {
        return isStasisActive;
    }

    public void SetStasisStatus(bool status)
    {
        isStasisActive = status;
    }

    public void ActivateStasis()
    {
        UnchildFromTile();
        turnsBeforeStasisCounter = turnsBeforeStasisIsActive;
        canActivateStasis = false;
        isStasisActive = true;
    }

    public void DeactivateStasis()
    {
        turnsBeforeStasisCounter = turnsBeforeStasisIsActive;
        coordinate.getCoordsFromPosition(transform.position, mapManager.GetComponent<MapManager>().columns, mapManager.GetComponent<MapManager>().rows);
        GameObject playerTile = mapManagerComponent.PickTileObject(coordinate);
        transform.SetParent(playerTile.transform);
        canActivateStasis = false;
        isStasisActive = false;
    }

    public void CheckDiamondStatusTimer()
    {
        if(hasDiamond)
        {

            if (isStasisActive)
            {
                DeactivateStasis();
            }
            else
            {
                if (turnsBeforeStasisCounter != 0)
                {
                    turnsBeforeStasisCounter--;
                }
                else
                {
                    canActivateStasis = true;
                }
            }
        }
    }

    public bool GetCanActivateStasis()
    {
        return canActivateStasis;
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

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            CheckForOtherPlayer(destinationTileComponent);

            if (!checkingCombat)
            {
                while (elapsedTime < animTime)
                {
                    transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (checkingCombat)
                {
                    yield return null;
                }
            }

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

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            CheckForOtherPlayer(destinationTileComponent);

            if (!checkingCombat)
            {
                while (elapsedTime < animTime)
                {
                    transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (checkingCombat)
                {
                    yield return null;
                }
            }

            UpdatePlayerPosition(destinationTileComponent);
            CheckForTraps(destinationTileComponent);

            yield return null;

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

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            CheckForOtherPlayer(destinationTileComponent);

            if (!checkingCombat)
            {
                while (elapsedTime < animTime)
                {
                    transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (checkingCombat)
                {
                    yield return null;
                }
            }

            UpdatePlayerPosition(destinationTileComponent);
            CheckForTraps(destinationTileComponent);

            yield return null;

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

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            CheckForOtherPlayer(destinationTileComponent);

            if (!checkingCombat)
            {
                while (elapsedTime < animTime)
                {
                    transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (checkingCombat)
                {
                    yield return null;
                }
            }

            UpdatePlayerPosition(destinationTileComponent);
            CheckForTraps(destinationTileComponent);

            yield return null;

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
        SetStartingPoint();
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


