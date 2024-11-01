using UnityEngine;
using Photon.Pun;

public class Disparo : MonoBehaviour
{
    public int damage = 10; // Da�o que causa la bala
    public float lifetime = 2f; // Tiempo de vida de la bala

    private void Start()
    {
        // Destruir el disparo despu�s de un tiempo
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si choca con un jugador
        if (collision.CompareTag("Player"))
        {
            // Obtener el componente PlayerController del jugador
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // Causar da�o al jugador
                player.TakeDamage(damage);
            }
            // Destruir el disparo al chocar
            Destroy(gameObject);
        }
    }

}
