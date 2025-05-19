using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    void Start()
    {
        // Mostrar y desbloquear el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}