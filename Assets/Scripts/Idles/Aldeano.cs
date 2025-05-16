using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum TipoAldeano { SinTrabajo, Granjero, Guerrero }
public class Aldeano : MonoBehaviour
{
    private Camera cam;
    private Vector3 offset;
    private Vector3 movementVector;
    private Animator animator;
    private float speed = 1.5f;
    private Rigidbody body;
    private bool arrastrando = false;
    private bool isWalking = false;
    private bool esDeNoche = false;
    private bool enCaminoACasa = false;
    private Vector3 destinoCasa;
    private LightingManager lightingManager;
    public Building home;
    private int maxVida = 100;
    private int vidaActual;
    private bool estaMuerto = false;
    private Arbol arbolObjetivo;
    public string nombre;
    public string habilidadBuena;
    public string habilidadMala;
    private bool estaTalando = false; 
    private bool trabajando = false; 
    private float tiempoTrabajando = 0f; 
    private GameObject objetoTrabajo;
    
    private float comida = 100f;
    public Image barraComida;
    
    private Building edificioCercano;
    
    private static List<string> nombresDisponibles = new List<string>()
    {
        "Osvaldo", "Pedro", "Elena", "Carlos", "Ana", "Miguel", "Sofía", "David", "Paula", "Danixel", "Dani", "Saskar", "Aroa", "Max", "Colipo", "Node", "Cid", "Gamba"
    };

    private static List<string> habilidadesBuenas = new List<string>()
    {
        "GoodFarmer (+1 trigo)", "StrongFighter (+1 honor)", "LowConsumption (Consume - comida)"
    };

    private static List<string> habilidadesMalas = new List<string>()
    {
        "BadFarmer (-1 trigo)", "WeakFighter (-1 honor)", "HighConsumption (Consume + comida)"
    };
    public TipoAldeano tipo = TipoAldeano.SinTrabajo;
    
    [Obsolete("Obsolete")]
    void Start()
    {
        cam = Camera.main;
        animator = transform.GetChild(0).GetComponent<Animator>();
        lightingManager = FindObjectOfType<LightingManager>();
        body = GetComponent<Rigidbody>();
        
        vidaActual = maxVida;
        StartCoroutine(ConsumirComida());
        StartCoroutine(AI_Movement());
        if (home == null) // 📌 Asegurar que el aldeano tiene una casa asignada
        {
            home = City.instance.ObtenerCasaMasCercana(transform.position);
        }
        if (lightingManager != null)
        {
            lightingManager.RegistrarAldeano(this);
        }
        AsignarNombre();
        AsignarHabilidades();
    }
    
