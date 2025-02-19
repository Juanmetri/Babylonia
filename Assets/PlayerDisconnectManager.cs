using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerDisconnectManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TMP_Text notificationText;

    private void Start()
    {
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
    }
    [PunRPC]
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"El jugador {otherPlayer.NickName} se ha desconectado.");

        //ID del jugador que abandonó
        string playerID = otherPlayer.CustomProperties.ContainsKey("PlayerID")
            ? otherPlayer.CustomProperties["PlayerID"].ToString()
            : "Desconocido";
        //Muestra el mensaje
        ShowDisconnectMessage(playerID);
    }

    [PunRPC]
    private void ShowDisconnectMessage(string playerID)
    {
        if (notificationText != null)
        {
            notificationText.text = $"El jugador con ID {playerID} se ha desconectado.";
        }

        if (notificationPanel != null)
        {
            notificationPanel.SetActive(true);
        }

        Invoke(nameof(HideDisconnectMessage), 5f);
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
