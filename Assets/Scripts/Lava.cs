using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class LavaFloor : MonoBehaviour
{
    // Referencia al transform del jugador
    public Transform player;

    // Punto inicial donde reaparece el jugador
    public Vector3 respawnPoint;

    private void Start()
    {
        // Configura el punto inicial automáticamente si no se especifica
        if (respawnPoint == Vector3.zero)
        {
            respawnPoint = player.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el jugador toca la lava
        if (other.CompareTag("Player"))
        {
            // Opcional: Agregar efectos visuales o de sonido al morir
            Debug.Log("¡El jugador tocó la lava!");

            // Reaparece el jugador en el punto inicial
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        player.position = respawnPoint;

        // Opcional: Reiniciar cualquier otro estado del jugador (vida, animaciones, etc.)
    }
}
