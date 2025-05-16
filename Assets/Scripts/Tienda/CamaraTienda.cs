using UnityEngine;

public class CamaraTienda : MonoBehaviour
{

    public float moveSpeed;
    
    public float minXRot;
    public float maxXRot;

    public float curXRot;
    public float curYRot;
    
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
    }
    
    void Update()
    {
        curZoom += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;
        curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);
        cam.transform.localPosition = Vector3.up * curZoom;

        //cuando mantenga el click derecho rote la camara
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            curXRot += -y * rotateSpeed;
            curYRot += x * rotateSpeed;

            curXRot = Mathf.Clamp(curXRot, minXRot, maxXRot);
            transform.rotation = Quaternion.Euler(curXRot, curYRot, 0);

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
