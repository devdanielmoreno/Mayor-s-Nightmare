using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    private bool currentlyPlacing;
    private bool currentlyBulldozering;
    
    private BuildingPreset curBuildingPreset;
    
    private float indicatorUpdateRate = 0.05f;
    private float lastUpdateTime;
    private Vector3 curIndicatorPos;
    
    public GameObject placementIndicator;
    public GameObject xIndicator;
    
    private Vector3 cityCenter;
    private float maxBuildRadius = 400f;
    private float currentRotation; // 🔄 Rotación temporal
    private const float ROTACION_INICIAL = 0;
    
    public AudioSource sonidoConstruccion;

    void Start()
    {
        Terrain terrain = Terrain.activeTerrain; // Obtener el terreno de la escena
        if (terrain != null)
        {
            float centerX = terrain.transform.position.x + terrain.terrainData.size.x / 2;
            float centerZ = terrain.transform.position.z + terrain.terrainData.size.z / 2;

            cityCenter = new Vector3(centerX, 0, centerZ); // Centro exacto del Terrain
        }
        else
        {
            Debug.LogWarning("No se encontró un Terrain en la escena. Se usará el origen (0,0,0) como centro.");
            cityCenter = Vector3.zero;
        }
    }

    void Update ()
    {
        if (MenuOpciones.instance != null && MenuOpciones.instance.EstaMenuAbierto())
            return;
        // cancel building placement
        if(Input.GetKeyDown(KeyCode.Q))
            CancelBuildingPlacement();
            
        // called every 0.05 seconds
        if(Time.time - lastUpdateTime > indicatorUpdateRate)
        {
            lastUpdateTime = Time.time;
            curIndicatorPos = Selector.instance.GetCurTilePosition();
            if(currentlyPlacing)
                placementIndicator.transform.position = curIndicatorPos;
            else if(currentlyBulldozering)
                xIndicator.transform.position = curIndicatorPos;
        }
        
        if(Input.GetMouseButtonDown(0) && currentlyPlacing)
            PlaceBuilding();
        else if (Input.GetMouseButtonDown(0) && currentlyBulldozering)
            XFunction();
        
        if (currentlyPlacing)
        {
            if (Input.GetMouseButtonDown(2))
            {
                currentRotation -= 90f;
                placementIndicator.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
                City.instance.MostrarMensaje("se ha rotado 90 grados hacia la izquierda");
            }
        }
    }
    
    public void BeginNewBuildingPlacement (BuildingPreset preset)
    {
        currentlyPlacing = true;
        curBuildingPreset = preset;
        placementIndicator.SetActive(true);
        placementIndicator.transform.position = new Vector3(0, -99, 0);
        
        currentRotation = ROTACION_INICIAL;
        placementIndicator.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

    }
    void CancelBuildingPlacement ()
    {
        currentlyPlacing = false;
        currentlyBulldozering = false;
        placementIndicator.SetActive(false);
        xIndicator.SetActive(false);
        
    }
    
    public void ToggleBulldoze ()
    {
        currentlyBulldozering = !currentlyBulldozering;
        xIndicator.SetActive(currentlyBulldozering);
        xIndicator.transform.position = new Vector3(0, -99, 0);
    }
    
    void PlaceBuilding()  
    {  
        Debug.Log($"🔍 Intentando construir: {curBuildingPreset.tipo}");  
      
        if (IsPositionOccupied(curIndicatorPos))  
        {  
            City.instance.MostrarMensaje("No puedes construir aquí, ya hay un edificio");  
            Debug.Log("🚫 Construcción cancelada: Ya hay un edificio en esta posición.");  
            return;  
        }  
      
        float distanceToCenter = Vector3.Distance(curIndicatorPos, cityCenter);  
        if (distanceToCenter > maxBuildRadius)  
        {  
            City.instance.MostrarMensaje("No puedes construir fuera de la ciudad");  
            Debug.Log("🚫 Construcción cancelada: Fuera del área de construcción.");  
            return;  
        }  
      
        if (curBuildingPreset.jobs > 0 && City.instance.maxPopulation == 0)  
        {  
            City.instance.MostrarMensaje("No puedes colocar edificios con trabajos sin población");  
            Debug.Log("🚫 Construcción cancelada: No hay población suficiente.");  
            return;  
        }  
        int costoDinero = curBuildingPreset.cost;
        bool recursosSuficientes = true;
        if (curBuildingPreset.prefab.CompareTag("Granja"))  
        {  
            Debug.Log("🔍 Verificando madera para la granja...");  
            if (!ResourceManager.instance.GastarRecurso("madera", 10))  
            {  
                City.instance.MostrarMensaje("No tienes suficiente madera para construir una granja (necesitas 10).");  
                Debug.Log("🚫 Construcción cancelada: Falta madera.");  
                return;  
            }  
        }  
        else if (curBuildingPreset.prefab.CompareTag("Herrería"))  
        {   
            if (!ResourceManager.instance.GastarRecurso("stone", 50) && !ResourceManager.instance.GastarRecurso("madera", 30))  
            {  
                City.instance.MostrarMensaje("No tienes suficiente para construir una herreria (necesitas 50 de piedra y 30 de madera)");  
                Debug.Log("🚫 Construcción cancelada: Falta madera.");  
                return;  
            }  
        }  
        
        if (!ResourceManager.instance.GastarRecurso("dinero", costoDinero))
        {
            City.instance.MostrarMensaje("❌ No tienes suficiente dinero para construir esto.");
            Debug.Log("🚫 Falta dinero para construir.");
            recursosSuficientes = false;
        }

        if (!recursosSuficientes)
        {
            Debug.Log("❌ Cancelado por falta de recursos.");
            return;
        }
      
        Quaternion rotacionSeleccionada = Quaternion.Euler(0, currentRotation, 0);  

        Debug.Log($"✅ Construyendo {curBuildingPreset.tipo} con rotación {rotacionSeleccionada.eulerAngles.y}°");  
        GameObject buildingObj = Instantiate(curBuildingPreset.prefab, curIndicatorPos, rotacionSeleccionada);
        EliminarHierbaPorCollider(buildingObj);
        City.instance.OnPlaceBuilding(buildingObj.GetComponent<Building>());  
      
        if (sonidoConstruccion != null)  
            sonidoConstruccion.Play();  
      
        City.instance.UpdateRecursosText();  
        Debug.Log($"✅ Construcción de {curBuildingPreset.tipo} realizada correctamente.");  
      
        CancelBuildingPlacement();  
    }
    bool IsPositionOccupied(Vector3 position)
    {
        GameObject ayuntamiento = GameObject.FindGameObjectWithTag("Ayuntamiento");

        if (ayuntamiento != null)
        {
            Vector3 ayuntamientoPos = ayuntamiento.transform.position;

            // 📌 Si la posición del edificio coincide con el Ayuntamiento, bloqueamos
            if (Vector3.Distance(position, ayuntamientoPos) < 16) // 🔥 Puedes ajustar el 3f según necesites
            {
                Debug.Log("❌ No puedes construir sobre el Ayuntamiento.");
                return true;
            }
        }

        foreach (Building building in City.instance.buildings)
        {
            Collider collider = building.GetComponentInChildren<Collider>();

            if (collider != null)
            {
                Bounds buildingBounds = collider.bounds; // 🔥 Obtener los límites del edificio existente

                Bounds newBuildingBounds = new Bounds(position, buildingBounds.size); // 🔥 Crear bounds simulando el nuevo edificio

                if (buildingBounds.Intersects(newBuildingBounds)) // ✅ Verifica si los edificios se solapan
                {
                    Debug.Log($"❌ No se puede construir, colisión con: {building.name}");
                    return true;
                }
            }
        }
        return false;
    }
    
    void XFunction()
    {
        Building buildingToDestroy = City.instance.buildings.Find(x => x.transform.position == curIndicatorPos );

        if (buildingToDestroy is not null)
        {
            City.instance.OnRemoveBuilding(buildingToDestroy);
        }
    }
    void EliminarHierbaPorCollider(GameObject edificio)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null || edificio == null) return;

        TerrainData datos = terrain.terrainData;
        Vector3 terrenoPos = terrain.transform.position;

        Collider col = edificio.GetComponentInChildren<Collider>();
        if (col == null) return;

        Bounds bounds = col.bounds;

        int detalleWidth = datos.detailWidth;
        int detalleHeight = datos.detailHeight;

        // Convertir posición del mundo a posición en detalle
        int xInicio = Mathf.FloorToInt((bounds.min.x - terrenoPos.x) / datos.size.x * detalleWidth);
        int xFin    = Mathf.CeilToInt((bounds.max.x - terrenoPos.x) / datos.size.x * detalleWidth);
        int yInicio = Mathf.FloorToInt((bounds.min.z - terrenoPos.z) / datos.size.z * detalleHeight);
        int yFin    = Mathf.CeilToInt((bounds.max.z - terrenoPos.z) / datos.size.z * detalleHeight);

        // Clamp para asegurarnos de no salirnos del array
        xInicio = Mathf.Clamp(xInicio, 0, detalleWidth - 1);
        xFin = Mathf.Clamp(xFin, 0, detalleWidth);
        yInicio = Mathf.Clamp(yInicio, 0, detalleHeight - 1);
        yFin = Mathf.Clamp(yFin, 0, detalleHeight);

        int ancho = xFin - xInicio;
        int alto = yFin - yInicio;

        if (ancho <= 0 || alto <= 0)
        {
            Debug.Log($"⚠️ No se eliminó hierba: ancho o alto inválido ({ancho}x{alto})");
            return;
        }

        for (int i = 0; i < datos.detailPrototypes.Length; i++)
        {
            try
            {
                int[,] detalles = datos.GetDetailLayer(xInicio, yInicio, ancho, alto, i);

                for (int x = 0; x < ancho; x++)
                {
                    for (int y = 0; y < alto; y++)
                    {
                        detalles[x, y] = 0;
                    }
                }

                datos.SetDetailLayer(xInicio, yInicio, i, detalles);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"🚨 Error al modificar layer de detalles: {e.Message}");
            }
        }
    }
}