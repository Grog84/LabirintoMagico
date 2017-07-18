using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{

    public GameObject walkButton, terraformingButton, crystalButton, passButton, cursor, portraitSelection,
        rotateTileButton, slideTilesButton, terraformBackButton, cardsCursor;
    public GameObject[] portraits;
    public GameObject[] panels;
    public Camera camera;

    private int playerPlaying, playerPlayingIdx, turnNbr, selectedButton, selectionDepth;
    private int[] playerOrder;
    private Vector3[] buttonsPosition;
    private bool canMove, canTerraform, canUseCrystal, cursorIsActive;
    private GameObject[] players, tmpPlayers;
    private Player[] playerComponent;
    private RectTransform[] buttonsTransform, panelsTransform;
    private RectTransform[][] cardsTransform;
    private RectTransform cursorTransform;
    private Animator[] buttonsAnimator;

    enum myButtons
    {
        Walk, Terraform, Crystal, Pass
    };

    public void AssignPanelsPosition()
    {
        panelsTransform[0] = panels[0].GetComponent<RectTransform>();
        panelsTransform[1] = panels[1].GetComponent<RectTransform>();
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
    }

    public void AssignCardsPosition()
    { 
        for (int i = 0; i < portraits.Length; i++)
        {
            cardsTransform[i] = portraits[i].GetComponentsInChildren<RectTransform>();
        }
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

    public IEnumerator MoveCamera(GameObject player)
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

    void ActivateTerraformPanel()
    {
        selectionDepth = 1;
        selectedButton = 4;
        panelsTransform[0].anchoredPosition = new Vector2(0, -50);
        panelsTransform[1].anchoredPosition = new Vector2(0, 25);
        cursorTransform.position = buttonsTransform[4].position;
    }

    void ActivateBasePanel()
    {
        selectionDepth = 0;
        selectedButton = 0;
        cursorTransform.position = buttonsTransform[0].position;
        panelsTransform[0].anchoredPosition = new Vector2(0, 25);
        panelsTransform[1].anchoredPosition = new Vector2(0, -50);
    }

    void ActivateCardSelection()
    {
        selectionDepth = 2;

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
                selectedButton = Mathf.Clamp(selectedButton + 1, 4, 6);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                selectedButton = Mathf.Clamp(selectedButton - 1, 4, 6);
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

        StartCoroutine(MoveCamera(players[playerPlayingIdx]));  

    }

    IEnumerator ActivateMovementPhase()
    {
        ActivatePlayer(playerComponent[playerPlayingIdx].playerNbr);
        buttonsAnimator[0].SetBool("isActive", false);

        yield return null;

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        cursorIsActive = true;
        ActivatePlayer(0);

        yield return null;
    }
    
    // Use this for initialization
    void Awake()
    {
        selectionDepth = 0; // corresponds to the first panel. 1 is the terraform panel. 2 is the card selection
        int numberOfButtons = 7, nbrOfPanels = 2;
        playerPlayingIdx = -1;
        selectedButton = 0;
        playerOrder = new int[4] { 1, 2, 3, 4 };
        buttonsPosition = new Vector3[numberOfButtons];
        buttonsTransform = new RectTransform[numberOfButtons];
        buttonsAnimator = new Animator[numberOfButtons];
        panelsTransform = new RectTransform[nbrOfPanels];
        cursorTransform = cursor.GetComponent<RectTransform>();
        cursorIsActive = true;

        // Assigns the position of ll the ui elements o the corresponding arrays
        AssignButtonsAnimators();
        AssignButtonsPosition();
        AssignPanelsPosition();
        AssignCardsPosition();

    }


    // Update is called once per frame
    void Update()
    {
        if (cursorIsActive)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
                MoveCursor();

            if (Input.GetKeyDown(KeyCode.Space))
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
                    
                }
                else if (cursorTransform.position == buttonsTransform[5].position) // Slide
                {

                }
                else if (cursorTransform.position == buttonsTransform[6].position) // Back to starting Panel
                {
                    ActivateBasePanel();
                }
            }
        }
    }
}
