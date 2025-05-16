using UnityEngine;

public class MovimientoPersonaje1rPersona : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (PlayerPrefs.HasKey("VolverX"))
        {
            float x = PlayerPrefs.GetFloat("VolverX");
            float y = PlayerPrefs.GetFloat("VolverY");
            float z = PlayerPrefs.GetFloat("VolverZ");
            transform.position = new Vector3(x, y, z);
            
            PlayerPrefs.DeleteKey("VolverX");
            PlayerPrefs.DeleteKey("VolverY");
            PlayerPrefs.DeleteKey("VolverZ");
        }
    }


    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        Vector3 newPos = rb.position + transform.TransformDirection(movement) * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
