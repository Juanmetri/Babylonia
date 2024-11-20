using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro; // Importación para TextMeshPro

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject startButton; // Botón para iniciar la partida
    [SerializeField] private TMP_Text statusText; // Texto para mostrar el estado de la sala
    [SerializeField] private TMP_Text playerStatusText; // Texto para mostrar el estado de cada jugador

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true); // El MasterClient puede iniciar la partida
        }
        else
        {
            startButton.SetActive(false); // Otros jugadores no pueden iniciar
        }
    }

    public void OnReadyButtonClicked()
    {
        // Marcar al jugador actual como listo
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Ready", true } });
    }
    [PunRPC]
    public void OnStartGameButtonClicked()
    {
        // El MasterClient verifica si todos los jugadores están listos
        if (PhotonNetwork.IsMasterClient && AllPlayersReady())
        {
            // Sincronizar el cambio de escena en todos los jugadores
            PhotonNetwork.LoadLevel("GamePlay");
        }
        else
        {
            statusText.text = "No todos los jugadores están listos.";
        }
    }

    private bool AllPlayersReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Si algún jugador no tiene la propiedad "Ready" o no está listo, devolvemos falso
            if (!player.CustomProperties.ContainsKey("Ready") || !(bool)player.CustomProperties["Ready"])
            {
                return false;
            }
        }
        return true;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // Si la propiedad "Ready" cambia, actualizamos el texto
        if (changedProps.ContainsKey("Ready"))
        {
            UpdatePlayerStatusText();
        }
    }

    private void UpdatePlayerStatusText()
    {
        // Actualizar el texto del estado de los jugadores
        string playerStatus = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            bool isReady = player.CustomProperties.ContainsKey("Ready") && (bool)player.CustomProperties["Ready"];
            playerStatus += $"{player.NickName}: {(isReady ? "Listo" : "No Listo")}\n";
        }
        playerStatusText.text = playerStatus;
    }
}
