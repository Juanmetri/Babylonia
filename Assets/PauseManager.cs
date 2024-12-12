using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel; // Panel del menú de pausa

    private bool isPaused = false;

    void Update()
    {
        // Detectar si el jugador presiona la tecla P
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseMenu();
        }
    }

    // Alternar el estado del menú de pausa
    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);
    }

    // Reanudar el juego
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("El juego se ha cerrado."); // Solo para pruebas en el editor
    }
}
