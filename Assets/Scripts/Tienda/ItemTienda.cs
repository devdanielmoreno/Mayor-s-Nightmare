using UnityEngine;
using UnityEngine.UI;

public class ItemTienda : MonoBehaviour
{
    public string nombreItem;
    public int precioCompra;
    public int precioVenta;

    private TiendaManager tienda;

    void Start()
    {
        tienda = FindObjectOfType<TiendaManager>();
    }

    public void Seleccionar()
    {
        tienda.SeleccionarItem(this);
    }
    
}