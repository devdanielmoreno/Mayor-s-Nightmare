using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;
    public GameObject gestionDiariaUI; // 📌 Menú de gestión de recursos

    private bool isPaused = false;

    void Awake()
    {
        instance = this;
    }

    public void EndDay()
    {
        Time.timeScale = 0; // ⏸️ Pausar el juego
        isPaused = true;
        gestionDiariaUI.SetActive(true); // 🔥 Mostrar la pantalla de gestión

        UIManager.instance.ActualizarUIGestion();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // ▶️ Reanudar el juego
        isPaused = false;
        gestionDiariaUI.SetActive(false);
        City.instance.EndTurn();
    }
}

