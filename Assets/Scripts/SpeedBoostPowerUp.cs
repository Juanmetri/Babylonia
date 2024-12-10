using UnityEngine;
using Photon.Pun;

public class SpeedBoostPowerUp : MonoBehaviour
{
    public float boostAmount = 2f;
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerView = other.GetComponent<PhotonView>();

            if (playerView != null && playerView.IsMine)
            {
                Debug.Log($"Player {PhotonNetwork.NickName} collected the power-up.");
                playerView.RPC("ActivateSpeedBoost", RpcTarget.All, boostAmount, duration);

                PhotonView myView = GetComponent<PhotonView>();
                if (myView != null && PhotonNetwork.IsConnected)
                {
                    Debug.Log($"Requesting destruction of object with ViewID: {myView.ViewID}");
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
                Debug.Log($"Destroying object with ViewID: {myView.ViewID}");
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("PhotonView not found or already destroyed.");
            }
        }
    }
}
