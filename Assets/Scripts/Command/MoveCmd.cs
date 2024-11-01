using UnityEngine;

public class MoveCmd : ICommand
{
    private Transform playerTransform;
    private Vector3 direction;
    private float speed;
    private Animator animator;


    public MoveCmd(Transform playerTransform, Vector3 direction, float speed, Animator animator)
    {
        this.playerTransform = playerTransform;
        this.direction = direction;
        this.speed = speed;
        this.animator = animator;
    }

    public void Execute()
    {
        playerTransform.position += direction * speed * Time.deltaTime;
        animator.SetBool("isRunning", true);
        if (direction.x < 0)
            playerTransform.localScale = new Vector3(-1, 1, 1); // Mirar a la izquierda
        else if (direction.x > 0)
            playerTransform.localScale = new Vector3(1, 1, 1); // Mirar a la derecha
    }
}