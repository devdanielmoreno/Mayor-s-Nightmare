Shader "Skybox/NightDay"
{
    Properties
    {
        _Texture1("Day Skybox", Cube) = "white" {} // 🌞 Skybox de día
        _Texture2("Night Skybox", Cube) = "white" {} // 🌙 Skybox de noche
        _Blend("Blend", Range(0, 1)) = 0.5 // Factor de mezcla
    }
    SubShader
    {
        Tags { "Queue"="Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            samplerCUBE _Texture1;
            samplerCUBE _Texture2;
            float _Blend;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz; // ✅ Usamos directamente coordenadas cúbicas
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex1 = texCUBE(_Texture1, i.texcoord);
                fixed4 tex2 = texCUBE(_Texture2, i.texcoord);
                return lerp(tex1, tex2, _Blend); // ✅ Transición suave
            }
            ENDCG
        }
    }
}
