using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalJuegoManager : MonoBehaviour
{
    public float tiempoEspera = 3f;
    private bool finalMostrado = false;

    void Update()
    {
        if (!finalMostrado &&
            JugadorManager.minijuego1Completado &&
            JugadorManager.minijuego2Completado &&
            JugadorManager.minijuego3Completado)
        {
            finalMostrado = true;
            Debug.Log("Has completado todos los minijuegos. ¡Final!");
            Invoke("CargarEscenaFinal", tiempoEspera);
        }
    }

    void CargarEscenaFinal()
    {
        SceneManager.LoadScene("FinalDEMO");
    }
}