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
        // RPC para sincronizar el disparo en todos los clientes
        playerPhotonView.RPC("ExecuteShoot", RpcTarget.All, disparoPrefabName, spawnPosition, shootDirection, bulletSpeed);
    }
}
