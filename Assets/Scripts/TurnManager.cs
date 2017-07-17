using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{

    public GameObject walkButton, terraformingButton, crystalButton, passButton, cursor, portraitSelection;
    public GameObject[] portraits;
    public Camera camera;

    private int playerPlaying, playerPlayingIdx, turnNbr, selectedButton;
    private int[] playerOrder;
    private Vector3[] buttonsPosition;
    private bool canMove, canTerraform, canUseCrystal, cursorIsActive;
    private GameObject[] players, tmpPlayers;
    private Player[] playerComponent;
    private RectTransform[] buttonsTransform;
    private RectTransform cursorTransform;
    private Animator[] buttonsAnimator;

    enum myButtons
    {
        Walk, Terraform, Crystal, Pass
    };

    public void AssignButtonsPosition()
    {
        buttonsTransform[0] = walkButton.GetComponent<RectTransform>();
        buttonsPosition[0] = buttonsTransform[0].position;
        buttonsTransform[1] = terraformingButton.GetComponent<RectTransform>();
        buttonsPosition[1] = buttonsTransform[1].position;
        buttonsTransform[2] = crystalButton.GetComponent<RectTransform>();
        buttonsPosition[2] = buttonsTransform[2].position;
        buttonsTransform[3] = passButton.GetComponent<RectTransform>();
        buttonsPosition[3] = buttonsTransform[3].position;
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

    void MoveCursor()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            selectedButton = Mathf.Clamp(selectedButton + 1, 0, 3);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedButton = Mathf.Clamp(selectedButton - 1, 0, 3);
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

        int numberOfButtons = 4;
        playerPlayingIdx = -1;
        selectedButton = 0;
        playerOrder = new int[4] { 1, 2, 3, 4 };
        buttonsPosition = new Vector3[numberOfButtons];
        buttonsTransform = new RectTransform[numberOfButtons];
        buttonsAnimator = new Animator[numberOfButtons];
        cursorTransform = cursor.GetComponent<RectTransform>();
        cursorIsActive = true;

        AssignButtonsAnimators();
        AssignButtonsPosition();

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
                else if (canTerraform && cursorTransform.position == buttonsTransform[1].position) // Terrafor
                {

                }
                else if (cursorTransform.position == buttonsTransform[3].position) // Pass
                {
                    PassTurn();
                }
            }
        }
    }
}
