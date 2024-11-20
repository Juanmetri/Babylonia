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

    [PunRPC]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView targetView = collision.GetComponent<PhotonView>();
            if (targetView != null && !targetView.IsMine)
            {
                int finalDamage = gameObject.name.Contains("Charged") ? chargedDamage : damage;

                targetView.RPC("TakeDamage", targetView.Owner, finalDamage);

                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                targetView.RPC("ApplyPush", targetView.Owner, pushDirection, pushForce);
            }

            Destroy(gameObject);
        }
    }
}
