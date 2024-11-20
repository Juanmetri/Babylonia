using UnityEngine;
using Photon.Pun;

public class ShootCmd : ICommand
{
    private GameObject disparoPrefab;
    private Transform bulletSpawnPoint;
    private float bulletSpeed;
    private PhotonView playerPhotonView;

    public ShootCmd(GameObject disparoPrefab, Transform bulletSpawnPoint, float bulletSpeed, PhotonView playerPhotonView)
    {
        this.disparoPrefab = disparoPrefab;
        this.bulletSpawnPoint = bulletSpawnPoint;
        this.bulletSpeed = bulletSpeed;
        this.playerPhotonView = playerPhotonView;
    }

    [PunRPC]
    public void Execute()
    {
        GameObject disparo = PhotonNetwork.Instantiate(disparoPrefab.name, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        disparo.GetComponent<Disparo>().SetOwner(playerPhotonView); // Set the owner

        Rigidbody2D rb = disparo.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = bulletSpawnPoint.right * bulletSpeed;
        }
    }
}
