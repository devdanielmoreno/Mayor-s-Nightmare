
Shader "Custom/CRTGlitchEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.3
        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0.1
        _DistortionStrength ("Distortion Strength", Range(0,0.1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScanlineIntensity;
            float _NoiseIntensity;
            float _DistortionStrength;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Scanline effect
                float scanline = sin(uv.y * 800.0) * _ScanlineIntensity;

                // Horizontal distortion
                uv.x += sin(uv.y * 40.0 + _Time.y * 5.0) * _DistortionStrength;

                // Noise effect
                float noise = (rand(uv + _Time.y) - 0.5) * _NoiseIntensity;

                fixed4 col = tex2D(_MainTex, uv);
                col.rgb += scanline + noise;

                return col;
            }
            ENDCG
        }
    }
}
