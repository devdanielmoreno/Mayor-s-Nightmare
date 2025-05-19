using UnityEngine;

public class RadioInteractiva : MonoBehaviour
{
    public AudioSource audioRadio;
    public Light luzBrilloRadio;
    public VentanaEnemyController enemigo;

    private bool activada = false;

    void Start()
    {
        if (luzBrilloRadio != null)
            luzBrilloRadio.enabled = false;
    }

    void OnMouseDown()
    {
        // No activar si no hay monstruo o si no estás escondido
        if (!enemigo.EnMonstruoPresente()) return;
        if (!enemigo.JugadorEscondidoCorrectamente()) return;

        if (audioRadio != null && !audioRadio.isPlaying)
            audioRadio.Play();

        if (luzBrilloRadio != null)
            luzBrilloRadio.enabled = true;

        activada = true;
        enemigo.RadioActivadaDesdeEscondite();
    }



    void OnMouseUp()
    {
        if (audioRadio != null)
            audioRadio.Stop();

        if (luzBrilloRadio != null)
            luzBrilloRadio.enabled = false;

        activada = false;
    }
}