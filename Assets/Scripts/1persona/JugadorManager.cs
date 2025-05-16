using UnityEngine;

public class JugadorManager : MonoBehaviour
{
    public static Vector3 ultimaPosicion;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}