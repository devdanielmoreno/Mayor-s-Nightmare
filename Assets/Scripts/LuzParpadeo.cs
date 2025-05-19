using UnityEngine;

public class LuzParpadeo : MonoBehaviour
{
    public Light luz; // arrastra aquí el SpotLight desde el Inspector
    public float intensidadMaxima = 3f;
    public float velocidadEncendido = 1f;
    public float tiempoEntreParpadeos = 5f;
    public float duracionParpadeo = 0.2f;

    private float tiempoSiguienteParpadeo;
    private bool encendida = true;

    void Start()
    {
        if (luz == null) luz = GetComponent<Light>();
        luz.intensity = 0;
        tiempoSiguienteParpadeo = Time.time + tiempoEntreParpadeos;
    }

    void Update()
    {
        // Encendido gradual
        if (luz.intensity < intensidadMaxima)
        {
            luz.intensity += velocidadEncendido * Time.deltaTime;
            if (luz.intensity > intensidadMaxima)
                luz.intensity = intensidadMaxima;
        }

        // Parpadeo cada X segundos
        if (Time.time >= tiempoSiguienteParpadeo)
        {
            StartCoroutine(Parpadear());
            tiempoSiguienteParpadeo = Time.time + tiempoEntreParpadeos;
        }
    }

    System.Collections.IEnumerator Parpadear()
    {
        luz.enabled = false;
        yield return new WaitForSeconds(duracionParpadeo);
        luz.enabled = true;
    }
}