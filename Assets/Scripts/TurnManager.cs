using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{

    public GameObject xButton, aButton, bButton, rotationCursor;
    public GameObject[] portraits;
    public GameObject[] panels;
    public GameObject[] playerWinsText;
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
    private Player[] playerComponent;
    private Player activePlayer;
    private Animator[] buttonsAnimator;
    private Vector2 panelParkingPosition, panelActivePosition, arrowsRelativePosition;
    private Vector3 rotationArrowsParkingPosition, rotationArrowsActivePosition;
    private List<Trap> trapsToActivate = new List<Trap>();
    private bool trapHasTriggered = false, diamondOnTable = true, attackHasHappened = false, resolvingCombat = true;
    private bool canBeActivated = true;
    private bool canBeRotated = true;

    enum myButtons
    {
        Walk, Terraform, Crystal, Pass
    };

    enum slideDirection
    {
        leftToRight, botToTop, rightToLeft, topToBot
    };

    enum panelSelection
    {
        basePanel, walkPanel, terraformPanel, slidePanel, rotatePanel
    };

    // Initialization Methods - variables assignments of the automatically generated objects

    public void AssignButtonsAnimators()
    {
        buttonsAnimator[0] = xButton.GetComponent<Animator>();
        buttonsAnimator[1] = aButton.GetComponent<Animator>();
        buttonsAnimator[2] = bButton.GetComponent<Animator>();
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
            players[playerOrder[i] - 1] = tmpPlayers[i];
            playerComponent[playerOrder[i] - 1] = tmpPlayers[i].GetComponent<Player>();
        }
    }

    public void setInsertArrows(GameObject[] inputArrows)
    {
        allInsertArrows = inputArrows;
    }

    // Turn Management

    public void PassTurn()
    {
        ActivatePlayer(0);

        playerPlayingIdx++;
        playerPlayingIdx %= 4;
        playerPlaying = playerOrder[playerPlayingIdx];
        activePlayer = playerComponent[playerPlayingIdx];

        mapManager.updateTilesConnection(playerPlaying);
        activePlayer.CheckDiamondStatusTimer();

        ResetButtons();
        ActivateTraps();

        selectedButton = 0;

        // StartCoroutine(MoveCamera(players[playerPlayingIdx]));  

    }

    public void ActivatePlayer(int nbr) // if 0 no player is active - The method acts on the flag inside player allowing movement
    {
        for (int i = 0; i < 4; i++)
        {
            playerComponent[i].isPlayerTurn = nbr;
        }
    }

    void ResetButtons() // TODO CAMBIARE CON MODIFICHE add reset of the animation
    {
        canMove = true;
        buttonsAnimator[0].SetBool("isActive", true);
        canTerraform = true;
        buttonsAnimator[1].SetBool("isActive", true);

        if (activePlayer.GetCanActivateStasis())
        {
            canUseCrystal = true;
            buttonsAnimator[2].SetBool("isActive", true);
        }
        else
        {
            canUseCrystal = false;
            buttonsAnimator[2].SetBool("isActive", false);
        }

    }

    void UpdatePlayersPosition()
    {
        foreach (Player player in playerComponent)
        {
            player.UpdatePlayerPosition();
            if (player.GetHasDiamond())
                UpdateDiamondPosition(player);
        }
    }

    public void makePlayersChild()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject playerTile = mapManager.PickTileObject(playerComponent[i].coordinate);
            playerComponent[i].gameObject.transform.SetParent(playerTile.transform);
            playerTile.GetComponent<Tile>().SetPlayerChild(playerComponent[i]);

            //playerComponent[i].gameObject.transform.localPosition = new Vector3 (0, 0, -1);
        }
    }

    public int GetActivePlayer()
    {
        return playerPlaying;
    }

    public void SetAttackHasHappened(bool status)
    {
        attackHasHappened = status;
    }

    public void SetResolvingCombat(bool status)
    {
        resolvingCombat = status;
    }

    IEnumerator BackToMenu()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("_Scenes/menu");
        yield return null;
    }
        
    public void EndGame(Player player)
    {
        playerWinsText[player.playerNbr - 1].GetComponent<RectTransform>().position = new Vector2(0, 0);
        StartCoroutine(BackToMenu());
    }

    // // UI

    // Panel Selection

    void ActivateBasePanel()  // MODIFICARE CON PASSAGGIO A BOTTONI
    {
        selectionDepth = 0;
        selectedButton = 0;
        cursorIsActive = true;
    }

    void ActivateTerraformPanel()
    {
        selectionDepth = 1;
        selectedButton = 4;

        cursorIsActive = true;
    }  // MODIFICARE CON PASSAGGIO A BOTTONI

    void ActivateSlidePanel()
    {
        selectionDepth = 2;
        selectedButton = 7;

    }

    void ActivateRotationPanel()
    {
        selectionDepth = 2;
        selectedButton = 7;

    }


    void EndTerraform()
    {
        mapManager.updateTilesConnection(playerPlaying);

        UpdatePlayersPosition();
        canTerraform = false;
        mapManager.UpdateTilesZOrder();
    }

    // Traps

    public void SetTrapHasTriggered(bool status)
    {
        trapHasTriggered = status;
    }

    public void AddToActivateTrapList(Trap trap)
    {
        trapsToActivate.Add(trap);
    }

    public void ActivateTraps()
    {
        for (int i = 0; i < trapsToActivate.Count; i++)
        {
            Trap thisTrap = trapsToActivate[i];
            bool isOverlappedToPlayer = false;
            foreach (Player player in playerComponent)
            {
                isOverlappedToPlayer = isOverlappedToPlayer || (player.GetCoordinates().isEqual(thisTrap.GetCoordiantes()));
            }

            if (!isOverlappedToPlayer)
            {
                thisTrap.Activate();
                trapsToActivate.RemoveAt(i);
            }
        }
    }

    // Diamond

    private void UpdateDiamondPosition(Player player)
    {
        mapManager.diamondCoords = player.coordinate;
    }

    private void UpdateDiamondPosition(Tile tile)
    {
        mapManager.diamondCoords = tile.GetCoordinatesCopy();
    }

    private bool ChecksForDiamond(Player player)
    {
        if (player.coordinate.isEqual(mapManager.GetDiamondCoords()) && diamondOnTable)
            return true;
        else
            return false;

    }

    public void CollectDiamond(Player player)
    {
        diamondOnTable = false;
        player.SetHasDiamond(true);
        mapManager.myDiamondInstance.transform.parent = player.transform;
        Vector3 collectedPosition = new Vector3(0, 0, 100);
        
        mapManager.myDiamondInstance.transform.localPosition = collectedPosition;
        mapManager.diamondCoords = player.coordinate;
    }

    public void DropDiamond(Player player)
    {
        player.hasDiamond = false;
        mapManager.myDiamondInstance.transform.parent = null;
        mapManager.myDiamondInstance.transform.position = new Vector3(mapManager.myDiamondInstance.transform.position.x,
                                                                      mapManager.myDiamondInstance.transform.position.y,
                                                                      -10);
    }

    private void ConnectToTile(GameObject tile)
    {
        mapManager.myDiamondInstance.transform.SetParent(tile.transform);
        mapManager.diamondCoords = tile.GetComponent<Tile>().getCoordinates();
    }

    public void ResetDiamondToStartingPosition()
    {
        mapManager.myDiamondInstance.transform.parent = null;
        Coordinate centralCoords = new Coordinate(mapManager.columns / 2, mapManager.rows / 2);
        GameObject centralTile = mapManager.PickTileObject(centralCoords);
        Vector3 position = centralTile.transform.position;
        position.z = -10;
        mapManager.myDiamondInstance.transform.position = position;
        mapManager.diamondCoords = centralCoords;
    }

    public void ActivateDiamondStasis()
    {
        mapManager.myDiamondInstance.transform.GetComponentInParent<Player>().ActivateStasis();
        canUseCrystal = false;
        buttonsAnimator[2].SetBool("isActive", false);
    }

    // Player Movement

    IEnumerator ActivateMovementPhase()
    {
        Player p = playerComponent[playerPlayingIdx];
        CameraStartFollowingPlayer(p);

        Coordinate movementStartingPosition = p.GetCoordinatesCopy();
        mapManager.updateTilesConnection(playerPlaying);
        p.BrightPossibleTiles();

        //p.UnchildFromTile();
        ActivatePlayer(playerComponent[playerPlayingIdx].playerNbr);

        yield return null;

        while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.B) &&  !trapHasTriggered && !attackHasHappened)
        {
            yield return null;
        }


        if (Input.GetKeyDown(KeyCode.Space) || trapHasTriggered || attackHasHappened)
        {
            canMove = false;
            buttonsAnimator[0].SetBool("isActive", false);

            if (trapHasTriggered)
                SetTrapHasTriggered(false);

            if (ChecksForDiamond(p))
            {
                CollectDiamond(p);
            }

            if (p.CheckForVictory())
            {
                EndGame(p);
            }

            if (p.GetHasDiamond())
            {
                UpdateDiamondPosition(p);
            }
        }
        else
        {
            p.UnchildFromTile();
            p.TeleportAtCoordinates(movementStartingPosition);
            ActivateBasePanel();
        }

        p.SwitchOffTiles();
        cursorIsActive = true;
        ActivatePlayer(0);
        makePlayersChild();

        CameraStopFollowingPlayer();

        yield return null;
    }

    // // Terraforming

    // Tile Slide
    IEnumerator InsertTile(int cardNbr, GameObject arrow, int slideDirection)
    {
        isSliding = true;

        Player fallingPlayer = null;
        Player frontPlayer = null, rearPlayer = null;
        bool repositionPlayer = false;

        Coordinate[] lineCoordinates = arrow.GetComponent<InsertArrow>().getPointedCoords();
        lineCoordinates = mapManager.KeepMovableTiles(lineCoordinates);

        attackHasHappened = false;
        resolvingCombat = false;
        int[] playerIdxInCoords = GetPlayersInCoordinatesIDX(lineCoordinates); // checks if players are found in the group of slided tiles
        if (playerIdxInCoords.Length > 1) // more than one player is on the slided tiles
        {
            Array.Sort(playerIdxInCoords);
            for (int i = 1; i < playerIdxInCoords.Length; i++) 
            {
                if (playerIdxInCoords[i] - playerIdxInCoords[i - 1] == 1) // players are positioned one tile away from each other in the slide direction
                {
                    // checks whether or not the player in the front has the stasis active
                    frontPlayer = GetPlayerAtCoordinates(lineCoordinates[playerIdxInCoords[i]]);
                    //frontPlayer = mapManager.PickTileComponent(lineCoordinates[playerIdxInCoords[i]]).GetPlayerChild(); // non funziona dato che è in stasi
                    if (frontPlayer.GetStasisStatus()) 
                    {
                        rearPlayer = mapManager.PickTileComponent(lineCoordinates[playerIdxInCoords[i-1]]).GetPlayerChild();
                        attackHasHappened = true;
                        resolvingCombat = true;
                        StartCoroutine(rearPlayer.AttackPlayerOnTileOnSlide(frontPlayer));
                    }
                }
            }
        }

        while (resolvingCombat) { yield return null; }

        Tile lastTile = mapManager.PickTileComponent(lineCoordinates[lineCoordinates.Length - 1]);
        int newCardType = lastTile.type;
        bool trapStatus = lastTile.GetIsTrapped();
        if (lastTile.GetPlayerChildNbr() != -1)
        {
            int playerIdx = GeneralMethods.FindElementIdx(playerOrder, lastTile.GetPlayerChildNbr());
            fallingPlayer = playerComponent[playerIdx];
            fallingPlayer.transform.parent = null;
            DropDiamond(fallingPlayer);
            diamondOnTable = true;
            fallingPlayer.TeleportOffScreen();
            repositionPlayer = true;
        }

        StartCoroutine(mapManager.SlideLine(lineCoordinates, slideDirection));

        int tileType = 0;

        while (isSliding)
        {
            yield return null;
        }

        if (fallingPlayer != null)
        {
            ConnectToTile(mapManager.PickTileObject(mapManager.diamondCoords));
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

        if (activatedCard.GetTrappedStatus())
            mapManager.InstantiateTileLive(tileType, lineCoordinates[0], true);
        else
            mapManager.InstantiateTileLive(tileType, lineCoordinates[0]);  // default is false

        if (repositionPlayer)
        {
            fallingPlayer.TeleportAtCoordinates(lineCoordinates[0]);
        }

        GameObject activePortrait = portraits[playerPlayingIdx];
        activeCards = activePortrait.GetComponentsInChildren<Card>();

        if (cardNbr == 1)
            activeCards[0].AssignType(newCardType, trapStatus);
        else
            activeCards[1].AssignType(newCardType, trapStatus);

        if (attackHasHappened)
        {
            CollectDiamond(rearPlayer);
            attackHasHappened = false;
        }

        arrow.GetComponent<Animator>().SetBool("isActive", false);
        arrows.transform.localPosition = new Vector3(0f, -100f, 0f);
        EndTerraform();
        ActivateBasePanel();

        yield return null;
    }

    IEnumerator ScrollTileInsertionSelection(int cardNbr)
    {
        int currentSelection = 39; // Starts in the Top Left Side
        allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);

        yield return null;

        bool firstClick = false;
        float firstClickWaitingTime = 0.5f;

        while (!Input.GetKeyDown(KeyCode.B) && !Input.GetKeyDown(KeyCode.Space) && !Input.GetButtonDown("Fire1joy") && !Input.GetButtonDown("Fire2joy"))
        {
            if ((Input.GetKey(KeyCode.D) || Input.GetAxis("HorizontalJoy") == 1))
            {
                if (currentSelection >= 26 && currentSelection <= 39)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                else if (currentSelection >= 0 && currentSelection <= 12)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                else if (currentSelection == 51)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection = 0;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                if (!firstClick)
                {
                    yield return new WaitForSeconds(firstClickWaitingTime);
                    firstClick = true;
                }
            }

            else if ((Input.GetKey(KeyCode.A) || Input.GetAxis("HorizontalJoy") == -1))
            {
                //canBeActivated = false;
                if (currentSelection >= 1 && currentSelection <= 13)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                else if (currentSelection >= 25 && currentSelection <= 38)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                else if (currentSelection == 0)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection = 51;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                if (!firstClick)
                {
                    Debug.Log("passa");
                    yield return new WaitForSeconds(firstClickWaitingTime);
                    firstClick = true;
                }
            }

            else if ((Input.GetKey(KeyCode.W) || Input.GetAxis("VerticalJoy") == -1))
            {
                //canBeActivated = false;
                if (currentSelection >= 12 && currentSelection <= 25)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                else if (currentSelection >= 39 && currentSelection <= 51)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                else if (currentSelection == 0)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection = 51;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                if (!firstClick)
                {
                    yield return new WaitForSeconds(firstClickWaitingTime);
                    firstClick = true;
                }
            }

            else if ((Input.GetKey(KeyCode.S) || Input.GetAxis("VerticalJoy") == 1))
            {
                //canBeActivated = false;
                if (currentSelection >= 13 && currentSelection <= 26)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                else if (currentSelection >= 38 && currentSelection <= 50)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                else if (currentSelection == 51)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    yield return null;
                    currentSelection = 0;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    yield return null;
                }
                if (!firstClick)
                {
                    yield return new WaitForSeconds(firstClickWaitingTime);
                    firstClick = true;
                }

            }

            else if ((Mathf.Abs(Input.GetAxis("VerticalJoy")) != 1 && Mathf.Abs(Input.GetAxis("HorizontalJoy")) != 1))
            {
                firstClick = false;
            }



            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2joy"))
        {
            allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
            StartCoroutine(ActivateCardRotation(cardNbr));
        }
        else
        {
            int mySlideDirection = 0;
            if (currentSelection >= 0 && currentSelection <= 12)
                mySlideDirection = (int)slideDirection.botToTop;
            else if (currentSelection >= 13 && currentSelection <= 25)
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

        while (!Input.GetKeyDown(KeyCode.B) && !Input.GetKeyDown(KeyCode.Space) && !Input.GetButtonDown("Fire1joy") && !Input.GetButtonDown("Fire2joy"))
        {
            if ((Input.GetKeyDown(KeyCode.D) || Input.GetAxis("RotationJoy") > 0) && canBeRotated)
            {
                canBeRotated = false;
                activatedCard.RotateTile(-1);
            }
            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetAxis("RotationJoy") < 0) && canBeRotated)
            {
                canBeRotated = false;
                activatedCard.RotateTile(1);
            }

            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2joy"))
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

    // Tile Rotation

    IEnumerator ActivateRotationCursorSelection()
    {
        yield return null;

        cursorArrows.transform.localPosition = rotationArrowsActivePosition;

        while (!Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D) && Input.GetAxis("RotationJoy") == 0)
        {
            yield return null;
        }

        Coordinate[] selectedCoords = rotationCursor.GetComponent<CursorMoving>().getSelectedCoords();

        if (Input.GetKeyDown(KeyCode.A) || Input.GetAxis("RotationJoy") < 0)
        {
            isRotating = true;
            StartCoroutine(mapManager.RotateTiles(selectedCoords, 1));
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetAxis("RotationJoy") > 0))
        {
            isRotating = true;
            StartCoroutine(mapManager.RotateTiles(selectedCoords, -1));
        }

        rotationCursor.GetComponent<CursorMoving>().SetAtPosition(rotationArrowsParkingPosition);
        cursorArrows.transform.localPosition = rotationArrowsParkingPosition;

        while (isRotating)
        {
            yield return null;
        }

        UpdatePlayersPosition();
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

        while (!Input.GetKeyDown(KeyCode.B) && !Input.GetKeyDown(KeyCode.Space) && !Input.GetButtonDown("Fire1joy") && !Input.GetButtonDown("Fire2joy"))
        {
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Fire2joy"))
        {
            rotationCursor.GetComponent<CursorMoving>().CursorDeactivate();
            rotationCursor.GetComponent<CursorMoving>().SetAtPosition(rotationArrowsParkingPosition);
            ActivateTerraformPanel();
        }
        else
        {
            canTerraform = false;
            buttonsAnimator[1].SetBool("isActive", false);
            rotationCursor.GetComponent<CursorMoving>().CursorDeactivate();
            StartCoroutine(ActivateRotationCursorSelection());
        }

        yield return null;
    }

    IEnumerator reactivateCursor()
    {
        canBeActivated = true;
        yield return null;
    }

    IEnumerator reactivateRotation()
    {
        canBeRotated = true;
        yield return null;
    }

    // Camera

    private void CameraStartFollowingPlayer(Player player)
    {
        camera.GetComponent<CameraMovement>().StartFollowingPlayer(player);
    }

    private void CameraStopFollowingPlayer()
    {
        camera.GetComponent<CameraMovement>().StopFollowingPlayer();
    }

    public void CameraSetRowsAndColumns()
    {
        camera.GetComponent<CameraMovement>().SetRowsAndColumns(mapManager);
    }

    // General Methods

    private Player GetPlayerAtCoordinates(Coordinate coord)
    {
        Player myPlayer = null;
        foreach (Player player in playerComponent)
        {
            if (coord.isEqual(player.coordinate))
            {
                myPlayer = player;
                break;
            }
        }
        return myPlayer;

    }
    
    private Player[] GetPlayersInCoordinates(Coordinate[] coords)
    {
        List<Player> playersInCoords = new List<Player>();
        foreach (var player in playerComponent)
        {
            if (player.FindInCoords(coords) != -1)
                playersInCoords.Add(player);
        }

        return playersInCoords.ToArray();
    }

    private int[] GetPlayersInCoordinatesIDX(Coordinate[] coords)
    {
        List<int> playersIdxInCoords = new List<int>();
        for (int i = 0; i < playerComponent.Length; i++)
        {
            int idx = playerComponent[i].FindInCoords(coords);
            if (idx != -1)
                playersIdxInCoords.Add(idx);
        }

        return playersIdxInCoords.ToArray();
    }

    // Unity Specific methods

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

        rotationArrowsParkingPosition = new Vector3(-1000f, 0, 0);
        rotationArrowsActivePosition = new Vector3(5, -1, 0f);

        // Assigns the position of ll the ui elements o the corresponding arrays
        AssignButtonsAnimators();
        AssignButtonsPosition();
        AssignPanelsPosition();
        AssignCardButtonComponent();

        //makePlayersChild();

    }


    void Update()
    {
        //// Check if needed
        //if ((Mathf.Abs(Input.GetAxis("HorizontalJoy")) != 1))
        //{
        //    StartCoroutine(reactivateCursor());
        //}

        //// Check if needed
        //if ((Input.GetAxis("RotationJoy")) == 0)
        //{
        //    StartCoroutine(reactivateRotation());
        //}

        //// Check if needed
        //if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetAxis("HorizontalJoy") != 0)
        //{
        //    canBeActivated = false;
        //}

        if (selectionDepth == (int)panelSelection.basePanel)
        {
            if (Input.GetKeyDown(KeyCode.Z)) // Walk
            {
                cursorIsActive = false;
                StartCoroutine(ActivateMovementPhase());
            }
            else if (Input.GetKeyDown(KeyCode.S)) // Terraform
            {
                ActivateTerraformPanel();
            }
            else if (Input.GetKeyDown(KeyCode.D)) // Nothing
            {
                
            }
        }
        else if (selectionDepth == (int)panelSelection.walkPanel)
        {
            if (Input.GetKeyDown(KeyCode.Z)) // Confirm Walk
            {

            }
            else if (Input.GetKeyDown(KeyCode.S)) // Nothing
            {
                
            }
            else if (Input.GetKeyDown(KeyCode.D)) // Back to base panel
            {

            }
        }
        else if (selectionDepth == (int)panelSelection.terraformPanel)
        {
            if (Input.GetKeyDown(KeyCode.Z)) // Slide
            {

            }
            else if (Input.GetKeyDown(KeyCode.S)) // Rotation
            {

            }
            else if (Input.GetKeyDown(KeyCode.D)) // Back to base panel
            {

            }
        }
        else if (selectionDepth == (int)panelSelection.slidePanel)
        {
            if (Input.GetKeyDown(KeyCode.Z)) // Confirm
            {

            }
            else if (Input.GetKeyDown(KeyCode.S)) // Nothing
            {

            }
            else if (Input.GetKeyDown(KeyCode.D)) // Back to terraform panel
            {

            }
        }
        else if (selectionDepth == (int)panelSelection.rotatePanel)
        {
            if (Input.GetKeyDown(KeyCode.Z)) // rotation counterclockwise
            {

            }
            else if (Input.GetKeyDown(KeyCode.S)) // rotation clockwise
            {

            }
            else if (Input.GetKeyDown(KeyCode.D)) // Back to terraform panel
            {

            }
        }
    }

    //deprecated
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

}
