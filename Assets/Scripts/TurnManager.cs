﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{

    public GameObject xButton, aButton, bButton, diamondButton, rotationCursor, pauseMenu, hasCrystalEffect, winScreen, UI;
    public GameObject[] portraits;
    public GameObject[] playerWinsText;
    public Camera myCamera;
    public MapManager mapManager;
    public bool isSliding = false, isRotating = false;
    public bool isAcceptingInputs = true;
    public bool isInPause = false;
    private bool[] isFirstTurn;
    private bool isEnd;
    public FadeManager fade;

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
    public GameObject passButton;
    private Vector3 oldCameraPosition;
    private float oldCameraSize;
    public GameObject dialogueManager;
    public bool insertedTrapFromSlide;
    private Coordinate[] allCornerCoords;

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
        insertedTrapFromSlide = false;

        playerPlayingIdx++;
        playerPlayingIdx %= 4;
        passButton.GetComponent<Animator>().SetInteger("Player", playerPlayingIdx + 1);
        playerPlaying = playerOrder[playerPlayingIdx];
        activePlayer = playerComponent[playerPlayingIdx];

        StartCoroutine(MoveToNextPlayer());
        //myCameraMovement.MoveToPosition(activePlayer.GetComponentInParent<Transform>().position);

        //while (movingCamera) { }

        isAcceptingInputs = false;

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
        isAcceptingInputs = true;

        if (isFirstTurn[playerPlayingIdx])
        {
            dialogueManager.GetComponent<Speaker>().PlayIntros(playerPlayingIdx);
            isFirstTurn[playerPlayingIdx] = false;
        }

        mapManager.CorrectDiamondOwnership();

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
        dialogueManager.GetComponent<Speaker>().PlayVictory(player.playerNbr);
        winScreen.GetComponent<WinScript>().winner(player.playerNbr-1);
        //UI.SetActive(false);
        winScreen.SetActive(true);
        isEnd = true;
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
        foreach (Animator btn in buttonsAnimator)
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
        for (int i = trapsToActivate.Count - 1; i >= 0; i--)
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
                trapsToActivate.Remove(thisTrap);
                //trapsToActivate.RemoveAt(i);
            }
        }
    }

    public void ResetActivatedTraps()
    {
        for (int i = trapsToActivate.Count - 1; i >= 0; i--)
        {
            if (insertedTrapFromSlide && i == 0)
                continue;
            else
            {
                Trap thisTrap = trapsToActivate[i];
                thisTrap.SetPlayerDropping(0);
                trapsToActivate.Remove(thisTrap);
            }
        }
    }

    // Diamond

    private void AssignDiamondAura(Player player)
    {
        hasCrystalEffect.transform.SetParent(player.GetComponentInParent<Transform>());
        hasCrystalEffect.transform.localPosition = new Vector3(0f, 0f, -0.1f);
    }

    private void RemoveDiamondAura()
    {
        hasCrystalEffect.transform.parent = null;
        hasCrystalEffect.transform.position = parkingPosition;
    }

    private void UpdateDiamondPosition(Player player)
    {
        mapManager.diamondCoords = player.coordinate;
    }

    private void UpdateDiamondPosition(Tile tile)
    {
        mapManager.diamondCoords = tile.GetCoordinatesCopy();
    }

    private void UpdateDiamondPosition()
    {
        Tile tile = mapManager.myDiamondInstance.GetComponentInParent<Tile>();
        mapManager.diamondCoords = tile.GetCoordinatesCopy();
    }

    private bool ChecksForDiamond(Player player)
    {
        if (player.coordinate.isEqual(mapManager.GetDiamondCoords()) && diamondOnTable)
            return true;
        else
            return false;

    }

    public void CollectDiamond(Player player, bool isPlayerTurn=true)
    {
        dialogueManager.GetComponent<Speaker>().PlayCrystalGrab(player.playerNbr);
        diamondOnTable = false;
        player.SetHasDiamond(true);
        player.SetCanActivateStasis(true);
        mapManager.myDiamondInstance.transform.parent = player.transform;
        Vector3 collectedPosition = new Vector3(0, 0, 100);
        AssignDiamondAura(player);
        mapManager.myDiamondInstance.transform.localPosition = collectedPosition;
        mapManager.diamondCoords = player.coordinate;
        if (isPlayerTurn)
        {
            canUseCrystal = true;
            StartCoroutine(SetDiamondAnimation(0));
        }
    }

    public void DropDiamond(Player player)
    {
        RemoveDiamondAura();
        player.SetHasDiamond(false);
        player.SetCanActivateStasis(false);
        canUseCrystal = false;
        StartCoroutine(SetDiamondAnimation(3));

        mapManager.myDiamondInstance.transform.parent = null;
        mapManager.myDiamondInstance.transform.position = new Vector3(mapManager.myDiamondInstance.transform.position.x,
                                                                      mapManager.myDiamondInstance.transform.position.y,
                                                                      -10);
        Tile myTile = mapManager.PickTileComponent(player.coordinate);
        myTile.hasDiamond = true;

        UpdateDiamondPosition(myTile);
        ConnectToTile(myTile.GetComponent<Transform>().gameObject);

        player.ResetTurnsBeforeStasis();
        diamondOnTable = true;
    }

    private void ConnectToTile(GameObject tile)
    {
        mapManager.myDiamondInstance.transform.SetParent(tile.transform);
        mapManager.diamondCoords = tile.GetComponent<Tile>().getCoordinates();
        tile.GetComponent<Tile>().hasDiamond = true;
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

    public IEnumerator ActivateDiamondStasis()
    {
        mapManager.myDiamondInstance.transform.GetComponentInParent<Player>().ActivateStasis();
        canUseCrystal = false;
        //buttonsAnimator[3].SetBool("isActive", true);
        dialogueManager.GetComponent<Speaker>().PlayStasys(0);
        yield return null;
        PassTurn();
    }

    public IEnumerator SetDiamondAnimation(int anim)
    {
        switch (anim)
        {
            case 0:
                buttonsAnimator[3].SetInteger("ActiveStatus", 3);
                while (!buttonsAnimator[3].GetCurrentAnimatorStateInfo(0).IsName("activated")) { yield return null; }
                break;
            case 1:
                buttonsAnimator[3].SetInteger("ActiveStatus", 2);
                while (!buttonsAnimator[3].GetCurrentAnimatorStateInfo(0).IsName("transient_2")) { yield return null; }
                break;
            case 2:
                buttonsAnimator[3].SetInteger("ActiveStatus", 1);
                while (!buttonsAnimator[3].GetCurrentAnimatorStateInfo(0).IsName("transient_1")) { yield return null; }
                break;
            case 3:
                buttonsAnimator[3].SetInteger("ActiveStatus", 0);
                while (!buttonsAnimator[3].GetCurrentAnimatorStateInfo(0).IsName("inactive")) { yield return null; }
                break;
            case 4:
                buttonsAnimator[3].SetInteger("ActiveStatus", 0);
                while (!buttonsAnimator[3].GetCurrentAnimatorStateInfo(0).IsName("inactive")) { yield return null; }
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

    public void DisconnectDiamond()
    {
        mapManager.myDiamondInstance.transform.parent = null;
    }

    // Player Movement

    IEnumerator ActivateMovementPhase()
    {
        Player p = playerComponent[playerPlayingIdx];

        CameraStartFollowingPlayer(p);
        StartCoroutine(p.DeactivateBarrier());

        Coordinate movementStartingPosition = p.GetCoordinatesCopy();
        mapManager.updateTilesConnection(playerPlaying);
        p.BrightPossibleTiles();

        //p.UnchildFromTile();
        ActivatePlayer(playerComponent[playerPlayingIdx].playerNbr);

        yield return null;

        bool waitInput = true;
        while (waitInput)
        {
            yield return null;
            if (isAcceptingInputs)
            {
                if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Fire1joy") || Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2joy") || Input.GetButtonDown("StartButton") || trapHasTriggered || attackHasHappened)
                    waitInput = false;
            }
        }


        if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Fire1joy") || trapHasTriggered || attackHasHappened)
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
            myCameraMovement.MoveToPosition(p.transform.position);
            inAction = false;
            ResetActivatedTraps();
        }

        p.SwitchOffTiles();
        cursorIsActive = true;
        ActivatePlayer(0);
        makePlayersChild();
        if (!p.hasDiamond)
            StartCoroutine(p.ActivateBarrier());
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
        if (diamondOnTable)
            UpdateDiamondPosition();

        canTerraform = false;
        buttonsAnimator[1].SetBool("canTerraform", false);
        StartCoroutine(ActivatePanel((int)panelSelection.basePanel));
        mapManager.UpdateTilesZOrder();
        inAction = false;
        isAcceptingInputs = true;
        yield return null;

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
        bool repositionPlayer = false, reconnectDiamond = false;

        Coordinate[] lineCoordinates = arrow.GetComponent<InsertArrow>().getPointedCoords();
        lineCoordinates = mapManager.KeepMovableTiles(lineCoordinates);

        isAcceptingInputs = false;

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

        // Possible Player on destroyed tile
        Tile lastTile = mapManager.PickTileComponent(lineCoordinates[lineCoordinates.Length - 1]);
        int newCardType = lastTile.type;
        bool trapStatus = lastTile.GetIsTrapped();
        if (lastTile.GetPlayerChildNbr() != -1)
        {
            Debug.Log(lineCoordinates[lineCoordinates.Length - 1].GetPositionFromCoords(mapManager.columns, mapManager.rows).ToString());
            myCameraMovement.MoveToHighlight(lineCoordinates[lineCoordinates.Length - 1].GetPositionFromCoords(mapManager.columns, mapManager.rows));
            yield return null;
            int playerIdx = GeneralMethods.FindElementIdx(playerOrder, lastTile.GetPlayerChildNbr());
            fallingPlayer = playerComponent[playerIdx];
            if (fallingPlayer.hasDiamond)
            {
                DropDiamond(fallingPlayer);
                reconnectDiamond = true;
                DisconnectDiamond();
            }
            yield return StartCoroutine(lastTile.BlackHole(true));
            StartCoroutine(ZoomToCenter());
            yield return new WaitForSeconds(1f);
            dialogueManager.GetComponent<Speaker>().PlaySomeoneFall();

            //fallingPlayer.transform.parent = null;
            //diamondOnTable = true;
            //fallingPlayer.TeleportOffScreen();

            repositionPlayer = true;
        }
        if (lastTile.GetPlayerChildNbr() == -1 && lastTile.hasDiamond)
        {
            reconnectDiamond = true;
            DisconnectDiamond();
        }
        Debug.Log(mapManager.diamondCoords.ToString());

        yield return StartCoroutine(mapManager.SlideLine(lineCoordinates, slideDirection));
        isSliding = false;

        Debug.Log(mapManager.diamondCoords.ToString());

        int tileType = 0;

        //while (isSliding)
        //{
        //    yield return null;
        //}

        if (fallingPlayer != null || reconnectDiamond)
        {
            ConnectToTile(mapManager.PickTileObject(mapManager.diamondCoords));
        }

        tileType = activeCard.GetTileType();

        Tile myNewTile = null;
        if (activeCard.GetTrappedStatus())
            myNewTile = mapManager.InstantiateTileLive(tileType, lineCoordinates[0], true);  // places a trap on top of the tile
        else
            myNewTile = mapManager.InstantiateTileLive(tileType, lineCoordinates[0]);  // default is false

        if (repositionPlayer)
        {
            yield return StartCoroutine(myNewTile.BlackHoleRespawn(fallingPlayer));
        }

        //MAYHERE
        yield return null;
        activeCard.AssignType(newCardType, trapStatus);

        if (attackHasHappened)
        {
            CollectDiamond(rearPlayer, false);
            attackHasHappened = false;
        }

        arrow.GetComponent<Animator>().SetBool("isActive", false);
        mapManager.SetInsertArrowsVisible(false);
        myCameraMovement.MoveToPosition(activePlayer.GetComponentInParent<Transform>().position);
        StartCoroutine(EndTerraform());
        Debug.Log(mapManager.diamondCoords.ToString());
        yield return null;
    }

    IEnumerator ScrollTileInsertionSelection()
    {
        StartCoroutine(ZoomToCenter());
        //mapManager.SetInsertArrowsVisible(true);
        int currentSelection = 39; // Starts in the Top Left Side
        allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
        mapManager.SetInsertArrowVisible(true, currentSelection);

        yield return null;

        bool firstClick = false;
        float firstClickWaitingTime = 0.5f;

        while (!Input.GetKeyDown(KeyCode.C) && !Input.GetKeyDown(KeyCode.X) && !Input.GetButtonDown("Fire1joy") && !Input.GetButtonDown("Fire2joy") && !Input.GetButtonDown("StartButton"))
        {
            if ((Input.GetKey(KeyCode.D) || Input.GetAxis("HorizontalJoy") == 1 || Input.GetAxis("HorizontalAnalog") >= 0.9f) && !isInPause)
            {
                if (currentSelection >= 26 && currentSelection <= 39)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                else if (currentSelection >= 0 && currentSelection <= 12)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                else if (currentSelection == 51)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection = 0;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                if (!firstClick)
                {
                    yield return new WaitForSeconds(firstClickWaitingTime);
                    firstClick = true;
                }
            }

            else if ((Input.GetKey(KeyCode.A) || Input.GetAxis("HorizontalJoy") == -1 || Input.GetAxis("HorizontalAnalog") <= -0.9f) && !isInPause)
            {
                //canBeActivated = false;
                if (currentSelection >= 1 && currentSelection <= 13)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                else if (currentSelection >= 25 && currentSelection <= 38)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                else if (currentSelection == 0)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection = 51;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                if (!firstClick)
                {
                    yield return new WaitForSeconds(firstClickWaitingTime);
                    firstClick = true;
                }
            }

            else if ((Input.GetKey(KeyCode.W) || Input.GetAxis("VerticalJoy") == 1 || Input.GetAxis("VerticalAnalog") >= 0.9f) && !isInPause)
            {
                //canBeActivated = false;
                if (currentSelection >= 12 && currentSelection <= 25)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                else if (currentSelection >= 39 && currentSelection <= 51)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                else if (currentSelection == 0)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection = 51;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                if (!firstClick)
                {
                    yield return new WaitForSeconds(firstClickWaitingTime);
                    firstClick = true;
                }
            }

            else if ((Input.GetKey(KeyCode.S) || Input.GetAxis("VerticalJoy") == -1 || Input.GetAxis("VerticalAnalog") <= -0.9f) && !isInPause)
            {
                //canBeActivated = false;
                if (currentSelection >= 13 && currentSelection <= 26)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection--;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                else if (currentSelection >= 38 && currentSelection <= 50)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection++;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
                    yield return null;
                }
                else if (currentSelection == 51)
                {
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
                    mapManager.SetInsertArrowVisible(false, currentSelection);
                    yield return null;
                    currentSelection = 0;
                    allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", true);
                    mapManager.SetInsertArrowVisible(true, currentSelection);
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

        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2joy") || Input.GetButtonDown("StartButton"))
        {
            inAction = false;
            mapManager.SetInsertArrowsVisible(false);
            allInsertArrows[currentSelection].GetComponent<Animator>().SetBool("isActive", false);
            myCameraMovement.MoveToPosition(activePlayer.GetComponentInParent<Transform>().position);
            StartCoroutine(ActivatePanel((int)panelSelection.terraformPanel));
        }
        else if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Fire1joy") && !isInPause)
        {
            mapManager.SetInsertArrowVisible(true, currentSelection);
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
        isAcceptingInputs = false;
        Player frontPlayer = null, rearPlayer = null;

        Coordinate[] selectedCoords = rotationCursor.GetComponent<CursorMoving>().getSelectedCoords();
        rotationCursor.GetComponent<CursorMoving>().SetAtPosition(parkingPosition);

        // Possible PvP
        attackHasHappened = false;
        resolvingCombat = false;
        int[] playerIdxInCoords = GetPlayersInCoordinatesIDX(selectedCoords); // checks if players are found in the group of slided tiles

        if (playerIdxInCoords.Length > 1) // more than one player is on the slided tiles
        {
            Array.Sort(playerIdxInCoords);
            for (int i = 1; i < playerIdxInCoords.Length; i++)
            {
                if (direction == 1)
                {
                    if (playerIdxInCoords[i] - playerIdxInCoords[i - 1] == 1) // players are positioned one tile away from each other in the slide direction
                    {
                        // checks whether or not the player in the front has the stasis active
                        frontPlayer = GetPlayerAtCoordinates(selectedCoords[playerIdxInCoords[i]]);
                        //frontPlayer = mapManager.PickTileComponent(lineCoordinates[playerIdxInCoords[i]]).GetPlayerChild(); // non funziona dato che è in stasi
                        if (frontPlayer.GetStasisStatus())
                        {
                            rearPlayer = mapManager.PickTileComponent(selectedCoords[playerIdxInCoords[i - 1]]).GetPlayerChild();
                            attackHasHappened = true;
                            resolvingCombat = true;
                            StartCoroutine(rearPlayer.AttackPlayerOnTileOnSlide(frontPlayer));
                        }
                    }
                }
                else
                {
                    if (playerIdxInCoords[i] - playerIdxInCoords[i - 1] == -1) // players are positioned one tile away from each other in the slide direction
                    {
                        // checks whether or not the player in the front has the stasis active
                        frontPlayer = GetPlayerAtCoordinates(selectedCoords[playerIdxInCoords[i]]);
                        //frontPlayer = mapManager.PickTileComponent(lineCoordinates[playerIdxInCoords[i]]).GetPlayerChild(); // non funziona dato che è in stasi
                        if (frontPlayer.GetStasisStatus())
                        {
                            rearPlayer = mapManager.PickTileComponent(selectedCoords[playerIdxInCoords[i - 1]]).GetPlayerChild();
                            attackHasHappened = true;
                            resolvingCombat = true;
                            StartCoroutine(rearPlayer.AttackPlayerOnTileOnSlide(frontPlayer));
                        }
                    }
                }
            }
        }

        while (resolvingCombat) { yield return null; }

        yield return StartCoroutine(mapManager.RotateTiles(selectedCoords, direction));

        if (attackHasHappened)
        {
            CollectDiamond(rearPlayer, false);
            attackHasHappened = false;
        }

        UpdatePlayersPosition();
        if (diamondOnTable)
            UpdateDiamondPosition();
        cursorIsActive = true;
        yield return StartCoroutine(EndTerraform());

    }

    IEnumerator ActivateRotationCursor()
    {

        rotationCursor.GetComponent<CursorMoving>().CursorActivate(playerComponent[playerPlayingIdx].coordinate);

        yield return null;

        while (!Input.GetKeyDown(KeyCode.Z) && !Input.GetKeyDown(KeyCode.X) && !Input.GetKeyDown(KeyCode.C) && !Input.GetButtonDown("Fire1joy") && !Input.GetButtonDown("Fire2joy") && !Input.GetButtonDown("Fire3joy") && !Input.GetButtonDown("StartButton"))
        {
            yield return null;
        }

        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2joy")  || Input.GetButtonDown("StartButton"))
            {
                inAction = false;
                rotationCursor.GetComponent<CursorMoving>().CursorDeactivate();
                rotationCursor.GetComponent<CursorMoving>().SetAtPosition(parkingPosition);
                StartCoroutine(ActivatePanel((int)panelSelection.terraformPanel));
            }
            else if ((Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire3joy")))
            {
                canTerraform = false;
                rotationCursor.GetComponent<CursorMoving>().CursorDeactivate();
                yield return StartCoroutine(ActivateRotation(1));
            }
            else if ((Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Fire1joy")))
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

    public float GetOldCameraSize()
    {
        return oldCameraSize;
    }

    public Vector3 GetOldCameraPosition()
    {
        return oldCameraPosition;
    }

    public void SetOldCameraSize()
    {
        oldCameraSize = myCamera.orthographicSize;
    }

    public void SetOldCameraSize(float value)
    {
        oldCameraSize = value;
    }

    public void SetOldCameraPosition()
    {
        oldCameraPosition = myCamera.transform.position;
    }

    public void SetOldCameraPosition(Vector3 position)
    {
        oldCameraPosition = position;
    }

    public CameraMovement GetCameraComponent()
    {
        return myCameraMovement;
    }

    // General Methods

    public Player GetPlayerAtCoordinates(Coordinate coord)
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

    public IEnumerator Pause()
    {
        yield return null;
        isInPause = true;
        pauseMenu.SetActive(true);
    }

    public IEnumerator Resume ()
    {
        yield return null;
        isInPause = false;
        pauseMenu.SetActive(false);
    }

    public Coordinate[] GetAllCornerCoordinates()
    {
        return allCornerCoords;
    }

    // Unity Specific methods

    void Awake()
    {
        isFirstTurn = new bool[4] { true, true, true, true };
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
        allCornerCoords = new Coordinate[4];
        allCornerCoords[0] = new Coordinate(0, mapManager.rows-1);
        allCornerCoords[1] = new Coordinate(mapManager.columns-1, mapManager.rows-1);
        allCornerCoords[2] = new Coordinate(0, 0);
        allCornerCoords[3] = new Coordinate(mapManager.columns-1, 0);
        //makePlayersChild();

    }

    private void Start()
    {
        StartCoroutine(fade.FadeInUI());
    }


    void Update()
    {
        if (isEnd && Input.GetButtonDown("Fire1joy")) StartCoroutine(fade.FadeOutUI("_Scenes/CubeScene"));
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

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("StartButton")) && !isInPause)
        {
            StartCoroutine(Pause());
        }
        else if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("StartButton")) && isInPause)
        {
            StartCoroutine(Resume());
        }

            if (isAcceptingInputs && !isInPause)
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
                StartCoroutine(ActivateDiamondStasis());
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
