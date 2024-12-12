using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerStatusText; // Texto para mostrar el estado de los jugadores
    [SerializeField] private TMP_Text roomStatusText;   // Texto para mostrar mensajes globales en la sala
    [SerializeField] private GameObject readyButton;    // Bot�n "Estoy Listo"
    [SerializeField] private GameObject startButton;    // Bot�n "Iniciar Partida" (solo visible para MasterClient)

    private string playerID;

    private const string RoomReadyKey = "AllPlayersReady"; // Clave para la propiedad global de la sala

    private void Start()
    {
        // Recuperar el ID del jugador guardado en PlayerPrefs
        playerID = PlayerPrefs.GetString("PlayerID", "DefaultPlayerID");
        Debug.Log($"Player ID retrieved in WaitingRoomManager: {playerID}");

        // Mostrar el bot�n "Iniciar Partida" solo al MasterClient
        startButton.SetActive(PhotonNetwork.IsMasterClient);

        // Actualizar el estado inicial de los jugadores
        UpdatePlayerStatusText();
    }

    public void OnReadyButtonClicked()
    {
        // Establecer al jugador como "listo" y guardar su PlayerID en las propiedades personalizadas
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "Ready", true },
            { "PlayerID", playerID }
        });

        // Desactivar el bot�n "Listo" despu�s de marcar al jugador como listo
        readyButton.SetActive(false);
    }

    public void OnStartGameButtonClicked()
    {
        // Solo el MasterClient puede iniciar la partida
        if (PhotonNetwork.IsMasterClient)
        {
            // Verificar que haya exactamente 2 jugadores en la sala
            if (PhotonNetwork.PlayerList.Length != 2)
            {
                roomStatusText.text = "Debe haber exactamente 2 jugadores para iniciar la partida.";
                return;
            }

            // Verificar si todos los jugadores est�n listos
            if (CheckAllPlayersReady())
            {
                // Establecer la propiedad global de la sala para iniciar el juego
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { RoomReadyKey, true } });
            }
            else
            {
                roomStatusText.text = "No todos los jugadores est�n listos.";
            }
        }
    }

    private bool CheckAllPlayersReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("Ready") || !(bool)player.CustomProperties["Ready"])
            {
                return false; // Si alg�n jugador no est� listo, devolvemos falso
            }
        }
        return true;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Obtener el ID del jugador que se uni�
        string playerID = newPlayer.CustomProperties.ContainsKey("PlayerID")
            ? newPlayer.CustomProperties["PlayerID"].ToString()
            : "UnknownPlayer";

        // Mostrar el mensaje
        roomStatusText.text = $"El jugador {playerID} se ha unido a la sala.";

        // Actualizar el estado de los jugadores
        UpdatePlayerStatusText();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Obtener el ID del jugador que abandon�
        string playerID = otherPlayer.CustomProperties.ContainsKey("PlayerID")
            ? otherPlayer.CustomProperties["PlayerID"].ToString()
            : "UnknownPlayer";

        // Mostrar el mensaje
        roomStatusText.text = $"El jugador {playerID} ha abandonado la sala.";

        // Actualizar el estado de los jugadores
        UpdatePlayerStatusText();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // Si cambian las propiedades "Ready" o "PlayerID"
        if (changedProps.ContainsKey("Ready") || changedProps.ContainsKey("PlayerID"))
        {
            // Actualizar el estado de los jugadores
            UpdatePlayerStatusText();

            // Si el MasterClient detecta que todos est�n listos, habilita la l�gica de inicio
            if (PhotonNetwork.IsMasterClient && CheckAllPlayersReady())
            {
                roomStatusText.text = "Todos los jugadores est�n listos. Puedes iniciar la partida.";
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // Si la sala est� lista para comenzar, inicia el juego
        if (propertiesThatChanged.ContainsKey(RoomReadyKey) && (bool)propertiesThatChanged[RoomReadyKey])
        {
            PhotonNetwork.LoadLevel("GamePlay"); // Sincroniza la carga de la nueva escena
        }
    }

    private void UpdatePlayerStatusText()
    {
        string status = "Estado de los jugadores:\n";

        // Iterar por todos los jugadores en la sala
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Obtener si el jugador est� listo
            bool isReady = player.CustomProperties.ContainsKey("Ready") && (bool)player.CustomProperties["Ready"];

            // Obtener el PlayerID del jugador
            string retrievedPlayerID = player.CustomProperties.ContainsKey("PlayerID")
                ? player.CustomProperties["PlayerID"].ToString()
                : "UnknownPlayer";

            // A�adir el estado del jugador a la UI
            status += $"{retrievedPlayerID} ({player.NickName}): {(isReady ? "Listo" : "No Listo")}\n";
        }

        // Actualizar el texto en la UI
        playerStatusText.text = status;
    }
}
