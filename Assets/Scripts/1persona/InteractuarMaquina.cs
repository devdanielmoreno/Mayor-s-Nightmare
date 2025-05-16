using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractuarMaquina : MonoBehaviour
{
    public float distanciaInteraccion = 3f;
    public string nombreTagMaquina = "Maquina";
    public string nombreTagMaquina2 = "Maquina2";
    public string nombreTagMaquina3 = "Maquina3";

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
            {
                if (hit.collider.CompareTag(nombreTagMaquina))
                {
                    JugadorManager.ultimaPosicion = GameObject.FindWithTag("Jugador").transform.position;
                    SceneManager.LoadScene("MiniJuego1");
                }
                if (hit.collider.CompareTag(nombreTagMaquina2))
                {
                    JugadorManager.ultimaPosicion = GameObject.FindWithTag("Jugador").transform.position;
                    SceneManager.LoadScene("MiniJuego2");
                }
                if (hit.collider.CompareTag(nombreTagMaquina3))
                {
                    JugadorManager.ultimaPosicion = GameObject.FindWithTag("Jugador").transform.position;
                    SceneManager.LoadScene("MiniJuego3");
                }
            }
        }
    }
}
