using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TiendaManager : MonoBehaviour
{
    public TextMeshProUGUI textoDinero;
    public Button botonComprar;
    public Button botonVender;
    public TMP_InputField cantidadInput;
    public RectTransform selectorVisual; 
    public GameObject panelTienda;

    private ItemTienda itemSeleccionado;
    public static TiendaManager instance;
    void Awake()
    {
        instance = this;
    }


    IEnumerator Start()
    {
        botonComprar.gameObject.SetActive(false);
        botonVender.gameObject.SetActive(false);
        cantidadInput.gameObject.SetActive(false);
        selectorVisual.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame(); // ⏱️ Espera un frame
        UpdateDineroUI(); // ✅ Ahora sí, después de que todo se haya inicializado
    }

    public void SeleccionarItem(ItemTienda item)
    {
        itemSeleccionado = item;
        
        botonComprar.gameObject.SetActive(true);
        botonVender.gameObject.SetActive(true);
        cantidadInput.gameObject.SetActive(true);
        
        selectorVisual.gameObject.SetActive(true);
        selectorVisual.position = item.GetComponent<RectTransform>().position;
    }

    public void Comprar()
    {
        int cantidad = int.Parse(cantidadInput.text);
        int costeTotal = itemSeleccionado.precioCompra * cantidad;

        if (ResourceManager.instance.dinero >= costeTotal)
        {
            ResourceManager.instance.GastarRecurso("dinero", costeTotal);
            ResourceManager.instance.AñadirRecurso(itemSeleccionado.nombreItem, cantidad);
            UpdateDineroUI();
        }
    }

    public void Vender()
    {
        int cantidad = int.Parse(cantidadInput.text);
        if (ResourceManager.instance.TieneRecurso(itemSeleccionado.nombreItem, cantidad))
        {
            int ganancias = itemSeleccionado.precioVenta * cantidad;
            ResourceManager.instance.AñadirRecurso("dinero", ganancias);
            ResourceManager.instance.GastarRecurso(itemSeleccionado.nombreItem, cantidad);
            UpdateDineroUI();
        }
    }

    public void Salir()
    {
        panelTienda.SetActive(false);
    }

    public void UpdateDineroUI()
    {
        textoDinero.text = $"{ResourceManager.instance.dinero}";
    }
    
    public bool EstaPopupAbierto()
    {
        Debug.Log("Verificando Popup: " + panelTienda.activeSelf); // 🔍 Verifica si el popup está activo
        return panelTienda.activeSelf;
    }
}