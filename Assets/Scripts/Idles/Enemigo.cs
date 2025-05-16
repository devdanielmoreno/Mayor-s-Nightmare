using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemigo : MonoBehaviour
{
    private Vector3 movementVector;
    private Animator animator;
    private float speed = 1.5f;
    private bool isWalking = false;
    private int maxVida = 100;
    private int vidaActual;
    private bool estaMuerto = false;
    private GameObject objetivoActual;

    public float distanciaDeteccion = 30f;
    public float distanciaAtaque = 2f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        vidaActual = maxVida;
        StartCoroutine(AI_Movement());
    }

    void Update()
    {
        if (isWalking && movementVector != Vector3.zero)
        {
            float altura = 0.5f;
            Vector3 nuevaPos = transform.position + (movementVector * speed * Time.deltaTime);
            transform.position = new Vector3(nuevaPos.x, altura, nuevaPos.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementVector), Time.deltaTime * 5f);
            animator.SetBool("Chase", true);
        }
        else
        {
            animator.SetBool("Chase", false);
        }
    }

    IEnumerator AI_Movement()
    {
        while (true)
        {
            BuscarObjetivo();

            if (objetivoActual != null)
            {
                float distancia = Vector3.Distance(transform.position, objetivoActual.transform.position);

                if (distancia > distanciaAtaque)
                {
                    movementVector = (objetivoActual.transform.position - transform.position).normalized;
                    isWalking = true;
                }
                else
                {
                    isWalking = false;
                    movementVector = Vector3.zero;
                    animator.SetTrigger("Attack");

                    if (objetivoActual.CompareTag("Aldeano"))
                    {
                        Aldeano aldeano = objetivoActual.GetComponent<Aldeano>();
                        aldeano?.RecibirDaño(20);
                    }
                    else if (objetivoActual.CompareTag("Ayuntamiento") && City.instance.diasParaLunaLlena == 0)
                    {
                        SceneManager.LoadScene("NombreDeLaEscenaDeSupervivencia");
                    }
                }
            }

            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
    }

    void BuscarObjetivo()
    {
        GameObject mejorObjetivo = null;
        float menorDistancia = Mathf.Infinity;

        bool lunaLlena = City.instance != null && City.instance.diasParaLunaLlena == 0;

        string[] prioridades = lunaLlena
            ? new[] { "Ayuntamiento", "Aldeano", "Casa", "Granja", "Herrería" }
            : new[] { "Aldeano", "Casa", "Granja", "Herrería", "Ayuntamiento" };

        foreach (string tag in prioridades)
        {
            GameObject[] posibles = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in posibles)
            {
                float distancia = Vector3.Distance(transform.position, obj.transform.position);
                if (distancia < menorDistancia && distancia < distanciaDeteccion)
                {
                    mejorObjetivo = obj;
                    menorDistancia = distancia;
                }
            }

            if (mejorObjetivo != null)
                break;
        }

        objetivoActual = mejorObjetivo;
    }

    public void RecibirDaño(int cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;
        Debug.Log("💥 Enemigo recibió daño, vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log("💀 Enemigo ha muerto.");
        estaMuerto = true;
        StopAllCoroutines();
        UIManager.instance?.OcultarPopup();
        StartCoroutine(DesaparecerTrasMuerte());
    }

    IEnumerator DesaparecerTrasMuerte()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
