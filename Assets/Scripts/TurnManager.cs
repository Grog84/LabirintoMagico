using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{

    public GameObject walkButton, terraformingButton, crystalButton, passButton, cursor;
    public Camera camera;

    private int playerPlaying, turnNbr;
    private int[] playerOrder;
    private Vector3[] buttonsPosition;
    private bool canMove, canTerraform, canUseCrystal;
    private GameObject[] players, tmpPlayers;
    private RectTransform[] buttonsTransform;
    private RectTransform cursorTransform;
    private Animator[] buttonsAnimator;

    enum myButtons
    {
        Walk, Terraform, Crystal, Pass
    };

    void AssignButtonsPosition()
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

    void AssignButtonsAnimators()
    {
        buttonsAnimator[0] = walkButton.GetComponent<Animator>();
        buttonsAnimator[1] = terraformingButton.GetComponent<Animator>();
        buttonsAnimator[2] = crystalButton.GetComponent<Animator>();
        buttonsAnimator[3] = passButton.GetComponent<Animator>();
    }

    void ArrangePlayersInTurnOrder()
    {
        // set it up to work with random order even though at the moment the playerOrder is predefined
        tmpPlayers = GetComponentInParent<MapManager>().allPlayers;
        players = new GameObject[4];

        //  here we should get also the component defining the plaer movement
        for (int i = 0; i < playerOrder.Length; i++)
        {
            players[playerOrder[i]] = tmpPlayers[i];
        }
    }

    void ResetButons()
    {
        canMove = true;
        canTerraform = true;
        canUseCrystal = false;
    }

    // Use this for initialization
    void Start()
    {

        int numberOfButtons = 4;
        playerOrder = new int[4] { 1, 2, 3, 4 };
        buttonsPosition = new Vector3[numberOfButtons];
        buttonsTransform = new RectTransform[numberOfButtons];
        buttonsAnimator = new Animator[numberOfButtons];
        cursorTransform = cursor.GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
