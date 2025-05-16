using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class City : MonoBehaviour
{
    public int money;
    public int day;
    public int curPopulation;
    public int curJobs;
    public int curFood;
    public int maxPopulation;
    public int maxJobs;
    public int incomePerJob;
    public TextMeshProUGUI stats;
    public TextMeshProUGUI recursosText;
    public TextMeshProUGUI mensajeUI;
    private Coroutine mensajeCoroutine;
    public List<Building> buildings = new List<Building>();
    
    public int diasParaLunaLlena = 7;
    public TextMeshProUGUI lunaLlenaText;

    public GameObject mensajeFondo;
    
    public GameObject aldeanoPrefab; // Prefab del aldeano
    public List<GameObject> aldeanos = new List<GameObject>(); // Lista de aldeanos en la ciudad

    private Aldeano aldeanoSeleccionado = null;
    public LightingManager lightingManager;
    
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;
    private bool portalAparecioHoy = false;
    
    public bool lunaLlenaActivaEnNoche = false;
    
    public static City instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mensajeFondo.SetActive(false);
        lightingManager = FindObjectOfType<LightingManager>(); // 🔥 Buscar LightingManager automáticamente
        if (lightingManager == null)
        {
            Debug.LogError("⚠️ LightingManager no encontrado en la escena. Asegúrate de que está presente.");
        }
        UpdateStatText();
        UpdateRecursosText();
        UpdateLunaLlenaText();
        InvokeRepeating("GenerarRecursos", 20f, 20f);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.R) && Input.GetMouseButtonDown(0))
        {
            AsignarTrabajoEdificio();
        }
        if (Input.GetKey(KeyCode.R) && Input.GetMouseButtonUp(0))
        {
            DetectarArbolYTalar();
        }

        if (Input.GetKeyUp(KeyCode.R)) // Soltar R deselecciona al aldeano
        {
            aldeanoSeleccionado = null;
            Debug.Log("Aldeano deseleccionado.");
        }
    }

    void DetectarArbolYTalar()
    {
        if (aldeanoSeleccionado == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Arbol arbol = hit.collider.GetComponent<Arbol>();

            if (arbol != null)
            {
                aldeanoSeleccionado.AsignarArbol(arbol);
                Debug.Log($"🪓 {aldeanoSeleccionado.name} comenzará a talar {arbol.name}");
            }
        }
    }
    public void MostrarMensaje(string mensaje, float duracion = 2f)
    {
        if (mensajeCoroutine != null)
        {
            StopCoroutine(mensajeCoroutine);
        }
        mensajeCoroutine = StartCoroutine(MostrarMensajeCoroutine(mensaje, duracion));
    }
    void GenerarEventoDiario()
    {
        int randomEvento = Random.Range(1, 100);
        
        if (!portalAparecioHoy)
        {
            int chancePortal = Random.Range(0, 100);
            if (chancePortal < 50)
            {
                CrearPortal();
                portalAparecioHoy = true;
            }
        }


        if (day > 2 && day < 6) // Eventos leves 🌿
        {
            if (randomEvento < 30)
            {
                MostrarMensaje("Ha llovido, las granjas producen +1 trigo hoy.");
                ResourceManager.instance.AñadirRecurso("trigo", 1);
            }
        }
        else if (day > 6 && day < 11) // Eventos moderados ⚔️
        {
            if (randomEvento < 20)
            {
                MostrarMensaje("Bandidos han robado 100 de dinero.");
                ResourceManager.instance.GastarRecurso("dinero", 100);
            }
        }
        else if (day > 11)
        {
            if (randomEvento < 10)
            {
                MostrarMensaje("Un monstruo ha atacado la ciudad, se han destruido 10 de madera.");
                ResourceManager.instance.GastarRecurso("madera", 10);
            }
        }
    }
    
    private IEnumerator MostrarMensajeCoroutine(string mensaje, float duracion)
    {
        mensajeUI.text = mensaje;
        mensajeFondo.SetActive(true); 
    
        yield return new WaitForSeconds(duracion);
    
        mensajeUI.text = "";
        mensajeFondo.SetActive(false); 
    }
    
    public void OnPlaceBuilding(Building building)
    {
        money = ResourceManager.instance.dinero; // 🔥 Asegurar que el dinero se sincroniza en City
        InventoryUI.instance.UpdateInventory(); // 🔄 Actualizar UI del inventario

        // 🔨 Construcción del edificio
        maxPopulation += building.preset.population;
        maxJobs += building.preset.jobs;
        buildings.Add(building);
        
        
        RecogidaRecursos recogidaUI = building.GetComponentInChildren<RecogidaRecursos>();
        if (recogidaUI != null)
        {
            recogidaUI.IniciarProgreso(); // ✅ ¡Inicia la barra de progreso inmediatamente!
        }

        // 🏡 Mensaje de construcción
        string tipo = building.preset.tipo.ToString();
        Debug.Log($"🏗️ Edificio construido: {tipo}");

        MissionManager.instance.AgregarEdificio(tipo);

        if (building.preset.population > 0)
        {
            int poblacionAñadida = Mathf.Min(building.preset.population, maxPopulation - curPopulation);
            curPopulation += poblacionAñadida;
            SpawnAldeanos(building, poblacionAñadida);
        }

        UpdateStatText();
    }

    void SpawnAldeanos(Building building, int cantidad)
    {
        Vector3 spawnOffset = new Vector3(0, (float)1.9, -3); // 📌 Offset para que los aldeanos aparezcan delante del edificio

        for (int i = 0; i < cantidad; i++)
        {
            Vector3 spawnPos = building.transform.position + spawnOffset + new Vector3(i * 1.5f, 0, 0);
            GameObject nuevoAldeano = Instantiate(aldeanoPrefab, spawnPos, Quaternion.identity);
            aldeanos.Add(nuevoAldeano);
        }
    }
    public Building ObtenerCasaMasCercana(Vector3 posicionAldeano)
    {
        Building casaCercana = null;
        float menorDistancia = Mathf.Infinity;

        foreach (Building building in buildings)
        {
            if (building.preset.population > 0) 
            {
                float distancia = Vector3.Distance(posicionAldeano, building.transform.position);
                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    casaCercana = building;
                }
            }
        }
        return casaCercana;
    }


    public void OnRemoveBuilding(Building building)
    {
        int dineroDevuelto = building.preset.cost / 2;
        money += dineroDevuelto;
        maxPopulation -= building.preset.population;
        curPopulation = Mathf.Clamp(curPopulation, 0, maxPopulation);
        maxJobs -= building.preset.jobs;
        buildings.Remove(building);

        // ❌ Elimina los aldeanos de esta casa
        List<GameObject> aldeanosAEliminar = new List<GameObject>();

        foreach (var aldeano in aldeanos)
        {
            Aldeano script = aldeano.GetComponent<Aldeano>();
            if (script.home == building)
            {
                aldeanosAEliminar.Add(aldeano);
                if (script.tipo == TipoAldeano.SinTrabajo) 
                {
                    curJobs--; // 🔥 Reducir trabajos activos si estaba trabajando
                    script.tipo = TipoAldeano.SinTrabajo; // 🔄 Dejar sin trabajo
                }
            }
        }

        foreach (var aldeano in aldeanosAEliminar)
        {
            aldeanos.Remove(aldeano);
            Destroy(aldeano);
        }
        CalculateJobs();
        Destroy(building.gameObject);
        UpdateStatText();
    }
    public void UpdateRecursosText()
    {
        recursosText.text = string.Format(
            "Madera: {0}  Trigo: {1}  Honor: {2}  Dinero: {3}€",
            ResourceManager.instance.madera,
            ResourceManager.instance.trigo,
            ResourceManager.instance.honor,
            ResourceManager.instance.dinero
        );
    }
    void GenerarRecursos()  
        {
            bool ahoraEsDeNoche = !(lightingManager.timeOfDay > 6 && lightingManager.timeOfDay < 18);

            if (!ahoraEsDeNoche)
            {
                foreach (Building building in buildings)
                {
                    int trabajadores = building.GetTrabajadoresActuales();

                    switch (building.preset.tipo)
                    {
                        case BuildingPreset.TipoEdificio.Casa:
                            ResourceManager.instance.AñadirRecurso("madera", 1);
                            Debug.Log($"🌲 {building.name} ha generado 1 de madera.");
                            RecogidaRecursos recogidaUI = building.GetComponentInChildren<RecogidaRecursos>();
                            if (recogidaUI != null)
                            {
                                recogidaUI.gameObject.SetActive(true);
                                recogidaUI.IniciarProgreso();
                            }
                            break;
                    }
                }

                foreach (GameObject aldeanoObj in aldeanos)
                {
                    int trigoProducido = 1;
                    if (aldeanoObj == null) continue;
                    Aldeano aldeano = aldeanoObj.GetComponent<Aldeano>();
                    if (aldeano == null) continue;

                    if (aldeano.tipo == TipoAldeano.Granjero)
                    {
                        if (aldeano.habilidadBuena.Contains("GoodFarmer")) trigoProducido += 1;
                        if (aldeano.habilidadMala.Contains("BadFarmer")) trigoProducido -= 1;
                        ResourceManager.instance.AñadirRecurso("trigo", trigoProducido);
                    }
                }

                foreach (GameObject aldeanoObj in aldeanos)
                {
                    int honorProducido = 1;
                    if (aldeanoObj == null) continue;
                    Aldeano aldeano = aldeanoObj.GetComponent<Aldeano>();
                    if (aldeano == null) continue;

                    if (aldeano.tipo == TipoAldeano.Guerrero)
                    {
                        if (aldeano.habilidadBuena.Contains("WeakFighter")) honorProducido -= 1;
                        if (aldeano.habilidadMala.Contains("StrongFighter")) honorProducido += 1;
                        ResourceManager.instance.AñadirRecurso("honor", honorProducido);
                        Debug.Log($"🏆 {aldeano.nombre} ha generado 1 de honor.");
                    }
                }
            }
            else
            {
                Debug.Log("no pueden de noche");
            }

            UpdateRecursosText();  
    }

    public void UpdateStatText()
    {
        stats.text = string.Format("Día: {0}" +
                                   "  Población: {1}/{2}" +
                                   "  Trabajo: {3}/{4}" +
                                   "  Comida: {5}", 
            new object[6] { day, curPopulation, maxPopulation, curJobs, maxJobs, curFood });
        
        InventoryUI.instance.UpdateInventory();
    }


    public void EndTurn()
    {
        bool esDeNoche = !(lightingManager.timeOfDay > 6 && lightingManager.timeOfDay < 18);
        day++;
        if (diasParaLunaLlena == 1 && esDeNoche) 
        {
            diasParaLunaLlena = 0; // 🌕 Es Luna Llena, se activa en este turno
            MostrarMensaje("¡Luna Llena ha llegado!");
        }
        else if (diasParaLunaLlena == 0)
        {
            diasParaLunaLlena = 7; // 🔄 Se reinicia después de la luna llena
        }
        else
        {
            diasParaLunaLlena--; // 🔥 Reducimos normalmente si no es luna llena
        }
        CalculateMoney();
        CalculatePopulation();
        CalculateJobs();
        CalculateFood();
        GenerarEventoDiario();
        UpdateStatText();
        UpdateRecursosText();
        UpdateLunaLlenaText();
        portalAparecioHoy = false;
    }
    
    public void CrearPortal()
    {
        GameObject portal = Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);
        MostrarMensaje("Un portal ha aparecido por tiempo limitado...");
        StartCoroutine(DestruirPortalTrasTiempo(portal, 20f));
    }

    private IEnumerator DestruirPortalTrasTiempo(GameObject portal, float segundos)
    {
        yield return new WaitForSeconds(segundos);
        if (portal != null)
        {
            Destroy(portal);
        }
    }
    void CalculateMoney()
    {
        money += curJobs * incomePerJob;
        ResourceManager.instance.dinero = money; // 🔥 Asegurar que se guarda en ResourceManager
        Debug.Log($"💰 Dinero actualizado en ResourceManager: {ResourceManager.instance.dinero}");
    }

    
    void CalculatePopulation()
    {
        if (curFood >= curPopulation && curPopulation < maxPopulation)
        {
            curFood -= curPopulation / 4;
            curPopulation = Mathf.Min(curPopulation + (curFood / 4), maxPopulation);
        }
        else if (curFood < curPopulation)
        {
            int diferencia = curPopulation - curFood;
            curPopulation = Mathf.Max(curFood, 0); // 🔥 Evita valores negativos
            Debug.Log($"⚠️ Hambre en la ciudad: {diferencia} aldeanos han muerto.");
        }
    }

    public void CalculateJobs()
    {
        int trabajadores = 0;

        Debug.Log($"🔍 Recalculando trabajos. Total de aldeanos en la lista: {aldeanos.Count}");

        foreach (Building building in buildings)
        {
            int trabajadoresEnEdificio = 0;

            foreach (GameObject aldeanoObj in aldeanos)
            {
                Aldeano aldeano = aldeanoObj.GetComponent<Aldeano>();

                if (aldeano == null)
                {
                    Debug.LogError($"⚠️ ERROR: No se encontró el script Aldeano en {aldeanoObj.name}");
                    continue;
                }

                // 📌 Verificar que el aldeano realmente trabaja en este edificio
                if (aldeano.home == building) // ✅ Solo contar aldeanos con este edificio asignado
                {
                    if ((aldeano.tipo == TipoAldeano.Granjero && building.CompareTag("Granja")) ||
                        (aldeano.tipo == TipoAldeano.Guerrero && building.CompareTag("Herrería")))
                    {
                        trabajadoresEnEdificio++;
                    }
                }
            }

            // 📌 Asegurar que no exceda el máximo permitido por edificio (2)
            trabajadores += Mathf.Min(trabajadoresEnEdificio, 2);
        }

        curJobs = Mathf.Min(trabajadores, maxJobs);
        UpdateStatText();

        Debug.Log($"✅ Trabajos actualizados correctamente: {curJobs}/{maxJobs}");
    }


    
    void CalculateFood()
    {
        int comidaProducida = 0;

        foreach (GameObject aldeanoObj in aldeanos)
        {
            Aldeano aldeano = aldeanoObj.GetComponent<Aldeano>();
            if (aldeano.tipo == TipoAldeano.Granjero)
            {
                comidaProducida += 10; // 🌾 Cada granjero produce 10 de comida

                // 📌 Modificar según habilidades del aldeano
                if (aldeano.habilidadBuena == "GoodFarmer (+20% comida)")
                {
                    comidaProducida += 2;
                }
                if (aldeano.habilidadMala == "BadFarmer (-20% comida)")
                {
                    comidaProducida -= 2;
                }
            }
        }

        int comidaConsumida = curPopulation;
        curFood = comidaProducida - comidaConsumida;

        if (curFood < 0)
        {
            int aldeanosMueren = Mathf.Abs(curFood);
            curPopulation = Mathf.Max(0, curPopulation - aldeanosMueren);
            curFood = 0;
            Debug.Log($"⚠️ ¡Hambre en la ciudad! {aldeanosMueren} aldeanos han muerto.");
        }
    }
    public void SeleccionarAldeano(Aldeano aldeano)
    {
        aldeanoSeleccionado = aldeano;
        Debug.Log($"{aldeano.nombre} seleccionado. Mantén R y haz clic en un edificio.");
    }

    void AsignarTrabajoEdificio()
    {
        if (aldeanoSeleccionado == null) return; // 🔥 Si no hay aldeano seleccionado, no hacer nada

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Building building = hit.collider.GetComponent<Building>();
            if (building != null)
            {
                if (curJobs < maxJobs) // ✅ Solo permitir si hay espacio para más trabajos
                {
                    // 🔥 Asigna el aldeano como trabajador en el edificio
                    if (building.CompareTag("Granja"))
                    {
                        aldeanoSeleccionado.tipo = TipoAldeano.Granjero;
                    }
                    else if (building.CompareTag("Herrería"))
                    {
                        aldeanoSeleccionado.tipo = TipoAldeano.Guerrero;
                    }
                    else
                    {
                        Debug.Log("🚫 Este edificio no permite trabajos.");
                        return;
                    }
                    building.AgregarTrabajador(); // ✅ Asegurar que el edificio registra al trabajador
                    aldeanoSeleccionado.home = building; 
                    
                    CalculateJobs();  // ✅ Incrementa trabajos ocupados
                    UpdateRecursosText();
                    UpdateStatText(); // 🔄 Actualiza UI

                    Debug.Log($"{aldeanoSeleccionado.nombre} ahora trabaja en {building.name}. Trabajos: {curJobs}/{maxJobs}");
                }
                else
                {
                    Debug.Log("🚫 No hay más trabajos disponibles.");
                }

                aldeanoSeleccionado = null; // 🔥 Deseleccionar después de asignarlo
            }
        }
    }

    public void UpdateLunaLlenaText()
    {
        if (lunaLlenaText != null)
        {
            lunaLlenaText.text = $"En {diasParaLunaLlena} días Luna Llena";
        }
    }

}