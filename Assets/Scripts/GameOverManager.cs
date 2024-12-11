using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Para usar TextMeshPro
using Photon.Pun;
using Photon.Realtime;

public class GameOverManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject winnerScreen; // Pantalla de ganador
    [SerializeField] private TMP_Text winnerText; // Texto para mostrar el ganador

    private void Start()
    {
        // Asegurarte de que la pantalla de ganador esté desactivada al inicio
        if (winnerScreen != null)
        {
            winnerScreen.SetActive(false);
        }
    }

    // Llamado cuando un jugador muere
    [PunRPC]
    public void ShowWinnerScreen(string winnerName, int winnerActorNumber)
    {
        // Retrieve the winner's PlayerID
        string winnerPlayerID = "UnknownPlayer"; // Default if no ID is found

        // Search for the winner in PhotonNetwork's PlayerList
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == winnerActorNumber) // Match by ActorNumber for accuracy
            {
                winnerPlayerID = player.CustomProperties.ContainsKey("PlayerID")
                    ? player.CustomProperties["PlayerID"].ToString()
                    : "UnknownPlayer";
                break;
            }
        }

        // Detener el tiempo
        Time.timeScale = 0;

        // Mostrar la pantalla de ganador en ambos jugadores
        if (winnerScreen != null && winnerText != null)
        {
            winnerScreen.SetActive(true);
            winnerText.text = $"ID: {winnerPlayerID} - {winnerName} Ganó la partida!";
        }
    }

    // Método para volver al menú principal
    public void ReturnToMenu()
    {
        // Reactivar el tiempo
        Time.timeScale = 1;

        // Desconectar y volver al menú
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu arranque");
    }
}
