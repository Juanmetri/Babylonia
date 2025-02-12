using UnityEngine;
using Photon.Pun;

public class JumpBoostPowerUp : MonoBehaviour
{
    public float boostAmount = 5f;
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerView = other.GetComponent<PhotonView>();

            if (playerView != null && playerView.IsMine)
            {
                Debug.Log($"Player {PhotonNetwork.NickName} collected the jump boost power-up.");

                // Apply the jump boost effect
                playerView.RPC("ActivateJumpBoost", RpcTarget.All, boostAmount, duration);

                // Request the MasterClient to destroy the power-up
                PhotonView myView = GetComponent<PhotonView>();
                if (myView != null && PhotonNetwork.IsConnected)
                {
                    Debug.Log("Requesting MasterClient to destroy the jump boost power-up.");
                    gameObject.SetActive(false); // Disable to prevent further interactions
                    myView.RPC("DestroyPowerUp", RpcTarget.MasterClient);
                }
            }
        }
    }

    [PunRPC]
    private void DestroyPowerUp()
    {
        PhotonView myView = GetComponent<PhotonView>();

        if (myView != null)
        {
            if (!myView.IsMine)
            {
                // Transferir propiedad al jugador que intenta destruirlo
                myView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }

            if (PhotonNetwork.IsMasterClient || myView.IsMine)
            {
                Debug.Log($"Destroying power-up with ViewID: {myView.ViewID}");
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("No tienes permisos para destruir este objeto.");
            }
        }
    }
}