    void Update()
    {
        if (trabajando)
        {
            tiempoTrabajando -= Time.deltaTime; // ⏳ Reduce el tiempo restante
        
            if (tiempoTrabajando <= 0f)
            {
                CompletarTrabajo(); // ✅ Ejecuta la acción al terminar
            }
        
            return; // 🚧 Evita que haga otras cosas mientras trabaja
        }
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemigo");
        foreach (GameObject enemigo in enemigos)
        {
            float distancia = Vector3.Distance(transform.position, enemigo.transform.position);
            if (distancia < 4f)
            {
                // 🚨 Huir del enemigo
                Vector3 direccionHuida = (transform.position - enemigo.transform.position).normalized;
                movementVector = direccionHuida;
                isWalking = true;
                animator.SetBool("Walking", true);

                Vector3 movimiento = direccionHuida * speed;
                movimiento.y = 0f; // 🔥 Evita que "vuele"
                body.linearVelocity = movimiento;

                // 🔄 Rotar sin inclinarse
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direccionHuida),
                    Time.deltaTime * 5f
                );

                // 💥 Corregir inclinación en eje X
                Vector3 rot = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(0f, rot.y, 0f); // 🔒 Fijar X y Z

                return; // Evita hacer más lógica ese frame
            }
        }

        if (estaTalando) return;
        bool ahoraEsDeNoche = (lightingManager.timeOfDay >= 18 || lightingManager.timeOfDay <= 6);

        // Detección de cambio de estado día/noche
        if (ahoraEsDeNoche != esDeNoche)
        {
            esDeNoche = ahoraEsDeNoche;
            
            if (esDeNoche)
            {
                StopAllCoroutines();
                enCaminoACasa = true;
                GoHomeAndDisappear();
            }
            else
            {
                Debug.Log("☀️ Amanece: reapareciendo aldeanos...");
                Reappear();
            }
            if (!esDeNoche && !gameObject.activeSelf) 
            {
                Debug.Log("⚡ Forzando reactivación del aldeano...");
                gameObject.SetActive(true);
                Reappear();
            }
        }

        if (isWalking && movementVector != Vector3.zero)
        {
            body.linearVelocity = movementVector * speed;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementVector), Time.deltaTime * 5f);
            animator.SetBool("Walking", true);  // 💡 Activar animación de caminar
        }
        else
        {
            body.linearVelocity = Vector3.zero;
            animator.SetBool("Walking", false); // 💡 Desactivar caminar -> pasará a Idle automáticamente
        }
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            IniciarArrastre();
        }

        if (arrastrando)
        {
            MoverAldeano();
        }

        if (Input.GetKeyUp(KeyCode.R)) 
        {
            SoltarAldeano();
        }

    }
    IEnumerator ConsumirComida()
    {
        while (!estaMuerto)
        {
            yield return new WaitForSeconds(15f);
            if (esDeNoche) continue;

            float consumo = 5f;
            if (habilidadMala.Contains("HighConsumption")) consumo += 2f;
            if (habilidadBuena.Contains("LowConsumption")) consumo -= 2f;

            comida -= consumo;
            ActualizarBarraComida();

            if (comida < 20)
            {
                if (ResourceManager.instance.trigo > 0)
                {
                    ResourceManager.instance.GastarRecurso("trigo", 1);
                    comida += 30f;
                    City.instance.MostrarMensaje($"{nombre} ha comido trigo");
                }
                else if (ResourceManager.instance.berri > 0)
                {
                    ResourceManager.instance.GastarRecurso("berri", 1);
                    comida += 15f;
                    City.instance.MostrarMensaje($"{nombre} ha comido berris");
                }
            }

            if (comida <= 0)
            {
                Morir();
                yield break;
            }

            comida = Mathf.Clamp(comida, 0f, 100f);
        }
    }
    void ActualizarBarraComida()
    {
        if (barraComida != null)
        {
            barraComida.fillAmount = comida / 100f;
        }
    }

    public void AsignarArbol(Arbol arbol)
    {
        arbolObjetivo = arbol;
    }

    void IniciarTrabajo(GameObject objeto, float tiempo)
    {
        if (trabajando) return; // 🚧 Evita interrumpir un trabajo en curso

        City.instance.MostrarMensaje($"{nombre} comenzará a trabajar por {tiempo} segundos");

        StopAllCoroutines();
        movementVector = Vector3.zero;
        body.linearVelocity = Vector3.zero;
        body.isKinematic = true;
        animator.SetBool("Walking", false);
        estaTalando = true;

        trabajando = true;
        tiempoTrabajando = tiempo;
        objetoTrabajo = objeto;
    }

    void CompletarTrabajo()
    {
        if (objetoTrabajo == null)
        {
            trabajando = false;
            estaTalando = false;
            Debug.Log($"⚠️ Error: {nombre} no tenía un objeto asignado para trabajar");
            return;
        }

        City.instance.MostrarMensaje($"{nombre} ha terminado de trabajar");

        if (objetoTrabajo.CompareTag("Arbol"))
        {
            objetoTrabajo.GetComponent<Arbol>()?.Cortar();
        }
        else if (objetoTrabajo.CompareTag("Berris"))
        {
            objetoTrabajo.GetComponent<Berri>()?.Cortar();
        }
        else if (objetoTrabajo.CompareTag("Piedra"))
        {
            objetoTrabajo.GetComponent<Roca>()?.Picar();
        }

        trabajando = false;
        estaTalando = false;
        objetoTrabajo = null;
        body.isKinematic = false;
        StartCoroutine(AI_Movement()); // 🔄 Retomar movimiento normal
    }

    public void RecolectarBerris(GameObject arbusto)
    {
        IniciarTrabajo(arbusto, 10f);
    }

    public void RecolectarRoca(GameObject roca)
    {
        IniciarTrabajo(roca, 50f);
    }

    public void TalarArbol(GameObject arbol)
    {
        IniciarTrabajo(arbol, 25f);
    }

    void IniciarArrastre()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject) 
            {
                arrastrando = true;
            }
        }
    }
    void MoverAldeano()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 nuevaPosicion = hit.point;
            nuevaPosicion.y = 1.9f; // 🔥 Mantenerlo en la altura correcta
            transform.position = nuevaPosicion;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void SoltarAldeano()
    {
        arrastrando = false;

        float radioDeteccion = 2f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radioDeteccion);

        GameObject arbolCercano = null; // Variable para detectar árbol cercano
        GameObject berriCercano = null;
        GameObject piedraCercana = null;

        foreach (Collider col in colliders)
        {
            Debug.Log($"🟢 Objeto detectado: {col.name}, Tag: {col.tag}");

            // 🏡 Verificar si es un edificio
            Building building = col.GetComponent<Building>();
            if (building != null)
            {
                Debug.Log($"🏠 Edificio detectado correctamente: {building.name}");
                AsignarTrabajo(building);
                return;
            }

            // 🌳 Verificar si es un árbol con la tag "Arbol"
            if (col.CompareTag("Arbol"))
            {
                if (Vector3.Distance(transform.position, col.transform.position) <= 5f)
                {
                    arbolCercano = col.gameObject;
                }
            }
            else if (col.CompareTag("Berris")) // 🍇 Detecta arbustos Berri
            {
                berriCercano = col.gameObject;
            }
            else if (col.CompareTag("Piedra")) // 🍇 Detecta arbustos Berri
            {
                piedraCercana = col.gameObject;
            }
        }

        // 🌲 Si hay un árbol cercano, iniciar tala
        if (arbolCercano != null)
        {
            Debug.Log($"🪓 Árbol detectado cerca: {arbolCercano.name}. Iniciando tala...");
            TalarArbol(arbolCercano); // ✅ Llamamos directamente a la nueva versión
            return;
        }
        else if (berriCercano != null)
        {
            Debug.Log($"🍇 Arbusto Berri detectado: {berriCercano.name}. Recolectando...");
            RecolectarBerris(berriCercano); // ✅ Llamamos directamente a la nueva versión
            return;
        }
        else if (piedraCercana != null)
        {
            Debug.Log($"🪨 Piedra detectada cerca: {piedraCercana.name}. Recolectando...");
            RecolectarRoca(piedraCercana); // ✅ Llamamos directamente a la nueva versión
            return;
        }

        Debug.Log($"❌ No se detectó ningún edificio ni objeto para trabajar cerca de {nombre}");
    }

    void AsignarTrabajo(Building building)
    {
        if (!building.TieneEspacioDisponible()) 
        {
            City.instance.MostrarMensaje("ya tiene 2 trabajadores. Construye otro edificio");
            return;
        }
        if (building.CompareTag("Granja"))
        {
            tipo = TipoAldeano.Granjero;
            Debug.Log($"{nombre} ahora es un Granjero. Tipo actual: {tipo}");
        }
        else if (building.CompareTag("Herrería"))
        {
            tipo = TipoAldeano.Guerrero;
            Debug.Log($"{nombre} ahora es un Guerrero. Tipo actual: {tipo}");
        }
        else
        {
            Debug.Log($"{nombre} no puede trabajar en {building.name}");
            return;
        }
        building.AgregarTrabajador();
        home = building;
        City.instance.CalculateJobs();  
        City.instance.UpdateStatText(); 
        MostrarPopup();
    }


    void AsignarNombre()
    {
        if (nombresDisponibles.Count > 0)
        {
            int index = Random.Range(0, nombresDisponibles.Count);
            nombre = nombresDisponibles[index];
            nombresDisponibles.RemoveAt(index); // 🔥 Evita repetir nombres
        }
        else
        {
            nombre = "Aldeano " + Random.Range(100, 999);
        }
    }

    void AsignarHabilidades()
    {
        int chance = Random.Range(0, 100); // Probabilidad para definir habilidades

        if (chance < 25) // 🎲 25% de probabilidad de ser NEUTRAL
        {
            habilidadBuena = "No tiene habilidad";
            habilidadMala = "";
        }
        else if (chance < 50) // 🎲 25% de probabilidad de tener SOLO 1 HABILIDAD BUENA
        {
            habilidadBuena = habilidadesBuenas[Random.Range(0, habilidadesBuenas.Count)];
            habilidadMala = "";
        }
        else if (chance < 75) // 🎲 25% de probabilidad de tener SOLO 1 HABILIDAD MALA
        {
            habilidadBuena = "";
            habilidadMala = habilidadesMalas[Random.Range(0, habilidadesMalas.Count)];
        }
        else // 🎲 25% de probabilidad de tener 1 BUENA y 1 MALA (sin contradicciones)
        {
            bool habilidadAsignada = false;

            while (!habilidadAsignada)
            {
                string buena = habilidadesBuenas[Random.Range(0, habilidadesBuenas.Count)];
                string mala = habilidadesMalas[Random.Range(0, habilidadesMalas.Count)];

                if (!SonContradictorias(buena, mala))
                {
                    habilidadBuena = buena;
                    habilidadMala = mala;
                    habilidadAsignada = true;
                }
            }
        }
    }

