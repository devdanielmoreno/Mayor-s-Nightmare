using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CargaEscena : MonoBehaviour
{
    public Slider barraProgreso;
    public string escenaDestino;
    
    void Start()
    {
        StartCoroutine(CargarEscenaAsync());
    }

    IEnumerator CargarEscenaAsync()
    {
        AsyncOperation operacion = SceneManager.LoadSceneAsync(escenaDestino);
        operacion.allowSceneActivation = false;

        float progresoMostrado = 0f; 

        while (!operacion.isDone)
        {
            float progresoReal = Mathf.Clamp01(operacion.progress / 0.9f);
            
            while (progresoMostrado < progresoReal)
            {
                progresoMostrado += Time.deltaTime * 0.5f; 
                barraProgreso.value = progresoMostrado;
                yield return null;
            }

            if (operacion.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f); 
                operacion.allowSceneActivation = true; 
            }

            yield return null;
        }
    }
}