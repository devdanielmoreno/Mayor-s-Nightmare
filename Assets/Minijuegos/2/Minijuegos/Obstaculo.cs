using UnityEngine;

public class Obstaculo : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        if (transform.position.x < -10f)
            Destroy(gameObject);
    }
}