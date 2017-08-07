using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int playerNbr;
    public int isPlayerTurn; // number corresponding to the player playing
    public float walkingTime;

    SpriteRenderer myRenderer;
    //Texture2D myTexture;
    Sprite mySprite;
    Animator myAnimator, myBarrierAnimator;
    //BoxCollider2D myCollider;
    public bool moving = false;
    public Coordinate coordinate;
    public GameObject mapManager, turnManager;
    public GameObject stasisEffectPrefab, stasisEffect;
    public TurnManager turnManagerComponent;
    public MapManager mapManagerComponent;
    public List<Tile> toBright;
    public bool hasDiamond = false;
    public PlayerAssignment playerSO;
    private Coordinate startingPoint;
    private bool checkingCombat = false, attack1active=false, attack2active = false;
    private bool isStasisActive = false, canActivateStasis = false;
    private int turnsBeforeStasisCounter = 3, turnsBeforeStasisIsActive = 3;
    private GameObject myBarrier;

    //public GameObject dialogueManager;

    // Accessing Variable

    public Coordinate GetCoordinates()
    {
        return coordinate;
    }

    public Coordinate GetCoordinatesCopy()
    {
        return new Coordinate(coordinate.getX(), coordinate.getY());
    }

    public void SetAttack1Status(bool status)
    {
        attack1active = status;
    }

    public void SetAttack2Status(bool status)
    {
        attack2active = status;
    }

    // Animation

    public IEnumerator ActivateBarrier()
    {
        myBarrier.transform.localPosition = new Vector3(0f, 0f, -0.01f);
        myBarrierAnimator.SetBool("isActive", true);
        yield return null;
    }

    public IEnumerator DeactivateBarrier()
    {
        myBarrierAnimator.SetBool("isActive", false);
        //while (myBarrierAnimator.GetCurrentAnimatorStateInfo(0).IsName("disappearing")) { yield return null; }
        yield return new WaitForSeconds(0.5f);
        myBarrier.transform.localPosition = new Vector3(0f, 0f, 10f);
        yield return null;
    }

    public void InvertTransformX()
    {
        transform.GetChild(0).transform.localScale = new Vector3(transform.GetChild(0).transform.localScale.x * -1,
                                                                     transform.GetChild(0).transform.localScale.y,
                                                                     transform.GetChild(0).transform.localScale.z);
    }

    public void setPlayerSprite()
    {
        mySprite = playerSO.GetSprite(playerNbr);
        myRenderer.sprite = mySprite;
        myAnimator = transform.GetChild(0).GetComponent<Animator>();
        myAnimator.runtimeAnimatorController = playerSO.GetAnimator(playerNbr);
        myBarrierAnimator.runtimeAnimatorController = playerSO.GetBarrierAnimator(playerNbr);

        //myCollider.size = new Vector2(myTexture.width/100f, myTexture.height/100f);

    }

    public void StartWalking()
    {
        myAnimator.SetBool("isWalking", true);
    }

    public void StopWalking()
    {
        myAnimator.SetBool("isWalking", false);
    }

    public void StartAnimationAttack()
    {
        myAnimator.SetBool("isAttacking", true);
        SetAttack1Status(true);
    }

    public IEnumerator StopAnimaitionAttack()
    {
        myAnimator.SetBool("isAttacking", false);
        myAnimator.SetBool("isAttackingEnding", true);
        yield return null;
        yield return StartCoroutine(WaitForAnimation("attack_2"));
        myAnimator.SetBool("isAttackingEnding", false);
    }

    private IEnumerator WaitForAnimation(string animationName)
    {
        if (animationName == "attack_1")
        {
            do
            {
                yield return null;
            } while (attack1active);
        }
        else if(animationName == "attack_2")
        {
            do
            {
                yield return null;
            } while (attack2active);
        }

        yield return null;

        //yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator CastBlackHole(Tile tile)
    {
        yield return tile.BlackHole();
        yield return null;
    }

    // Initial Assignements

    public void SetStartingPoint()
    {
        switch (playerNbr)
        {

            case 1:
                startingPoint = new Coordinate(0, mapManagerComponent.rows - 1);
                break;
            case 2:
                startingPoint = new Coordinate(mapManagerComponent.columns - 1, mapManagerComponent.rows - 1);
                break;
            case 3:
                startingPoint = new Coordinate(0, 0);
                break;
            case 4:
                startingPoint = new Coordinate(mapManagerComponent.columns - 1, 0);
                break;
            default:
                break;
        }
    }

    // Walking Path

    public bool CheckForOtherPlayerCorner(Coordinate coord)
    {
        bool isOtherPlayerCorner = false;
        Coordinate[] allCornerCoord = turnManagerComponent.GetAllCornerCoordinates();
        int idx = GeneralMethods.FindElementIdx(allCornerCoord, coord);
        if (idx != -1 && idx != playerNbr-1)
            isOtherPlayerCorner = true;
        return isOtherPlayerCorner;
    }

    public void BrightPossibleTiles()
    {
        Tile nextToAdd;
        Tile currentPosition = mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>();

        toBright.Add(currentPosition);
        for (int i = 0; i < toBright.Count; i++)
        {
            if (toBright[i].effectiveConnections[0])
            {
                nextToAdd = mapManagerComponent.myMap[toBright[i].myCoord.getX(), toBright[i].myCoord.getY() + 1].GetComponent<Tile>();
                if (!toBright.Contains(nextToAdd) && !CheckForOtherPlayerCorner(nextToAdd.myCoord))
                {
                    toBright.Add(nextToAdd);
                }
            }

            if (toBright[i].effectiveConnections[1])
            {
                nextToAdd = mapManagerComponent.myMap[toBright[i].myCoord.getX() + 1, toBright[i].myCoord.getY()].GetComponent<Tile>();
                if (!toBright.Contains(nextToAdd) && !CheckForOtherPlayerCorner(nextToAdd.myCoord))
                {
                    toBright.Add(nextToAdd);
                }
            }

            if (toBright[i].effectiveConnections[2])
            {
                nextToAdd = mapManagerComponent.myMap[toBright[i].myCoord.getX(), toBright[i].myCoord.getY() - 1].GetComponent<Tile>();
                if (!toBright.Contains(nextToAdd) && !CheckForOtherPlayerCorner(nextToAdd.myCoord))
                {
                    toBright.Add(nextToAdd);
                }
            }

            if (toBright[i].effectiveConnections[3])
            {
                nextToAdd = mapManagerComponent.myMap[toBright[i].myCoord.getX() - 1, toBright[i].myCoord.getY()].GetComponent<Tile>();
                if (!toBright.Contains(nextToAdd) && !CheckForOtherPlayerCorner(nextToAdd.myCoord))
                {
                    toBright.Add(nextToAdd);
                }
            }
        }

        for (int i = 0; i < toBright.Count; i++)
        {
            toBright[i].SetSelected(true);
        }
    }

    public void SwitchOffTiles()
    {
        for (int i = 0; i < toBright.Count; i++)
        {
            toBright[i].SetSelected(false);
            //toBright[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
        toBright.Clear();
    }

    // Player position update

    public void UpdatePlayerPosition(Tile myTile)  // ULTIMA MODIFICA
    {
        UnchildFromTile();
        coordinate = myTile.GetCoordinatesCopy();
        ChildTile(myTile);
    }

    public void UpdatePlayerPosition()
    {
        if (!isStasisActive)
        {
            Tile myTile = transform.parent.GetComponent<Tile>();
            coordinate = myTile.GetCoordinatesCopy();
        }
    }

    public void ResetToStartingPosition()
    {
        TeleportAtCoordinates(startingPoint);
    }

    public void TeleportAtCoordinates(Coordinate coord)
    {
        GameObject targetTile;

        if (transform.parent != null)
            transform.parent = null;

        coordinate = coord;
        targetTile = mapManagerComponent.PickTileObject(coord);
        transform.position = new Vector3(targetTile.transform.position.x, targetTile.transform.position.y, -5);
        UpdatePlayerPosition(targetTile.GetComponent<Tile>());

        targetTile.GetComponent<Tile>().SetPlayerChild(this);

    }

    public void TeleportOffScreen()
    {
        transform.parent = null;
        transform.position = new Vector3(-1000, 0, -5);
    }

    public IEnumerator CheckForTraps(Tile tile)
    {
        if (tile.GetIsTrapped())
        {
            Trap trap = tile.GetTrap();
            if (trap.GetIsActive() && trap.GetPlayerDropping() != playerNbr)
            {
                CameraMovement myCamera = turnManagerComponent.GetCameraComponent();
                myCamera.MoveToHighlight(tile.GetComponent<Transform>().position);

                turnManagerComponent.dialogueManager.GetComponent<Speaker>().PlayTrapTrigger(tile.myTrapComponent.GetPlayerDropping());

                myCamera.StopFollowingPlayer();

                if (hasDiamond)
                {
                    turnManager.GetComponent<TurnManager>().DropDiamond(this);
                }
                yield return StartCoroutine(tile.myTrapComponent.Trigger());
                turnManager.GetComponent<TurnManager>().SetTrapHasTriggered(true);
                //tile.SetPlayerChild();

                yield return new WaitForSeconds(1f);
                myCamera.StartFollowingPlayer(this);
                yield return new WaitForSeconds(1f);
            }
            else if (!trap.GetIsActive() && trap.GetPlayerDropping() == 0)
            {
                turnManagerComponent.dialogueManager.GetComponent<Speaker>().PlayTrapActivated(playerNbr);
                trap.SetPlayerDropping(playerNbr);
                turnManager.GetComponent<TurnManager>().AddToActivateTrapList(trap);
                yield return null;
            }
        }
    }

    public bool CheckForVictory()
    {
        if (hasDiamond && coordinate.isEqual(startingPoint))
            return true;
        else
            return false;
    }

    public void CheckForOtherPlayer(Tile tile)
    {
        int other = tile.GetPlayerChildNbr();
        if (other != -1)
        {
            turnManagerComponent.SetOldCameraPosition(GeneralMethods.GetVect3Midpoint(transform.position, tile.GetComponent<Transform>().position));
            turnManagerComponent.SetOldCameraSize();

            checkingCombat = true;
            StartCoroutine(AttackPlayerOnTile(tile));
        }
        else
        {
            foreach (var thisPlayer in turnManagerComponent.GetAllPayers())
            {
                if (thisPlayer.FindInCoords(tile.getCoordinates()))
                {
                    turnManagerComponent.SetOldCameraPosition(GeneralMethods.GetVect3Midpoint(transform.position, tile.GetComponent<Transform>().position));
                    turnManagerComponent.SetOldCameraSize();

                    checkingCombat = true;
                    StartCoroutine(AttackPlayerOnTile(tile, true));
                }
            }
        }
    }

    public IEnumerator AttackPlayerOnTile(Tile tile, bool isStasisActive=false)
    {
        Player otherPlayer = null;
        if (isStasisActive)
        {
            otherPlayer = turnManagerComponent.GetPlayerAtCoordinates(tile.getCoordinates());
            otherPlayer.DeactivateStasis();
        }
        else
            otherPlayer = tile.gameObject.transform.GetComponentInChildren<Player>();

        CameraMovement myCamera = turnManagerComponent.GetCameraComponent();
        myCamera.MoveToHighlight(GeneralMethods.GetVect3Midpoint(transform.position, tile.GetComponent<Transform>().position));

        StartAnimationAttack();
        yield return null;

        yield return StartCoroutine(WaitForAnimation("attack_1"));
        turnManagerComponent.dialogueManager.GetComponent<Speaker>().PlayPvP(this.playerNbr, otherPlayer.playerNbr);
        yield return StartCoroutine(CastBlackHole(tile));
        SetAttack2Status(true);
        yield return StartCoroutine(StopAnimaitionAttack());

        //turnManager.GetComponent<TurnManager>().DropDiamond(otherPlayer);
        //otherPlayer.ResetToStartingPosition();

        float elapsedTime = 0;
        float animTime = 1f;

        Vector3 destination = tile.GetComponent<Transform>().position;
        destination.z--;
        StartWalking();
        yield return null;
        StopWalking();

        while (elapsedTime < animTime)
        {
            transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        myCamera.MoveToPositionAndZoom(turnManagerComponent.GetOldCameraPosition(), turnManagerComponent.GetOldCameraSize());

        turnManager.GetComponent<TurnManager>().CollectDiamond(this);
        turnManager.GetComponent<TurnManager>().SetAttackHasHappened(true);
        checkingCombat = false;
        yield return null;
    }

    public IEnumerator AttackPlayerOnTileOnSlide(Player otherPlayer)
    {
        CameraMovement myCamera = turnManagerComponent.GetCameraComponent();
        myCamera.MoveToHighlight(GeneralMethods.GetVect3Midpoint(transform.position, otherPlayer.GetComponent<Transform>().position));

        Tile tile = mapManagerComponent.PickTileComponent(otherPlayer.coordinate);
        StartAnimationAttack();
        yield return null;

        turnManagerComponent.dialogueManager.GetComponent<Speaker>().PlayPvP(this.playerNbr, otherPlayer.playerNbr);
        yield return StartCoroutine(WaitForAnimation("attack_1"));
        yield return StartCoroutine(CastBlackHole(tile));
        SetAttack2Status(true);
        //turnManager.GetComponent<TurnManager>().DropDiamond(otherPlayer);
        //otherPlayer.ResetToStartingPosition();
        yield return StartCoroutine(StopAnimaitionAttack());

        turnManager.GetComponent<TurnManager>().SetResolvingCombat(false);
        checkingCombat = false;
        yield return null;
    }

    public void UnchildFromTile()
    {
        if(!isStasisActive && transform.parent != null)
        {
            GameObject parentTile = transform.parent.gameObject;
            parentTile.GetComponent<Tile>().SetPlayerChild();
            transform.parent = null;
        }
    }

    public void ChildTile(Tile tile)
    {
        tile.SetPlayerChild(this);
    }

    // Diamond

    private void InstantiateStasisEffect()
    {
        stasisEffect = Instantiate(stasisEffectPrefab, transform);
        stasisEffect.transform.localPosition = new Vector3(0, 0, -0.1f);
    }

    private IEnumerator DestroyStasisEffect()
    {
        stasisEffect.GetComponent<Animator>().SetBool("isActive", false);
        yield return new WaitForSeconds(0.5f);
        Destroy(stasisEffect);
    }

    public void ResetTurnsBeforeStasis()
    {
        turnsBeforeStasisCounter = turnsBeforeStasisIsActive+1;
    }

    public void SetHasDiamond(bool hasDiamond)
    {
        this.hasDiamond = hasDiamond;
        if (hasDiamond)
            turnsBeforeStasisCounter = 0;
    }

    public bool GetHasDiamond()
    {
        return hasDiamond;
    }

    public bool GetStasisStatus()
    {
        return isStasisActive;
    }

    public void SetStasisStatus(bool status)
    {
        isStasisActive = status;
    }

    public void ActivateStasis()
    {
        UnchildFromTile();
        turnsBeforeStasisCounter = turnsBeforeStasisIsActive;
        canActivateStasis = false;
        isStasisActive = true;
        InstantiateStasisEffect();
    }

    public void DeactivateStasis()
    {
        //turnsBeforeStasisCounter = turnsBeforeStasisIsActive;
        coordinate.GetCoordsFromPosition(transform.position, mapManager.GetComponent<MapManager>().columns, mapManager.GetComponent<MapManager>().rows);

        GameObject myTile = mapManagerComponent.PickTileObject(coordinate);
        transform.SetParent(myTile.transform);

        canActivateStasis = false;
        isStasisActive = false;
        StartCoroutine(DestroyStasisEffect());
    }

    public int GetTurnsBeforeStasisCounter()
    {
        return turnsBeforeStasisCounter;
    }

    public void CheckDiamondStatusTimer()
    {
        if(hasDiamond)
        {

            if (isStasisActive)
            {
                DeactivateStasis();
            }
            else
            {
                if (turnsBeforeStasisCounter != 0)
                {
                    turnsBeforeStasisCounter--;
                }
                if(turnsBeforeStasisCounter == 0)
                {
                    canActivateStasis = true;
                }
            }
        }
    }

    public bool GetCanActivateStasis()
    {
        return canActivateStasis;
    }

    public void SetCanActivateStasis(bool status)
    {
        canActivateStasis = status;
    }

    // Movement Coroutines

    public IEnumerator MoveRight()
    {
        //Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[1]);
        //Debug.Log("effective: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[1]);
        moving = true;
        turnManagerComponent.isAcceptingInputs = false;

        if (transform.GetChild(0).transform.localScale.x < 0f)
            InvertTransformX();

        if (mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[1] == true)
        {
            float elapsedTime = 0;

            GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX() + 1, coordinate.getY()];
            Vector3 destination = destinationTile.GetComponent<Transform>().position;
            destination.z--;

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();

            if (!CheckForOtherPlayerCorner(destinationTileComponent.myCoord))
            {
                CheckForOtherPlayer(destinationTileComponent);

                if (!checkingCombat)
                {
                    StartWalking();
                    yield return null;
                    StopWalking();
                    while (elapsedTime < walkingTime)
                    {
                        transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / walkingTime);

                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    while (checkingCombat)
                    {
                        yield return null;
                    }
                }

                UpdatePlayerPosition(destinationTileComponent);
                yield return StartCoroutine(CheckForTraps(destinationTileComponent));
            }
            yield return null;

            //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
            //coordinate.setCoordinate(coordinate.getX() + 1, coordinate.getY());
        }

        //yield return new WaitForSeconds(0.1f);

        //StopWalking();
        moving = false;
        turnManagerComponent.isAcceptingInputs = true;
    }

    public IEnumerator MoveUp()
    {
        //Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[0]);
        moving = true;
        turnManagerComponent.isAcceptingInputs = false;

        if (mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[0] == true)
        {

            float elapsedTime = 0;

            GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX(), coordinate.getY() + 1];
            Vector3 destination = destinationTile.GetComponent<Transform>().position;
            destination.z--;

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            if (!CheckForOtherPlayerCorner(destinationTileComponent.myCoord))
            {
                CheckForOtherPlayer(destinationTileComponent);

                if (!checkingCombat)
                {
                    StartWalking();
                    yield return null;
                    StopWalking();
                    while (elapsedTime < walkingTime)
                    {
                        transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / walkingTime);

                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    while (checkingCombat)
                    {
                        yield return null;
                    }
                }

                UpdatePlayerPosition(destinationTileComponent);
                yield return StartCoroutine(CheckForTraps(destinationTileComponent));
            }
            yield return null;

            //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
            //coordinate.setCoordinate(coordinate.getX(), coordinate.getY() + 1);
        }

        //yield return new WaitForSeconds(0.1f);

        //StopWalking();
        turnManagerComponent.isAcceptingInputs = true;
        moving = false;
    }

    public IEnumerator MoveDown()
    {
        //Debug.Log("effective:" + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[2]);
        //Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[2]);
        moving = true;
        turnManagerComponent.isAcceptingInputs = false;

        if (mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[2] == true)
        {
            float elapsedTime = 0;

            GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX(), coordinate.getY() - 1];
            Vector3 destination = destinationTile.GetComponent<Transform>().position;
            destination.z--;

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            if (!CheckForOtherPlayerCorner(destinationTileComponent.myCoord))
            {
                CheckForOtherPlayer(destinationTileComponent);

                if (!checkingCombat)
                {
                    StartWalking();
                    yield return null;
                    StopWalking();
                    while (elapsedTime < walkingTime)
                    {
                        transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / walkingTime);

                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    while (checkingCombat)
                    {
                        yield return null;
                    }
                }

                UpdatePlayerPosition(destinationTileComponent);
                yield return StartCoroutine(CheckForTraps(destinationTileComponent));
            }
            yield return null;

            //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
            //coordinate.setCoordinate(coordinate.getX(), coordinate.getY() - 1);
        }

        //yield return new WaitForSeconds(0.1f);

        //StopWalking();
        turnManagerComponent.isAcceptingInputs = true;
        moving = false;
    }

    public IEnumerator MoveLeft()
    {
        //Debug.Log("possible: " + MapManager.GetComponent<MapManager>().myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().possibleConnections[3]);
        moving = true;
        turnManagerComponent.isAcceptingInputs = false;

        if (transform.GetChild(0).transform.localScale.x > 0f)
            InvertTransformX();

        if (mapManagerComponent.myMap[coordinate.getX(), coordinate.getY()].GetComponent<Tile>().effectiveConnections[3] == true)
        {
            float elapsedTime = 0;

            GameObject destinationTile = mapManagerComponent.myMap[coordinate.getX() - 1, coordinate.getY()];
            Vector3 destination = destinationTile.GetComponent<Transform>().position;
            destination.z--;

            Tile destinationTileComponent = destinationTile.GetComponent<Tile>();
            if (!CheckForOtherPlayerCorner(destinationTileComponent.myCoord))
            {
                CheckForOtherPlayer(destinationTileComponent);

                if (!checkingCombat)
                {
                    StartWalking();
                    yield return null;
                    StopWalking();
                    while (elapsedTime < walkingTime)
                    {
                        transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / walkingTime);

                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    while (checkingCombat)
                    {
                        yield return null;
                    }
                }

                UpdatePlayerPosition(destinationTileComponent);
                yield return StartCoroutine(CheckForTraps(destinationTileComponent));
            }
            yield return null;

            //myPlayer.transform.position = tiledMap[playerGridY, playerGridX].RightTile.transform.position;
            //coordinate.setCoordinate(coordinate.getX() - 1, coordinate.getY());
        }

        //yield return new WaitForSeconds(0.1f);

        //StopWalking();
        turnManagerComponent.isAcceptingInputs = true;
        moving = false;
    }

    // General Methods

    public int FindInCoords(Coordinate[] coords)
    {
        int idx = -1;
        for (int i = 0; i < coords.Length; i++)
        {
            if (coordinate.isEqual(coords[i]))
            {
                idx = i;
                break;
            }
        }
        return idx;
    }

    public bool FindInCoords(Coordinate coords)
    {
        bool isAtCoord = false;
        if (coordinate.isEqual(coords))
        {
            isAtCoord = true;
        }
        return isAtCoord;
    }


    // Unity Specific methods

    void Awake()
    {
        myRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //myCollider = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();
        mapManager = GameObject.FindGameObjectWithTag("MapManager");
        turnManager = GameObject.FindGameObjectWithTag("TurnManager");
        turnManagerComponent = turnManager.GetComponent<TurnManager>();
        toBright = new List<Tile>();
        mapManagerComponent = mapManager.GetComponent<MapManager>();
        SetStartingPoint();
        stasisEffectPrefab = (GameObject)Resources.Load("Stasis");
        myBarrier = Instantiate((GameObject)Resources.Load("Barrier"));

        myBarrier.transform.SetParent(transform);
        myBarrierAnimator = myBarrier.GetComponent<Animator>();
    }

    void Update()
    {
        if (turnManagerComponent.isAcceptingInputs)
        {
            if (!moving && playerNbr == isPlayerTurn)
            {
                if (Input.GetKeyDown(KeyCode.D) || Input.GetAxis("HorizontalJoy") == 1 || Input.GetAxis("HorizontalAnalog") >= 0.9f)
                {
                    StartCoroutine(MoveRight());
                }
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetAxis("HorizontalJoy") == -1 || Input.GetAxis("HorizontalAnalog") <= -0.9f)
                {
                    StartCoroutine(MoveLeft());
                }
                else if (Input.GetKeyDown(KeyCode.W) || Input.GetAxis("VerticalJoy") == 1 || Input.GetAxis("VerticalAnalog") >= 0.9f)
                {
                    StartCoroutine(MoveUp());
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetAxis("VerticalJoy") == -1 || Input.GetAxis("VerticalAnalog") <= -0.9f)
                {
                    StartCoroutine(MoveDown());
                }
            }
        }
    }

}


