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
    private bool moving = false;
    public Coordinate coordinate;
    public GameObject mapManager;
    public MapManager mapManagerComponent;
    public List<Tile> toBright;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Tile")
        {
            Tile myTile = collision.gameObject.GetComponent<Tile>();
            UpdatePlayerPosition(myTile);
        }
    }

    public void UpdatePlayerPosition(Tile myTile)
    {
        coordinate = myTile.GetCoordinatesCopy();
    }

    public void UpdatePlayerPosition()
    {
        Tile myTile = transform.parent.GetComponent<Tile>();
        coordinate = myTile.GetCoordinatesCopy();
    }

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
        myCollider.size = new Vector2(myTexture.width, myTexture.height);

    }

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

    public void ResetToStartingPosition()
    {
        Vector3 cornerTransform;

        switch (playerNbr)
        {

            case 1:
                cornerTransform = mapManagerComponent.myMap[0, mapManagerComponent.rows - 1].transform.position;
                transform.position = new Vector3(cornerTransform.x, cornerTransform.y, -1);
                transform.SetParent(mapManagerComponent.myMap[0, mapManagerComponent.rows - 1].transform);
                
                break;
            case 2:
                cornerTransform = mapManagerComponent.myMap[mapManagerComponent.columns - 1, mapManagerComponent.rows - 1].transform.position;
                transform.position = new Vector3(cornerTransform.x, cornerTransform.y, -1);
                transform.SetParent(mapManagerComponent.myMap[mapManagerComponent.columns - 1, mapManagerComponent.rows - 1].transform);
                break;
            case 3:
                cornerTransform = mapManagerComponent.myMap[0, 0].transform.position;
                transform.position = new Vector3(cornerTransform.x, cornerTransform.y, -1);
                transform.SetParent(mapManagerComponent.myMap[0, 0].transform);
                break;
            case 4:
                cornerTransform = mapManagerComponent.myMap[mapManagerComponent.columns - 1, 0].transform.position;
                transform.position = new Vector3(cornerTransform.x, cornerTransform.y, -1);
                transform.SetParent(mapManagerComponent.myMap[mapManagerComponent.columns - 1, 0].transform);
                break;
            default:
                break;
        }
    }

    // Use this for initialization
    void Awake()
    {

        myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
        mapManager = GameObject.FindGameObjectWithTag("MapManager");
        toBright = new List<Tile>();
        mapManagerComponent = mapManager.GetComponent<MapManager>();
    }

    // Update is called once per frame
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

    public IEnumerator MoveRight()
    {
        //Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[1]);
        //Debug.Log("effective: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[1]);
        moving = true;

        if (mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[1] == true)
        {
            float elapsedTime = 0;
            float animTime = 0.2f;

            while (elapsedTime < animTime)
            {
                GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX() + 1, coordinate.getY()];
                Vector3 destination = destinationTile.GetComponent<Transform>().position;
                destination.z--;
                transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }


            //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
            coordinate.setCoordinate(coordinate.getX() + 1, coordinate.getY());
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

        while (elapsedTime < animTime)
        {
                GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX(), coordinate.getY() + 1];
                Vector3 destination = destinationTile.GetComponent<Transform>().position;
                destination.z--;
                transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                elapsedTime += Time.deltaTime;
                yield return null;
        }


        //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
        coordinate.setCoordinate(coordinate.getX(), coordinate.getY() + 1);
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

            while (elapsedTime < animTime)
            {
                GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX(), coordinate.getY() - 1];
                Vector3 destination = destinationTile.GetComponent<Transform>().position;
                destination.z--;
                transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }


            //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
            coordinate.setCoordinate(coordinate.getX(), coordinate.getY() - 1);
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

            while (elapsedTime < animTime)
            {
                GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX() - 1, coordinate.getY()];
                Vector3 destination = destinationTile.GetComponent<Transform>().position;
                destination.z--;
                transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }


        //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
        coordinate.setCoordinate(coordinate.getX() - 1, coordinate.getY());
        }

        //yield return new WaitForSeconds(0.1f);

        moving = false;
    }
}


