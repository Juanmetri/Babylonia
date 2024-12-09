using UnityEngine;
using Photon.Pun;

public class ShootCmd : ICommand
{
    private string disparoPrefabName;
    private Vector3 spawnPosition;
    private Vector2 shootDirection;
    private float bulletSpeed;
    private PhotonView playerPhotonView;

    public ShootCmd(string disparoPrefabName, Vector3 spawnPosition, Vector2 shootDirection, float bulletSpeed, PhotonView playerPhotonView)
    {
        this.disparoPrefabName = disparoPrefabName;
        this.spawnPosition = spawnPosition;
        this.shootDirection = shootDirection;
        this.bulletSpeed = bulletSpeed;
        this.playerPhotonView = playerPhotonView;
    }

    [PunRPC]
    public void Execute()
    {
        if (!playerPhotonView.IsMine) return;

        Debug.Log($"ShootCmd ejecutado por {PhotonNetwork.NickName} en {Time.time}");

        // Instanciar disparo y configurar el PhotonView correctamente
        GameObject disparo = PhotonNetwork.Instantiate(disparoPrefabName, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = disparo.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = shootDirection * bulletSpeed;
        }

        PhotonView disparoView = disparo.GetComponent<PhotonView>();
        if (disparoView != null)
        {
            disparoView.RPC("SetOwner", RpcTarget.AllBuffered, playerPhotonView.ViewID);
        }
    }
}
