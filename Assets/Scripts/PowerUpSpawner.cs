using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs; // Array of power-up prefabs
    public Vector2 spawnAreaMin = new Vector2(-11, -2); // Set default area
    public Vector2 spawnAreaMax = new Vector2(25, -2);
    public float spawnInterval = 5f; // Base spawn interval
    public float spawnIntervalRandomness = 5f; // Adds variability
    public int maxActivePowerUps = 5; // Limit active power-ups

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting power-up spawner on MasterClient.");
            StartCoroutine(SpawnPowerUps());
        }
        else
        {
            Debug.Log("Not the MasterClient; spawner disabled.");
        }
    }

    private IEnumerator SpawnPowerUps()
    {
        while (true)
        {
            // Wait for a randomized interval
            float interval = spawnInterval + Random.Range(-spawnIntervalRandomness, spawnIntervalRandomness);
            yield return new WaitForSeconds(interval);

            // Limit active power-ups
            if (GameObject.FindGameObjectsWithTag("PowerUp").Length >= maxActivePowerUps)
            {
                Debug.Log("Maximum active power-ups reached. Skipping spawn.");
                continue;
            }

            // Generate a random spawn position
            Vector2 spawnPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            // Select a random power-up prefab
            GameObject selectedPowerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];

            // Spawn the selected power-up
            if (selectedPowerUp != null)
            {
                PhotonNetwork.Instantiate(selectedPowerUp.name, spawnPosition, Quaternion.identity);
                Debug.Log($"Spawned {selectedPowerUp.name} at {spawnPosition}");
            }
            else
            {
                Debug.LogError("Power-up prefab is not assigned in the inspector.");
            }
        }
    }
}
