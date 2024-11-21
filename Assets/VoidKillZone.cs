using UnityEngine;
using Photon.Pun;

public class VoidKillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si el objeto que entra en el trigger es un jugador
        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                // Destruir el jugador sincronizadamente
                PhotonNetwork.Destroy(other.gameObject);
                Debug.Log("Jugador cayó al vacío y fue destruido.");
            }
        }
        else
        {
            Debug.Log($"Objeto que cayó al vacío: {other.name} (No es un jugador)");
        }
    }
}
