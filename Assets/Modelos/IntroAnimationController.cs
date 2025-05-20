using UnityEngine;
using System.Collections;

public class IntroAnimController : MonoBehaviour
{
    public Animator cameraAnimator;             // Animator asignado a la cámara
    public string nombreAnimacion = "Intro";    // Nombre de la animación a reproducir
    public GameObject elementosDelJuego;        // Elementos del juego que se activan después
    public AudioSource audioIntro;              // Audio que acompaña a la intro
    public float tiempoAudio = 230f;            // Tiempo tras el cual empieza el audio
    public float duracionIntro = 260f;          // Duración total de la animación

    void Start()
    {
        // 🔇 Ocultar elementos del juego al inicio
        if (elementosDelJuego != null)
            elementosDelJuego.SetActive(false);

        // ▶️ Activar animator y reproducir animación
        if (cameraAnimator != null)
        {
            cameraAnimator.enabled = true;
            cameraAnimator.Play(nombreAnimacion, 0, 0f);
            StartCoroutine(ControlarIntro());
        }
    }

    IEnumerator ControlarIntro()
    {
        // 🎧 Esperar tiempo para reproducir el audio
        yield return new WaitForSeconds(tiempoAudio);

        if (audioIntro != null)
            audioIntro.Play();

        // ⏳ Esperar el resto de la duración de la intro
        float tiempoRestante = duracionIntro - tiempoAudio;
        if (tiempoRestante > 0)
            yield return new WaitForSeconds(tiempoRestante);

        // ⏹ Detener animador en la última pose
        if (cameraAnimator != null)
        {
            cameraAnimator.Play(nombreAnimacion, 0, 1f);
            cameraAnimator.Update(0f);
            cameraAnimator.enabled = false;
        }

        // ✅ Activar los elementos del juego
        if (elementosDelJuego != null)
            elementosDelJuego.SetActive(true);
    }
}