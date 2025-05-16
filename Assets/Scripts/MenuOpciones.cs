using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuOpciones : MonoBehaviour
{
    public static MenuOpciones instance;

    public GameObject menuPanel;
    public GameObject controlesPanel; // Panel de controles
    public Slider musicaSlider;
    public AudioSource audioSource;
    private bool menuAbierto = false;
    private bool enControles = false; // 🔥 Indica si estamos viendo los controles

    void Awake()
    {
        instance = this;
        menuPanel.SetActive(false);
        controlesPanel.SetActive(false); // 🔥 Ocultar controles al inicio

        // 🔥 Establecer volumen en la mitad al inicio
        musicaSlider.value = 0.2f;
        audioSource.volume = musicaSlider.value;

        // 🔥 Asegurar que el slider actualiza el volumen en tiempo real
        musicaSlider.onValueChanged.AddListener(delegate { BajarMusica(); });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 📌 🔥 Primero, verificar si hay un popup de aldeano abierto
            if (UIManager.instance != null && UIManager.instance.EstaPopupAbierto())
            {
                UIManager.instance.OcultarPopup(); // 🔥 Cierra el popup en vez de abrir ajustes
                return;
            }

            // 📌 🔥 Si el panel de misiones está abierto, lo cerramos en vez de abrir ajustes
            if (MissionManager.instance != null && MissionManager.instance.EstaMisionesAbierto())
            {
                MissionManager.instance.CerrarMisionesPanel();
                return;
            }
            if (TiendaManager.instance != null && TiendaManager.instance.EstaPopupAbierto())
            {
                TiendaManager.instance.Salir();
                return;
            }
            
            if (InventoryUI.instance != null && InventoryUI.instance.EstaMisionesAbierto())
            {
                InventoryUI.instance.CerrarInventario();
                return;
            }

            // 📌 🔥 Si estamos en los controles, volvemos al menú de opciones
            if (menuAbierto && enControles)
            {
                VolverAlMenu();
                return;
            }

            // 📌 🔥 Si ya está el menú de ajustes abierto, lo cerramos
            if (menuAbierto)
            {
                CerrarMenu();
            }
            else
            {
                AbrirMenu();
            }
        }
    }


    public void AbrirMenu()
    {
        menuPanel.SetActive(true);
        controlesPanel.SetActive(false); // 🔥 Asegurar que no se abren juntos
        Time.timeScale = 0f; // 🔥 PAUSAR EL JUEGO TOTALMENTE
        menuAbierto = true;
        enControles = false;

        // 🔥 Bloquear inputs del jugador
        BloquearJugador(true);
    }

    public void CerrarMenu()
    {
        menuPanel.SetActive(false);
        controlesPanel.SetActive(false);
        Time.timeScale = 1f; // 🔥 Reanudar el juego
        menuAbierto = false;
        enControles = false;

        // 🔥 Desbloquear inputs del jugador
        BloquearJugador(false);
    }

    public void BajarMusica()
    {
        audioSource.volume = musicaSlider.value;
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");
        
        UnityEditor.EditorApplication.isPlaying = false; // 🔥 Solo en el editor de Unity

        Application.Quit();

    }


    public void MostrarControles()
    {
        menuPanel.SetActive(false);
        controlesPanel.SetActive(true);
        enControles = true; // 🔥 Indicar que estamos en el panel de controles
    }

    public void VolverAlMenu()
    {
        controlesPanel.SetActive(false);
        menuPanel.SetActive(true);
        enControles = false; // 🔥 Ya no estamos en el panel de controles
    }

    private void BloquearJugador(bool estado)
    {
        if (FindObjectOfType<CameraController>() != null)
        {
            FindObjectOfType<CameraController>().enabled = !estado;
        }
    }
    public bool EstaMenuAbierto()
    {
        return menuAbierto;
    }

}
