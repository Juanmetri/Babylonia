using UnityEngine;

public class LavaDamage : MonoBehaviour
{
    // Se ejecuta cuando un objeto entra en contacto con la lava
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el objeto que colisiona es el jugador
        if (other.CompareTag("Player"))
        {
            // Destruye el objeto jugador cuando entra en la lava
            Destroy(other.gameObject);
            Debug.Log("¡Jugador tocó la lava y fue destruido!");
        }
    }
}
