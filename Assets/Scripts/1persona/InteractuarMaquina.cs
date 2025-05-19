using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class InteractuarMaquina : MonoBehaviour
{
    public float distanciaInteraccion = 3f;
    public TextMeshProUGUI mensajeTexto;
    public Camera camaraPrincipal;

    private float tiempoMensaje = 2f;
    private float temporizador = 0f;
    private bool mostrandoMensaje = false;

    private string escenaPendiente = "";
    private bool enTransicion = false;

    void Start()
    {
        if (camaraPrincipal == null)
        {
            camaraPrincipal = Camera.main;
        }
    }

    void Update()
    {
        if (enTransicion) return;

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
                        StartCoroutine(AnimarCamaraYEjecutar("MiniJuego1", hit.point));
                }
                else if (tag == "Maquina2")
                {
                    if (JugadorManager.minijuego2Completado)
                        MostrarMensaje("¡Ya has completado este minijuego!");
                    else
                        StartCoroutine(AnimarCamaraYEjecutar("MiniJuego2", hit.point));
                }
                else if (tag == "Maquina3")
                {
                    if (JugadorManager.minijuego3Completado)
                        MostrarMensaje("¡Ya has completado este minijuego!");
                    else
                        StartCoroutine(AnimarCamaraYEjecutar("MiniJuego3", hit.point));
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

IEnumerator AnimarCamaraYEjecutar(string escena, Vector3 objetivo)
{
    enTransicion = true;
    
    float fovOriginal = camaraPrincipal.fieldOfView;
    float fovDestino = 10f;
    

    float duracion = 1.5f;
    float tiempo = 0f;

    while (tiempo < duracion)
    {
        float t = tiempo / duracion;
        
        camaraPrincipal.fieldOfView = Mathf.Lerp(fovOriginal, fovDestino, t);

        tiempo += Time.deltaTime;
        yield return null;
    }
        
    camaraPrincipal.fieldOfView = fovDestino;

    yield return new WaitForSeconds(0.5f);

    GameObject jugador = GameObject.FindWithTag("Jugador");
    if (jugador != null)
    {
        JugadorManager.ultimaPosicion = jugador.transform.position;
        JugadorManager.volverDesdeMinijuego = true;
    }

    SceneManager.LoadScene(escena);
}

}
