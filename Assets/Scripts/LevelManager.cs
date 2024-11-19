using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Botonstart() 
    {

        SceneManager.LoadScene(2);
    
    
    }

    public void botonsalir() 
    {
        Debug.Log("salir del juego");
        Application.Quit();
    
    }

}
