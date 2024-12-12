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

    // Método para determinar al ganador y mostrar la pantalla de ganador
    public void DetermineWinner()
    {
        Player winner = null;
        int highestHealth = -1;

        // Iterar por todos los jugadores en la sala para encontrar al que tiene más vida
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("Health"))
            {
                int playerHealth = (int)player.CustomProperties["Health"];
                if (playerHealth > highestHealth)
                {
                    highestHealth = playerHealth;
                    winner = player;
                }
            }
        }

        // Si se encuentra un ganador, mostrar la pantalla de ganador
        if (winner != null)
        {
            string winnerPlayerID = winner.CustomProperties.ContainsKey("PlayerID")
                ? winner.CustomProperties["PlayerID"].ToString()
                : "UnknownPlayer";

            // Detener el tiempo de juego
            Time.timeScale = 0;

            // Enviar un RPC para mostrar la pantalla del ganador en ambos clientes
            photonView.RPC("ShowWinnerScreen", RpcTarget.All, winnerPlayerID);
        }
        else
        {
            Debug.LogError("No se pudo determinar el ganador.");
        }
    }

    // RPC para mostrar la pantalla de ganador sincronizadamente
    [PunRPC]
    public void ShowWinnerScreen(string winnerPlayerID)
    {
        // Mostrar la pantalla del ganador
        if (winnerScreen != null && winnerText != null)
        {
            winnerScreen.SetActive(true);
            winnerText.text = $"¡Ganador!\nID: {winnerPlayerID}";
        }

        // Detener el tiempo de juego
        Time.timeScale = 0;
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
