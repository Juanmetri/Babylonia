using UnityEngine;
using Photon.Pun;

public class Disparo : MonoBehaviour
{
    public int damage = 10;
    public int chargedDamage = 30; // Damage for the charged shot
    public float pushForce = 5f;
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
            if (targetView != null && targetView.IsMine)
            {
                // Determinar el daño según el tipo de disparo
                int finalDamage = gameObject.name.Contains("Charged") ? chargedDamage : damage;

                // Aplicar daño al jugador
                targetView.RPC("TakeDamage", RpcTarget.All, finalDamage);

                // Empujar al jugador
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                targetView.RPC("ApplyPush", RpcTarget.All, pushDirection, pushForce);
            }
        }

        // Destruir el disparo si el cliente tiene permiso
        if (PhotonNetwork.IsMasterClient || ownerPhotonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("El cliente no tiene permiso para destruir este objeto.");
        }
    }
}
