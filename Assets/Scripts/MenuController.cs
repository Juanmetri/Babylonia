using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public InputField idInputField; // Aca donde el jugador escribe su ID 
    public Button connectButton; // Boton de conexion 
    public Text errorMessageText; // 

    private string playerID;

    void Start()
    {
        // mensaje de error al inicio 
        errorMessageText.gameObject.SetActive(false);

        // lisata al boton de conexion 
        connectButton.onClick.AddListener(OnConnectButtonClicked);
    }

    // cuando el jugador presiona "Connect" 
    void OnConnectButtonClicked()
    {
        playerID = idInputField.text.Trim(); // ID ingresado y eliminar espacios al inicio y fin

        if (string.IsNullOrEmpty(playerID))
        {
            // Si no se ha ingresado un ID, mostramos el mensaje de error 
            ShowErrorMessage("Por favor, ingresa un ID válido.");
        }
        else
        {
            // Si el ID es válido, pasa a la escena de carga
            PlayerPrefs.SetString("PlayerID", playerID); // Guardar el ID para usarlo en la siguiente escena
            SceneManager.LoadScene("Menu arranque"); // Cambiar a la escena Loading
        }
    }

    // Metodo  mostrar mensajes de error 
    void ShowErrorMessage(string message)
    {
        errorMessageText.gameObject.SetActive(true);
        errorMessageText.text = message;
    }
}