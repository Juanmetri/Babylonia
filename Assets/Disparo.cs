using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Disparo : MonoBehaviour
{
    public int damage = 10;
    private PhotonView ownerPhotonView;

    public void SetOwner(PhotonView owner)
    {
        ownerPhotonView = owner;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView targetView = collision.GetComponent<PhotonView>();
            if (targetView != null && !targetView.IsMine) // Solo aplica da�o si el jugador no es el due�o del PhotonView.
            {
                targetView.RPC("TakeDamage", RpcTarget.Others, damage); // Aplica da�o al propietario del jugador impactado.
            }

            Destroy(gameObject); // Destruye la bala tras la colisi�n.
        }
    }

}