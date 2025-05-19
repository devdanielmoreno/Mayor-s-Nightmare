using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InteractuarMaquina : MonoBehaviour
{
    public float distanciaInteraccion = 3f;
    public TextMeshProUGUI mensajeTexto;
    private float tiempoMensaje = 2f;
    private float temporizador = 0f;
    private bool mostrandoMensaje = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
            {
                string tag = hit.collider.tag;

                if (tag == "Maquina")
                {
                    if (JugadorManager.minijuego1Completado)
                        MostrarMensaje("¡Ya has completado este minijuego!");
                    else
                        EntrarEnMinijuego("MiniJuego1");
                }

                else if (tag == "Maquina2")
                {
                    if (JugadorManager.minijuego2Completado)
                        MostrarMensaje("¡Ya has completado este minijuego!");
                    else
                        EntrarEnMinijuego("MiniJuego2");
                }

                else if (tag == "Maquina3")
                {
                    if (JugadorManager.minijuego3Completado)
                        MostrarMensaje("¡Ya has completado este minijuego!");
                    else
                        EntrarEnMinijuego("MiniJuego3");
                }
            }
        }

        if (mostrandoMensaje)
        {
            temporizador -= Time.deltaTime;
            if (temporizador <= 0f)
            {
                mensajeTexto.gameObject.SetActive(false);
                mostrandoMensaje = false;
            }
        }
    }

    void MostrarMensaje(string texto)
    {
        mensajeTexto.text = texto;
        mensajeTexto.gameObject.SetActive(true);
        temporizador = tiempoMensaje;
        mostrandoMensaje = true;
    }

    void EntrarEnMinijuego(string escena)
    {
        GameObject jugador = GameObject.FindWithTag("Jugador");
        if (jugador != null)
        {
            JugadorManager.ultimaPosicion = jugador.transform.position;
            JugadorManager.volverDesdeMinijuego = true;
            SceneManager.LoadScene(escena);
        }
    }
}
