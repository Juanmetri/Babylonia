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
    private int currentHealth;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        camera = GetComponentInChildren<Camera>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {

        currentHealth = health;
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
        Vector3 moveDirection = Vector3.zero;

        // Movimiento horizontal
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection = transform.right; // Usa la orientación actual del objeto
            ICommand moveRight = new MoveCmd(transform, moveDirection, moveSpeed, animator, pv);
            moveRight.Execute();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveDirection = -transform.right; // Inversa de la orientación actual
            ICommand moveLeft = new MoveCmd(transform, moveDirection, moveSpeed, animator, pv);
            moveLeft.Execute();
        }
        else
        {
            pv.RPC("SetAnimatorBool", RpcTarget.All, "isRunning", false);
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            ICommand jump = new JumpCmd(rb, jumpForce, animator, pv);
            jump.Execute();
            isGrounded = false;
        }

        // Disparo
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

    [PunRPC]
    public void SetAnimatorBool(string paramName, bool value)
    {
        animator.SetBool(paramName, value);
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!pv.IsMine) return; // Solo el jugador dueño del objeto puede tomar daño.

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Salud actual: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        gameObject.SetActive(false);
        Debug.Log($"{gameObject.name} ha muerto.");
    }
}