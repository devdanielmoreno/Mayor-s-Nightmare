using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Fnaf4CameraController : MonoBehaviour
{
    public float sensibilidadRaton = 2f;
    public float velocidadMovimiento = 2f;

    public Transform camPivot;
    public Transform puntoCentral;
    public Transform ventanaIzquierda;
    public Transform ventanaDerecha;
    public Transform armario;
    public Light luzVentanaDerecha;
    public VentanaDerechaEnemyController enemigoDerecha;

    public Light luzDerecha; 

    public Animator animadorArmarioIzq;
    public Animator animadorArmarioDer;
    public Animator animadorCamara;

    public GameObject miraUI;
    public GameObject iconoD;
    public GameObject iconoA;
    public GameObject iconoS;
    public GameObject iconoW;

    private Transform destinoActual = null;
    private float rotX = 0f;
    private float rotY = 0f;

    private bool yaMoviendose = false;
    private bool yaVolviendo = false;
    private bool armarioAbierto = false;
    private bool armarioCerradoDesdeDentro = false;
    
    [SerializeField] private bool estabaEnVentanaDerecha = false;
    [SerializeField] private bool escondidoEnVentanaDerecha = false;

    public bool EstabaEnVentanaDerecha => estabaEnVentanaDerecha;
    public bool EscondidoEnVentanaDerecha => escondidoEnVentanaDerecha;
    public bool EscondidoEnArmario => armarioCerradoDesdeDentro;
    
    [SerializeField] private bool estabaEnVentanaIzquierda = false;
    [SerializeField] private bool escondidoEnVentanaIzquierda = false;

// Propiedades públicas para que otros scripts puedan leerlo:
    public bool EstabaEnVentanaIzquierda => estabaEnVentanaIzquierda;
    public bool EscondidoEnVentanaIzquierda => escondidoEnVentanaIzquierda;

    
    private bool inputBloqueado = false;

    
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        destinoActual = puntoCentral;

        rotX = camPivot.eulerAngles.y;
        rotY = camPivot.eulerAngles.x;

        if (iconoD != null) iconoD.SetActive(false);
        if (iconoA != null) iconoA.SetActive(false);
        if (iconoS != null) iconoS.SetActive(false);
        if (iconoW != null) iconoW.SetActive(false);
        
    }

    void Update()
    {
        if (inputBloqueado) return;

        if (animadorCamara == null || !animadorCamara.enabled)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensibilidadRaton;
            float mouseY = Input.GetAxis("Mouse Y") * sensibilidadRaton;

            rotX += mouseX;
            rotY -= mouseY;
            rotY = Mathf.Clamp(rotY, -60f, 60f);

            camPivot.rotation = Quaternion.Euler(rotY, rotX, 0f);
        }

        if (destinoActual != null && (animadorCamara == null || !animadorCamara.enabled))
        {
            float distancia = Vector3.Distance(camPivot.position, destinoActual.position);

            if (distancia > 0.05f)
            {
                if (!yaMoviendose)
                {
                    yaMoviendose = true;
                    if (miraUI != null) miraUI.SetActive(false);
                }

                camPivot.position = Vector3.Lerp(camPivot.position, destinoActual.position,
                    Time.deltaTime * velocidadMovimiento);
            }
            else
            {
                camPivot.position = destinoActual.position;

                if (yaMoviendose)
                {
                    yaMoviendose = false;
                    if (miraUI != null) miraUI.SetActive(true);
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                switch (hit.collider.tag)
                {
                    case "ventanaIzquierda":
                        StartCoroutine(IrVentanaIzquierdaAnimada());
                        break;

                    case "ventanaDerecha":
                        StartCoroutine(IrVentanaDerechaAnimada());
                        break;

                    case "armario":
                        if (!yaMoviendose && !yaVolviendo)
                        {
                            if (!armarioAbierto && !armarioCerradoDesdeDentro)
                            {
                                animadorArmarioIzq.Play("ArmarioAbrirIzquierda");
                                animadorArmarioDer.Play("ArmarioAbrirDerecha");
                                armarioAbierto = true;
                            }
                            else if (armarioAbierto)
                            {
                                StartCoroutine(BloquearInputDurante(2.2f));
                                StartCoroutine(IrAlArmarioAnimado());
                            }
                        }
                        break;
                }
            }
        }

        if (estabaEnVentanaIzquierda && !escondidoEnVentanaIzquierda && Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(EsconderseVentanaIzquierda());
        }

        if (escondidoEnVentanaIzquierda && Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(NoEsconderseVentanaIzquierda());
        }

        if (estabaEnVentanaDerecha && !escondidoEnVentanaDerecha && Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(EsconderseVentanaDerecha());
        }

        if (escondidoEnVentanaDerecha && Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(NoEsconderseVentanaDerecha());
        }

        if (Input.GetKeyDown(KeyCode.S) && !yaVolviendo && !inputBloqueado
            && !escondidoEnVentanaIzquierda && !escondidoEnVentanaDerecha)
        {
            if (estabaEnVentanaIzquierda)
            {
                StartCoroutine(BloquearInputDurante(4.8f));
                StartCoroutine(VolverDesdeVentanaIzquierda());
            }
            else if (estabaEnVentanaDerecha)
            {
                StartCoroutine(BloquearInputDurante(2.55f));
                StartCoroutine(VolverDesdeVentanaDerecha());
            }
            else if (armarioCerradoDesdeDentro)
            {
                StartCoroutine(BloquearInputDurante(3.95f));
                StartCoroutine(VolverDesdeArmarioConAnimacion());
            }
            else
            {
                StartCoroutine(VolverDesdeArmario());
            }
        }

        // 🟡 ENCENDER LUZ CON W (ventana derecha + escondido)
        if (estabaEnVentanaDerecha && escondidoEnVentanaDerecha && Input.GetKeyDown(KeyCode.W))
        {
            if (luzDerecha != null)
            {
                luzDerecha.enabled = true;
                StartCoroutine(ApagarLuzDerechaTrasTiempo(3f));
            }
        }
    }

    IEnumerator ApagarLuzDerechaTrasTiempo(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        if (luzDerecha != null)
            luzDerecha.enabled = false;
    }

    IEnumerator BloquearInputDurante(float segundos)
    {
        inputBloqueado = true;
        yield return new WaitForSeconds(segundos);
        inputBloqueado = false;
    }
    IEnumerator EsconderseVentanaIzquierda()
    {
        inputBloqueado = true;
        
        animadorCamara.enabled = true;
        animadorCamara.Play("EsconderseVentanaIzquierda", 0, 0f);
        if (iconoD != null) iconoD.SetActive(false);
        if (iconoS != null) iconoS.SetActive(false);

        yield return new WaitForSeconds(1.11f); // duración de la animación

        animadorCamara.enabled = false;

        Vector3 rotacionFinal = camPivot.eulerAngles;
        rotacionFinal.x = 0f;
        camPivot.rotation = Quaternion.Euler(rotacionFinal);
        rotX = rotacionFinal.y;
        rotY = 0f;

        escondidoEnVentanaIzquierda = true;
        if (iconoA != null) iconoA.SetActive(true);
        inputBloqueado = false;
    }

    public void BloquearInput()
    {
        inputBloqueado = true;
    }

    IEnumerator NoEsconderseVentanaIzquierda()
    {
        inputBloqueado = true;
        animadorCamara.enabled = true;
        animadorCamara.Play("NoEsconderseVentanaIzquierda", 0, 0f);
        if (iconoA != null) iconoA.SetActive(false);

        yield return new WaitForSeconds(1.2f); // duración de la animación

        animadorCamara.enabled = false;

        Vector3 rotacionFinal = camPivot.eulerAngles;
        rotacionFinal.x = 0f;
        camPivot.rotation = Quaternion.Euler(rotacionFinal);
        rotX = rotacionFinal.y;
        rotY = 0f;

        escondidoEnVentanaIzquierda = false;
        if (iconoD != null && estabaEnVentanaIzquierda) iconoD.SetActive(true);
        if (iconoS != null && estabaEnVentanaIzquierda) iconoS.SetActive(true);
        
        inputBloqueado = false;
    }


    IEnumerator IrAlArmarioAnimado()
    {
        inputBloqueado = true;
        
        yaMoviendose = true;
        animadorCamara.enabled = true;
        animadorCamara.Play("IrArmario", 0, 0f);
        destinoActual = null;
        if (miraUI != null) miraUI.SetActive(false);

        yield return new WaitForSeconds(2.2f);

        animadorCamara.enabled = false;

        Vector3 rotacionFinal = camPivot.eulerAngles;
        rotacionFinal.x = 0f;
        camPivot.rotation = Quaternion.Euler(rotacionFinal);

        rotX = rotacionFinal.y;
        rotY = 0f;

        animadorArmarioIzq.Play("ArmarioCerrarIzquierda");
        animadorArmarioDer.Play("ArmarioCerrarDerecha");
        
        
        
        estabaEnVentanaDerecha = false;
        escondidoEnVentanaDerecha = false;


        armarioAbierto = false;
        armarioCerradoDesdeDentro = true;
        
        if (iconoD != null) iconoD.SetActive(false);
        if (iconoA != null) iconoA.SetActive(false);
        inputBloqueado = false;
    }

    IEnumerator IrVentanaDerechaAnimada()
    {
        inputBloqueado = true;
        animadorCamara.enabled = true;
        animadorCamara.Play("IrVentanaDerecha", 0, 0f);
        destinoActual = null;
        estabaEnVentanaDerecha = true;
        escondidoEnVentanaDerecha = false;

        if (miraUI != null) miraUI.SetActive(false);

        yield return new WaitForSeconds(2f);

        animadorCamara.enabled = false;

        Vector3 rotacionFinal = camPivot.eulerAngles;
        rotacionFinal.x = 0f;
        camPivot.rotation = Quaternion.Euler(rotacionFinal);

        rotX = rotacionFinal.y;
        rotY = 0f;

        if (iconoD != null) iconoD.SetActive(true);
        if (iconoS != null) iconoS.SetActive(true);
        if (miraUI != null) miraUI.SetActive(true);

        inputBloqueado = false;
    }

    
    IEnumerator EsconderseVentanaDerecha()
    {
        inputBloqueado = true;
        animadorCamara.enabled = true;
        animadorCamara.Play("EsconderseVentanaDerecha", 0, 0f);
        if (iconoD != null) iconoD.SetActive(false);
        if (iconoS != null) iconoS.SetActive(false);

        yield return new WaitForSeconds(1.26f);

        animadorCamara.enabled = false;

        Vector3 rotacionFinal = camPivot.eulerAngles;
        rotacionFinal.x = 0f;
        camPivot.rotation = Quaternion.Euler(rotacionFinal);
        rotX = rotacionFinal.y;
        rotY = 0f;
        
        if (iconoW != null) iconoW.SetActive(true);
        if (iconoA != null) iconoA.SetActive(true);
        escondidoEnVentanaDerecha = true;
        inputBloqueado = false;
    }

    IEnumerator NoEsconderseVentanaDerecha()
    {
        inputBloqueado = true;
        animadorCamara.enabled = true;
        animadorCamara.Play("NoEsconderseVentanaDerecha", 0, 0f);
        if (iconoA != null) iconoA.SetActive(false);
        if (iconoW != null) iconoW.SetActive(false);
        yield return new WaitForSeconds(1.50f);

        animadorCamara.enabled = false;

        Vector3 rotacionFinal = camPivot.eulerAngles;
        rotacionFinal.x = 0f;
        camPivot.rotation = Quaternion.Euler(rotacionFinal);
        rotX = rotacionFinal.y;
        rotY = 0f;

        escondidoEnVentanaDerecha = false;
        if (iconoD != null && estabaEnVentanaDerecha) iconoD.SetActive(true);
        if (iconoS != null && estabaEnVentanaDerecha) iconoS.SetActive(true);
        inputBloqueado = false;
    }



    IEnumerator IrVentanaIzquierdaAnimada()
    {
        inputBloqueado = true;
        
        animadorCamara.enabled = true;
        animadorCamara.Play("IrVentanaIzquierda", 0, 0f);
        destinoActual = null;
        estabaEnVentanaIzquierda = true;
        escondidoEnVentanaIzquierda = false;
        if (miraUI != null) miraUI.SetActive(false);

        yield return new WaitForSeconds(2.5f);

        animadorCamara.enabled = false;

        Vector3 rotacionFinal = camPivot.eulerAngles;
        rotacionFinal.x = 0f;
        camPivot.rotation = Quaternion.Euler(rotacionFinal);

        rotX = rotacionFinal.y;
        rotY = 0f;

        if (iconoD != null) iconoD.SetActive(true);
        if (iconoS != null) iconoS.SetActive(true);
        if (miraUI != null) miraUI.SetActive(true);
        
        inputBloqueado = false;
    }

    IEnumerator VolverDesdeArmarioConAnimacion()
    {
        inputBloqueado = true;
        
        yaVolviendo = true;

        animadorArmarioIzq.Play("ArmarioAbrirIzquierda");
        animadorArmarioDer.Play("ArmarioAbrirDerecha");

        yield return new WaitForSeconds(0.5f);

        animadorCamara.enabled = true;
        animadorCamara.Play("VolverArmario", 0, 0f);

        yield return new WaitForSeconds(2f);

        animadorCamara.enabled = false;

        Vector3 rotacionFinal = camPivot.eulerAngles;
        rotacionFinal.x = 0f;
        camPivot.rotation = Quaternion.Euler(rotacionFinal);

        rotX = rotacionFinal.y;
        rotY = 0f;

        animadorArmarioIzq.Play("ArmarioCerrarIzquierda");
        animadorArmarioDer.Play("ArmarioCerrarDerecha");

        armarioAbierto = false;
        armarioCerradoDesdeDentro = false;
        yaMoviendose = false;

        destinoActual = null;
        yaVolviendo = false;
        
        estabaEnVentanaDerecha = false;
        escondidoEnVentanaDerecha = false;

        if (iconoD != null) iconoD.SetActive(false);
        if (iconoA != null) iconoA.SetActive(false);
        if (miraUI != null) miraUI.SetActive(true);
        
        inputBloqueado = false;
    }

    IEnumerator VolverDesdeVentanaIzquierda()
    {
        inputBloqueado = true;
        
        yaVolviendo = true;

        animadorCamara.enabled = true;
        animadorCamara.Play("VolverVentanaIzquierda", 0, 0f);

        yield return new WaitForSeconds(3.6f);

        animadorCamara.enabled = false;

        Vector3 rotacionFinal = camPivot.eulerAngles;
        rotacionFinal.x = 0f;
        camPivot.rotation = Quaternion.Euler(rotacionFinal);

        rotX = rotacionFinal.y;
        rotY = 0f;

        destinoActual = null;
        yaVolviendo = false;
        estabaEnVentanaIzquierda = false;

        if (iconoD != null) iconoD.SetActive(false);
        if (iconoA != null) iconoA.SetActive(false);
        if (iconoS != null) iconoS.SetActive(false);
        if (miraUI != null) miraUI.SetActive(true);
        
        inputBloqueado = false;
    }

    IEnumerator VolverDesdeVentanaDerecha()
    {
        inputBloqueado = true;
        
        yaVolviendo = true;
        inputBloqueado = true;
        estabaEnVentanaDerecha = false;

        animadorCamara.enabled = true;
        animadorCamara.Play("VolverVentanaDerecha", 0, 0f);

        yield return new WaitForSeconds(2.60f);

        animadorCamara.enabled = false;

        Vector3 rotacionFinal = camPivot.eulerAngles;
        rotacionFinal.x = 0f;
        camPivot.rotation = Quaternion.Euler(rotacionFinal);

        rotX = rotacionFinal.y;
        rotY = 0f;

        destinoActual = null;
        yaVolviendo = false;
        inputBloqueado = false;

        if (iconoD != null) iconoD.SetActive(false);
        if (iconoA != null) iconoA.SetActive(false);
        if (iconoS != null) iconoS.SetActive(false);
        if (miraUI != null) miraUI.SetActive(true);
        
        inputBloqueado = false;
    }

    IEnumerator VolverDesdeArmario()
    {
        inputBloqueado = true;
        
        yaVolviendo = true;

        yield return new WaitForSeconds(0.5f);

        if (armarioCerradoDesdeDentro)
        {
            animadorArmarioIzq.Play("ArmarioAbrirIzquierda");
            animadorArmarioDer.Play("ArmarioAbrirDerecha");

            yield return new WaitForSeconds(0.5f);

            destinoActual = puntoCentral;

            yield return new WaitForSeconds(0.5f);

            animadorArmarioIzq.Play("ArmarioCerrarIzquierda");
            animadorArmarioDer.Play("ArmarioCerrarDerecha");
            
            if (iconoD != null) iconoD.SetActive(false);
            if (iconoA != null) iconoA.SetActive(false);
            estabaEnVentanaDerecha = false;
            escondidoEnVentanaDerecha = false;

            armarioAbierto = false;
            armarioCerradoDesdeDentro = false;
            yaMoviendose = false;
            
            inputBloqueado = false;
        }
        else
        {
            destinoActual = puntoCentral;
        }

        yaVolviendo = false;
    }

}
