using UnityEngine;

public class MovimientoAleatorio : MonoBehaviour
{
    private Vector2 destino;

    void Start()
    {
        ElegirDestino();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, destino, Time.deltaTime * 2);
        if (Vector2.Distance(transform.position, destino) < 0.1f)
            ElegirDestino();
    }

    void ElegirDestino()
    {
        destino = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
    }
}
