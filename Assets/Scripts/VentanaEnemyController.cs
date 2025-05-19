using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VentanaEnemyController : MonoBehaviour
{
    public string nombreVentana;
    public float tiempoEntreAtaquesMin = 10f;
    public float tiempoEntreAtaquesMax = 25f;

    public GameObject monstruoVisual;
    public AudioSource sonidoAdvertencia;
    public AudioSource sonidoMonstruoSeVa;

    public Fnaf4CameraController camaraController;
    public Animator animadorMonstruo;

    public VentanaDerechaEnemyController monstruoDerecho;

    // 👇 NUEVO: referencias para el jumpscare
    public GameObject monstruoQuietoIzquierda;
    public GameObject jumpscareIzquierda;
    public Transform camaraJugador;
    public AudioSource sonidoJumpscare;

    private bool radioActivada = false;
    private bool monstruoPresente = false;
    private bool ataqueEnCurso = false;
    private bool puedeSerAhuyentado = false;
    private bool ataqueBloqueado = false;

    void Start()
    {
        StartCoroutine(IniciarAtaques());
    }

    IEnumerator IniciarAtaques()
    {
        while (true)
        {
            float espera = Random.Range(tiempoEntreAtaquesMin, tiempoEntreAtaquesMax);
            yield return new WaitForSeconds(espera);

            if (ataqueEnCurso || ataqueBloqueado) continue;
            if (monstruoDerecho != null && monstruoDerecho.EnMonstruoPresente()) continue;

            if (monstruoDerecho != null)
                monstruoDerecho.BloquearAtaque();

            StartCoroutine(AtacarVentana());
        }
    }

    IEnumerator AtacarVentana()
    {
        ataqueEnCurso = true;
        monstruoPresente = true;
        radioActivada = false;
        ataqueBloqueado = true;

        if (sonidoAdvertencia != null)
        {
            sonidoAdvertencia.panStereo = -1f;
            sonidoAdvertencia.Play();
        }

        yield return new WaitForSeconds(3f);

        if (monstruoVisual != null)
            monstruoVisual.SetActive(true);

        if (animadorMonstruo != null)
        {
            animadorMonstruo.Rebind();
            animadorMonstruo.Update(0f);
            animadorMonstruo.enabled = true;
            animadorMonstruo.Play("AparecerDoll", 0, 0f);
        }

        puedeSerAhuyentado = true;

        float tiempoParaEsconderse = 5f;
        float contador = 0f;

        while (contador < tiempoParaEsconderse)
        {
            if (camaraController != null && camaraController.EstabaEnVentanaIzquierda &&
                camaraController.EscondidoEnVentanaIzquierda && radioActivada)
            {
                FinalizarAtaque(true);
                yield break;
            }

            contador += Time.deltaTime;
            yield return null;
        }

        // ❌ Falló el escondite → jumpscare
        StartCoroutine(ActivarJumpscareIzquierda());
    }

    IEnumerator ActivarJumpscareIzquierda()
    {
        // 1. Cerrar ojos
        ParpadeoManager parpadeo = FindObjectOfType<ParpadeoManager>();
        if (parpadeo != null) parpadeo.CerrarOjos();
        yield return new WaitForSeconds(1f);

        // 2. Ocultar monstruo de la ventana
        if (monstruoVisual != null)
            monstruoVisual.SetActive(false);

        // 3. Activar monstruo jumpscare delante del jugador
        if (jumpscareIzquierda != null && camaraJugador != null)
        {
            // Activamos el objeto
            jumpscareIzquierda.SetActive(true);

            Vector3 offsetFrente = camaraJugador.forward.normalized * 1f;     // un poco más cerca
            Vector3 offsetBajada = Vector3.down * 16f;                          // bajamos más
            Vector3 offsetAtras = -camaraJugador.forward.normalized * -16f;     // un poco hacia atrás
            Vector3 posicionFrente = camaraJugador.position + offsetFrente + offsetBajada + offsetAtras;


            jumpscareIzquierda.transform.position = posicionFrente;

            // Rotación: que mire al jugador
            jumpscareIzquierda.transform.rotation = Quaternion.LookRotation(camaraJugador.forward);

            // Si tiene animador, reproducimos
            Animator anim = jumpscareIzquierda.GetComponent<Animator>();
            if (anim != null)
            {
                anim.enabled = true;
                anim.Rebind();
                anim.Update(0f);
                anim.Play("JumpscareIzquierda", 0, 0f);
            }

            // Sonido fuerte
            if (sonidoJumpscare != null)
                sonidoJumpscare.Play();
        }

        // 4. Abrir ojos
        if (parpadeo != null) parpadeo.AbrirOjos();

        // 5. Bloquear input para que no puedas girarte
        if (camaraController != null) camaraController.BloquearInput();

        // 6. Esperar sin desactivar el monstruo
        yield return new WaitForSeconds(2f);

        // 7. Finalizamos ataque, pero NO desactivamos el monstruo jumpscare
        FinalizarAtaque(false);
        SceneManager.LoadScene("GameOver");

    }


    public void RadioActivadaDesdeEscondite()
    {
        if (!monstruoPresente || !puedeSerAhuyentado || radioActivada) return;

        radioActivada = true;
        StartCoroutine(RadioAhuyentaMonstruo());
    }

    public bool JugadorEscondidoCorrectamente()
    {
        if (nombreVentana == "izquierda")
            return camaraController != null &&
                   camaraController.EstabaEnVentanaIzquierda &&
                   camaraController.EscondidoEnVentanaIzquierda;

        if (nombreVentana == "derecha")
            return camaraController != null &&
                   camaraController.EstabaEnVentanaDerecha &&
                   camaraController.EscondidoEnVentanaDerecha;

        return false;
    }

    IEnumerator RadioAhuyentaMonstruo()
    {
        if (animadorMonstruo != null)
        {
            animadorMonstruo.Play("SeVaDoll", 0, 0f);

            AnimationClip[] clips = animadorMonstruo.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == "SeVaDoll")
                {
                    yield return new WaitForSeconds(clip.length);
                    break;
                }
            }

            animadorMonstruo.enabled = false;
        }

        if (sonidoMonstruoSeVa != null)
            sonidoMonstruoSeVa.Play();

        FinalizarAtaque(true);
    }

    public void ReactivarMonstruo()
    {
        ataqueBloqueado = false;
    }

    public void BloquearAtaque() { ataqueBloqueado = true; }
    public void DesbloquearAtaque() { ataqueBloqueado = false; }

    public bool EnMonstruoPresente()
    {
        return monstruoPresente;
    }

    void FinalizarAtaque(bool salvado)
    {
        ataqueEnCurso = false;
        puedeSerAhuyentado = false;
        monstruoPresente = false;

        if (monstruoVisual != null)
            monstruoVisual.SetActive(false);

        if (animadorMonstruo != null)
            animadorMonstruo.enabled = false;

        // ✅ Solo ocultamos el monstruo "quieto" si se ahuyentó (no si hubo jumpscare)
        if (salvado && monstruoQuietoIzquierda != null)
            monstruoQuietoIzquierda.SetActive(false);
    }

}
