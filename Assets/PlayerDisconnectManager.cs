using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerDisconnectManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject notificationPanel; // Panel donde se mostrará el mensaje
    [SerializeField] private TMP_Text notificationText; // Texto del mensaje

    private void Start()
    {
        // Asegúrate de que el panel esté desactivado al inicio
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }
    [PunRPC]
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"El jugador {otherPlayer.NickName} se ha desconectado.");

        // Obtener el PlayerID del jugador que abandonó
        string playerID = otherPlayer.CustomProperties.ContainsKey("PlayerID")
            ? otherPlayer.CustomProperties["PlayerID"].ToString()
            : "Desconocido";

        // Mostrar el mensaje
        ShowDisconnectMessage(playerID);
    }

    [PunRPC]
    private void ShowDisconnectMessage(string playerID)
    {
        // Configurar el texto del mensaje
        if (notificationText != null)
        {
            notificationText.text = $"El jugador con ID {playerID} se ha desconectado.";
        }

        // Mostrar el panel
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(true);
        }

        // Ocultar el mensaje después de un tiempo
        Invoke(nameof(HideDisconnectMessage), 5f); // Oculta después de 5 segundos
    }

    [PunRPC]
    private void HideDisconnectMessage()
    {
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }
}
