using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;

    public GameObject misionesPanel;
    public TextMeshProUGUI missionText;

    private int progreso = 0; // 🔥 Número de edificios construidos en la misión actual
    private int objetivo = 1; // 📌 Cantidad requerida para completar la misión
    private int recompensa = 100; // 💰 Recompensa base

    private int misionActual = 0; // 🔥 Controla la misión en progreso
    private string tipoEdificio; // 🏡 Tipo de edificio actual

    void Awake()
    {
        instance = this;
        misionesPanel.SetActive(false);
    }

    void Start()
    {
        GenerarNuevaMision(); // 📌 Primera misión al inicio
    }

    public void AgregarEdificio(string tipo)
    {
        // 🔥 Convertir string a enum para evitar errores de nombres
        if (System.Enum.TryParse(tipo, out BuildingPreset.TipoEdificio tipoEdificio))
        {
            if (tipoEdificio.ToString() == tipoEdificio.ToString()) // 💡 Verifica con el enum correcto
            {
                progreso++;
                ActualizarMision();
            }
        }
    }


    public void ActualizarMision()
    {
        missionText.text = $"Construye {progreso}/{objetivo} {tipoEdificio}(s).\nRecompensa: {recompensa}€";

        if (progreso >= objetivo)
        {
            ResourceManager.instance.dinero += recompensa; // 💰 Dar recompensa
            missionText.text = "¡Misión completada! ✅";

            // ⏳ Esperar un poco antes de la siguiente misión
            Invoke(nameof(GenerarNuevaMision), 2f);
        }
    }

    void GenerarNuevaMision()
    {
        progreso = 0; // 🔥 Reiniciar progreso

        switch (misionActual)
        {
            case 0:
                tipoEdificio = "Casa"; objetivo = 1; recompensa = 50;
                break;
            case 1:
                tipoEdificio = "Casa"; objetivo = 3; recompensa = 100;
                break;
            case 2:
                tipoEdificio = "Granja"; objetivo = 1; recompensa = 150;
                break;
            case 3:
                tipoEdificio = "Granja"; objetivo = 3; recompensa = 100;
                break;
            case 4:
                tipoEdificio = "Herrería"; objetivo = 1; recompensa = 200;
                break;
            case 5:
                tipoEdificio = "Herrería"; objetivo = 3; recompensa = 300;
                break;
            default:
                missionText.text = "🏆 ¡Has completado todas las misiones!";
                return;
        }

        missionText.text = $"Nueva misión:\nConstruye {objetivo} {tipoEdificio}(s).\nRecompensa: {recompensa}€";
        misionActual++; // 🔥 Pasar a la siguiente misión
    }

    public bool EstaMisionesAbierto()
    {
        Debug.Log("Verificando Misiones: " + misionesPanel.activeSelf);
        return misionesPanel.activeSelf;
    }

    public void ToggleMissionPanel()
    {
        if (InventoryUI.instance.inventoryPanel.activeSelf)
        {
            InventoryUI.instance.CerrarInventario();
        }

        misionesPanel.SetActive(!misionesPanel.activeSelf);
    }

    public void CerrarMisionesPanel()
    {
        misionesPanel.SetActive(false);
    }
}
