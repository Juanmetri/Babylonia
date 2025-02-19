using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerStatusText; // Texto para mostrar el estado de los jugadores
    [SerializeField] private TMP_Text roomStatusText;   // Texto para mostrar mensajes globales en la sala
    [SerializeField] private GameObject readyButton;    // Botón "Estoy Listo"
    [SerializeField] private GameObject startButton;    // Botón "Iniciar Partida" (solo visible para MasterClient)

    private string playerID;

    private const string RoomReadyKey = "AllPlayersReady";

    private void Start()
    {
        playerID = PlayerPrefs.GetString("PlayerID", "DefaultPlayerID");
        Debug.Log($"Player ID retrieved in WaitingRoomManager: {playerID}");
        startButton.SetActive(PhotonNetwork.IsMasterClient);
        UpdatePlayerStatusText();
    }

    public void OnReadyButtonClicked()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "Ready", true },
            { "PlayerID", playerID }
        });
        readyButton.SetActive(false);
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.PlayerList.Length != 2)
            {
                roomStatusText.text = "Debe haber exactamente 2 jugadores para iniciar la partida.";
                return;
            }
            if (CheckAllPlayersReady())
            {
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
                return false;
            }
        }
        return true;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string playerID = newPlayer.CustomProperties.ContainsKey("PlayerID")
            ? newPlayer.CustomProperties["PlayerID"].ToString()
            : "UnknownPlayer";
        roomStatusText.text = $"El jugador {playerID} se ha unido a la sala.";
        UpdatePlayerStatusText();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string playerID = otherPlayer.CustomProperties.ContainsKey("PlayerID")
            ? otherPlayer.CustomProperties["PlayerID"].ToString()
            : "UnknownPlayer";
        roomStatusText.text = $"El jugador {playerID} ha abandonado la sala.";
        UpdatePlayerStatusText();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready") || changedProps.ContainsKey("PlayerID"))
        {
            UpdatePlayerStatusText();
            if (PhotonNetwork.IsMasterClient && CheckAllPlayersReady())
            {
                roomStatusText.text = "Todos los jugadores están listos. Puedes iniciar la partida.";
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(RoomReadyKey) && (bool)propertiesThatChanged[RoomReadyKey])
        {
            PhotonNetwork.LoadLevel("GamePlay");
        }
    }

    private void UpdatePlayerStatusText()
    {
        string status = "Estado de los jugadores:\n";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            bool isReady = player.CustomProperties.ContainsKey("Ready") && (bool)player.CustomProperties["Ready"];
            string retrievedPlayerID = player.CustomProperties.ContainsKey("PlayerID")
                ? player.CustomProperties["PlayerID"].ToString()
                : "UnknownPlayer";
            status += $"{retrievedPlayerID} ({player.NickName}): {(isReady ? "Listo" : "No Listo")}\n";
        }
        playerStatusText.text = status;
    }
}
