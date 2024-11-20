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
        pv.RPC("SetAnimatorBool", RpcTarget.All, "isRunning", true);

        // Flip del sprite dependiendo de la dirección de movimiento
        if (direction.x < 0)
            spriteRenderer.flipX = true;  // Mirar a la izquierda
        else if (direction.x > 0)
            spriteRenderer.flipX = false; // Mirar a la derecha
    }
}
