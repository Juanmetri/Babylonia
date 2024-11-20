using Photon.Pun;
using UnityEngine;

public class MoveCmd : ICommand
{
    private Transform playerTransform;
    private Vector3 direction;
    private float speed;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer
    private PhotonView pv;

    public MoveCmd(Transform playerTransform, Vector3 direction, float speed, Animator animator, PhotonView pv)
    {
        this.playerTransform = playerTransform;
        this.direction = direction;
        this.speed = speed;
        this.animator = animator;
        this.pv = pv;

        // Obtener el SpriteRenderer del personaje
        this.spriteRenderer = playerTransform.GetComponent<SpriteRenderer>();
    }

    [PunRPC]
    public void Execute()
    {
        playerTransform.position += direction * speed * Time.deltaTime;

        // Activar animación de correr
        pv.RPC("SetAnimatorBool", RpcTarget.All, "isRunning", true);

        // Cambiar dirección del sprite y ajustar el bulletSpawnPoint
        if (direction.x < 0 && !spriteRenderer.flipX)
        {
            pv.RPC("SetFlipX", RpcTarget.AllBuffered, true); // Girar hacia la izquierda
        }
        else if (direction.x > 0 && spriteRenderer.flipX)
        {
            pv.RPC("SetFlipX", RpcTarget.AllBuffered, false); // Girar hacia la derecha
        }
    }
}
