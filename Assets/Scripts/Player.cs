using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int playerNbr;

    SpriteRenderer myRenderer;
    Texture2D myTexture;
    Sprite mySprite;
    BoxCollider2D myCollider;
    private bool moving = false;
    public Coordinate coordinate;
    public GameObject MapManager;

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

    // Use this for initialization
    void Awake()
    {

        myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
        MapManager = GameObject.FindGameObjectWithTag("MapManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving && playerNbr == 1)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(MoveRight());
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(MoveLeft());
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(MoveUp());
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(MoveDown());
            }
        }
    }

    public IEnumerator MoveRight()
    {
        Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[1]);
        Debug.Log("effective: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[1]);
        moving = true;

        if (MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[1] == true)
        {
            float elapsedTime = 0;
            float animTime = 0.2f;

            while (elapsedTime < animTime)
            {
                GameObject destinationTile = MapManager.GetComponent<MapManager>().myMap[coordinate.getX() + 1, coordinate.getY()];
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
        Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[0]);
        moving = true;

        if (MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[0] == true)
        {
        float elapsedTime = 0;
        float animTime = 0.2f;

        while (elapsedTime < animTime)
        {
                GameObject destinationTile = MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY() + 1];
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
        Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[2]);
        moving = true;

        if (MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[2] == true)
        {
            float elapsedTime = 0;
            float animTime = 0.2f;

            while (elapsedTime < animTime)
            {
                GameObject destinationTile = MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY() - 1];
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
        Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[3]);
        moving = true;

        if (MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[3] == true)
        {
            float elapsedTime = 0;
            float animTime = 0.2f;

            while (elapsedTime < animTime)
            {
                GameObject destinationTile = MapManager.GetComponent<MapManager>().myMap[coordinate.getX() - 1, coordinate.getY()];
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


