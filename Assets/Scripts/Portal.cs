using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    public Camera camaraNormal;
    public Camera camaraSuccion;
    public Animator animTransicion;
    public Transform jugador;
    public Vector3 nuevaPosicion = new Vector3(10000f, 1.9f, 10000f);
    public float duracionTransicion = 1.5f;

    private bool enTransicion = false;

    public void IniciarTransicion()
    {
        if (!enTransicion)
        {
            StartCoroutine(TransicionPortal());
        }
    }

    private IEnumerator TransicionPortal()
    {
        enTransicion = true;

        // Cambiar a cámara de succión
        camaraNormal.enabled = false;
        camaraSuccion.enabled = true;
        Debug.Log("🎥 Cámara de succión activada");

        // Ejecutar animación si existe
        if (animTransicion != null)
        {
            animTransicion.SetTrigger("Succion");
        }

        yield return new WaitForSeconds(duracionTransicion);
        if (animTransicion != null)
        {
            animTransicion.Play("New State"); // ← asegúrate que ese estado existe
            Debug.Log("⏹️ Animación forzada a parar (Idle)");
        }
        // Teletransportar al jugador
        if (jugador != null)
        {
            jugador.position = nuevaPosicion;
            Debug.Log("📦 Teletransportado a: " + nuevaPosicion);
        }

        // Volver a cámara normal
        camaraSuccion.enabled = false;
        camaraNormal.enabled = true;
        Debug.Log("🎥 Cámara principal activada");

        enTransicion = false;
    }

    void Start()
    {
        if (jugador == null)
        {
            Camera mainCam = Camera.main;
            jugador = mainCam.transform.parent ?? mainCam.transform;
        }

        camaraNormal.enabled = true;
        camaraSuccion.enabled = false;
    }

    public void VolverACamaraNormal()
    {
        Debug.Log("🎥 Volviendo a cámara normal...");
        camaraSuccion.enabled = false;
        camaraNormal.enabled = true;
        enTransicion = false;
    }
}