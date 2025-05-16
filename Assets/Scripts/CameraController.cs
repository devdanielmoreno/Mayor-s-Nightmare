using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    
    public float minXRot;
    public float maxXRot;

    public float curXRot;
    
    public float minZoom;
    public float maxZoom;
    
    public float zoomSpeed;
    public float rotateSpeed;
    
    private float curZoom;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        curZoom = cam.transform.localPosition.y;

        // 📌 Obtener el objeto Terrain en la escena
        Terrain terrain = Terrain.activeTerrain;

        if (terrain != null)
        {
            // 📌 Calcular el centro del Terrain automáticamente
            float centerX = terrain.transform.position.x + terrain.terrainData.size.x / 2;
            float centerZ = terrain.transform.position.z + terrain.terrainData.size.z / 2;

            // 📌 Posicionar la cámara en el centro del Terrain
            transform.position = new Vector3(centerX, transform.position.y, centerZ);
        }
        else
        {
            Debug.LogWarning("No se encontró un Terrain en la escena. La cámara no pudo ser centrada.");
        }
    }


    void Update()
    {
        //zoom
        curZoom += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;
        curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);
        cam.transform.localPosition = Vector3.up * curZoom;
        
        //cuando mantenga el click derecho rote la camara
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            
            curXRot += -y * rotateSpeed;
            curXRot = Mathf.Clamp(curXRot, minXRot, maxXRot);
            transform.eulerAngles = new Vector3(curXRot, transform.eulerAngles.y + (x * rotateSpeed), 0);
        }
        
        //movimiento
        Vector3 forward = cam.transform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        
        Vector3 right = cam.transform.right;
        
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        
        Vector3 dir = forward * moveZ + right * moveX;
        dir.Normalize();
        
        dir *= moveSpeed * Time.deltaTime;
        
        transform.position += dir;
    }
}
