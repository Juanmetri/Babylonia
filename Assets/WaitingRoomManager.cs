using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text playerListText;
    [SerializeField] private Button readyButton;

    private bool isReady = false;

    void Start()
    {
        // Configura el botón "Listo".
        readyButton.onClick.AddListener(() =>
        {
            isReady = !isReady;
            SetReadyState(isReady);
        });

        UpdatePlayerList(); // Muestra la lista inicial.
    }

    private void UpdatePlayerList()
    {
        playerListText.text = ""; // Limpia la lista.
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            bool ready = player.CustomProperties.ContainsKey("Ready") && (bool)player.CustomProperties["Ready"];
            playerListText.text += $"{player.NickName} - {(ready ? "Listo" : "No Listo")}\n";
        }
    }

    public void SetReadyState(bool ready)
    {
        // Obtenemos las propiedades actuales del jugador
        var playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

        // Si el valor de "Ready" es distinto al que se quiere establecer, lo actualizamos
        if (!playerProperties.ContainsKey("Ready") || (bool)playerProperties["Ready"] != ready)
        {
            playerProperties["Ready"] = ready; // Establecemos la propiedad "Ready" del jugador
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties); // Sincronizamos la propiedad
        }

        // Cambiamos el texto del botón para que refleje el estado
        readyButton.GetComponentInChildren<Text>().text = ready ? "Cancelar" : "Listo";

        // Verificamos si todos los jugadores están listos
        CheckAllPlayersReady();
    }

    void CheckAllPlayersReady()
    {
        // Recorremos todos los jugadores en la sala
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            bool ready = false;
            if (player.CustomProperties.ContainsKey("Ready"))
            {
                ready = (bool)player.CustomProperties["Ready"];
            }
            else
            {
                Debug.LogWarning($"El jugador {player.NickName} no tiene configurada la propiedad 'Ready'.");
            }

            if (!ready)
            {
                Debug.Log("No todos los jugadores están listos.");
                return; // Si algún jugador no está listo, salimos del método
            }
        }

        // Si todos los jugadores están listos, iniciamos el juego
        StartGameplay();
    }

    [PunRPC]
    private void StartGameplay()
    {
        // Cambia a la escena de Gameplay.
        PhotonNetwork.LoadLevel("Gameplay");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // Actualiza la lista de jugadores si cambian las propiedades.
        if (changedProps.ContainsKey("Ready"))
        {
            UpdatePlayerList();
            CheckAllPlayersReady();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
}
