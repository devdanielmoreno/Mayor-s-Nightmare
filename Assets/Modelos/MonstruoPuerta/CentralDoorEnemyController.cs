using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CentralDoorEnemyController : MonoBehaviour
{
    public Fnaf4CameraController camaraController;
    public Animator animadorPuerta;
    public Animator animadorMonstruo;
    public AudioSource sonidoGolpe;
    public AudioSource sonidoPuertaAbriendo;
    public AudioSource sonidoPuertaCerrando;

    public VentanaEnemyController monstruoIzquierdo;
    public VentanaDerechaEnemyController monstruoDerecho;

    public float tiempoEntreGolpesMin = 15f;
    public float tiempoEntreGolpesMax = 30f;
    public float tiempoParaEsconderseDespuesApertura = 4f;
    public float cooldownEntreMonstruos = 5f;

    private int contadorGolpes = 0;
    private bool ataqueEnCurso = false;
    private bool ataqueBloqueado = false;
    private bool monstruoPresente = false;

    // 👻 Jumpscare central
    public GameObject jumpscareCentral;
    public AudioSource sonidoJumpscareCentral;
    public Transform camaraJugador;

    void Start()
    {
        StartCoroutine(ComportamientoGolpes());
        animadorPuerta.enabled = false;
        animadorMonstruo.enabled = false;

        if (jumpscareCentral != null)
            jumpscareCentral.SetActive(false);
    }

    IEnumerator ComportamientoGolpes()
    {
        while (true)
        {
            if (ataqueEnCurso || ataqueBloqueado)
            {
                yield return null;
                continue;
            }

            if ((monstruoIzquierdo != null && monstruoIzquierdo.EnMonstruoPresente()) ||
                (monstruoDerecho != null && monstruoDerecho.EnMonstruoPresente()))
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(Random.Range(tiempoEntreGolpesMin, tiempoEntreGolpesMax));
            StartCoroutine(GolpearPuerta());
        }
    }

    IEnumerator GolpearPuerta()
    {
        ataqueEnCurso = true;

        contadorGolpes++;
        if (sonidoGolpe != null)
            sonidoGolpe.Play();

        Debug.Log("Golpe número: " + contadorGolpes);

        if (contadorGolpes >= 3)
        {
            if ((monstruoIzquierdo != null && monstruoIzquierdo.EnMonstruoPresente()) ||
                (monstruoDerecho != null && monstruoDerecho.EnMonstruoPresente()))
            {
                Debug.Log("Otros monstruos presentes. Reiniciando contador de golpes.");
                contadorGolpes = 0;
                yield return new WaitForSeconds(cooldownEntreMonstruos);
                ataqueEnCurso = false;
                yield break;
            }

            yield return new WaitForSeconds(0.5f);

            if (animadorPuerta != null)
            {
                animadorPuerta.enabled = true;
                animadorPuerta.Play("PuertaAbriendoseFuerte");
                if (sonidoPuertaAbriendo != null)
                    sonidoPuertaAbriendo.Play();
            }

            if (animadorMonstruo != null)
            {
                animadorMonstruo.enabled = true;
                animadorMonstruo.Play("MonstruoAsomaPuerta");
            }

            Debug.Log("Puerta abierta y monstruo asoma. Jugador tiene 4 segundos para esconderse.");
            yield return new WaitForSeconds(tiempoParaEsconderseDespuesApertura);

            if (camaraController != null && !camaraController.EscondidoEnArmario)
            {
                Debug.LogWarning("💀 Jumpscare! El jugador no se escondió.");
                StartCoroutine(ActivarJumpscareCentral());
                yield break;
            }
            else
            {
                Debug.Log("✅ El jugador estaba escondido a tiempo.");
            }

            if (animadorMonstruo != null)
                animadorMonstruo.Play("MonstruoSeVaPuerta");

            yield return new WaitForSeconds(2f);

            if (animadorPuerta != null)
            {
                animadorPuerta.Play("PuertaCerrandose");
                if (sonidoPuertaCerrando != null)
                    sonidoPuertaCerrando.Play();
            }

            yield return new WaitForSeconds(2f);

            contadorGolpes = 0;

            if (monstruoIzquierdo != null)
                monstruoIzquierdo.BloquearAtaque();

            if (monstruoDerecho != null)
                monstruoDerecho.BloquearAtaque();

            yield return new WaitForSeconds(cooldownEntreMonstruos);

            if (monstruoIzquierdo != null)
                monstruoIzquierdo.DesbloquearAtaque();

            if (monstruoDerecho != null)
                monstruoDerecho.DesbloquearAtaque();
        }

        ataqueEnCurso = false;
        yield return new WaitForSeconds(cooldownEntreMonstruos);
    }

    IEnumerator ActivarJumpscareCentral()
    {
        // 1. Cerrar ojos
        ParpadeoManager parpadeo = FindObjectOfType<ParpadeoManager>();
        if (parpadeo != null) parpadeo.CerrarOjos();
        yield return new WaitForSeconds(0.5f);

        // 2. Ocultar el monstruo de la puerta
        if (animadorMonstruo != null)
            animadorMonstruo.gameObject.SetActive(false);

        // 3. Activar el jumpscare delante del jugador
        if (jumpscareCentral != null && camaraJugador != null)
        {
            jumpscareCentral.SetActive(true);

            Vector3 offsetFrente = camaraJugador.forward * 11f;
            Vector3 offsetLado = camaraJugador.right * -0.5f;
            Vector3 offsetBajada = Vector3.down * 10f;
            Vector3 posicionFinal = camaraJugador.position + offsetFrente + offsetLado + offsetBajada;

            jumpscareCentral.transform.position = posicionFinal;

            // ✅ Corrección final: de pie y mirando al jugador
            jumpscareCentral.transform.rotation = Quaternion.LookRotation(camaraJugador.forward) * Quaternion.Euler(-90f, 180f, 0f);

            Animator anim = jumpscareCentral.GetComponent<Animator>();
            if (anim != null)
            {
                anim.enabled = true;
                anim.Rebind();
                anim.Update(0f);
                anim.Play("JumpscareCentral", 0, 0f);
            }

            if (sonidoJumpscareCentral != null)
                sonidoJumpscareCentral.Play();
        }

        // 4. Abrir ojos
        if (parpadeo != null) parpadeo.AbrirOjos();

        // 5. Bloquear input
        if (camaraController != null)
            camaraController.BloquearInput();

        // 6. Esperar para terminar jumpscare
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("GameOver");
    }

    public void BloquearAtaque() { ataqueBloqueado = true; }
    public void DesbloquearAtaque() { ataqueBloqueado = false; }
    public bool EnMonstruoPresente() { return monstruoPresente; }
}
