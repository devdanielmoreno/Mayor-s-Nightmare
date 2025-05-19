using UnityEngine;
using UnityEngine.UI;

public class LamparaController : MonoBehaviour
{
    public Light luzLámpara;
    public Renderer conoRenderer;
    public Material materialApagado;
    public Material materialEncendido;

    public Slider barraCordura;
    public float velocidadRecuperacion = 10f;
    public float velocidadPerdida = 2f;

    public VentanaDerechaEnemyController enemigoDerecha; // 👉 Añade esta referencia desde el Inspector
    private CameraShake cameraShake;

    private bool clicSostenido = false;
    private bool jumpscareActivado = false;
    private bool shakeActivado = false;

    void Start()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ActivarLuz(true);
                    clicSostenido = true;
                    return;
                }
            }
        }

        ActivarLuz(false);
        clicSostenido = false;
    }

    void FixedUpdate()
    {
        if (barraCordura == null || jumpscareActivado) return;

        if (clicSostenido)
        {
            barraCordura.value = Mathf.Max(0, barraCordura.value - velocidadRecuperacion * Time.fixedDeltaTime);
        }
        else
        {
            barraCordura.value = Mathf.Min(100, barraCordura.value + velocidadPerdida * Time.fixedDeltaTime);
        }

        // Activar Jumpscare al 100
        if (barraCordura.value >= 100f && !jumpscareActivado)
        {
            jumpscareActivado = true;
            if (enemigoDerecha != null)
            {
                enemigoDerecha.StartCoroutine(enemigoDerecha.ActivarJumpscareDerecha());
            }
        }

        // Activar shake al 50
        if (barraCordura.value >= 50f && !shakeActivado)
        {
            shakeActivado = true;
            if (cameraShake != null)
                cameraShake.TriggerShake(0.5f); // sacude durante 0.5 segundos
        }
        else if (barraCordura.value < 50f)
        {
            shakeActivado = false; // permite reactivar si vuelve a pasar del 50
        }
    }

    void ActivarLuz(bool estado)
    {
        if (luzLámpara != null)
            luzLámpara.enabled = estado;

        if (conoRenderer != null)
            conoRenderer.material = estado ? materialEncendido : materialApagado;
    }
}
