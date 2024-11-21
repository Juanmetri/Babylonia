using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Para usar TextMeshPro
using Photon.Pun;

public class GameOverManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject winnerScreen; // Pantalla de ganador
    [SerializeField] private TMP_Text winnerText; // Texto para mostrar el ganador

    private void Start()
    {
        // Asegurarte de que la pantalla de ganador est� desactivada al inicio
        if (winnerScreen != null)
        {
            winnerScreen.SetActive(false);
        }
    }

    // Llamado cuando un jugador muere
    [PunRPC]
    public void ShowWinnerScreen(string winnerName)
    {
        // Detener el tiempo
        Time.timeScale = 0;

        // Mostrar la pantalla de ganador en ambos jugadores
        if (winnerScreen != null && winnerText != null)
        {
            winnerScreen.SetActive(true);
            winnerText.text = $"{winnerName} Gan� la partida!";
        }
    }

    // M�todo para volver al men� principal
    public void ReturnToMenu()
    {
        // Reactivar el tiempo
        Time.timeScale = 1;

        // Desconectar y volver al men�
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu arranque");
    }
}
