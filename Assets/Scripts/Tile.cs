using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour {

    public int type;
    public Sprite mySprite;
    public SpriteRenderer myRenderer;
    public Texture2D myTexture;
    public Coordinate myCoord;
    public GameObject trap, blackHole;

    public bool canBeMoved = true;
    public bool[] possibleConnections, effectiveConnections;

    private BoxCollider2D myCollider;
    public GameObject myTrap;
    public Trap myTrapComponent;
    public bool isTrapped, hasDiamond;
    public int childPlayerNbr;
    private Player childPlayerComponent;
    private Player[] allPlayersComponent;
    private TurnManager turnManager;
    private bool blackHolePlaying = false;

    private Vector3 playerPosition;  // check whether it is actually needed or not

    enum tileTypes // B - bottom, R - right, T - top, L - left, V - vertical, H - horizontal
    {
        Curve_BR, Curve_LB, Curve_RT, Curve_TL, Straight_V, Straight_H, T_B, T_L, T_T, T_R, Cross,
        Curve_BR_alt, Curve_LB_alt, Curve_RT_alt, Curve_TL_alt, T_B_alt, T_L_alt, T_T_alt, T_R_alt, Goal
    };

    // Animation

    public void SetBlackHolePlaying(bool status)
    {
        blackHolePlaying = status;
    }

    public void SetSprite(int type)
    {
        switch (type)
        {
            case (int)tileTypes.Curve_BR:
                myTexture = (Texture2D)Resources.Load("Tiles/curva");
                this.type = type;
                break;
            case (int)tileTypes.Curve_LB:
                myTexture = (Texture2D)Resources.Load("Tiles/curva2");
                this.type = type;
                break;
            case (int)tileTypes.Curve_RT:
                myTexture = (Texture2D)Resources.Load("Tiles/curva3");
                this.type = type;
                break;
            case (int)tileTypes.Curve_TL:
                myTexture = (Texture2D)Resources.Load("Tiles/curva4");
                this.type = type;
                break;
            case (int)tileTypes.Straight_V:
                myTexture = (Texture2D)Resources.Load("Tiles/Straight");
                this.type = type;
                break;
            case (int)tileTypes.Straight_H:
                myTexture = (Texture2D)Resources.Load("Tiles/Straight2");
                this.type = type;
                break;
            case (int)tileTypes.T_B:
                myTexture = (Texture2D)Resources.Load("Tiles/t");
                this.type = type;
                break;
            case (int)tileTypes.T_L:
                myTexture = (Texture2D)Resources.Load("Tiles/t2");
                this.type = type;
                break;
            case (int)tileTypes.T_T:
                myTexture = (Texture2D)Resources.Load("Tiles/t3");
                this.type = type;
                break;
            case (int)tileTypes.T_R:
                myTexture = (Texture2D)Resources.Load("Tiles/t4");
                this.type = type;
                break;
            case (int)tileTypes.Cross:
                myTexture = (Texture2D)Resources.Load("Tiles/cross");
                this.type = type;
                break;
            case (int)tileTypes.Curve_BR_alt:
                myTexture = (Texture2D)Resources.Load("Tiles/curva_alt");
                this.type = (int)tileTypes.Curve_BR;
                break;
            case (int)tileTypes.Curve_LB_alt:
                myTexture = (Texture2D)Resources.Load("Tiles/curva2_alt");
                this.type = (int)tileTypes.Curve_LB;
                break;
            case (int)tileTypes.Curve_RT_alt:
                myTexture = (Texture2D)Resources.Load("Tiles/curva3_alt");
                this.type = (int)tileTypes.Curve_RT;
                break;
            case (int)tileTypes.Curve_TL_alt:
                myTexture = (Texture2D)Resources.Load("Tiles/curva4_alt");
                this.type = (int)tileTypes.Curve_TL;
                break;
            case (int)tileTypes.T_B_alt:
                myTexture = (Texture2D)Resources.Load("Tiles/t_alt");
                this.type = (int)tileTypes.T_B;
                break;
            case (int)tileTypes.T_L_alt:
                myTexture = (Texture2D)Resources.Load("Tiles/t2_alt");
                this.type = (int)tileTypes.T_L;
                break;
            case (int)tileTypes.T_T_alt:
                myTexture = (Texture2D)Resources.Load("Tiles/t3_alt");
                this.type = (int)tileTypes.T_T;
                break;
            case (int)tileTypes.T_R_alt:
                myTexture = (Texture2D)Resources.Load("Tiles/t4_alt");
                this.type = (int)tileTypes.T_R;
                break;
            case (int)tileTypes.Goal:
                myTexture = (Texture2D)Resources.Load("Tiles/goal");
                this.type = (int)tileTypes.Cross;
                break;
            default:
                break;
        }

        mySprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.66f));
        myRenderer.sprite = mySprite;
        myCollider.size = new Vector2(myTexture.width/100f, myTexture.height/100f);
    }

    public IEnumerator WaitForBlackholeAnim(GameObject myBlackHole, string animName)
    {

        Animator blackHoleAnimator = myBlackHole.GetComponent<Animator>();
        do
        {
            yield return null;
        } while (blackHoleAnimator.GetCurrentAnimatorStateInfo(0).IsName(animName));

        
        yield return null;
    }

    public IEnumerator BlackHole()
    {
        Debug.Log("Started Tile Black Hole");
        SetBlackHolePlaying(true);
        GameObject myBlackHole = Instantiate(blackHole, transform);
        yield return null;
        myBlackHole.transform.localPosition = new Vector3(0f, 0f, -3f);

        myBlackHole.GetComponent<blackHole>().AssignTile(this);
        yield return StartCoroutine(WaitForBlackholeAnim(myBlackHole, "blackHole_1"));
        ClearTile();
        yield return StartCoroutine(WaitForBlackholeAnim(myBlackHole, "blackHole_2"));
        Destroy(myBlackHole);
        yield return null;
        Debug.Log("Finished Tile Black Hole");
    }

    // Player Child

    public void SetPlayerChild(Player player)
    {
        if (player.transform.parent != null)
            player.transform.parent = null;

        childPlayerNbr = player.playerNbr;
        childPlayerComponent = player;
        player.transform.SetParent(transform);
    }

    public void SetPlayerChild()
    {
        childPlayerNbr = -1;
        childPlayerComponent = null;
    }

    public int GetPlayerChildNbr()
    {
        return childPlayerNbr;
    }

    public Player GetPlayerChild()
    {
        return childPlayerComponent;
    }

    public void ClearTile()
    {
        if (childPlayerNbr != -1)
        {
            turnManager.GetComponent<TurnManager>().DropDiamond(childPlayerComponent);
            childPlayerComponent.ResetToStartingPosition();
        }
        else
        {
            foreach (var player in turnManager.GetAllPayers())
            {
                if (player.coordinate.isEqual(myCoord))  // if true it mean that a player has the stasis active
                {
                    turnManager.GetComponent<TurnManager>().DropDiamond(player);
                    player.ResetToStartingPosition();
                }
            }

        }
    }


    // Tiles connectivity

    public void SetPossibleConnections(int type)
    {
        switch (type)
        {
            case (int)tileTypes.Curve_BR:
            case (int)tileTypes.Curve_BR_alt:
                possibleConnections[0] = false;
                possibleConnections[1] = true;
                possibleConnections[2] = true;
                possibleConnections[3] = false;
                break;
            case (int)tileTypes.Curve_LB:
            case (int)tileTypes.Curve_LB_alt:
                possibleConnections[0] = false;
                possibleConnections[1] = false;
                possibleConnections[2] = true;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.Curve_RT:
            case (int)tileTypes.Curve_RT_alt:
                possibleConnections[0] = true;
                possibleConnections[1] = true;
                possibleConnections[2] = false;
                possibleConnections[3] = false;
                break;
            case (int)tileTypes.Curve_TL:
            case (int)tileTypes.Curve_TL_alt:
                possibleConnections[0] = true;
                possibleConnections[1] = false;
                possibleConnections[2] = false;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.Straight_V:
                possibleConnections[0] = true;
                possibleConnections[1] = false;
                possibleConnections[2] = true;
                possibleConnections[3] = false;
                break;
            case (int)tileTypes.Straight_H:
                possibleConnections[0] = false;
                possibleConnections[1] = true;
                possibleConnections[2] = false;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.T_B:
            case (int)tileTypes.T_B_alt:
                possibleConnections[0] = false;
                possibleConnections[1] = true;
                possibleConnections[2] = true;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.T_L:
            case (int)tileTypes.T_L_alt:
                possibleConnections[0] = true;
                possibleConnections[1] = false;
                possibleConnections[2] = true;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.T_T:
            case (int)tileTypes.T_T_alt:
                possibleConnections[0] = true;
                possibleConnections[1] = true;
                possibleConnections[2] = false;
                possibleConnections[3] = true;
                break;
            case (int)tileTypes.T_R:
            case (int)tileTypes.T_R_alt:
                possibleConnections[0] = true;
                possibleConnections[1] = true;
                possibleConnections[2] = true;
                possibleConnections[3] = false;
                break;
            case (int)tileTypes.Cross:
            case (int)tileTypes.Goal:
                possibleConnections[0] = true;
                possibleConnections[1] = true;
                possibleConnections[2] = true;
                possibleConnections[3] = true;
                break;
                       
            default:
                break;
        }
    }

    public void CheckConnections(Tile other, int lato, int playerPlayingNbr = -1)
    {
        if (other != null)
        {

            if (other.GetPlayerChildNbr() == -1 || other.GetPlayerChildNbr() == playerPlayingNbr || (other.GetPlayerChild().GetHasDiamond()))
            {
                switch (lato)
                {
                    case 0:
                        if (possibleConnections[lato] && other.possibleConnections[2])
                        {
                            effectiveConnections[lato] = true;
                            //other.effectiveConnections[2] = true;
                        }
                        break;
                    case 1:
                        if (possibleConnections[lato] && other.possibleConnections[3])
                        {
                            effectiveConnections[lato] = true;
                            //other.effectiveConnections[3] = true;
                        }
                        break;
                    case 2:
                        if (possibleConnections[lato] && other.possibleConnections[0])
                        {
                            effectiveConnections[lato] = true;
                            //other.effectiveConnections[0] = true;
                        }
                        break;
                    case 3:
                        if (possibleConnections[lato] && other.possibleConnections[1])
                        {
                            effectiveConnections[lato] = true;
                            //other.effectiveConnections[1] = true;
                        }
                        break;
                    default:
                        break;

                }
            }
        }
        
    }

    // Position update

    public IEnumerator MoveToPosition(Vector2 movement, float animTime)
    {
        // Might there be a child unchild issue?

        //float elapsedTime = 0;

        Vector3 destination = new Vector3(transform.position.x + movement[0], transform.position.y + movement[1], transform.position.z);
        transform.DOMove(destination, animTime);

        //while (elapsedTime < animTime)
        //{
        //    transform.position = Vector3.Lerp(transform.position, destination, elapsedTime / animTime);
        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}

        yield return null;
    }

    public void setCoordinates(int x, int y)
    {
        myCoord = new Coordinate(x, y);
    }

    public Coordinate getCoordinates()
    {
        return myCoord;
    }

    public Coordinate GetCoordinatesCopy()
    {
        return new Coordinate(myCoord.getX(), myCoord.getY());
    }

    public void resetEffectiveConnectionMap()
    {
        effectiveConnections = new bool[4] { false, false, false, false };
    }

    public void UpdateZOrder()
    {
        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0.0001f * transform.position.y);
        transform.position = position;
    }

    //public void SetPlayerPosition()
    //{
    //    playerPosition = transform.position + new Vector3(0f, playerOffset, 0f);
    //}

    // Trap

    public bool GetIsTrapped()
    {
        return isTrapped;
    }

    public void SetIsTrapped(bool status)
    {
        isTrapped = status;
    }

    public void SetTrap(int playerNbr)
    {
        myTrap = Instantiate(trap, transform);
        myTrapComponent = myTrap.GetComponent<Trap>();
        myTrapComponent.SetPlayerDropping(playerNbr);
        myTrapComponent.SetCoordiantes(myCoord);
        isTrapped = true;
        // isTrapped = true; true whena active?
    }

    public Trap GetTrap()
    {
        return myTrapComponent;
    }

    public void SetTurnManager()
    {
        turnManager = transform.parent.gameObject.GetComponentInChildren<TurnManager>();
        //allPlayersComponent = turnManager.GetAllPayers();
    }

    // Unity Specific methods

    void Awake () {
        myRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
        possibleConnections = new bool[4];
        effectiveConnections = new bool[4];
        childPlayerNbr = -1;
        hasDiamond = false;

        //Debug.Log(possibleConnections.Length);
    }
	
	void Update () {
		
	}
}
