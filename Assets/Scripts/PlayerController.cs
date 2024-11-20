using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    private Camera camera;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public GameObject disparoPrefab;
    public Transform bulletSpawnPoint;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float bulletSpeed = 10f;
    private bool isGrounded = true;
    public int health = 100;
    private int currentHealth;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        camera = GetComponentInChildren<Camera>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        currentHealth = health;
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
        Vector3 moveDirection = Vector3.zero;

        // Movimiento a la derecha
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection = Vector3.right;
            ICommand moveRight = new MoveCmd(transform, moveDirection, moveSpeed, animator, pv);
            moveRight.Execute();
        }
        // Movimiento a la izquierda
        else if (Input.GetKey(KeyCode.A))
        {
            moveDirection = Vector3.left;
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
            Shoot();
        }
    }

    private void Shoot()
    {
        if (disparoPrefab == null || bulletSpawnPoint == null) return;

        // Determinar la dirección del disparo basado en flipX
        Vector2 shootDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        // Instanciar el disparo utilizando el comando ShootCmd
        ICommand shoot = new ShootCmd(
            disparoPrefab.name,
            bulletSpawnPoint.position,
            shootDirection,
            bulletSpeed,
            pv
        );

        shoot.Execute();
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
    public void ExecuteShoot(string disparoPrefabName, Vector3 spawnPosition, Vector2 shootDirection, float bulletSpeed)
    {
        // Instanciar el disparo
        GameObject disparo = PhotonNetwork.Instantiate(disparoPrefabName, spawnPosition, Quaternion.identity);

        // Girar el sprite de la bala si dispara hacia la izquierda
        if (shootDirection.x < 0)
        {
            Vector3 newScale = disparo.transform.localScale;
            newScale.x = -Mathf.Abs(newScale.x); // Invertir en el eje X
            disparo.transform.localScale = newScale;
        }
        else
        {
            Vector3 newScale = disparo.transform.localScale;
            newScale.x = Mathf.Abs(newScale.x); // Asegurar que esté normal hacia la derecha
            disparo.transform.localScale = newScale;
        }

        // Configurar la dirección y velocidad del disparo
        Rigidbody2D rb = disparo.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = shootDirection * bulletSpeed;
        }

        Debug.Log($"Disparo creado en {spawnPosition} con dirección {shootDirection}, velocidad {bulletSpeed}");
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!pv.IsMine) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Salud actual: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    [PunRPC]
    public void ApplyPush(Vector2 direction, float force)
    {
        if (!pv.IsMine) return;

        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
    }

    [PunRPC]
    public void SetFlipX(bool flipX)
    {
        spriteRenderer.flipX = flipX;

        // Cambiar la posición del bulletSpawnPoint en el eje X
        if (bulletSpawnPoint != null)
        {
            Vector3 newPosition = bulletSpawnPoint.localPosition;
            newPosition.x = flipX ? -Mathf.Abs(newPosition.x) : Mathf.Abs(newPosition.x);
            bulletSpawnPoint.localPosition = newPosition;
        }
    }
}
