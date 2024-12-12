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
                // Sincronizar la vida a 0 para el jugador que cae al vacío
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                {
                    { "Health", 0 }
                });

                // Llamar al GameOverManager para determinar el ganador
                if (gameOverManager != null)
                {
                    gameOverManager.DetermineWinner(); // Lógica para determinar el ganador
                }
                else
                {
                    Debug.LogError("GameOverManager no está configurado.");
                }

                // Destruir el objeto del jugador que cayó
                PhotonNetwork.Destroy(other.gameObject);
            }
        }
        else
        {
            Debug.Log($"Objeto que cayó al vacío: {other.name} (No es un jugador)");
        }
    }
}
