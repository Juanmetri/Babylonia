using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LavaRising : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap; // El Tilemap que contiene tanto el piso como la lava
    [SerializeField] private Tile lavaTile; // Tile de lava
    [SerializeField] private Tile floorTileTop; // Tile superior del piso
    [SerializeField] private Tile floorTileMiddle; // Tile medio del piso
    [SerializeField] private Tile floorTileBottom; // Tile inferior del piso
    [SerializeField] private float riseInterval = 5f; // Intervalo en segundos para que la lava suba
    [SerializeField] private int maxLavaHeight = 5; // Altura máxima de la lava
    [SerializeField] private int startLavaHeight = 0; // Altura inicial de la lava

    private float timeSinceLastRise = 0f;
    private int currentLavaHeight;

    private void Start()
    {
        currentLavaHeight = startLavaHeight;
        
    }
    private void Update()
    {
        // Actualiza el temporizador
        timeSinceLastRise += Time.deltaTime;
        // Si el tiempo transcurrido ha superado el intervalo de subida
        if (timeSinceLastRise >= riseInterval)
        {
            RiseLava();
            timeSinceLastRise = 0f; // Resetea el temporizador
        }
    }
    private void RiseLava()
    {
        // Evitar que se repita demasiado rápido
        if (currentLavaHeight < maxLavaHeight)
        {
            

            // Verifica si la posición está dentro de los límites del Tilemap
            for (int x = -27; x < tilemap.size.x; x++) // Recorremos todas las columnas
            {
                Vector3Int tilePosition = tilemap.WorldToCell(new Vector3(x, currentLavaHeight, 0));
                if (!tilemap.cellBounds.Contains(tilePosition))
                {
                    // Si la posición está fuera del rango del Tilemap, la ignoramos.
                    continue;
                }
                TileBase currentTile = tilemap.GetTile(tilePosition);

                if (currentTile == null || currentTile == floorTileTop || currentTile == floorTileMiddle || currentTile == floorTileBottom)
                {
                    tilemap.SetTile(tilePosition, lavaTile); // Reemplazamos el tile de piso por lava
                }
                else
                {
                    tilemap.SetTile(tilePosition, lavaTile); // Coloca un tile de lava en la nueva fila
                }
            }

            currentLavaHeight++;
        }
    }

}
