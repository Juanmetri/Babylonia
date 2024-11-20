using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCmd : ICommand
{

    private Rigidbody2D rb;
    private float jumpForce;
    private Animator animator;
    private PhotonView pv;
    public JumpCmd(Rigidbody2D rb, float jumpForce, Animator animator, PhotonView pv)
    {
        this.rb = rb;
        this.jumpForce = jumpForce;
        this.animator = animator;
        this.pv = pv;
    }
    [PunRPC]
    public void Execute()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        pv.RPC("SetAnimatorBool", RpcTarget.All, "isJumping", true);
    }
}
