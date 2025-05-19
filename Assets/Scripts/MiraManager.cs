using UnityEngine;
using UnityEngine.UI;

public class Mira3DController : MonoBehaviour
{
    private RawImage miraImage;
    public Color colorNormal = Color.white;
    public Color colorIluminado = Color.cyan;
    public float velocidadParpadeo = 2f;

    private bool encimaDeObjeto = false;

    private readonly string[] tagsInteractivos = { "ventanaIzquierda", "ventanaDerecha", "puerta", "armario", "Radio" };

    void Start()
    {
        miraImage = GetComponent<RawImage>();
        if (miraImage != null)
        {
            miraImage.color = colorNormal;
        }
    }

    void Update()
    {
        if (miraImage == null) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            encimaDeObjeto = EsTagInteractivo(hit.collider.tag);
        }
        else
        {
            encimaDeObjeto = false;
        }

        if (encimaDeObjeto)
        {
            float t = Mathf.PingPong(Time.time * velocidadParpadeo, 1f);
            miraImage.color = Color.Lerp(colorNormal, colorIluminado, t);
        }
        else
        {
            miraImage.color = Color.Lerp(miraImage.color, colorNormal, Time.deltaTime * 5f); // Vuelve suavemente al color base
        }
    }

    private bool EsTagInteractivo(string tag)
    {
        foreach (string t in tagsInteractivos)
        {
            if (tag == t) return true;
        }
        return false;
    }
}