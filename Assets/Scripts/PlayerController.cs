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
    public float jumpForce = 10f;
    public float bulletSpeed = 10f;
    public float chargedBulletSpeed = 20f; // Speed for the charged shot
    private bool isGrounded = true;
    public int health = 100;
    private int currentHealth;
    private float lastFireTime = 0f; // Momento del último disparo
    public float fireCooldown = 0.5f; // Intervalo mínimo entre disparos

    private bool isCharging = false; // Charging state
    private float chargeTime = 0f; // Charge duration
    public float maxChargeTime = 1f; // Max charge time for charged shot
    private bool canShoot = true;

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
    [PunRPC]
    private void HandleInput()
    {
        Vector3 moveDirection = Vector3.zero;

        // Movimiento a la derecha
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection = transform.right; // Mover basado en la rotación
            ICommand moveRight = new MoveCmd(transform, moveDirection, moveSpeed, animator, pv);
            moveRight.Execute();
        }
        // Movimiento a la izquierda
        else if (Input.GetKey(KeyCode.A))
        {
            moveDirection = -transform.right; // Invertir dirección
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

        // Disparo común con la tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            canShoot = false;
            Shoot();
            StartCoroutine(ResetShoot());
        }

        // Carga del disparo cargado con la tecla Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isCharging = true;
            chargeTime = 0f;
            Debug.Log("Carga iniciada");
        }

        // Incrementar la carga mientras se mantiene presionado Q
        if (Input.GetKey(KeyCode.Q) && isCharging)
        {
            chargeTime += Time.deltaTime;

            if (chargeTime >= maxChargeTime)
            {
                Debug.Log("Carga completa");
            }
        }

        // Ejecutar disparo cargado al soltar Q
        if (Input.GetKeyUp(KeyCode.Q) && isCharging)
        {
            isCharging = false;

            if (chargeTime >= maxChargeTime)
            {
                ShootCharged();
                Debug.Log("Ejecutando disparo cargado");
            }
            else
            {
                Debug.Log("No se alcanzó el tiempo para disparo cargado.");
            }

            chargeTime = 0f; // Reiniciar el tiempo de carga
        }
    }
    private IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(0.1f); // Delay mínimo entre disparos
        canShoot = true;
    }
    [PunRPC]
    private void Shoot()
    {
        Debug.Log($"Disparo ejecutado por {PhotonNetwork.NickName} en {Time.time}");

        if (disparoPrefab == null || bulletSpawnPoint == null)
        {
            Debug.LogError("DisparoPrefab o BulletSpawnPoint no están configurados.");
            return;
        }

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
    [PunRPC]
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
        if (!PhotonNetwork.IsMasterClient && !pv.IsMine) return; // Solo permitir al MasterClient o al Owner ejecutar esto

        Debug.Log($"RPC ExecuteShoot llamado por {PhotonNetwork.NickName} en {Time.time}");

        GameObject disparo = PhotonNetwork.Instantiate(disparoPrefabName, spawnPosition, Quaternion.identity);

        Rigidbody2D rb = disparo.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = shootDirection * bulletSpeed;
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        // Aplicar daño independientemente de si es propietario o no
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Salud actual: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die(); // Llamar al método Die cuando la salud llegue a 0
        }
    }

    [PunRPC]
    public void ApplyPush(Vector2 direction, float force)
    {
        // Aplicar empuje independientemente de si es propietario o no
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    [PunRPC]
    public void Die()
    {
        if (!pv.IsMine) return;

        Debug.Log($"{gameObject.name} ha muerto.");

        // Determinar al ganador
        string winnerName = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.NickName != PhotonNetwork.NickName)
            {
                winnerName = player.NickName; // Encuentra el nombre del otro jugador
            }
        }

        // Llamar al RPC para mostrar la pantalla del ganador a todos
        PhotonView.FindObjectsOfType<GameOverManager>()[0].photonView.RPC("ShowWinnerScreen", RpcTarget.All, winnerName);

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
