using UnityEngine;

public class MoveCmd : ICommand
{
    private Transform playerTransform;
    private Vector3 direction;
    private float speed;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer

    public MoveCmd(Transform playerTransform, Vector3 direction, float speed, Animator animator)
    {
        this.playerTransform = playerTransform;
        this.direction = direction;
        this.speed = speed;
        this.animator = animator;

        // Obtener el SpriteRenderer del personaje
        this.spriteRenderer = playerTransform.GetComponent<SpriteRenderer>();
    }

    public void Execute()
    {
        playerTransform.position += direction * speed * Time.deltaTime;
        animator.SetBool("isRunning", true);

        // Flip del sprite dependiendo de la dirección de movimiento
        if (direction.x < 0)
            spriteRenderer.flipX = true;  // Mirar a la izquierda
        else if (direction.x > 0)
            spriteRenderer.flipX = false; // Mirar a la derecha
    }
}
