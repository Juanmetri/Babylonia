using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public InputField idInputField;
    public Button connectButton;
    public Text errorMessageText; 

    private string playerID;

    void Start()
    {
        errorMessageText.gameObject.SetActive(false);

        connectButton.onClick.AddListener(OnConnectButtonClicked);
    }

    void OnConnectButtonClicked()
    {
        playerID = idInputField.text.Trim();

        if (string.IsNullOrEmpty(playerID))
        {
            ShowErrorMessage("Por favor, ingresa un ID válido.");
        }
        else
        {
            PlayerPrefs.SetString("PlayerID", playerID);
            SceneManager.LoadScene("Menu arranque");
        }
    }
    void ShowErrorMessage(string message)
    {
        errorMessageText.gameObject.SetActive(true);
        errorMessageText.text = message;
    }
}