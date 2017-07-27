using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float zoomOutSizeLimit, zoomInSizeLimit, zoomingSpeed, movingSpeed;
    public float maxXDisplacement, maxYDisplacement;

    private int mapColumns, mapRows;
    private float[] xLimits, yLimits;
    private Camera thisCamera;
    private bool isFollowingPlayer;
    private Player followedPlayer;

    public void SetRowsAndColumns(MapManager mapManager)
    {
        mapColumns = mapManager.columns;
        mapRows = mapManager.rows;
    }

    public void StartFollowingPlayer(Player player)
    {
        followedPlayer = player;
        isFollowingPlayer = true;
    }

    public void StopFollowingPlayer()
    {
        followedPlayer = null;
        isFollowingPlayer = false;
    }

    private void MoveCamera()
    {
        if ((Input.GetKey(KeyCode.RightArrow)))
        {
            float newPosX = transform.position.x + movingSpeed * Time.deltaTime;
            newPosX = Mathf.Clamp(newPosX, xLimits[0], xLimits[1]);
            transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
        }
        if ((Input.GetKey(KeyCode.LeftArrow)))
        {
            float newPosX = transform.position.x - movingSpeed * Time.deltaTime;
            newPosX = Mathf.Clamp(newPosX, xLimits[0], xLimits[1]);
            transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
        }
        if ((Input.GetKey(KeyCode.UpArrow)))
        {
            float newPosY = transform.position.y + movingSpeed * Time.deltaTime;
            newPosY = Mathf.Clamp(newPosY, yLimits[0], yLimits[1]);
            transform.position = new Vector3(transform.position.x, newPosY, transform.position.z);
        }
        if ((Input.GetKey(KeyCode.DownArrow)))
        {
            float newPosY = transform.position.y - movingSpeed * Time.deltaTime;
            newPosY = Mathf.Clamp(newPosY, yLimits[0], yLimits[1]);
            transform.position = new Vector3(transform.position.x, newPosY, transform.position.z);
        }
    }

    private void ZoomCamera()
    {
        if ((Input.GetKey(KeyCode.U)))
        {
            float newSize = thisCamera.orthographicSize + zoomingSpeed * Time.deltaTime;
            newSize = Mathf.Clamp(newSize, zoomInSizeLimit, zoomOutSizeLimit);
            thisCamera.orthographicSize = newSize;
        }
        if ((Input.GetKey(KeyCode.I)))
        {
            float newSize = thisCamera.orthographicSize - zoomingSpeed * Time.deltaTime;
            newSize = Mathf.Clamp(newSize, zoomInSizeLimit, zoomOutSizeLimit);
            thisCamera.orthographicSize = newSize;
        }

        UpdatePositionLimits();
        RepositionInsideLimits();
    }

    private void FollowPlayer()
    {
        var playerPosition = followedPlayer.coordinate.GetPositionFromCoords(mapColumns, mapRows);
        float cameraX = Mathf.Clamp(playerPosition.x, xLimits[0], xLimits[1]);
        float cameraY = Mathf.Clamp(playerPosition.y, yLimits[0], yLimits[1]);
        var cameraPosition = new Vector3(cameraX, cameraY, transform.position.z);
        transform.position = cameraPosition;
    }

    private void UpdatePositionLimits()
    {
        float xLim = (maxXDisplacement * (zoomOutSizeLimit - thisCamera.orthographicSize)) / (zoomOutSizeLimit - zoomInSizeLimit);
        xLimits = new float[2] { -xLim, xLim };

        float yLim = (maxYDisplacement * (zoomOutSizeLimit - thisCamera.orthographicSize)) / (zoomOutSizeLimit - zoomInSizeLimit);
        yLimits = new float[2] { -yLim, yLim };
    }

    private void RepositionInsideLimits()
    {
        float newPosX = Mathf.Clamp(transform.position.x, xLimits[0], xLimits[1]);
        float newPosY = Mathf.Clamp(transform.position.y, yLimits[0], yLimits[1]);
        transform.position = new Vector3(newPosX, newPosY, transform.position.z);
    }

    // Use this for initialization
    void Start () {

        //zoomOutSizeLimit = 46f;
        //zoomInSizeLimit = 10f;

        xLimits = new float[2] { 0f, 0f };
        yLimits = new float[2] { 0f, 0f };

        thisCamera = GetComponent<Camera>();

        isFollowingPlayer = false;

    }
	
	// Update is called once per frame
	void Update () {

        if (isFollowingPlayer)
            FollowPlayer();
        else
            MoveCamera();

        ZoomCamera();

    }
}
