using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CircleDrawer : MonoBehaviour
{
    public GameObject objetivo;
    public LineRenderer lineRenderer;
    public int cierresNecesarios = 4;
    private int cierresActuales = 0;
    public TextMeshProUGUI progresoTexto;
    public AudioSource audioPunto;

    private List<Vector2> puntos = new List<Vector2>();
    private bool dibujando = false;
    private float distanciaMinima = 0.1f;
    
    void Start()
    {
        ActualizarProgreso();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            puntos.Clear();
            lineRenderer.positionCount = 0;
            dibujando = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            dibujando = false;
            ComprobarEncerrado();
        }

        if (dibujando)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (puntos.Count == 0 || Vector2.Distance(pos, puntos[puntos.Count - 1]) > distanciaMinima)
            {
                puntos.Add(pos);
                lineRenderer.positionCount = puntos.Count;
                lineRenderer.SetPosition(puntos.Count - 1, pos);
            }
        }
    }
    void ActualizarProgreso()
    {
        if (progresoTexto != null)
            progresoTexto.text = cierresActuales + "/" + cierresNecesarios;
    }

    void ComprobarEncerrado()
    {
        if (puntos.Count < 3) return;

        if (Vector2.Distance(puntos[0], puntos[puntos.Count - 1]) < 3f)
        {
            if (PuntoDentroPoligono(objetivo.transform.position, puntos))
            {
                cierresActuales++;
                ActualizarProgreso();
                if (audioPunto != null && audioPunto.clip != null)
                    audioPunto.PlayOneShot(audioPunto.clip);

                Debug.Log("¡Objetivo encerrado! Total: " + cierresActuales);
                if (cierresActuales >= cierresNecesarios)
                {
                    JugadorManager.minijuego3Completado = true;
                    SceneManager.LoadScene("FinalScene");
                }
            }
        }
    }

    bool PuntoDentroPoligono(Vector2 punto, List<Vector2> poligono)
    {
        int intersecciones = 0;
        for (int i = 0; i < poligono.Count; i++)
        {
            Vector2 a = poligono[i];
            Vector2 b = poligono[(i + 1) % poligono.Count];

            if ((a.y > punto.y) != (b.y > punto.y))
            {
                float x = (b.x - a.x) * (punto.y - a.y) / (b.y - a.y) + a.x;
                if (x > punto.x)
                    intersecciones++;
            }
        }
        return (intersecciones % 2) == 1;
    }
}
