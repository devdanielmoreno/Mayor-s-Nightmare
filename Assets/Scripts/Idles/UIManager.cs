using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public GameObject panelPopup;
    public RectTransform panelPopupRect; 
    public RectTransform habilidadBuenaRect;
    public RectTransform habilidadMalaRect;
    public TextMeshProUGUI nombreTexto;
    public TextMeshProUGUI habilidadBuenaTexto;
    public Image barraComida;
    public TextMeshProUGUI habilidadMalaTexto;
    public TextMeshProUGUI trabajoTexto;
    private Transform aldeanoSeguido;
    
    public TextMeshProUGUI recursosText; // 📌 Barra de Recursos
    
    void Awake()
    {
        instance = this;
        panelPopup.SetActive(false); // 🔥 Ocultar de inicio
    }

    void Update()
    {
        if (aldeanoSeguido != null)
        {
            // 🔥 Obtener la posición de la cabeza del aldeano en el mundo
            Vector3 worldPosition = aldeanoSeguido.position + new Vector3(0, 1.8f, 0); // 📌 Ajustar altura

            // 🔥 Convertir la posición mundial a coordenadas de pantalla
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            // 🛑 Comprobar si está detrás de la cámara (evita errores)
            if (screenPosition.z < 0)
            {
                panelPopup.SetActive(false); // Esconder popup si está detrás de la cámara
                return;
            }
            else
            {
                panelPopup.SetActive(true);
            }

            // 🔥 Aplicar la posición del popup en la UI
            panelPopup.transform.position = worldPosition;
            
            panelPopup.transform.LookAt(Camera.main.transform);
            panelPopup.transform.Rotate(0, 180f, 0);
            // 🔍 Debug para ver si la posición es correcta
            Debug.Log("Popup Posición (Pantalla): " + screenPosition);
        }
    }
    public void ActualizarUIGestion()
    {
        recursosText.text = string.Format(
            "Madera: {0}  Trigo: {1}  Honor: {2}  Dinero: {3}€",
            ResourceManager.instance.madera,
            ResourceManager.instance.trigo,
            ResourceManager.instance.honor,
            ResourceManager.instance.dinero
        );
    }

    public void VenderRecurso(string tipo, int cantidad)
    {
        if (ResourceManager.instance.GastarRecurso(tipo, cantidad))
        {
            ResourceManager.instance.AñadirRecurso("dinero", cantidad * 2);
            ActualizarUIGestion();
        }
    }

    public void ConstruirEdificio(string tipo)
    {
        if (tipo == "Herreria" && ResourceManager.instance.madera >= 20 && ResourceManager.instance.trigo >= 20)
        {
            ResourceManager.instance.GastarRecurso("madera", 20);
            ResourceManager.instance.GastarRecurso("trigo", 20);
            Debug.Log("⚒️ Has construido una Herrería");
        }
        else if (tipo == "Cuartel" && ResourceManager.instance.honor >= 40 && ResourceManager.instance.dinero >= 10)
        {
            ResourceManager.instance.GastarRecurso("honor", 40);
            ResourceManager.instance.GastarRecurso("dinero", 10);
            Debug.Log("🏰 Has construido un Cuartel");
        }
        else
        {
            Debug.Log("❌ No tienes suficientes recursos para construir esto.");
        }
        ActualizarUIGestion();
    }
    public void MostrarPopup(string nombre,string tipo, string habilidadBuena, string habilidadMala, Transform aldeano)
    {
        nombreTexto.text = nombre;
        habilidadBuenaTexto.text = habilidadBuena != "" ? habilidadBuena : "";
        habilidadMalaTexto.text = habilidadMala != "" ? habilidadMala : "";
        trabajoTexto.text = tipo;

        aldeanoSeguido = aldeano;
        panelPopup.SetActive(true);

        AjustarTamañoPopup(habilidadBuena, habilidadMala);
    }

    void AjustarTamañoPopup(string habilidadBuena, string habilidadMala)
    {
        int height = 2; // 📌 Tamaño base mínimo

        if (habilidadBuena != "" && habilidadMala != "") 
        {
            height = 4; // 📌 Tamaño grande para 2 habilidades
            habilidadBuenaTexto.rectTransform.anchoredPosition = new Vector2(0, -0.2f);
            barraComida.rectTransform.anchoredPosition = new Vector2(0.2f, 0);
            habilidadMalaTexto.rectTransform.anchoredPosition = new Vector2(0, -1.1f); 
            trabajoTexto.rectTransform.anchoredPosition = new Vector2(0, 0.4f);
        }
        else if (habilidadBuena != "" || habilidadMala != "")
        {
            height = 3; // 📌 Tamaño mediano si hay solo 1 habilidad
            habilidadBuenaTexto.rectTransform.anchoredPosition = new Vector2(0, -0.78f); // 📌 Centrar si solo hay 1
            barraComida.rectTransform.anchoredPosition = new Vector2(0.2f, -0.6f);
            habilidadMalaTexto.rectTransform.anchoredPosition = new Vector2(0, -0.78f); // 📌 Centrar si solo hay 1
            trabajoTexto.rectTransform.anchoredPosition = new Vector2(0, -0.2f);
        }

        panelPopupRect.sizeDelta = new Vector2(panelPopupRect.sizeDelta.x, height); // 🔥 Cambiar el tamaño
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelPopupRect);
    }
    public bool EstaPopupAbierto()
    {
        Debug.Log("Verificando Popup: " + panelPopup.activeSelf); // 🔍 Verifica si el popup está activo
        return panelPopup.activeSelf;
    }

    public void OcultarPopup()
    {
        aldeanoSeguido = null;
        panelPopup.SetActive(false);
    }
    
}