using System;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    
    [Header("Luz Global")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField, Range(0, 24)] public float timeOfDay;

    [Header("Duración del Día")]
    [SerializeField] private float dayLengthInMinutes = 3f; // ⏳ Ahora el día dura 3 min en vez de 5
    private float timeMultiplier;
    private bool hasDayIncremented = false;
    public bool esDeNoche = false;

    [Header("Skybox Configuración")]
    [SerializeField] private Material skyboxMaterial; // ✅ Un solo Skybox con Blend
    [SerializeField] private float transitionSpeed = 2f; // ✅ Aumentamos la velocidad de transición
    private float blendValue = 0f; // ✅ Factor de transición entre Día y Noche
    
    private List<Aldeano> aldeanos = new List<Aldeano>();
    [Obsolete("Obsolete")]
    private void Start()
    {
        QualitySettings.shadowDistance = 50f;  // 🔥 Aumenta la distancia de sombras
        QualitySettings.shadowResolution = ShadowResolution.High; // 🔥 Mejora la resolución de sombras
        QualitySettings.shadowCascades = 4; // 🔥 Más detalles en sombras
        QualitySettings.shadowProjection = ShadowProjection.CloseFit;
        
        timeMultiplier = 24f / (dayLengthInMinutes * 60f);
        timeOfDay = 8f; // 🔆 Comenzamos de Día
        RenderSettings.skybox = skyboxMaterial; // ✅ Usamos solo un Skybox
        RenderSettings.skybox.SetFloat("_Blend", blendValue); // 🔆 Iniciamos en Día
        UpdateLighting(timeOfDay / 24f);
        DynamicGI.UpdateEnvironment();
        aldeanos.AddRange(FindObjectsOfType<Aldeano>());
    }

    private void Update()
    {
        if (preset == null) return;

        timeOfDay += Time.deltaTime * timeMultiplier;
        timeOfDay %= 24;
        UpdateLighting(timeOfDay / 24f);
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170, 0));

            float sunHeight = directionalLight.transform.eulerAngles.x;
            if (sunHeight > 180) sunHeight -= 360;
            directionalLight.intensity = Mathf.Clamp01(sunHeight / 90f)* 2.0f;
        }
        bool ahoraEsDeNoche = directionalLight.intensity < 0.01f; 
        
        if (ahoraEsDeNoche != esDeNoche) // Cambio de estado Día/Noche
        {
            esDeNoche = ahoraEsDeNoche;

            if (esDeNoche) 
            {
                Debug.Log("🌙 Se hace de noche, los aldeanos van a casa...");
                foreach (Aldeano aldeano in aldeanos)
                {
                    if (aldeano != null)
                    {
                        aldeano.GoHomeAndDisappear();
                    }
                }
            } 
            else 
            {
                Debug.Log("☀️ Amanece, los aldeanos reaparecen...");
                foreach (Aldeano aldeano in aldeanos)
                {
                    if (aldeano != null)
                    {
                        aldeano.gameObject.SetActive(true); // 📌 Reactivar aldeano
                        aldeano.Reappear();
                    }
                }
            }
        }

        // 🌅 **Transición de Skybox**
        blendValue = Mathf.Lerp(blendValue, esDeNoche ? 1f : 0f, transitionSpeed * Time.deltaTime);
        RenderSettings.skybox.SetFloat("_Blend", blendValue);

        if (!esDeNoche && !hasDayIncremented)
        {
            City.instance.EndTurn();
            hasDayIncremented = true;
        }
        else if (esDeNoche)
        {
            hasDayIncremented = false;
        }
        

        // 🌅 **Transición Suave pero Más Rápida entre Día y Noche** 🌙
        if (directionalLight.intensity > 0.01f) // Sigue siendo de día
        {
            blendValue = Mathf.Lerp(blendValue, 0f, transitionSpeed * Time.deltaTime);
            RenderSettings.skybox.SetFloat("_Blend", blendValue); // 🔆 Se mantiene de Día
            RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);

            if (!hasDayIncremented)
            {
                City.instance.EndTurn();
                hasDayIncremented = true;
            }
        }
        else // Es de noche
        {
            blendValue = Mathf.Lerp(blendValue, 1f, transitionSpeed * Time.deltaTime);
            RenderSettings.skybox.SetFloat("_Blend", blendValue); // 🌙 Se hace de noche
            RenderSettings.ambientLight = new Color(0.137f, 0.388f, 0.302f); // ✅ Color nocturno corregido
            hasDayIncremented = false;
        }

        DynamicGI.UpdateEnvironment();
    }

    private void OnValidate()
    {
        if (directionalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
    public void RegistrarAldeano(Aldeano aldeano)
    {
        if (!aldeanos.Contains(aldeano))
        {
            aldeanos.Add(aldeano);
        }
    }
}