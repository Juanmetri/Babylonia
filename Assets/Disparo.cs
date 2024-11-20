using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Disparo : MonoBehaviour
{
    public int damage = 10;
    public float pushForce = 5f; // Fuerza de empuje
    private PhotonView ownerPhotonView;

    public void SetOwner(PhotonView owner)
    {
        ownerPhotonView = owner;
    }
    [PunRPC]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView targetView = collision.GetComponent<PhotonView>();
            if (targetView != null && !targetView.IsMine) // Solo afecta a otros jugadores
            {
                // Aplicar daño
                targetView.RPC("TakeDamage", targetView.Owner, damage);

                // Empujar al jugador
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized; // Dirección del empuje
                targetView.RPC("ApplyPush", targetView.Owner, pushDirection, pushForce);
            }

            Destroy(gameObject); // Destruir la bala tras la colisión
        }
    }
}