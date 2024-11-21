using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerStatusText; // Texto para mostrar el estado de los jugadores
    [SerializeField] private TMP_Text roomStatusText;   // Texto para mostrar el estado global de la sala
    [SerializeField] private GameObject readyButton;    // Bot�n "Estoy Listo"
    [SerializeField] private GameObject startButton;    // Bot�n "Iniciar Partida" (solo visible para MasterClient)

    private const string RoomReadyKey = "AllPlayersReady"; // Clave para la propiedad global de la sala

    private void Start()
    {
        // Mostrar el bot�n "Iniciar Partida" solo al MasterClient
        startButton.SetActive(PhotonNetwork.IsMasterClient);

        // Actualizar el estado inicial de los jugadores
        UpdatePlayerStatusText();
    }

    public void OnReadyButtonClicked()
    {
        // Establecer al jugador como "listo" en las Custom Properties
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Ready", true } });

        readyButton.SetActive(false); // Desactivar el bot�n una vez marcado como listo
    }

    public void OnStartGameButtonClicked()
    {
        // Solo el MasterClient puede iniciar la partida
        if (PhotonNetwork.IsMasterClient)
        {
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // Si cambia la propiedad "Ready", actualizamos el estado
        if (changedProps.ContainsKey("Ready"))
        {
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
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            bool isReady = player.CustomProperties.ContainsKey("Ready") && (bool)player.CustomProperties["Ready"];
            status += $"{player.NickName}: {(isReady ? "Listo" : "No Listo")}\n";
        }
        playerStatusText.text = status;
    }
}
