using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float zoomOutSizeLimit, zoomInSizeLimit, zoomCurrentSize, zoomingSpeed, movingSpeed;
    public float maxXDisplacement, maxYDisplacement;

    private float[] xLimits, yLimits;
    private Camera thisCamera;

    private void MoveCamera()
    {
        if ((Input.GetKeyDown(KeyCode.RightArrow)))
        {
            float newPosX = transform.position.x + movingSpeed * Time.deltaTime;
            newPosX = Mathf.Clamp(newPosX, xLimits[0], xLimits[1]);
            transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
        }
        if ((Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            float newPosX = transform.position.x - movingSpeed * Time.deltaTime;
            newPosX = Mathf.Clamp(newPosX, xLimits[0], xLimits[1]);
            transform.position = new Vector3(newPosX, transform.position.y, transform.position.z);
        }
        if ((Input.GetKeyDown(KeyCode.UpArrow)))
        {
            float newPosY = transform.position.y + movingSpeed * Time.deltaTime;
            newPosY = Mathf.Clamp(newPosY, yLimits[0], yLimits[1]);
            transform.position = new Vector3(transform.position.x, newPosY, transform.position.z);
        }
        if ((Input.GetKeyDown(KeyCode.DownArrow)))
        {
            float newPosY = transform.position.y - movingSpeed * Time.deltaTime;
            newPosY = Mathf.Clamp(newPosY, yLimits[0], yLimits[1]);
            transform.position = new Vector3(transform.position.x, newPosY, transform.position.z);
        }
    }

    private void ZoomCamera()
    {
        if ((Input.GetKeyDown(KeyCode.U)))
        {
            float newSize = thisCamera.orthographicSize + zoomingSpeed * Time.deltaTime;
            newSize = Mathf.Clamp(newSize, zoomInSizeLimit, zoomOutSizeLimit);
            thisCamera.orthographicSize = newSize;
        }
        if ((Input.GetKeyDown(KeyCode.I)))
        {
            float newSize = thisCamera.orthographicSize - zoomingSpeed * Time.deltaTime;
            newSize = Mathf.Clamp(newSize, zoomInSizeLimit, zoomOutSizeLimit);
            thisCamera.orthographicSize = newSize;
        }
    }

    private void FollowPlayer(GameObject player)
    {
        float cameraX = Mathf.Clamp(player.transform.position.x, xLimits[0], xLimits[1]);
        float cameraY = Mathf.Clamp(player.transform.position.y, yLimits[0], yLimits[1]);
        var cameraPosition = new Vector3(cameraX, cameraY, transform.position.z);
    }

    private void UpdatePositionLimits()
    {
        float xLim = (maxXDisplacement * (zoomOutSizeLimit - zoomCurrentSize)) / (zoomOutSizeLimit - zoomInSizeLimit);
        xLimits = new float[2] { -xLim, xLim };

        float yLim = (maxYDisplacement * (zoomOutSizeLimit - zoomCurrentSize)) / (zoomOutSizeLimit - zoomInSizeLimit);
        yLimits = new float[2] { -yLim, yLim };
    }



    // Use this for initialization
    void Start () {

        //zoomOutSizeLimit = 46f;
        //zoomInSizeLimit = 10f;

        xLimits = new float[2] { 0f, 0f };
        yLimits = new float[2] { 0f, 0f };

        thisCamera = GetComponent<Camera>();

    }
	
	// Update is called once per frame
	void Update () {

        MoveCamera();
        ZoomCamera();

    }
}
