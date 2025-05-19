using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0f; // Duración del temblor
    public float shakeAmount = 0.7f; // Intensidad del temblor
    public float decreaseFactor = 1.0f; // Factor de disminución

    Vector3 originalPos;

    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPos;
        }
    }

    // Método público para iniciar el temblor
    public void TriggerShake(float duration)
    {
        shakeDuration = duration;
    }
}