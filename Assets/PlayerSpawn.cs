using Photon.Pun;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private PhotonView pv;

    private void Awake()
    {
        // Obtener el PhotonView del GameObject actual
        pv = GetComponent<PhotonView>();
        if (pv == null)
        {
            Debug.LogError("El GameObject que contiene PlayerSpawn necesita un PhotonView.");
        }
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected || playerPrefab == null)
        {
            Debug.LogError("Photon no está conectado o no se asignó el prefab del jugador.");
            return;
        }

        // Determinar posición y rotación basándose en el actor número
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        Vector2 spawnPosition;
        Quaternion spawnRotation;

        if (actorNumber % 2 == 0)
        {
            spawnPosition = new Vector2(4, 0); // Ajusta según tu escena
            spawnRotation = Quaternion.identity; // Mirando hacia la izquierda
        }
        else
        {
            spawnPosition = new Vector2(-4, 0);
            spawnRotation = Quaternion.identity; // Mirando hacia la derecha
        }

        // Instanciar al jugador en red
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);
        Debug.Log($"Spawneando jugador en posición {spawnPosition} con rotación {spawnRotation.eulerAngles}");

        // Cambiar color del jugador
        int colorIndex = actorNumber % 2;
        Color playerColor = (colorIndex == 0) ? Color.red : Color.yellow;

        // Usar el PhotonView del GameObject que maneja el spawn para enviar el RPC
        if (pv != null)
        {
            pv.RPC("SetPlayerColor", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, playerColor.r, playerColor.g, playerColor.b, playerColor.a);
        }
    }

    [PunRPC]
    private void SetPlayerColor(int playerViewID, float r, float g, float b, float a)
    {
        // Encontrar el PhotonView del jugador
        PhotonView targetPhotonView = PhotonView.Find(playerViewID);
        if (targetPhotonView != null)
        {
            SpriteRenderer spriteRenderer = targetPhotonView.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Aplicar el color
                spriteRenderer.color = new Color(r, g, b, a);
                Debug.Log($"Color aplicado al jugador {targetPhotonView.ViewID}: {spriteRenderer.color}");
            }
        }
        else
        {
            Debug.LogError($"No se encontró un PhotonView con el ID {playerViewID}.");
        }
    }
}
