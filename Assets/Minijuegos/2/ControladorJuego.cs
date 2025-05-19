using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorJuego : MonoBehaviour
{
    public float tiempoObjetivo = 5f;
    private float tiempoActual = 0f;
    private bool juegoActivo = true;

    void Update()
    {
        if (!juegoActivo) return;

        tiempoActual += Time.deltaTime;
        if (tiempoActual >= tiempoObjetivo)
        {
            juegoActivo = false;
            CambiarEscena();
        }
    }

    public void FinDelJuego()
    {
        if (!juegoActivo) return;

        juegoActivo = false;
        Debug.Log("¡Has sido golpeado!");
    }

    void CambiarEscena()
    {
        JugadorManager.minijuego1Completado = true;
        SceneManager.LoadScene("FinalScene");
    }
}