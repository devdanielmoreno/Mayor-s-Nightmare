using UnityEngine;

public class PortalClick : MonoBehaviour
{
    public Portal portalControlador;

    private void Awake()
    {
        // Si no está asignado, lo busca automáticamente en la escena
        if (portalControlador == null)
        {
            portalControlador = FindObjectOfType<Portal>();
        }
    }

    private void OnMouseDown()
    {
        if (portalControlador != null)
        {
            portalControlador.IniciarTransicion();
        }
        else
        {
            Debug.LogWarning("⚠️ No se ha encontrado ningún PortalControlador en la escena.");
        }
    }
}