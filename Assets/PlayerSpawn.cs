using Photon.Pun;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected || playerPrefab == null)
        {
            Debug.LogError("Photon no est� conectado o no se asign� el prefab del jugador.");
            return;
        }

        // Determinar posici�n y rotaci�n bas�ndose en el actor n�mero
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        Vector2 spawnPosition;
        Quaternion spawnRotation;

        // Asignar posiciones espec�ficas seg�n el n�mero de jugador
        if (actorNumber % 2 == 0)
        {
            // Jugador B (derecha)
            spawnPosition = new Vector2(4, 0); // Ajusta seg�n tu escena
            spawnRotation = Quaternion.Euler(0, 180, 0); // Mirando hacia la izquierda
        }
        else
        {
            // Jugador A (izquierda)
            spawnPosition = new Vector2(-4, 0); // Ajusta seg�n tu escena
            spawnRotation = Quaternion.identity; // Mirando hacia la derecha
        }

        // Instanciar al jugador en red
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);

        // Establecer color seg�n el actor n�mero
        int colorIndex = actorNumber % 2; // Alternar entre colores
        Color playerColor = (colorIndex == 0) ? Color.red : Color.yellow;

        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null)
        {
            pv.RPC("SetPlayerColor", RpcTarget.AllBuffered, pv.ViewID, playerColor);
        }
    }

    [PunRPC]
    private void SetPlayerColor(int playerViewID, Color color)
    {
        PhotonView targetPhotonView = PhotonView.Find(playerViewID);
        if (targetPhotonView != null)
        {
            targetPhotonView.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
