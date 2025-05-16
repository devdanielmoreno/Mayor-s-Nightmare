using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    public float duracionJuego = 15f;

    void Start()
    {
        Invoke("VolverAEscenaPrincipal", duracionJuego);
    }

    void VolverAEscenaPrincipal()
    {
        PlayerPrefs.SetFloat("VolverX", 100f); 
        PlayerPrefs.SetFloat("VolverY", 30f);
        PlayerPrefs.SetFloat("VolverZ", 0f);
        SceneManager.LoadScene("FinalScene");
    }
}