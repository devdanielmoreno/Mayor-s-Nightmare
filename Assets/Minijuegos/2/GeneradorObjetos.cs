using UnityEngine;

public class GeneradorObjetos : MonoBehaviour
{
    public GameObject objetoPrefab;
    public float intervalo = 1f;
    public float rangoX = 8f;

    void Start()
    {
        InvokeRepeating("GenerarObjeto", 0f, intervalo);
    }

    void GenerarObjeto()
    {
        float posX = Random.Range(-rangoX, rangoX);
        Vector3 posicion = new Vector3(posX, transform.position.y, 0);
        Instantiate(objetoPrefab, posicion, Quaternion.identity);
    }
}