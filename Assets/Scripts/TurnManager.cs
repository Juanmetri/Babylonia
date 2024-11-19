using System.Collections;
using UnityEngine;
using Photon.Pun;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager Instance;

    public float turnDuration = 10f; // Time each player has per turn
    private float currentTurnTime;

    private int currentPlayerTurn; // Index of the current player
    private bool isGameStarted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    private void Update()
    {
        if (!isGameStarted) return;

        // Update timer only on the master client
        if (PhotonNetwork.IsMasterClient)
        {
            currentTurnTime -= Time.deltaTime;

            if (currentTurnTime <= 0)
            {
                photonView.RPC("EndTurn", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void StartGame()
    {
        isGameStarted = true;
        currentPlayerTurn = 0;
        currentTurnTime = turnDuration;
    }

    [PunRPC]
    public void EndTurn()
    {
        currentTurnTime = turnDuration;
        currentPlayerTurn = (currentPlayerTurn + 1) % PhotonNetwork.CurrentRoom.PlayerCount;

        photonView.RPC("UpdateTurn", RpcTarget.All, currentPlayerTurn);
    }

    [PunRPC]
    private void UpdateTurn(int newTurn)
    {
        currentPlayerTurn = newTurn;
        Debug.Log("Player " + currentPlayerTurn + "'s turn");
    }

    public bool IsCurrentPlayerTurn(int playerIndex)
    {
        return currentPlayerTurn == playerIndex;
    }
}
