using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sonidoClickFuerte;
    
    public void StartGame()
    {
        audioSource.PlayOneShot(sonidoClickFuerte);
        Invoke("CargarEscena", sonidoClickFuerte.length);
    }

    void CargarEscena()
    {
        SceneManager.LoadScene("CargaScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Abriendo Ajustes...");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}