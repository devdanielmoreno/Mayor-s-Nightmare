using UnityEngine;
using UnityEngine.EventSystems;
public class Selector : MonoBehaviour
{
    private Camera cam;
    
    public static Selector instance;
    private Aldeano aldeanoSeleccionado;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        if (MenuOpciones.instance != null && MenuOpciones.instance.EstaMenuAbierto())
            return;
        if (Input.GetMouseButtonDown(0))
        {
            SeleccionarAldeanoOCasa();
        }
    }
    void SeleccionarAldeanoOCasa()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // ⛔ Evita clics en UI

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Aldeano aldeano = hit.collider.GetComponent<Aldeano>();
            if (aldeano != null)
            {
                SeleccionarAldeano(aldeano);
                return;
            }
            
            if (aldeanoSeleccionado != null)
            {
                aldeanoSeleccionado = null; // 🔥 Deseleccionamos después de mover
            }
        }
    }
    void SeleccionarAldeano(Aldeano aldeano)
    {
        aldeanoSeleccionado = aldeano;
        Debug.Log("👤 Aldeano seleccionado: " + aldeano.nombre);
        aldeano.MostrarPopup();
    }
    public Vector3 GetCurTilePosition()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return new Vector3(0, -99, 0);
        

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float rayOut = 0.0f;

        if (plane.Raycast(ray, out rayOut))
        {
            Vector3 newPos = ray.GetPoint(rayOut) - new Vector3(0.5f, 0.0f, 0.5f);
            newPos = new Vector3(Mathf.CeilToInt(newPos.x), 0.0f, Mathf.CeilToInt(newPos.z));
            return newPos;
        }
        return new Vector3(0, -99, 0);
    }
}