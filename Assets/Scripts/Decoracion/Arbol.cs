using UnityEngine;
using System.Collections;

public class Arbol : MonoBehaviour
{
    public int maderaPorArbol = 5; // Cantidad de madera obtenida
    private bool cayendo = false; // Controla si el árbol está cayendo
    private float velocidadCaida = 45f; // Velocidad de rotación en grados por segundo
    private float anguloMaximo = 90f; // Ángulo máximo de caída

    public void Cortar()
    {
        if (!cayendo)
        {
            StartCoroutine(CaerYDestruir());
        }
    }

    IEnumerator CaerYDestruir()
    {
        Debug.Log("🌳 El árbol está cayendo...");

        float anguloCaido = 0f;
        while (anguloCaido < anguloMaximo)
        {
            float rotacionFrame = velocidadCaida * Time.deltaTime;
            transform.Rotate(Vector3.right, rotacionFrame);
            anguloCaido += rotacionFrame;
            yield return null;
        }

        Debug.Log($"🌲 Árbol caído. +{maderaPorArbol} de madera");
        ResourceManager.instance.AñadirRecurso("madera", maderaPorArbol);
        City.instance.MostrarMensaje($"+{maderaPorArbol} de madera");

        Destroy(gameObject); // 🔥 Elimina el árbol una vez caído
    }
}