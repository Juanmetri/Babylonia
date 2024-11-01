using UnityEngine;
using Photon.Pun;

public class ShootCmd : ICommand
{
    private GameObject disparoPrefab;
    private Transform bulletSpawnPoint;
    private float bulletSpeed;

    public ShootCmd(GameObject disparoPrefab, Transform bulletSpawnPoint, float bulletSpeed)
    {
        this.disparoPrefab = disparoPrefab;
        this.bulletSpawnPoint = bulletSpawnPoint;
        this.bulletSpeed = bulletSpeed;
    }

    public void Execute()
    {
        GameObject disparo = PhotonNetwork.Instantiate(disparoPrefab.name, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody2D rb = disparo.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = bulletSpawnPoint.right * bulletSpeed;
        }
    }
}