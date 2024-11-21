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
    public GameObject chargedDisparoPrefab; // Prefab for the charged shot
    public Transform bulletSpawnPoint;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float bulletSpeed = 10f;
    public float chargedBulletSpeed = 20f; // Speed for the charged shot
    private bool isGrounded = true;
    public int health = 100;
    private int currentHealth;

    private bool isCharging = false; // Charging state
    private float chargeTime = 0f; // Charge duration
    public float maxChargeTime = 1f; // Max charge time for charged shot

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
            moveDirection = transform.right; // Mover basado en la rotaci�n
            ICommand moveRight = new MoveCmd(transform, moveDirection, moveSpeed, animator, pv);
            moveRight.Execute();
        }
        // Movimiento a la izquierda
        else if (Input.GetKey(KeyCode.A))
        {
            moveDirection = -transform.right; // Invertir direcci�n
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            isCharging = true;
            chargeTime = 0f;
        }

        if (Input.GetKey(KeyCode.E) && isCharging)
        {
            chargeTime += Time.deltaTime;

        }

        if (Input.GetKeyUp(KeyCode.E) && isCharging)
        {
            isCharging = false;
            if (chargeTime >= maxChargeTime)
            {
                ShootCharged();
            }
            else
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        if (disparoPrefab == null || bulletSpawnPoint == null) return;

        Vector2 shootDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        ICommand shoot = new ShootCmd(
            disparoPrefab.name,
            bulletSpawnPoint.position,
            shootDirection,
            bulletSpeed,
            pv
        );

        shoot.Execute();
    }

    private void ShootCharged()
    {
        if (chargedDisparoPrefab == null || bulletSpawnPoint == null) return;

        Vector2 shootDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        ICommand shootCharged = new ShootCmd(
            chargedDisparoPrefab.name,
            bulletSpawnPoint.position,
            shootDirection,
            chargedBulletSpeed,
            pv
        );

        shootCharged.Execute();
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
        GameObject disparo = PhotonNetwork.Instantiate(disparoPrefabName, spawnPosition, Quaternion.identity);

        if (shootDirection.x < 0)
        {
            Vector3 newScale = disparo.transform.localScale;
            newScale.x = -Mathf.Abs(newScale.x);
            disparo.transform.localScale = newScale;
        }
        else
        {
            Vector3 newScale = disparo.transform.localScale;
            newScale.x = Mathf.Abs(newScale.x);
            disparo.transform.localScale = newScale;
        }

        Rigidbody2D rb = disparo.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = shootDirection * bulletSpeed;
        }

        Debug.Log($"Disparo creado en {spawnPosition} con direcci�n {shootDirection}, velocidad {bulletSpeed}");
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!pv.IsMine) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibi� {damage} de da�o. Salud actual: {currentHealth}");

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

        if (bulletSpawnPoint != null)
        {
            Vector3 newPosition = bulletSpawnPoint.localPosition;
            newPosition.x = flipX ? -Mathf.Abs(newPosition.x) : Mathf.Abs(newPosition.x);
            bulletSpawnPoint.localPosition = newPosition;
        }
    }
}
