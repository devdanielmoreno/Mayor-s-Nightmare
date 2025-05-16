using System.Collections;
using UnityEngine;

public class Roca : MonoBehaviour
{
    public int piedraPorRoca = 30; // Cantidad de piedra obtenida
    public ParticleSystem efectoDestruccion; // 💥 Efecto visual de destrucción

    public void Picar()
    {
        ResourceManager.instance.AñadirRecurso("stone", piedraPorRoca);
        City.instance.MostrarMensaje($"+{piedraPorRoca} de piedra");

        StartCoroutine(DestruirConEfecto()); // 💥 Iniciar efecto antes de destruir
    }

    IEnumerator DestruirConEfecto()
    {
        
        efectoDestruccion.Play(); // 🎇 Reproducir efecto
        yield return new WaitForSeconds(1.5f); // ⏳ Esperar para que se vea


        Destroy(gameObject); // 🔥 Destruir después del efecto
    }
}