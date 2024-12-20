using UnityEngine;
using Photon.Pun;

public class LavaDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si el objeto que toca la lava es el jugador
        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                // Destruir el jugador sincronizadamente
                PhotonNetwork.Destroy(other.gameObject);
                Debug.Log("�Jugador toc� la lava y fue destruido!");
            }
        }
        else
        {
            Debug.Log($"Objeto que toc� la lava: {other.name} (No es un jugador)");
        }
    }
}
