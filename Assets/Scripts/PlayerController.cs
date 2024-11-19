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
    public float bulletSpeed = 2f;
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

        // Verificar si este es el jugador 2 y ajustar la rotación inicial
        if (!pv.IsMine)
        {
            transform.eulerAngles = new Vector3(0, 180, 0); // Orienta al jugador 2 en dirección opuesta
        }
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
        if (!TurnManager.Instance.IsCurrentPlayerTurn(PhotonNetwork.LocalPlayer.ActorNumber - 1))
        {
            return; // Do nothing if it's not this player's turn
        }

        Vector3 moveDirection = Vector3.zero;

        // Movement logic
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection = pv.IsMine ? Vector3.right : Vector3.left;
            ICommand moveRight = new MoveCmd(transform, moveDirection, moveSpeed, animator);
            moveRight.Execute();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveDirection = pv.IsMine ? Vector3.left : Vector3.right;
            ICommand moveLeft = new MoveCmd(transform, moveDirection, moveSpeed, animator);
            moveLeft.Execute();
        }
        else
        {
            animator.SetBool("isRunning", false); // Vuelve a Idle si no se mueve
        }

        // Jump logic
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            ICommand jump = new JumpCmd(rb, jumpForce, animator);
            jump.Execute();
            isGrounded = false;
        }

        // Shooting logic
        if (Input.GetKeyDown(KeyCode.E))
        {
            ICommand shoot = new ShootCmd(disparoPrefab, bulletSpawnPoint, bulletSpeed, pv);
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