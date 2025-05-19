using UnityEngine;
using System.Collections;

public class ParpadeoManager : MonoBehaviour
{
    public Animator animParpadoArriba;
    public Animator animParpadoAbajo;

    private float duracionAnimCerrar = 0.3f;
    private float duracionAnimAbrir = 0.3f;

    void Start()
    {
        if (animParpadoArriba == null || animParpadoAbajo == null)
        {
            Debug.LogError("❌ Falta asignar Animator de los párpados");
            return;
        }

        // ✅ Solo desactivamos los Animator, no los GameObjects
        animParpadoArriba.enabled = false;
        animParpadoAbajo.enabled = false;

        StartCoroutine(AbrirOjosAutomatico());
    }

    public void AbrirOjos()
    {
        StartCoroutine(AbrirCoroutine());
    }

    public void CerrarOjos()
    {
        StartCoroutine(CerrarCoroutine());
    }

    IEnumerator AbrirCoroutine()
    {
        Debug.Log("🔓 Abrir ojos");

        animParpadoArriba.enabled = true;
        animParpadoAbajo.enabled = true;

        ReiniciarAnimator(animParpadoArriba);
        ReiniciarAnimator(animParpadoAbajo);

        animParpadoArriba.Play("AbrirOjosArriba", 0, 0f);
        animParpadoAbajo.Play("AbrirOjosAbajo", 0, 0f);

        yield return new WaitForSeconds(duracionAnimAbrir + 0.1f);

        animParpadoArriba.enabled = false;
        animParpadoAbajo.enabled = false;
    }

    IEnumerator CerrarCoroutine()
    {
        Debug.Log("👁️ Cerrar ojos");

        animParpadoArriba.enabled = true;
        animParpadoAbajo.enabled = true;

        ReiniciarAnimator(animParpadoArriba);
        ReiniciarAnimator(animParpadoAbajo);

        animParpadoArriba.Play("CerrarOjosArriba", 0, 0f);
        animParpadoAbajo.Play("CerrarOjosAbajo", 0, 0f);

        yield return new WaitForSeconds(duracionAnimCerrar + 0.1f); // ← aseguramos tiempo

        // ✅ Ahora también desactivamos tras cerrar
        animParpadoArriba.enabled = false;
        animParpadoAbajo.enabled = false;
    }

    void ReiniciarAnimator(Animator anim)
    {
        anim.Rebind();
        anim.Update(0f);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator AbrirOjosAutomatico()
    {
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(AbrirCoroutine()); // ✅ Así sí espera la animación
    }

}
