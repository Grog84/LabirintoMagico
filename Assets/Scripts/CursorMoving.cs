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

    // Activation/Deactivation

    public void CursorActivate (Coordinate pcoord)
    {
        int x = Mathf.Clamp(pcoord.getX(), 0, mapManagerComponent.columns-2);
        int y = Mathf.Clamp(pcoord.getY(), 1, mapManagerComponent.rows-1);
        coordinate.setCoordinate(x, y);
        isActive = true;
        Vector3 destination = mapManagerComponent.myMap[x,y].transform.position;
        destination.z = -2;
        transform.position = destination;   
    }

    public void CursorDeactivate ()
    {
        isActive = false;
    }

    // Movement Utilities

    public void SetAtPosition(Vector3 position)
    {
        transform.position = position;
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

    // Movement Coroutines

    public IEnumerator MoveUp()
    {
        moving = true;

        float elapsedTime = 0;
        float animTime = 0.2f;

        int newX = Mathf.Clamp(coordinate.getX(), 0, mapManagerComponent.columns - 2);
        int newY = Mathf.Clamp(coordinate.getY() + 1, 1, mapManagerComponent.rows - 1);
        GameObject destinationTile = mapManagerComponent.myMap[newX, newY];
        Vector3 destination = destinationTile.GetComponent<Transform>().position;
        destination.z--;

        while (elapsedTime < animTime)
        {
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

        int newX = Mathf.Clamp(coordinate.getX() + 1, 0, mapManagerComponent.columns - 2);
        int newY = Mathf.Clamp(coordinate.getY(), 1, mapManagerComponent.rows - 1);
        GameObject destinationTile = mapManagerComponent.myMap[newX, newY];
        Vector3 destination = destinationTile.GetComponent<Transform>().position;
        destination.z--;

        while (elapsedTime < animTime)
        {
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

        int newX = Mathf.Clamp(coordinate.getX(), 0, mapManagerComponent.columns - 2);
        int newY = Mathf.Clamp(coordinate.getY() - 1, 1, mapManagerComponent.rows - 1);
        GameObject destinationTile = mapManagerComponent.myMap[newX, newY];
        Vector3 destination = destinationTile.GetComponent<Transform>().position;
        destination.z--;

        while (elapsedTime < animTime)
        {
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

        int newX = Mathf.Clamp(coordinate.getX() - 1, 0, mapManagerComponent.columns - 2);
        int newY = Mathf.Clamp(coordinate.getY(), 1, mapManagerComponent.rows - 1);
        GameObject destinationTile = mapManagerComponent.myMap[newX, newY];
        Vector3 destination = destinationTile.GetComponent<Transform>().position;
        destination.z--;

        while (elapsedTime < animTime)
        {
            transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        coordinate.setCoordinate(coordinate.getX() - 1, coordinate.getY());
        moving = false;
    }

    // Unity Specific methods

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
            if ((Input.GetKeyDown(KeyCode.D) || Input.GetAxis("HorizontalJoy") == 1 || Input.GetAxis("HorizontalAnalog") >= 0.9f) && coordinate.getX() < mapManagerComponent.columns-2)
            {
                StartCoroutine(MoveRight());
            }
            if ((Input.GetKeyDown(KeyCode.A) || Input.GetAxis("HorizontalJoy") == -1 || Input.GetAxis("HorizontalAnalog") <= -0.9f) && coordinate.getX() >= 0)
            {
                StartCoroutine(MoveLeft());
            }
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetAxis("VerticalJoy") == 1 || Input.GetAxis("VerticalAnalog") >= 0.9f) && coordinate.getY() < mapManagerComponent.rows - 1)
            {
                StartCoroutine(MoveUp());
            }
            if ((Input.GetKeyDown(KeyCode.S) || Input.GetAxis("VerticalJoy") == -1 || Input.GetAxis("VerticalAnalog") <= -0.9f) && coordinate.getY() >= 1)
            {
                StartCoroutine(MoveDown());
            }

        }
    }

    // deprecated methods

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

}
