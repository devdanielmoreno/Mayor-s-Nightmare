using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    private Transform ayuntamiento;
    public bool ataqueEnProgreso = false;

    public GameObject enemigoPrefab;
    public int cantidadNormal = 5;
    public int cantidadLunaLlena = 50;
    public float radioSpawn = 100f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GameObject ayuntamientoObj = GameObject.FindGameObjectWithTag("Ayuntamiento");
        if (ayuntamientoObj != null)
        {
            ayuntamiento = ayuntamientoObj.transform;
        }
        else
        {
            Debug.LogError("⚠️ No se encontró el Ayuntamiento. Asegúrate de que tenga el tag 'Ayuntamiento'.");
        }
        
    }

    void Update()
    {
        if (!ataqueEnProgreso && DetectarInvasion())
        {
            Debug.Log("⚠️ ¡Invasión detectada cerca del ayuntamiento!");

            if (City.instance.diasParaLunaLlena == 0)
            {
                Debug.Log("🌕 ¡Luna llena! Activando supervivencia.");
                ataqueEnProgreso = true;
                GameManager.instance.IniciarSupervivencia();
            }
        }
    }

    public void SpawnOleadaEnemigos()
    {
        if (ayuntamiento == null) return;

        // 💡 Solo spawnea si es de noche
        bool esDeNoche = !(City.instance.lightingManager.timeOfDay > 6 && City.instance.lightingManager.timeOfDay < 18);
        bool esLunaLlena = City.instance.diasParaLunaLlena == 0;

        if (!esDeNoche && !esLunaLlena) return; // No spawnear salvo que sea de noche o luna llena

        int cantidad = esLunaLlena ? cantidadLunaLlena : cantidadNormal;

        for (int i = 0; i < cantidad; i++)
        {
            Vector3 spawnPos = GenerarPosicionAleatoriaFueraCiudad();
            Instantiate(enemigoPrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log($"🧟‍♂️ Oleada generada: {cantidad} enemigos {(esLunaLlena ? "(luna llena 🌕)" : "")}");
    }


    Vector3 GenerarPosicionAleatoriaFueraCiudad()
    {
        Vector3 centro = new Vector3(373f, 0f, 308f); // Centro personalizado
        Terrain terreno = Terrain.activeTerrain;

        for (int intentos = 0; intentos < 20; intentos++)
        {
            float distancia = Random.Range(radioSpawn * 0.8f, radioSpawn);
            float angulo = Random.Range(0f, 360f);
            float x = centro.x + Mathf.Cos(angulo * Mathf.Deg2Rad) * distancia;
            float z = centro.z + Mathf.Sin(angulo * Mathf.Deg2Rad) * distancia;

            if (terreno != null && x >= terreno.transform.position.x && x <= terreno.transform.position.x + terreno.terrainData.size.x &&
                z >= terreno.transform.position.z && z <= terreno.transform.position.z + terreno.terrainData.size.z)
            {
                float y = terreno.SampleHeight(new Vector3(x, 0, z)) + 0.5f;
                return new Vector3(x, y, z);
            }
        }

        Debug.LogWarning("⚠️ No se pudo generar una posición válida para enemigo. Usando fallback.");
        return centro + new Vector3(radioSpawn, 1, 0);
    }

    bool DetectarInvasion()
    {
        if (ayuntamiento == null) return false;

        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemigo");
        foreach (GameObject enemigo in enemigos)
        {
            if (Vector3.Distance(enemigo.transform.position, ayuntamiento.position) < 17f)
                return true;
        }
        return false;
    }
}
