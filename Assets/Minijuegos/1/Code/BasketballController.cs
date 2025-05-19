using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasketballController : MonoBehaviour {

    public float MoveSpeed = 10;
    public Transform Ball;
    public Transform PosDribble;
    public Transform PosOverHead;
    public Transform Arms;
    public Transform Target;

    private bool IsBallInHands = true;
    private bool IsBallFlying = false;
    private float T = 0;

    private int throwCount = 0;
    private bool isReturning = false;

    void Update() {
        if (isReturning) return;

        // Movimiento
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.position += direction * MoveSpeed * Time.deltaTime;
        if (direction != Vector3.zero)
            transform.LookAt(transform.position + direction);

        // Bola en manos
        if (IsBallInHands) {
            if (Input.GetKey(KeyCode.Space)) {
                Ball.position = PosOverHead.position;
                Arms.localEulerAngles = Vector3.right * 180;
                transform.LookAt(Target.parent.position);
            } else {
                Ball.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5));
                Arms.localEulerAngles = Vector3.right * 0;
            }

            if (Input.GetKeyUp(KeyCode.Space)) {
                IsBallInHands = false;
                IsBallFlying = true;
                T = 0;

                StartCoroutine(CountThrowAfterDelay(1.5f)); // ← esperar 1.5 segundos antes de contar
            }
        }

        // Bola en el aire
        if (IsBallFlying) {
            T += Time.deltaTime;
            float duration = 0.66f;
            float t01 = T / duration;

            Vector3 A = PosOverHead.position;
            Vector3 B = Target.position;
            Vector3 pos = Vector3.Lerp(A, B, t01);
            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * Mathf.PI);

            Ball.position = pos + arc;

            if (t01 >= 1) {
                IsBallFlying = false;
                Ball.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsBallInHands && !IsBallFlying) {
            IsBallInHands = true;
            Ball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    // ← Corutina que cuenta después de 1.5 segundos
    private IEnumerator CountThrowAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);

        throwCount++;
        Debug.Log("Lanzamiento #" + throwCount);

        if (throwCount >= 5 && !isReturning) {
            isReturning = true;
            StartCoroutine(ReturnToScene());
        }
    }

    private IEnumerator ReturnToScene() {
        yield return new WaitForSeconds(1f);
        JugadorManager.minijuego2Completado = true;
        SceneManager.LoadScene("FinalScene");
    }
}
