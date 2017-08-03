using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{

    public GameObject xButton, aButton, bButton, diamondButton, rotationCursor;
    public GameObject[] portraits;
    public GameObject[] playerWinsText;
    public Camera myCamera;
    public MapManager mapManager;
    public bool isSliding = false, isRotating = false;

    private int playerPlaying, playerPlayingIdx, turnNbr, selectedButton, selectionDepth;
    private int[] playerOrder;
    private bool canMove, canTerraform, canUseCrystal, cursorIsActive;
    private GameObject[] players, tmpPlayers, allInsertArrows;
    private GameObject activePortrait;
    private Card activeCard;
    private Player[] playerComponent;
    private Player activePlayer;
    private Animator[] buttonsAnimator;
    private Vector2 panelParkingPosition, panelActivePosition, arrowsRelativePosition;
    private Vector3 parkingPosition;
    private List<Trap> trapsToActivate = new List<Trap>();
    private bool trapHasTriggered = false, diamondOnTable = true, attackHasHappened = false, resolvingCombat = true;
    private bool inAction;
    private bool canBeActivated = true;
    private bool canBeRotated = true;
    private CameraMovement myCameraMovement;
    private bool isAcceptingInputs=true;

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

    public Player[] GetAllPayers()
    {
        return playerComponent;
    }

    public void AssignButtonsAnimators()
    {
        buttonsAnimator[0] = xButton.GetComponent<Animator>();
        buttonsAnimator[1] = aButton.GetComponent<Animator>();
        buttonsAnimator[2] = bButton.GetComponent<Animator>();
        buttonsAnimator[3] = diamondButton.GetComponent<Animator>();
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

        StartCoroutine(MoveToNextPlayer());
        //myCameraMovement.MoveToPosition(activePlayer.GetComponentInParent<Transform>().position);

        //while (movingCamera) { }

        mapManager.updateTilesConnection(playerPlaying);
        activePlayer.CheckDiamondStatusTimer();

        if (activePortrait != null)
            activePortrait.GetComponent<Animator>().SetBool("isActive", false);
        activePortrait = portraits[playerPlayingIdx];
        activePortrait.GetComponent<Animator>().SetBool("isActive", true);
        activeCard = activePortrait.GetComponentInChildren<Card>();

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

    void ResetButtons()
    {
        canMove = true;
        buttonsAnimator[0].SetBool("canWalk", true);
        canTerraform = true;
        buttonsAnimator[1].SetBool("canTerraform", true);

        StartCoroutine(ActivatePanel((int)panelSelection.basePanel));

        if (activePlayer.GetCanActivateStasis())
        {
            canUseCrystal = true;
            StartCoroutine(SetDiamondAnimation(0));
            buttonsAnimator[3].SetBool("isActive", false);
            //buttonsAnimator[3].SetInteger("ActiveStatus", 3);
        }
        else
        {
            canUseCrystal = false;
            buttonsAnimator[3].SetBool("isActive", false);
            StartCoroutine(SetDiamondAnimation(activePlayer.GetTurnsBeforeStasisCounter()));
            //buttonsAnimator[3].SetBool("isActive", false);
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

    public bool ChecksForPassTurnCondition()
    {
        if (!canMove && !canTerraform && !canUseCrystal)
            return true;
        else
            return false;
    }

    IEnumerator MoveToNextPlayer()
    {
        myCameraMovement.MoveToCenter();
        yield return new WaitForSeconds(2f);
        myCameraMovement.MoveToPosition(activePlayer.GetComponentInParent<Transform>().position);
        yield return null;
    }

    // // UI

    // Panel Selection

    //TODO ADD INACTIVE STATUS
    IEnumerator ActivatePanel(int panelType)
    {
        isAcceptingInputs = false;
        foreach (var btn in buttonsAnimator)
        {
            //btn.SetBool("hasChanged", true);
            btn.SetInteger("ButtonStatus", 0);
        }

        yield return null;

        switch (panelType)
        {
            case (int)panelSelection.basePanel:
                ActivateBasePanel();
                break;
            case (int)panelSelection.walkPanel:
                ActivateWalkPanel();
                break;
            case (int)panelSelection.terraformPanel:
                ActivateTerraformPanel();
                break;
            case (int)panelSelection.slidePanel:
                ActivateSlidePanel();
                break;
            case (int)panelSelection.rotatePanel:
                ActivateRotatePanel();
                break;
            default:
                break;
        }

        yield return null;
        isAcceptingInputs = true;
    }

    void ActivateBasePanel()  
    {
        selectionDepth = (int)panelSelection.basePanel;
        foreach (var btn in buttonsAnimator)
        {
            btn.SetInteger("ButtonStatus", 1);
        }
    }

    void ActivateWalkPanel()
    {
        selectionDepth = (int)panelSelection.walkPanel;
        foreach (var btn in buttonsAnimator)
        {
            btn.SetInteger("ButtonStatus", 2);
        }
    }

    void ActivateTerraformPanel()
    {
        selectionDepth = (int)panelSelection.terraformPanel;
        foreach (var btn in buttonsAnimator)
        {
            btn.SetInteger("ButtonStatus", 3);
        }
    }  

    void ActivateSlidePanel()
    {
        selectionDepth = (int)panelSelection.slidePanel;
        foreach (var btn in buttonsAnimator)
        {
            btn.SetInteger("ButtonStatus", 4);
        }
    }

    void ActivateRotatePanel()
    {
        selectionDepth = (int)panelSelection.rotatePanel;
        foreach (var btn in buttonsAnimator)
        {
            btn.SetInteger("ButtonStatus", 5);
        }
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

        StartCoroutine(SetDiamondAnimation(0));
    }

    public void DropDiamond(Player player)
    {
        player.hasDiamond = false;
        mapManager.myDiamondInstance.transform.parent = null;
        mapManager.myDiamondInstance.transform.position = new Vector3(mapManager.myDiamondInstance.transform.position.x,
                                                                      mapManager.myDiamondInstance.transform.position.y,
                                                                      -10);
        player.ResetTurnsBeforeStasis();
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
        buttonsAnimator[3].SetBool("isActive", true);
    }

    public IEnumerator SetDiamondAnimation(int anim)
    {
        switch (anim)
        {
            case 0:
                buttonsAnimator[3].SetInteger("ActiveStatus", 3);
                break;
            case 1:
                buttonsAnimator[3].SetInteger("ActiveStatus", 2);
                break;
            case 2:
                buttonsAnimator[3].SetInteger("ActiveStatus", 1);
                break;
            case 3:
                buttonsAnimator[3].SetInteger("ActiveStatus", 0);
                break;
            case 4:
                buttonsAnimator[3].SetInteger("ActiveStatus", 0);
                break;
            case 5:
                buttonsAnimator[3].SetBool("isActive", true);
                break;

            default:
                break;
        }

        yield return null;
        buttonsAnimator[3].SetInteger("ActiveStatus", -1);
        yield return null;
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

        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetButtonDown("Fire3joy") && !Input.GetKeyDown(KeyCode.C) && !Input.GetButtonDown("Fire2joy") &&  !trapHasTriggered && !attackHasHappened)
        {
            yield return null;
        }


        if (Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire3joy") || trapHasTriggered || attackHasHappened)
        {
            canMove = false;
            if (trapHasTriggered)
                SetTrapHasTriggered(false);

            if(attackHasHappened)
                attackHasHappened = false;

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

            buttonsAnimator[0].SetBool("canWalk", false);
        }
        else
        {
            p.UnchildFromTile();
            p.TeleportAtCoordinates(movementStartingPosition);
            inAction = false;
        }

        p.SwitchOffTiles();
        cursorIsActive = true;
        ActivatePlayer(0);
        makePlayersChild();
        StartCoroutine(ActivatePanel((int)panelSelection.basePanel));
        
        CameraStopFollowingPlayer();
        inAction = false;

        if (ChecksForPassTurnCondition())
            PassTurn();

        yield return null;
    }

    // // Terraforming

    IEnumerator EndTerraform()
    {
        mapManager.updateTilesConnection(playerPlaying);

        UpdatePlayersPosition();
        canTerraform = false;
        buttonsAnimator[1].SetBool("canTerraform", false);
        StartCoroutine(ActivatePanel((int)panelSelection.basePanel));
        mapManager.UpdateTilesZOrder();
        inAction = false;

        if (ChecksForPassTurnCondition())
            PassTurn();

        yield return null;
    }

    // Tile Slide

    IEnumerator ZoomToCenter()
    {
        myCameraMovement.MoveToCenter();
        yield return null;
    }

    IEnumerator InsertTile(GameObject arrow, int slideDirection)
    {
        isSliding = true;

        Player fallingPlayer = null;
        Player frontPlayer = null, rearPlayer = null;
        bool repositionPlayer = false;

        Coordinate[] lineCoordinates = arrow.GetComponent<InsertArrow>().getPointedCoords();
        lineCoordinates = mapManager.KeepMovableTiles(lineCoordinates);

        // Possible PvP
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

        // Possible Playeer on destroyed tile
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

        yield return StartCoroutine(mapManager.SlideLine(lineCoordinates, slideDirection));
        isSliding = false;

        int tileType = 0;

        //while (isSliding)
        //{
        //    yield return null;
        //}

        if (fallingPlayer != null)
        {
            ConnectToTile(mapManager.PickTileObject(mapManager.diamondCoords));
        }

        tileType = activeCard.GetTileType();

        if (activeCard.GetTrappedStatus())
            mapManager.InstantiateTileLive(tileType, lineCoordinates[0], true);  // places a trap on top of the tile
        else
            mapManager.InstantiateTileLive(tileType, lineCoordinates[0]);  // default is false

        if (repositionPlayer)
        {
            fallingPlayer.TeleportAtCoordinates(lineCoordinates[0]);
        }

        activeCard.AssignType(newCardType, trapStatus);

        if (attackHasHappened)
        {
            CollectDiamond(rearPlayer);
            attackHasHappened = false;
        }

        arrow.GetComponent<Animator>().SetBool("isActive", false);
        mapManager.SetInsertArrowsVisible(false);
        StartCoroutine(EndTerraform());

        yield return null;
    }

    IEnumerator ScrollTileInsertionSelection()
    {
        StartCoroutine(ZoomToCenter());
        mapManager.SetInsertArrowsVisible(true);
        int currentSelection = 39; // Starts in the Top Left Side
        allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);

        yield return null;

        bool firstClick = false;
        float firstClickWaitingTime = 0.5f;

        while (!Input.GetKeyDown(KeyCode.C) && !Input.GetKeyDown(KeyCode.Z) && !Input.GetButtonDown("Fire3joy") && !Input.GetButtonDown("Fire2joy"))
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

            else if ((Input.GetKey(KeyCode.W) || Input.GetAxis("VerticalJoy") == 1))
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

            else if ((Input.GetKey(KeyCode.S) || Input.GetAxis("VerticalJoy") == -1))
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

        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2joy"))
        {
            inAction = false;
            mapManager.SetInsertArrowsVisible(false);
            allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
            StartCoroutine(ActivatePanel((int)panelSelection.terraformPanel));
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

            StartCoroutine(InsertTile(allInsertArrows[currentSelection], mySlideDirection));
        }

        yield return null;
    }


    // Tile Rotation

    IEnumerator ActivateRotation(int direction)
    {
        Coordinate[] selectedCoords = rotationCursor.GetComponent<CursorMoving>().getSelectedCoords();
        rotationCursor.GetComponent<CursorMoving>().SetAtPosition(parkingPosition);
        yield return StartCoroutine(mapManager.RotateTiles(selectedCoords, direction));

        UpdatePlayersPosition();
        cursorIsActive = true;
        yield return StartCoroutine(EndTerraform());

    }

    IEnumerator ActivateRotationCursor()
    {

        rotationCursor.GetComponent<CursorMoving>().CursorActivate(playerComponent[playerPlayingIdx].coordinate);

        yield return null;

        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetKeyDown(KeyCode.X) && !Input.GetKeyDown(KeyCode.C) && !Input.GetButtonDown("Fire1joy") && !Input.GetButtonDown("Fire2joy") && !Input.GetButtonDown("Fire3joy"))
        {
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2joy"))
        {
            inAction = false;
            rotationCursor.GetComponent<CursorMoving>().CursorDeactivate();
            rotationCursor.GetComponent<CursorMoving>().SetAtPosition(parkingPosition);
            StartCoroutine(ActivatePanel((int)panelSelection.terraformPanel));
        }
        else if (Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire3joy"))
        {
            canTerraform = false;
            rotationCursor.GetComponent<CursorMoving>().CursorDeactivate();
            yield return StartCoroutine(ActivateRotation(1));
        }
        else if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Fire1joy"))
        {
            canTerraform = false;
            rotationCursor.GetComponent<CursorMoving>().CursorDeactivate();
            yield return StartCoroutine(ActivateRotation(-1));
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
        myCameraMovement.StartFollowingPlayer(player);
    }

    private void CameraStopFollowingPlayer()
    {
        myCameraMovement.StopFollowingPlayer();
    }

    public void CameraSetRowsAndColumns()
    {
        myCameraMovement.SetRowsAndColumns(mapManager);
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
        int numberOfButtons = 4;
        playerPlayingIdx = -1;
        selectedButton = 0;
        playerOrder = new int[4] { 1, 2, 3, 4 };
        buttonsAnimator = new Animator[numberOfButtons];
        inAction = false;
        myCameraMovement = myCamera.GetComponent<CameraMovement>();

        panelParkingPosition = new Vector2(0, -100);
        panelActivePosition = new Vector2(0, 50);
        arrowsRelativePosition = new Vector2(25, 0);

        parkingPosition = new Vector3(-1000f, 0, 0);

        // Assigns the position of ll the ui elements o the corresponding arrays
        AssignButtonsAnimators();

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
        if (isAcceptingInputs)
        {
            if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene("_Scenes/scenaprova");

            if (selectionDepth == (int)panelSelection.basePanel)
            {
                if ((Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire3joy")) && canMove) // Walk
                {
                    inAction = true;
                    StartCoroutine(ActivatePanel((int)panelSelection.walkPanel));
                    StartCoroutine(ActivateMovementPhase());
                }
                else if ((Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Fire1joy")) && canTerraform) // Terraform
                {
                    StartCoroutine(ActivatePanel((int)panelSelection.terraformPanel));
                }
                else if (Input.GetKeyDown(KeyCode.C)) // Nothing
                {
                }
            }

            //else if (selectionDepth == (int)panelSelection.walkPanel)
            //{
            //    if (Input.GetKeyDown(KeyCode.Z)) // Confirm Walk
            //    {
            //    }
            //    else if (Input.GetKeyDown(KeyCode.S)) // Nothing
            //    {
            //    }
            //    else if (Input.GetKeyDown(KeyCode.D)) // Back to base panel
            //    {
            //        ActivatePanel((int)panelSelection.basePanel);
            //    }
            //}

            else if (selectionDepth == (int)panelSelection.terraformPanel)
            {
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire3joy")) // Slide
                {
                    inAction = true;
                    StartCoroutine(ActivatePanel((int)panelSelection.slidePanel));
                    StartCoroutine(ScrollTileInsertionSelection());
                }
                else if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Fire1joy")) // Rotation
                {
                    inAction = true;
                    StartCoroutine(ActivatePanel((int)panelSelection.rotatePanel));
                    StartCoroutine(ActivateRotationCursor());
                }
                else if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2joy")) // Back to base panel
                {
                    StartCoroutine(ActivatePanel((int)panelSelection.basePanel));
                }
            }

            else if (selectionDepth == (int)panelSelection.slidePanel)
            {
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire3joy")) // Confirm
                {
                }
                else if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Fire1joy")) // Nothing
                {
                }
                else if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2joy")) // Back to terraform panel
                {
                    StartCoroutine(ActivatePanel((int)panelSelection.terraformPanel));
                }
            }

            else if (selectionDepth == (int)panelSelection.rotatePanel)
            {
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire3joy")) // rotation counterclockwise
                {
                }
                else if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Fire1joy")) // rotation clockwise
                {
                }
                else if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2joy")) // Back to terraform panel
                {
                    StartCoroutine(ActivatePanel((int)panelSelection.terraformPanel));
                }
            }

            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("PassTurn")) && !inAction)
            {
                PassTurn();
            }

            if ((Input.GetKeyDown(KeyCode.M) || Input.GetButtonDown("Fire4joy")) && canUseCrystal)
            {
                ActivateDiamondStasis();
            }

            // cristallo
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    PassTurn();
            //}
        }

        //deprecated
        /*
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
        }*/
    }
}
