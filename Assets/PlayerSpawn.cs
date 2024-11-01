using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private GameObject player;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        // Determina la posición y rotación en función del número de jugadores
        Vector2 spawnPosition;
        Quaternion spawnRotation;

        // Verifica si hay un jugador en la sala
        if (PhotonNetwork.PlayerList.Length % 2 == 0)
        {
            // Jugador B (derecha)
            spawnPosition = new Vector2(4, 0); // Ajusta según tu escena
            spawnRotation = Quaternion.Euler(0, 180, 0); // Mirando hacia la izquierda
        }
        else
        {
            // Jugador A (izquierda)
            spawnPosition = new Vector2(-4, 0); // Ajusta según tu escena
            spawnRotation = Quaternion.identity; // Mirando hacia la derecha
        }

        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation, 0);

        // Lógica de cambio de color
        int playerIndex = PhotonNetwork.PlayerList.Length;
        pv.RPC("ChangeColor", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, playerIndex);
    }

    [PunRPC]
    private void ChangeColor(int playerViewID, int playerIndex)
    {
        PhotonView targetPhotonView = PhotonView.Find(playerViewID);

        if (targetPhotonView != null)
        {
            targetPhotonView.gameObject.GetComponent<SpriteRenderer>().color = (playerIndex == 1) ? Color.red : Color.blue;
        }
    }
}
