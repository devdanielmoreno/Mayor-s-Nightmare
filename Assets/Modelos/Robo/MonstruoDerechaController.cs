using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VentanaDerechaEnemyController : MonoBehaviour
{
    public float tiempoEntreAtaquesMin = 10f;
    public float tiempoEntreAtaquesMax = 25f;
    public float cooldownDespuesDeAhuyentar = 10f;
    public float cooldownEntreMonstruos = 3f;

    public GameObject monstruoVisual;
    public AudioSource sonidoAdvertencia;
    public AudioSource sonidoMonstruoSeVa;

    public Fnaf4CameraController camaraController;
    public Animator animadorMonstruo;
    public Light luzAhuyentar;

    public VentanaEnemyController monstruoIzquierdo;
    public GameObject monstruoQuietoDerecha; // El monstruo que se queda quieto unos segundos
    


    // Jumpscare
    public GameObject jumpscareDerecha;     // Prefab con animación
    public Transform camaraJugador;         // Asignar camPivot
    public AudioSource sonidoJumpscare;     // Sonido fuerte

    private bool monstruoPresente = false;
    private bool puedeSerAhuyentado = false;
    private bool enCooldown = false;
    private bool ataqueBloqueado = false;
    
    private ParpadeoManager parpadeo;

    void Start()
    {
        parpadeo = FindObjectOfType<ParpadeoManager>();
        StartCoroutine(ActivarAtaques());
        //SceneManager.LoadScene("GameOver");
    }

    IEnumerator ActivarAtaques()
    {
        while (true)
        {
            float espera = Random.Range(tiempoEntreAtaquesMin, tiempoEntreAtaquesMax);
            yield return new WaitForSeconds(espera);

            if (enCooldown || ataqueBloqueado) continue;
            if (monstruoIzquierdo != null && monstruoIzquierdo.EnMonstruoPresente()) continue;

            if (monstruoIzquierdo != null)
                monstruoIzquierdo.BloquearAtaque();

            StartCoroutine(AparecerMonstruo());
        }
    }

    IEnumerator AparecerMonstruo()
    {
        if (sonidoAdvertencia != null)
        {
            sonidoAdvertencia.panStereo = 1f;
            sonidoAdvertencia.Play();
        }

        yield return new WaitForSeconds(3f);

        monstruoPresente = true;
        puedeSerAhuyentado = false;
        monstruoVisual.SetActive(true);

        if (animadorMonstruo != null)
        {
            animadorMonstruo.Rebind();
            animadorMonstruo.Update(0f);
            animadorMonstruo.enabled = true;
            animadorMonstruo.Play("IrAVentana", 0, 0f);

            AnimationClip[] clips = animadorMonstruo.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == "IrAVentana")
                {
                    yield return new WaitForSeconds(clip.length);
                    break;
                }
            }
        }

        puedeSerAhuyentado = true;

        float tiempoLimite = 6f;
        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoLimite)
        {
            if (camaraController != null &&
                camaraController.EstabaEnVentanaDerecha &&
                camaraController.EscondidoEnVentanaDerecha &&
                luzAhuyentar != null &&
                luzAhuyentar.enabled)
            {
                yield return AhuyentarMonstruo();
                yield break;
            }

            tiempoPasado += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(ActivarJumpscareDerecha());
    }

    IEnumerator AhuyentarMonstruo()
    {
        puedeSerAhuyentado = false;

        if (animadorMonstruo != null)
        {
            animadorMonstruo.Play("IrseDeVentana", 0, 0f);

            AnimationClip[] clips = animadorMonstruo.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == "IrseDeVentana")
                {
                    yield return new WaitForSeconds(clip.length);
                    break;
                }
            }
        }

        if (sonidoMonstruoSeVa != null)
            sonidoMonstruoSeVa.Play();

        FinalizarAtaque(true);

        enCooldown = true;
        yield return new WaitForSeconds(cooldownDespuesDeAhuyentar);
        enCooldown = false;

        yield return new WaitForSeconds(cooldownEntreMonstruos);

        if (monstruoIzquierdo != null)
            monstruoIzquierdo.DesbloquearAtaque();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public IEnumerator ActivarJumpscareDerecha()
    {
        // 1. Cerrar ojos
        if (parpadeo != null) parpadeo.CerrarOjos();
        yield return new WaitForSeconds(0.1f);

        // 2. Ocultar monstruo de la ventana
        if (monstruoVisual != null) monstruoVisual.SetActive(false);

        // 3. Activar jumpscare monstruo delante del jugador
        if (jumpscareDerecha != null && camaraJugador != null)
        {
            jumpscareDerecha.SetActive(true);
            jumpscareDerecha.transform.position = camaraJugador.position + camaraJugador.forward * 1.5f;
            jumpscareDerecha.transform.rotation = Quaternion.LookRotation(camaraJugador.forward);

            Animator anim = jumpscareDerecha.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);
                anim.Play("JumpscareDerecha");
            }

            // 🔊 Sonido fuerte justo cuando empieza la animación
            if (sonidoJumpscare != null)
                sonidoJumpscare.Play();
        }

        // 4. Abrir ojos después de que el monstruo esté colocado
        if (parpadeo != null) parpadeo.AbrirOjos();

        // 5. Bloquear input cámara de inmediato para que no se gire
        if (camaraController != null) camaraController.BloquearInput();

        // 6. Esperar mientras se muestra el jumpscare
        // 6. Esperar mientras se muestra el jumpscare
        Invoke("CargarGameOver", 2f); // tras 2 segundos (puedes ajustar)


    }

    void CargarGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    
    void FinalizarAtaque(bool salvado)
    {
        monstruoPresente = false;
        puedeSerAhuyentado = false;
    }

    public bool EnMonstruoPresente()
    {
        return monstruoPresente;
    }

    public void BloquearAtaque() { ataqueBloqueado = true; }
    public void DesbloquearAtaque() { ataqueBloqueado = false; }
}
