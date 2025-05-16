using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public GameObject minimapUI;
    public Camera minimapCamera;

    void Start()
    {
        minimapUI.SetActive(false); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            minimapUI.SetActive(!minimapUI.activeSelf);
            minimapCamera.enabled = minimapUI.activeSelf;
        }
    }
}