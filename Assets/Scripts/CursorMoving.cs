using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMoving : MonoBehaviour {

    bool isActive;
    bool moving;
    private Coordinate coordinate;
    public GameObject mapManager;
    private MapManager mapManagerComponent;
    public GameObject turnManager;
    private TurnManager turnManagerComponent;

	void Awake ()
    {
        mapManagerComponent = mapManager.GetComponent<MapManager>();
        turnManagerComponent = turnManager.GetComponent<TurnManager>();
        coordinate = new Coordinate(0, 0);
    }
	
	void Update ()
    {
        if (isActive && !moving)
        {
            if (Input.GetKeyDown(KeyCode.D) && coordinate.getX() < mapManagerComponent.columns-1)
            {
                StartCoroutine(MoveRight());
                Debug.Log(coordinate.getX());
            }
            if (Input.GetKeyDown(KeyCode.A) && coordinate.getX() >= 0)
            {
                StartCoroutine(MoveLeft());
            }
            if (Input.GetKeyDown(KeyCode.W) && coordinate.getY() < mapManagerComponent.rows - 1)
            {
                StartCoroutine(MoveUp());
            }
            if (Input.GetKeyDown(KeyCode.S) && coordinate.getY() >= 0)
            {
                StartCoroutine(MoveDown());
            }
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    StartCoroutine(RotateTiles());
            //}

        }
    }

    public void CursorActivate (Coordinate pcoord)
    {
        int x = pcoord.getX();
        int y = pcoord.getY();
        coordinate.setCoordinate(x, y);
        isActive = true;
        Vector3 destination = mapManagerComponent.myMap[x,y].transform.position;
        destination.z = -2;
        transform.position = destination;   
    }

    public void CursorDeactivate ()
    {
        isActive = false;
        Vector3 reset = new Vector3 (0, 0, 2);
        transform.position = reset;
    }

    public Coordinate[] getSelectedCoords()
    {
        var selectedCoords = new Coordinate[4];
        selectedCoords[0] = new Coordinate(coordinate.getX(), coordinate.getY());
        selectedCoords[1] = new Coordinate(coordinate.getX(), coordinate.getY() - 1);
        selectedCoords[2] = new Coordinate(coordinate.getX()+1, coordinate.getY()-1);
        selectedCoords[3] = new Coordinate(coordinate.getX()+1, coordinate.getY());

        return selectedCoords;
    }

    // deprecated method
    public IEnumerator RotateTiles(int rotationDirection) // 1 clockwise, -1 counterclockwise
    {
        turnManagerComponent.makePlayersChild();

        GameObject tmpTile = mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()];
        GameObject tmpBottomTile = mapManagerComponent.myMap[coordinate.getX(), coordinate.getY() - 1];
        GameObject tmpBottomRightTile = mapManagerComponent.myMap[coordinate.getX() + 1, coordinate.getY() - 1];
        GameObject tmpRightTile = mapManagerComponent.myMap[coordinate.getX() + 1, coordinate.getY()];

        mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()] = tmpBottomTile;

        mapManagerComponent.myMap[coordinate.getX(), coordinate.getY() - 1] = tmpBottomRightTile;

        mapManagerComponent.myMap[coordinate.getX() + 1, coordinate.getY() - 1] = tmpRightTile;

        mapManagerComponent.myMap[coordinate.getX() + 1, coordinate.getY()] = tmpTile;

        tmpTile.GetComponent<Tile>().myCoord.setCoordinate(coordinate.getX(), coordinate.getY());
        tmpBottomTile.GetComponent<Tile>().myCoord.setCoordinate(coordinate.getX(), coordinate.getY() - 1);
        tmpBottomRightTile.GetComponent<Tile>().myCoord.setCoordinate(coordinate.getX() + 1, coordinate.getY() - 1);
        tmpRightTile.GetComponent<Tile>().myCoord.setCoordinate(coordinate.getX() + 1, coordinate.getY());


        Vector3 newPosition = tmpTile.transform.position;
        newPosition.x = newPosition.x + 5;
        tmpTile.transform.position = newPosition;

        newPosition.y = newPosition.y - 5;
        tmpRightTile.transform.position = newPosition;

        newPosition.x = newPosition.x - 5;
        tmpBottomRightTile.transform.position = newPosition;

        newPosition.y = newPosition.y + 5;
        tmpBottomTile.transform.position = newPosition;

        yield return null;
    }

    public IEnumerator MoveUp()
    {
        moving = true;

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
            
        coordinate.setCoordinate(coordinate.getX(), coordinate.getY() + 1);
        moving = false;
    }

    public IEnumerator MoveRight()
    {
        moving = true;

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

        coordinate.setCoordinate(coordinate.getX() + 1, coordinate.getY());
        moving = false;
    }

    public IEnumerator MoveDown()
    {
        moving = true;

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

        coordinate.setCoordinate(coordinate.getX(), coordinate.getY() - 1);
        moving = false;
    }

    public IEnumerator MoveLeft()
    {
        moving = true;

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

        coordinate.setCoordinate(coordinate.getX() - 1, coordinate.getY());
        moving = false;
    }

}
