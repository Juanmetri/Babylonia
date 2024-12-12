using UnityEngine;
using Photon.Pun;

public class DamageBoostPowerUp : MonoBehaviour
{
    public float boostAmount = 10f; // Amount to increase damage
    public float duration = 5f; // How long the boost lasts

    [PunRPC]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerView = other.GetComponent<PhotonView>();

            if (playerView != null && playerView.IsMine)
            {
                Debug.Log($"Player {PhotonNetwork.NickName} collected the damage boost power-up.");

                // Apply the damage boost effect
                playerView.RPC("ActivateDamageBoost", RpcTarget.All, boostAmount, duration);

                // Request the MasterClient to destroy the power-up
                PhotonView myView = GetComponent<PhotonView>();
                if (myView != null && PhotonNetwork.IsConnected)
                {
                    Debug.Log("Requesting MasterClient to destroy the damage boost power-up.");
                    gameObject.SetActive(false); // Disable to prevent further interactions
                    myView.RPC("DestroyPowerUp", RpcTarget.MasterClient);
                }
            }
        }
    }

    [PunRPC]
    private void DestroyPowerUp()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView myView = GetComponent<PhotonView>();
            if (myView != null && PhotonView.Find(myView.ViewID) != null)
            {
                Debug.Log($"Destroying damage boost power-up with ViewID: {myView.ViewID}");
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("PhotonView not found or already destroyed.");
            }
        }
    }
}
