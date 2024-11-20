using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int requiredPlayers = 2; // Número de jugadores necesarios para iniciar.
    private bool gameStarted = false;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Esperando a que se conecten jugadores...");
        }
    }

    // Se llama cuando un jugador entra en la sala.
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Jugador conectado: {newPlayer.NickName}. Total: {PhotonNetwork.CurrentRoom.PlayerCount}");

        // Solo el MasterClient verifica si hay suficientes jugadores.
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == requiredPlayers)
        {
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    // Se llama cuando un jugador sale de la sala.
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Jugador desconectado: {otherPlayer.NickName}. Total: {PhotonNetwork.CurrentRoom.PlayerCount}");

        if (!gameStarted)
        {
            Debug.Log("El juego no puede iniciar sin ambos jugadores.");
        }
    }

    [PunRPC]
    private void StartGame()
    {
        if (gameStarted) return; // Evita iniciar varias veces.
        gameStarted = true;
        Debug.Log("¡El juego comienza ahora!");

        // Lógica para iniciar el juego, por ejemplo, cargar una escena:
        // SceneManager.LoadScene("GameScene");
    }
}
