using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCmd : ICommand
{

    private Rigidbody2D rb;
    private float jumpForce;
    private Animator animator;

    public JumpCmd(Rigidbody2D rb, float jumpForce, Animator animator)
    {
        this.rb = rb;
        this.jumpForce = jumpForce;
        this.animator = animator;
    }
    public void Execute()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator.SetBool("isJumping", true);
    }
}
