using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{

    public GameObject walkButton, terraformingButton, crystalButton, passButton, cursor, portraitSelection,
        rotateTileButton, slideTilesButton, terraformBackButton, card1Button, card2Button, cardsBackButton, rotationCursor;
    public GameObject[] portraits;
    public GameObject[] panels;
    public GameObject arrows, cursorArrows; // one is children of the other, thst's why there is no need for an array
    public Camera camera;
    public MapManager mapManager;
    public bool isSliding = false, isRotating = false;

    private int playerPlaying, playerPlayingIdx, turnNbr, selectedButton, selectionDepth;
    private int[] playerOrder;
    private Vector3[] buttonsPosition;
    private bool canMove, canTerraform, canUseCrystal, cursorIsActive;
    private GameObject[] players, tmpPlayers, allInsertArrows;
    private Card[] activeCards;
    private CardButton[] cardsButtonComponent;
    private Player[] playerComponent;
    private RectTransform[] buttonsTransform, panelsTransform;
    private RectTransform cursorTransform;
    private Animator[] buttonsAnimator;
    private Vector2 panelParkingPosition, panelActivePosition, arrowsRelativePosition;
    private Vector3 rotationArrowsParkingPosition, rotationArrowsActivePosition;

    enum myButtons
    {
        Walk, Terraform, Crystal, Pass
    };

    enum slideDirection
    {
        leftToRight, botToTop, rightToLeft, topToBot
    };

    public void setInsertArrows(GameObject[] inputArrows)
    {
        allInsertArrows = inputArrows;
    }

    public void AssignCardButtonComponent()
    {
        cardsButtonComponent[0] = card1Button.GetComponent<CardButton>();
        cardsButtonComponent[1] = card2Button.GetComponent<CardButton>();
    }

    public void AssignPanelsPosition()
    {
        panelsTransform[0] = panels[0].GetComponent<RectTransform>();
        panelsTransform[1] = panels[1].GetComponent<RectTransform>();
        panelsTransform[2] = panels[2].GetComponent<RectTransform>();
    }

    public void AssignButtonsPosition()
    {
        // First Panel Buttons
        buttonsTransform[0] = walkButton.GetComponent<RectTransform>();
        buttonsPosition[0] = buttonsTransform[0].position;
        buttonsTransform[1] = terraformingButton.GetComponent<RectTransform>();
        buttonsPosition[1] = buttonsTransform[1].position;
        buttonsTransform[2] = crystalButton.GetComponent<RectTransform>();
        buttonsPosition[2] = buttonsTransform[2].position;
        buttonsTransform[3] = passButton.GetComponent<RectTransform>();
        buttonsPosition[3] = buttonsTransform[3].position;

        // Terraform Panel Buttons
        buttonsTransform[4] = rotateTileButton.GetComponent<RectTransform>();
        buttonsPosition[4] = buttonsTransform[4].position;
        buttonsTransform[5] = slideTilesButton.GetComponent<RectTransform>();
        buttonsPosition[5] = buttonsTransform[5].position;
        buttonsTransform[6] = terraformBackButton.GetComponent<RectTransform>();
        buttonsPosition[6] = buttonsTransform[6].position;

        // Cards Panel Buttons
        buttonsTransform[7] = card1Button.GetComponent<RectTransform>();
        buttonsPosition[7] = buttonsTransform[7].position;
        buttonsTransform[8] = card2Button.GetComponent<RectTransform>();
        buttonsPosition[8] = buttonsTransform[8].position;
        buttonsTransform[9] = cardsBackButton.GetComponent<RectTransform>();
        buttonsPosition[9] = buttonsTransform[9].position;
    }

    public void AssignButtonsAnimators()
    {
        buttonsAnimator[0] = walkButton.GetComponent<Animator>();
        buttonsAnimator[1] = terraformingButton.GetComponent<Animator>();
        buttonsAnimator[2] = crystalButton.GetComponent<Animator>();
        buttonsAnimator[3] = passButton.GetComponent<Animator>();
    }

    public void ArrangePlayersInTurnOrder()
    {
        // set it up to work with random order even though at the moment the playerOrder is predefined
        tmpPlayers = GetComponentInParent<MapManager>().allPlayers;
        players = new GameObject[4];
        playerComponent = new Player[4];

        //  here we should get also the component defining the plaer movement
        for (int i = 0; i < playerOrder.Length; i++)
        {
            players[playerOrder[i]-1] = tmpPlayers[i];
            playerComponent[playerOrder[i]-1] = tmpPlayers[i].GetComponent<Player>();
        }
    }

    public void ActivatePlayer(int nbr) // if 0 no player is active - The method acts on the flag inside player allowing movement
    {
        for (int i = 0; i < 4; i++)
        {
            playerComponent[i].isPlayerTurn = nbr;
        }
    }

    public IEnumerator MoveCamera(GameObject player) // Temporarly deactivated untill the camera position is properly fixed
    {
        float elapsedTime = 0;
        float animTime = 1f;

        Vector3 old_position = camera.transform.GetComponentInParent<Transform>().position;

        camera.transform.parent = null;
        camera.transform.position = old_position;
        Vector3 destination = player.GetComponent<Transform>().position;
        destination.z = camera.transform.position.z; // 

        while (elapsedTime < animTime)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, destination, elapsedTime / animTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        camera.transform.SetParent(player.transform);
        //ActivatePlayer(player.GetComponent<Player>().playerNbr);
    }

    void ResetButtons() // add reset of the animation
    {
        canMove = true;
        buttonsAnimator[0].SetBool("isActive", true);
        canTerraform = true;
        buttonsAnimator[1].SetBool("isActive", true);
        canUseCrystal = false;

    }

    void ActivateBasePanel()
    {
        selectionDepth = 0;
        selectedButton = 0;
        cursorTransform.position = buttonsTransform[selectedButton].position;
        panelsTransform[0].anchoredPosition = panelActivePosition;
        panelsTransform[1].anchoredPosition = panelParkingPosition;
        panelsTransform[2].anchoredPosition = panelParkingPosition;
    }

    void ActivateTerraformPanel()
    {
        selectionDepth = 1;
        selectedButton = 4;
        cursorTransform.position = buttonsTransform[selectedButton].position;
        panelsTransform[0].anchoredPosition = panelParkingPosition;
        panelsTransform[1].anchoredPosition = panelActivePosition;
        panelsTransform[2].anchoredPosition = panelParkingPosition;
    }

    void ActivateCardSelection()
    {
        selectionDepth = 2;
        selectedButton = 7;
        cursorTransform.position = buttonsTransform[selectedButton].position;
        panelsTransform[0].anchoredPosition = panelParkingPosition;
        panelsTransform[1].anchoredPosition = panelParkingPosition;
        panelsTransform[2].anchoredPosition = panelActivePosition;
    }

    void EndTerraform()
    {
        // TODO Update Connectivity Map
        canTerraform = false;
        terraformingButton.GetComponent<Animator>().SetBool("isActive", false);
    }

    // TODO
    IEnumerator RotateTiles(int rotationDirection) // 1 clockwise, -1 counterclockwise
    {
        Coordinate[] selectedCoords = rotationCursor.GetComponent<CursorMoving>().getSelectedCoords();
        selectedCoords = mapManager.KeepMovableTiles(selectedCoords);

        if (rotationDirection == 1)
        {
            var tmp = new Coordinate[selectedCoords.Length];
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[tmp.Length - 1 - i] = selectedCoords[i];
            }
        }

        Tile tmpTile;
        var tmpTileArray = new Tile[selectedCoords.Length];
        GameObject tmpTileObj;
        var tmpTileObjArray = new GameObject[selectedCoords.Length];

        float animationTime = 3f;
        var myMovement = new Vector2(0, 0);

        for (int i = 0; i < selectedCoords.Length; i++)
        {
            tmpTile = mapManager.PickTileComponent(selectedCoords[i]);
            tmpTileObj = mapManager.PickTileObject(selectedCoords[i]);
            tmpTileArray[i] = tmpTile;
            tmpTileObjArray[i] = tmpTileObj;

            myMovement[0] = mapManager.PickTileObject(selectedCoords[(i + 1) % (selectedCoords.Length)]).transform.position.x - tmpTileObj.transform.position.x;
            myMovement[1] = mapManager.PickTileObject(selectedCoords[(i + 1) % (selectedCoords.Length)]).transform.position.y - tmpTileObj.transform.position.y;
            StartCoroutine(tmpTile.MoveToPosition(myMovement, animationTime));
        }

        for (int i = 0; i < selectedCoords.Length; i++)
        {
            mapManager.myMap[selectedCoords[(i + 1) % (selectedCoords.Length)].getX(), selectedCoords[(i + 1) % (selectedCoords.Length)].getY()] = tmpTileObjArray[i];
            mapManager.myMapTiles[selectedCoords[(i + 1) % (selectedCoords.Length)].getX(), selectedCoords[(i + 1) % (selectedCoords.Length)].getY()] = tmpTileArray[i];
        }

        float waitingTime = 0;

        while (waitingTime < animationTime + 0.5f)
        {
            waitingTime += Time.deltaTime;
            if (waitingTime > animationTime)
                isRotating = false;
            yield return null;
        }

        yield return null;

    }

    IEnumerator InsertTile(int cardNbr, GameObject arrow, int slideDirection)
    {
        isSliding = true;
        Coordinate[] lineCoordinates = arrow.GetComponent<InsertArrow>().getPointedCoords();
        lineCoordinates = mapManager.KeepMovableTiles(lineCoordinates);
        StartCoroutine(mapManager.SlideLine(lineCoordinates, slideDirection));

        int tileType = 0;

        while (isSliding)
        {
            yield return null;
        }

        CardButton activatedCard = null;
        if (cardNbr == 1)
        {
            activatedCard = cardsButtonComponent[0];
        }
        else if (cardNbr == 2)
        {
            activatedCard = cardsButtonComponent[1];
        }

        tileType = activatedCard.GetTileType();

        mapManager.InstantiateTileLive(tileType, lineCoordinates[0]);

        arrow.GetComponent<Animator>().SetBool("isActive", false);
        EndTerraform();
        ActivateBasePanel();

        yield return null;
    }

    IEnumerator ScrollTileInsertionSelection(int cardNbr)
    {
        int currentSelection = 39; // Starts in the Top Left Side
        allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);

        yield return null;

        while (!Input.GetKeyDown(KeyCode.B) && !Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (currentSelection >= 26 && currentSelection <= 39)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
                else if (currentSelection >= 0 && currentSelection <= 12)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
                else if (currentSelection == 51)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection = 0;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
            }

            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (currentSelection >= 1 && currentSelection <= 13)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
                else if (currentSelection >= 25 && currentSelection <= 38)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
                else if (currentSelection == 0)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection = 51;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
            }

            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (currentSelection >= 12 && currentSelection <= 25)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
                else if (currentSelection >= 39 && currentSelection <= 51)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
                else if (currentSelection == 0)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection = 51;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
            }

            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (currentSelection >= 13 && currentSelection <= 26)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
                else if (currentSelection >= 38 && currentSelection <= 50)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
                else if (currentSelection == 51)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection = 0;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                }
            }


            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
            StartCoroutine(ActivateCardRotation(cardNbr)); 
        }
        else
        {
            int mySlideDirection = 0;
            if (currentSelection >= 0 && currentSelection <= 12)
                mySlideDirection = (int)slideDirection.botToTop;
            else if(currentSelection >= 13 && currentSelection <= 25)
                mySlideDirection = (int)slideDirection.rightToLeft;
            else if (currentSelection >= 26 && currentSelection <= 38)
                mySlideDirection = (int)slideDirection.topToBot;
            else if (currentSelection >= 39 && currentSelection <= 51)
                mySlideDirection = (int)slideDirection.leftToRight;

            StartCoroutine(InsertTile(cardNbr, allInsertArrows[currentSelection], mySlideDirection));
        }

        yield return null;
    }

    IEnumerator ActivateCardRotation(int cardNbr)
    {
        selectionDepth = 3;
        CardButton activatedCard = null;

        // set the arrows in position
        if (cardNbr == 1)
        {
            arrows.GetComponent<RectTransform>().anchoredPosition = card1Button.GetComponent<RectTransform>().anchoredPosition + arrowsRelativePosition;
            activatedCard = cardsButtonComponent[0];
        }
        else if (cardNbr == 2)
        {
            arrows.GetComponent<RectTransform>().anchoredPosition = card2Button.GetComponent<RectTransform>().anchoredPosition + arrowsRelativePosition;
            activatedCard = cardsButtonComponent[1];
        }

        yield return null;

        while (!Input.GetKeyDown(KeyCode.B) && !Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.D))
                activatedCard.RotateTile(-1);
            else if (Input.GetKeyDown(KeyCode.A))
                activatedCard.RotateTile(1);

            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            arrows.GetComponent<RectTransform>().anchoredPosition += panelParkingPosition;
            ActivateCardSelection();
        }
        else
        {
            StartCoroutine(ScrollTileInsertionSelection(cardNbr));
        }

        yield return null;
    }

    void AssignCardsToButtons()
    {
        GameObject activePortrait = portraits[playerPlayingIdx];
        activeCards = activePortrait.GetComponentsInChildren<Card>();

        int card1_type = activeCards[0].getTileType();
        int card2_type = activeCards[1].getTileType();

        cardsButtonComponent[0].setTileType(card1_type);
        cardsButtonComponent[1].setTileType(card2_type);
    }

    void MoveCursor()
    {
        if (selectionDepth == 0)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                selectedButton = Mathf.Clamp(selectedButton + 1, 0, 3);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                selectedButton = Mathf.Clamp(selectedButton - 1, 0, 3);
            }
        }
        else if (selectionDepth == 1)
        {
            if(Input.GetKeyDown(KeyCode.D))
            {
                selectedButton = Mathf.Clamp(selectedButton + 1, 4, 6);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                selectedButton = Mathf.Clamp(selectedButton - 1, 4, 6);
            }

        }
        else if (selectionDepth == 2)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                selectedButton = Mathf.Clamp(selectedButton + 1, 7, 9);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                selectedButton = Mathf.Clamp(selectedButton - 1, 7, 9);
            }

        }
        cursorTransform.position = buttonsTransform[selectedButton].position;
    }

    public void PassTurn()
    {
        ActivatePlayer(0);
        ResetButtons();

        playerPlayingIdx ++;
        playerPlayingIdx %= 4;
        playerPlaying = playerOrder[playerPlayingIdx];

        portraitSelection.transform.position = portraits[playerPlaying - 1].transform.position;
        AssignCardsToButtons();

        // StartCoroutine(MoveCamera(players[playerPlayingIdx]));  

    }

    IEnumerator ActivateMovementPhase()
    {
        Player p = playerComponent[playerPlayingIdx];
        p.transform.parent = null;
        p.BrightPossibleTiles();
        ActivatePlayer(playerComponent[playerPlayingIdx].playerNbr);
        buttonsAnimator[0].SetBool("isActive", false);

        yield return null;

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        p.SwitchOffTiles();
        cursorIsActive = true;
        ActivatePlayer(0);
        makePlayersChild();

        yield return null;
    }

    IEnumerator ActivateRotationCursorSelection()
    {
        yield return null;

        cursorArrows.transform.localPosition = rotationArrowsActivePosition;

        while (!Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D))
        {
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            isRotating = true;
            StartCoroutine(RotateTiles(1));
        }
        else
        {
            isRotating = true;
            StartCoroutine(RotateTiles(-1));
        }

        yield return null;

        while (isRotating)
        {

        }

        rotationCursor.GetComponent<CursorMoving>().CursorDeactivate();
        cursorIsActive = true;
        //ActivatePlayer(0);  // serve?
        EndTerraform();
        ActivateBasePanel();

        yield return null;
    }

    IEnumerator ActivateRotationCursor()
    {
        rotationCursor.GetComponent<CursorMoving>().CursorActivate(playerComponent[playerPlayingIdx].coordinate);

        yield return null;

        while (!Input.GetKeyDown(KeyCode.B) && !Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ActivateTerraformPanel();
        }
        else
        {
            canTerraform = false;
            buttonsAnimator[1].SetBool("isActive", false);
            StartCoroutine(ActivateRotationCursorSelection());
        }

        yield return null;
    }

    public void makePlayersChild()
    {
        for (int i = 0; i < 4; i++)
        {
            playerComponent[i].gameObject.transform.SetParent(mapManager.PickTileObject(playerComponent[i].coordinate).transform);
            //playerComponent[i].gameObject.transform.localPosition = new Vector3 (0, 0, -1);
        }
    }

    // Use this for initialization
    void Awake()
    {
        selectionDepth = 0; // corresponds to the first panel. 1 is the terraform panel. 2 is the card selection
        int numberOfButtons = 10, nbrOfPanels = 3;
        playerPlayingIdx = -1;
        selectedButton = 0;
        int nbrOfCards = 2;
        playerOrder = new int[4] { 1, 2, 3, 4 };
        buttonsPosition = new Vector3[numberOfButtons];
        buttonsTransform = new RectTransform[numberOfButtons];
        buttonsAnimator = new Animator[numberOfButtons];
        panelsTransform = new RectTransform[nbrOfPanels];
        cardsButtonComponent = new CardButton[nbrOfCards];
        cursorTransform = cursor.GetComponent<RectTransform>();
        cursorIsActive = true;

        panelParkingPosition = new Vector2(0, -100);
        panelActivePosition = new Vector2(0, 50);
        arrowsRelativePosition = new Vector2(25, 0);

        rotationArrowsParkingPosition = new Vector3(-500f, 0, 0);
        rotationArrowsActivePosition = new Vector3(5, -1, 0f);

        // Assigns the position of ll the ui elements o the corresponding arrays
        AssignButtonsAnimators();
        AssignButtonsPosition();
        AssignPanelsPosition();
        AssignCardButtonComponent();

        //makePlayersChild();

    }


    // Update is called once per frame
    void Update()
    {
        if (cursorIsActive)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
                MoveCursor();

            if (selectionDepth<=2 && Input.GetKeyDown(KeyCode.Space))
            {
                if (canMove && cursorTransform.position == buttonsTransform[0].position) // Walk
                {
                    cursorIsActive = false;
                    canMove = false;
                    StartCoroutine(ActivateMovementPhase());

                }
                else if (canTerraform && cursorTransform.position == buttonsTransform[1].position) // Terraform
                {
                    ActivateTerraformPanel();
                }
                else if (cursorTransform.position == buttonsTransform[3].position) // Pass
                {
                    PassTurn();
                }
                else if (cursorTransform.position == buttonsTransform[4].position) // Rotate
                {
                    cursorIsActive = false;
                    StartCoroutine(ActivateRotationCursor());
                }
                else if (cursorTransform.position == buttonsTransform[5].position) // Slide
                {
                    ActivateCardSelection();
                }
                else if (cursorTransform.position == buttonsTransform[6].position) // Back to starting Panel
                {
                    ActivateBasePanel();
                }
                else if (cursorTransform.position == buttonsTransform[7].position) // Card1
                {
                    StartCoroutine(ActivateCardRotation(1));
                }
                else if (cursorTransform.position == buttonsTransform[8].position) // Card2
                {
                    StartCoroutine(ActivateCardRotation(2));
                }
                else if (cursorTransform.position == buttonsTransform[9].position) // Back to Terraform Panel
                {
                    ActivateTerraformPanel();
                }
            }
        }
    }
}
