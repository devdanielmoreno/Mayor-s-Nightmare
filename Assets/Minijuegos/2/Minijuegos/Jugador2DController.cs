using UnityEngine;

public class Jugador2DController : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float moveY = Input.GetAxis("Vertical");
        transform.Translate(Vector2.up * moveY * speed * Time.deltaTime);
    }
}
