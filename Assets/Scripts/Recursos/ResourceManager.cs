using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public int madera;
    public int trigo;
    public int honor;
    public int dinero; // 💰 Nuevo sistema de dinero
    public int berri;
    public int stone;

    void Awake()
    {
        instance = this;
    }
    
    public bool TieneRecurso(string recurso, int cantidad)
    {
        switch (recurso)
        {
            case "madera": return madera >= cantidad;
            case "trigo": return trigo >= cantidad;
            case "honor": return honor >= cantidad;
            case "dinero": return dinero >= cantidad;
            case "berri": return berri >= cantidad;
            case "stone": return stone >= cantidad;
            default: return false;
        }
    }


    public void AñadirRecurso(string recurso, int cantidad)
    {
        switch (recurso)
        {
            case "madera": madera += cantidad; break;
            case "trigo": trigo += cantidad; break;
            case "honor": honor += cantidad; break;
            case "dinero": dinero += cantidad; break;
            case "berri": berri += cantidad; break;
            case "stone": stone += cantidad; break;
        }
        InventoryUI.instance.UpdateInventory(); // 🔥 Actualiza la UI del inventario al añadir recursos
    }

    public bool GastarRecurso(string recurso, int cantidad)
    {
        bool resultado = false;

        switch (recurso)
        {
            case "madera":
                if (madera >= cantidad) { madera -= cantidad; resultado = true; }
                break;
            case "trigo":
                if (trigo >= cantidad) { trigo -= cantidad; resultado = true; }
                break;
            case "honor":
                if (honor >= cantidad) { honor -= cantidad; resultado = true; }
                break;
            case "dinero":
                if (dinero >= cantidad) { dinero -= cantidad; resultado = true; }
                break;
            case "berri":
                if (berri >= cantidad) { berri -= cantidad; resultado = true; }
                break;
            case "stone":
                if (stone >= cantidad) { stone -= cantidad; resultado = true; }
                break;
        }

        InventoryUI.instance.UpdateInventory(); // 🔥 Actualiza la UI
        return resultado;
    }
}
