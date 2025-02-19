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
        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                {
                    { "Health", 0 }
                });
                if (gameOverManager != null)
                {
                    gameOverManager.DetermineWinner();
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
