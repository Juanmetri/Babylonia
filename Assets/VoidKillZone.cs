using UnityEngine;
using Photon.Pun;

public class VoidKillZone : MonoBehaviour
{
    private GameOverManager gameOverManager; // Referencia al GameOverManager

    private void Start()
    {
        // Encuentra el objeto GameOverManager en la escena
        gameOverManager = FindObjectOfType<GameOverManager>();

        // Verifica que el GameOverManager esté configurado correctamente
        if (gameOverManager == null)
        {
            Debug.LogError("GameOverManager no encontrado en la escena.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si el objeto que entra en el trigger es un jugador
        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                // Determinar al ganador
                string winnerName = "";
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (player.NickName != PhotonNetwork.NickName)
                    {
                        winnerName = player.NickName; // Encuentra el nombre del otro jugador
                    }
                }

                // Mostrar pantalla de ganador sincronizadamente
                if (gameOverManager != null)
                {
                    gameOverManager.photonView.RPC("ShowWinnerScreen", RpcTarget.All, winnerName);
                }
                else
                {
                    Debug.LogError("GameOverManager no está configurado.");
                }

      
            }
        }
        else
        {
            Debug.Log($"Objeto que cayó al vacío: {other.name} (No es un jugador)");
        }
    }
}
