using UnityEngine;

public class LunaLlenaManager : MonoBehaviour
{
    public AudioSource musicaNormal;
    public AudioSource musicaLunaLlena;
    private bool lunaLlenaActiva = false;

    void Update()
    {
        bool esNoche = !(City.instance.lightingManager.timeOfDay > 6 && City.instance.lightingManager.timeOfDay < 18);
        bool esLunaLlena = City.instance.diasParaLunaLlena == 0;

        if (esNoche && esLunaLlena && !lunaLlenaActiva)
        {
            ActivarLunaLlena();
        }
        else if ((!esNoche || !esLunaLlena) && lunaLlenaActiva)
        {
            DesactivarLunaLlena();
        }
    }


    void ActivarLunaLlena()
    {
        lunaLlenaActiva = true;
        City.instance.lunaLlenaActivaEnNoche = true;
        EnemyManager.instance.SpawnOleadaEnemigos();
        

        if (musicaNormal != null) musicaNormal.Stop();
        if (musicaLunaLlena != null) musicaLunaLlena.Play();
        
        Debug.Log("🌕 Luna llena activada: Skybox y música cambiados.");
    }
    void DesactivarLunaLlena()
    {
        lunaLlenaActiva = false;
        City.instance.lunaLlenaActivaEnNoche = false;
        

        if (musicaLunaLlena != null) musicaLunaLlena.Stop();
        if (musicaNormal != null && !musicaNormal.isPlaying) musicaNormal.Play();

        Debug.Log("🌕 Fin de luna llena: restaurado Skybox y música normal.");
    }

}