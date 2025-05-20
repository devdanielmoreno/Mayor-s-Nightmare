using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool enSupervivencia = false;

    public GameObject panelSonido;       // Panel negro con texto
    public AudioClip sonidoAlIniciar;    // ← Asigna este sonido desde el Inspector

    private AudioSource audioSource;     // Reproduce el sonido

    void Awake()
    {
        instance = this;

        // Crear un AudioSource si no existe
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D
    }

    public void IniciarSupervivencia()
    {
        StartCoroutine(MostrarMensajeYIniciar());
    }

    IEnumerator MostrarMensajeYIniciar()
    {
        if (panelSonido != null)
            panelSonido.SetActive(true);
        
        foreach (var source in FindObjectsOfType<AudioSource>())
        {
            source.Stop();
        }
        
        if (sonidoAlIniciar != null)
            audioSource.PlayOneShot(sonidoAlIniciar);

        yield return new WaitForSeconds(2f);

        enSupervivencia = true;
        SceneManager.LoadScene("SupervivenciaScene");
    }


    public void FinalizarSupervivencia()
    {
        enSupervivencia = false;
        SceneManager.LoadScene("JuegoScene");
    }
}