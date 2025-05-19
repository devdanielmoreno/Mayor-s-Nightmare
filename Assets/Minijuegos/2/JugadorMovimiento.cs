using UnityEngine;

public class JugadorMovimiento : MonoBehaviour
{
    public float velocidad = 5f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float movimiento = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;
        transform.Translate(movimiento, 0, 0);
        
        if (Input.GetAxis("Horizontal") > 0.01f)
            spriteRenderer.flipX = true;  
        else if (Input.GetAxis("Horizontal") < -0.01f)
            spriteRenderer.flipX = false; 
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ObjetoCayendo"))
        {
            FindObjectOfType<ControladorJuego>().FinDelJuego();
        }
    }
}