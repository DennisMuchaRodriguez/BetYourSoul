using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Entidad : MonoBehaviour
{
    public GameObject menuMinijuegos;   

    public void MostrarMenuMinijuegos()
    {
        menuMinijuegos.SetActive(true);
        Cursor.lockState = CursorLockMode.None; 
        Time.timeScale = 0; 
    }

    public void IniciarMinijuegoCuchillo()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("Minigame_Knife");
    }

    public void IniciarMinijuego2()
    {
        Debug.Log("Modo no disponible");
    }
}
