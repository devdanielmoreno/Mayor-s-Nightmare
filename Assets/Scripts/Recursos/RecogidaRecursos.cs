using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RecogidaRecursos : MonoBehaviour
{
    public Image imagenProgreso; // 🟢 Referencia a la imagen de progreso
    public Image iconoRecurso; // 📌 Referencia al icono del recurso
    private float duracion = 20f; // ⏳ Tiempo de recolección en segundos
    private Coroutine progresoCoroutine;
    private LightingManager lightingManager;
    private bool esDeNoche;
    
    void Start()
    {
        imagenProgreso.fillAmount = 0f; // 🔄 Asegurar que siempre inicia en 0
        lightingManager = FindObjectOfType<LightingManager>(); // 🔥 Buscar LightingManager

        if (lightingManager == null)
        {
            Debug.LogError("⚠️ LightingManager no encontrado en la escena.");
        }
    }
    void Update()
    {
        bool ahoraEsDeNoche = !(lightingManager.timeOfDay > 6 && lightingManager.timeOfDay < 18);

        if (ahoraEsDeNoche != esDeNoche) // 🔄 Si cambia el estado día/noche
        {
            esDeNoche = ahoraEsDeNoche;
            if (esDeNoche)
            {
                OcultarProgreso();
            }
            else
            {
                MostrarProgresoDesdeCero();
            }
        }
    }

    public void IniciarProgreso()
    {
        if (progresoCoroutine != null)
        {
            StopCoroutine(progresoCoroutine); // ❌ Evitar que haya coroutines anteriores activas
        }

        progresoCoroutine = StartCoroutine(RellenarProgreso());
    }

    private IEnumerator RellenarProgreso()
    {
        Building building = GetComponentInParent<Building>();

        if (building != null)
        {
            if (building.preset.tipo == BuildingPreset.TipoEdificio.Casa)
            {
                imagenProgreso.gameObject.SetActive(true); // 🔥 Las casas siempre muestran progreso
            }
            else
            {
                imagenProgreso.gameObject
                    .SetActive(false); // 🔥 Granjas y herrerías ocultan progreso hasta tener trabajadores
            }
        }

        while (true) // Mantener en un bucle infinito para revisar constantemente
        {
            int trabajadores = building.GetTrabajadoresActuales();
            if (building != null && (building.preset.tipo == BuildingPreset.TipoEdificio.Granja ||
                                     building.preset.tipo == BuildingPreset.TipoEdificio.Herrería))
            {
                if (trabajadores > 0 && !esDeNoche)
                {
                    imagenProgreso.gameObject.SetActive(true);
                    iconoRecurso.gameObject.SetActive(true);// 🔥 Activar la barra cuando hay aldeanos

                    float tiempo = 0f;
                    while (tiempo < duracion)
                    {
                        tiempo += Time.deltaTime;
                        imagenProgreso.fillAmount = tiempo / duracion;
                        yield return null;
                    }

                    // 🔄 Verificar de nuevo si aún hay trabajadores
                    trabajadores = building.GetTrabajadoresActuales();
                    if (trabajadores == 0 || esDeNoche)
                    {
                        OcultarProgreso();
                    }
                }
                else
                {
                    OcultarProgreso();
                }
            }

            if (building.preset.tipo == BuildingPreset.TipoEdificio.Casa)
            {
                float tiempo = 0f;
                while (tiempo < duracion)
                {
                    tiempo += Time.deltaTime;
                    imagenProgreso.fillAmount = tiempo / duracion;
                    yield return null;
                }

                trabajadores = building.GetTrabajadoresActuales();
            }
            yield return new WaitForSeconds(1f);
        }
    }
    void OcultarProgreso()
    {
        Debug.Log("🌙 Es de noche, ocultando progreso.");
        imagenProgreso.gameObject.SetActive(false);
        iconoRecurso.gameObject.SetActive(false);
        imagenProgreso.fillAmount = 0f;
    }

    void MostrarProgresoDesdeCero()
    {
        Debug.Log("☀️ Es de día, reiniciando progreso.");
        imagenProgreso.gameObject.SetActive(true);
        iconoRecurso.gameObject.SetActive(true);
        imagenProgreso.fillAmount = 0f;
        IniciarProgreso();
    }
}

