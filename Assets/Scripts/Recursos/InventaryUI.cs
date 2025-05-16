using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    public GameObject inventoryPanel; // 📌 Panel del inventario

    
    public TextMeshProUGUI maderaText;
    public TextMeshProUGUI trigoText;
    public TextMeshProUGUI honorText;
    public TextMeshProUGUI dineroText;
    public TextMeshProUGUI berriText;
    public TextMeshProUGUI stoneText;

    private bool isOpen = false;
    void Awake()
    {
        instance = this;
        inventoryPanel.SetActive(false);
    }
    void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false); // 🔥 Inventario oculto al iniciar
        }
        else
        {
            Debug.LogError("⚠️ No se ha asignado el panel del inventario en el inspector.");
        }

        UpdateInventory(); // 🆕 Actualiza los valores al inicio
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
    }

    public void UpdateInventory()
    {
        if (ResourceManager.instance == null)
        {
            Debug.LogError("⚠️ No se encontró el ResourceManager. Asegúrate de que está presente en la escena.");
            return;
        }

        if (maderaText != null) maderaText.text = ResourceManager.instance.madera.ToString();
        if (trigoText != null) trigoText.text = ResourceManager.instance.trigo.ToString();
        if (honorText != null) honorText.text = ResourceManager.instance.honor.ToString();
        if (dineroText != null) dineroText.text = ResourceManager.instance.dinero.ToString();
        if (berriText != null) berriText.text = ResourceManager.instance.berri.ToString();
        if(stoneText != null) stoneText.text = ResourceManager.instance.stone.ToString();
    }
    public bool EstaMisionesAbierto()
    {
        Debug.Log("Verificando Misiones: " + inventoryPanel.activeSelf);
        return inventoryPanel.activeSelf;
    }
    
    public void ToggleInventory()
    {
        // 🔥 Si el panel de misiones está activo, lo cerramos antes de abrir el inventario
        if (MissionManager.instance.misionesPanel.activeSelf)
        {
            MissionManager.instance.CerrarMisionesPanel();
        }

        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    public void CerrarInventario()
    {
        inventoryPanel.SetActive(false);
    }
}