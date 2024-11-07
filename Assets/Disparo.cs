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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PhotonView pv = collision.gameObject.GetComponent<PhotonView>();
        // Ignora colisión si es el propietario
        if (pv != null && pv == ownerPhotonView) return;

        if (pv != null && collision.gameObject.CompareTag("Player"))
        {
            if (PhotonView.Get(this).IsMine)
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            }

            if (PhotonView.Get(this).IsMine || PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            if (PhotonView.Get(this).IsMine || PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