// 📌 **Método para evitar habilidades contradictorias**
    bool SonContradictorias(string buena, string mala)
    {
        return (buena.Contains("Farmer") && mala.Contains("Farmer")) ||
               (buena.Contains("Fighter") && mala.Contains("Fighter")) ||
               (buena.Contains("Consumption") && mala.Contains("Consumption"));
    }

    public void MostrarPopup()
    {
        string tipoTexto = tipo switch
        {
            TipoAldeano.Granjero => "Granjero",
            TipoAldeano.Guerrero => "Guerrero",
            _ => "Sin Trabajo"
        };

        UIManager.instance.MostrarPopup(nombre, tipoTexto,habilidadBuena, habilidadMala, transform);
    }


    IEnumerator AI_Movement()
    {
        while (true)
        {
            if (!esDeNoche) // 📌 Solo permite movimiento aleatorio de día
            {
                isWalking = Random.Range(0, 2) == 1;

                if (isWalking)
                {
                    float moveX = Random.Range(-1f, 1f);
                    float moveZ = Random.Range(-1f, 1f);
                    movementVector = new Vector3(moveX, 0, moveZ).normalized;
                }
                else
                {
                    movementVector = Vector3.zero;
                    animator.SetBool("Walking", false);
                }
            }
            else
            {
                isWalking = false; // 📌 Si es de noche, se asegura de detenerse
                animator.SetBool("Walking", false);
                movementVector = Vector3.zero;
            }

            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }
    public void RecibirDaño(int cantidad)
    {
        if (estaMuerto) return; // 🔥 Si ya está muerto, no recibe más daño

        vidaActual -= cantidad;
        Debug.Log("💥 Aldeano recibió daño, vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }
    void Morir()
    {
        Debug.Log("💀 Aldeano ha muerto...");

        estaMuerto = true;
        StopAllCoroutines();

        UIManager.instance.OcultarPopup();
        
        StartCoroutine(DesaparecerTrasMuerte());
    }
    IEnumerator DesaparecerTrasMuerte()
    {
        yield return new WaitForSeconds(2f);  // ⏳ Espera 2 segundos antes de eliminarlo
        Destroy(gameObject);  // 🚮 Elimina al aldeano del juego
    }
    public void GoHomeAndDisappear()
    {
        if (estaMuerto) return;
        if (home != null)
        {
            destinoCasa = home.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 1.9f, Random.Range(-0.5f, 0.5f));

            vidaActual = maxVida;
            Debug.Log("🌙 Aldeano se cura al máximo: " + vidaActual);
            // 📌 Desactivar colisiones y física antes de moverse a casa
            if (body != null)
            {
                body.isKinematic = true;  // 🔥 Evita que las físicas lo afecten
                body.linearVelocity = Vector3.zero; // 🔥 Asegurar que no se quede con velocidad residual
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;  // 🔥 Desactiva el collider para evitar choques
            }

            Debug.Log("Destino Casa: " + destinoCasa);
            StartCoroutine(MoveToHomeAndDisappear());
        }
    }


    IEnumerator MoveToHomeAndDisappear()
    {
        while (Vector3.Distance(transform.position, destinoCasa) > 1.5f) // 🔥 Bajamos la distancia de parada
        {
            Debug.Log("Distancia a casa: " + Vector3.Distance(transform.position, destinoCasa));

            Vector3 direccion = (destinoCasa - transform.position).normalized;

            float alturaFinal = 1.9f;
            // 📌 Si el Rigidbody es Kinematic, movemos con transform.position
            if (body.isKinematic)
            {
                transform.position = Vector3.MoveTowards(
                    new Vector3(transform.position.x, alturaFinal, transform.position.z), 
                    new Vector3(destinoCasa.x, alturaFinal, destinoCasa.z), 
                    speed * Time.deltaTime
                );
            }
            else
            {
                body.linearVelocity = new Vector3(direccion.x * speed, 0, direccion.z * speed);
            }

            // 📌 Rotar el aldeano para que siempre mire en la dirección de movimiento
            if (direccion != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            animator.SetBool("Walking", true);
            yield return null;
        }

        // 📌 Detener movimiento completamente antes de desaparecer
        if (!body.isKinematic) 
        {
            body.linearVelocity = Vector3.zero;
        }
        animator.SetBool("Walking", false);

        Debug.Log("Aldeano desapareciendo...");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }


    public void Reappear()
    {
        if (home != null)
        {
            Debug.Log("Reapareciendo aldeano en la mañana...");

            float alturaFinal = 1.9f;
            float offsetX = Random.Range(-2f, 2f);
            float offsetZ = Random.Range(-2f, 2f);
            Vector3 spawnPos = new Vector3(home.transform.position.x + offsetX, alturaFinal, home.transform.position.z - 4f + offsetZ);

            transform.position = spawnPos;
            gameObject.SetActive(true);

            // 📌 Reactivar físicas y colisiones
            if (body != null)
            {
                body.isKinematic = false;  // 🔥 Reactivar físicas
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;  // 🔥 Volver a activar colisiones
            }

            StopAllCoroutines();
            StartCoroutine(AI_Movement());
        }
    }
}