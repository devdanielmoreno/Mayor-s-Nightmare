using System.Collections;
using UnityEngine;

public class Berri : MonoBehaviour
{
    public int berriPorArbusto = 5; // Cantidad de madera obtenida
    public ParticleSystem efectoDestruccion;
    public void Cortar()
    {
        ResourceManager.instance.AñadirRecurso("berri", berriPorArbusto);
        City.instance.MostrarMensaje($"+{berriPorArbusto} Berris");

        StartCoroutine(DestruirConEfecto()); // 💥 Iniciar efecto antes de destruir
    }

    IEnumerator DestruirConEfecto()
    {

        efectoDestruccion.Play(); // 🎇 Reproducir efecto
        yield return new WaitForSeconds(1.4f); // ⏳ Esperar para que se vea


        Destroy(gameObject); // 🔥 Destruir después del efecto
    }
}