using UnityEngine;

public class TiendaInteractiva : MonoBehaviour
{
    public GameObject panelTienda; 

    private bool tiendaAbierta = false;

    void Start()
    {
        panelTienda.SetActive(false);
    }
    void OnMouseDown()
    {
        if (panelTienda != null)
        {
            panelTienda.SetActive(true); // 👁️ Mostrar la tienda
            tiendaAbierta = true;
            
            if (TiendaManager.instance != null)
            {
                TiendaManager.instance.UpdateDineroUI();
            }
        }
    }

    void Update()
    {
        if (tiendaAbierta && Input.GetKeyDown(KeyCode.Escape))
        {
            panelTienda.SetActive(false);
            tiendaAbierta = false;
        }
    }
    public bool EstaPopupAbierto()
    {
        Debug.Log("Verificando Popup: " + panelTienda.activeSelf); // 🔍 Verifica si el popup está activo
        return panelTienda.activeSelf;
    }
    public void Salir()
    {
        panelTienda.SetActive(false);
    }
}