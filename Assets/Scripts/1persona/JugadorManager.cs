using UnityEngine;

public class JugadorManager : MonoBehaviour
{
    public static Vector3 ultimaPosicion = Vector3.zero;
    public static bool volverDesdeMinijuego = false;

    public static bool minijuego1Completado = false;
    public static bool minijuego2Completado = false;
    public static bool minijuego3Completado = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}