using System.Collections;
using UnityEngine;

public class PortalVuelta : MonoBehaviour
{
    public Camera camaraNormal;
    public Camera camaraSuccion;
    public Animator animTransicion;
    public Transform jugador;

    private Terrain terreno;
    private bool enTransicion = false;

    void Start()
    {
        terreno = Terrain.activeTerrain;

        if (jugador == null)
        {
            Camera mainCam = Camera.main;
            jugador = mainCam.transform.parent != null ? mainCam.transform.parent : mainCam.transform;
        }

        camaraNormal.enabled = true;
        camaraSuccion.enabled = false;
    }

    void OnMouseDown()
    {
        if (enTransicion) return;

        enTransicion = true;

        Debug.Log("¡Has hecho clic en el portal!"); // ← Añade esto

        camaraNormal.enabled = false;
        camaraSuccion.enabled = true;

        if (animTransicion != null)
        {
            animTransicion.SetTrigger("Succion");
        }
    }
    
    public void TeletransportarAlCentro()
    {
        Debug.Log("📍 Teletransportando al centro...");
        jugador.position = new Vector3(512f, 1.9f, 512f);
    }

    public void VolverACamaraNormal()
    {
        Debug.Log("🎥 Volviendo a cámara normal...");
        camaraSuccion.enabled = false;
        camaraNormal.enabled = true;
        enTransicion = false;
    }

}