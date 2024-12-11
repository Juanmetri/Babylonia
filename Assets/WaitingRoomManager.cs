using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerStatusText; // Text to show player statuses
    [SerializeField] private TMP_Text roomStatusText;   // Text to show room status
    [SerializeField] private GameObject readyButton;    // "Ready" button
    [SerializeField] private GameObject startButton;    // "Start Game" button (only visible to MasterClient)

    private string playerID;

    private const string RoomReadyKey = "AllPlayersReady"; // Key for global room property

    private void Start()
    {
        // Retrieve the saved player ID from PlayerPrefs
        playerID = PlayerPrefs.GetString("PlayerID", "DefaultPlayerID");
        Debug.Log($"Player ID retrieved in WaitingRoomManager: {playerID}");

        // Show the "Start Game" button only to the MasterClient
        startButton.SetActive(PhotonNetwork.IsMasterClient);

        // Update the initial player status display
        UpdatePlayerStatusText();
    }

    public void OnReadyButtonClicked()
    {
        // Set the player as "ready" and include their PlayerID in custom properties
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "Ready", true },
            { "PlayerID", playerID }
        });

        // Disable the ready button after marking the player as ready
        readyButton.SetActive(false);
    }

    public void OnStartGameButtonClicked()
    {
        // Only the MasterClient can start the game
        if (PhotonNetwork.IsMasterClient)
        {
            // Ensure exactly 2 players are in the room
            if (PhotonNetwork.PlayerList.Length != 2)
            {
                roomStatusText.text = "Debe haber exactamente 2 jugadores para iniciar la partida.";
                return;
            }

            // Check if all players are ready
            if (CheckAllPlayersReady())
            {
                // Set the room property to signal that the game should start
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { RoomReadyKey, true } });
            }
            else
            {
                roomStatusText.text = "No todos los jugadores están listos.";
            }
        }
    }

    private bool CheckAllPlayersReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("Ready") || !(bool)player.CustomProperties["Ready"])
            {
                return false; // If any player is not ready, return false
            }
        }
        return true;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // If either the "Ready" or "PlayerID" property changes
        if (changedProps.ContainsKey("Ready") || changedProps.ContainsKey("PlayerID"))
        {
            // Refresh the player status text to reflect the updated properties
            UpdatePlayerStatusText();

            // If the MasterClient detects that all players are ready, update the room status
            if (PhotonNetwork.IsMasterClient && CheckAllPlayersReady())
            {
                roomStatusText.text = "Todos los jugadores están listos. Puedes iniciar la partida.";
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // If the room is ready to start, load the gameplay scene
        if (propertiesThatChanged.ContainsKey(RoomReadyKey) && (bool)propertiesThatChanged[RoomReadyKey])
        {
            PhotonNetwork.LoadLevel("GamePlay"); // Synchronize the new scene load
        }
    }

    private void UpdatePlayerStatusText()
    {
        string status = "Estado de los jugadores:\n";

        // Iterate through all players in the room
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Check if the player is ready
            bool isReady = player.CustomProperties.ContainsKey("Ready") && (bool)player.CustomProperties["Ready"];

            // Retrieve the PlayerID from the custom properties
            string retrievedPlayerID = player.CustomProperties.ContainsKey("PlayerID")
                ? player.CustomProperties["PlayerID"].ToString()
                : "UnknownPlayer";

            // Add the player's ID, nickname, and readiness status to the display
            status += $"ID:{retrievedPlayerID}-{player.NickName}:{(isReady ? "Listo" : "No Listo")}\n";
        }

        // Update the status text UI element
        playerStatusText.text = status;
    }
}
