using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    private Camera camera;
    private Animator animator;
    private Rigidbody2D rb;

    public GameObject disparoPrefab;
    public Transform bulletSpawnPoint;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float bulletSpeed = 10f;
    private bool isGrounded = true;
    public int health = 100;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        camera = GetComponentInChildren<Camera>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        camera.gameObject.SetActive(pv.IsMine);
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        // Movimiento horizontal
        if (Input.GetKey(KeyCode.D))
        {
            ICommand moveRight = new MoveCmd(transform, Vector3.right, moveSpeed, animator);
            moveRight.Execute();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            ICommand moveLeft = new MoveCmd(transform, Vector3.left, moveSpeed, animator);
            moveLeft.Execute();
        }
        else
        {
            animator.SetBool("isRunning", false); // Vuelve a Idle si no se mueve
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            ICommand jump = new JumpCmd(rb, jumpForce, animator);
            jump.Execute();
            isGrounded = false;
        }

        // Disparo
        if (Input.GetKeyDown(KeyCode.E))
        {
            ICommand shoot = new ShootCmd(disparoPrefab, bulletSpawnPoint, bulletSpeed);
            shoot.Execute();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!pv.IsMine) return;

        health -= damage;
        pv.RPC("UpdateHealth", RpcTarget.All, health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}
