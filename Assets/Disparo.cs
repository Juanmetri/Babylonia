using UnityEngine;
using Photon.Pun;

public class Disparo : MonoBehaviour
{
    public int damage = 10;
    public int chargedDamage = 30; // Damage for the charged shot
    public float pushForce = 5f;
    private PhotonView ownerPhotonView;

    [PunRPC]
    public void SetOwner(int viewID)
    {
        PhotonView ownerView = PhotonView.Find(viewID);
        if (ownerView != null)
        {
            ownerPhotonView = ownerView;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView targetView = collision.GetComponent<PhotonView>();
            if (targetView != null)
            {
                // Determinar el daño según el tipo de disparo
                int finalDamage = gameObject.name.Contains("Charged") ? chargedDamage : damage;

                // Aplicar daño al jugador usando RPC
                targetView.RPC("TakeDamage", RpcTarget.AllBuffered, finalDamage);

                // Calcular la dirección del empuje
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                targetView.RPC("ApplyPush", RpcTarget.AllBuffered, pushDirection, pushForce);
            }
        }

        // Destruir el disparo desde el cliente que lo creó
        if (PhotonNetwork.IsMasterClient || (ownerPhotonView != null && ownerPhotonView.IsMine))
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("El cliente no tiene permiso para destruir este objeto.");
        }


    }
}
