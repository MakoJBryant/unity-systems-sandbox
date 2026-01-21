Shader "Custom/Atmosphere"
{
    Properties
    {
        _DayColor("Day Sky Color", Color) = (0.15, 0.35, 0.8, 1)
        _SunsetColor("Sunset Color", Color) = (1.0, 0.5, 0.2, 1)
        _NightColor("Night Color", Color) = (0.02, 0.02, 0.05, 1)
        _FogColor("Fog Color", Color) = (0.6, 0.7, 0.9, 1)
        _FogDensity("Fog Density", Range(0.001, 0.05)) = 0.01
        _RayleighPower("Rayleigh Power", Range(1, 10)) = 4
        _SunsetSharpness("Sunset Sharpness", Range(1, 20)) = 8
        _SunGlowIntensity("Sun Glow Intensity", Range(0, 2)) = 1

        _SunDirection("Sun Direction", Vector) = (0,1,0,0)
        _PlanetCenter("Planet Center", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Cull Front
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            float4 _DayColor;
            float4 _SunsetColor;
            float4 _NightColor;
            float4 _FogColor;
            float _FogDensity;
            float _RayleighPower;
            float _SunsetSharpness;
            float _SunGlowIntensity;

            float3 _SunDirection;
            float3 _PlanetCenter;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // --- Local Up from planet center ---
                float3 localUp = normalize(i.worldPos - _PlanetCenter);

                // --- Sun direction ---
                float3 sunDir = normalize(_SunDirection);

                // --- Rayleigh scattering: blue sky overhead ---
                float cosTheta = dot(localUp, sunDir);
                float rayleighFactor = pow(saturate(cosTheta), _RayleighPower);
                float3 scatter = lerp(_NightColor.rgb, _DayColor.rgb, rayleighFactor);

                // --- Sunset near horizon ---
                float horizon = 1.0 - saturate(dot(localUp, sunDir)); // 1 near horizon
                float sunsetFactor = pow(horizon, _SunsetSharpness);
                scatter = lerp(scatter, _SunsetColor.rgb, sunsetFactor);

                // --- Optional sun glow ---
                float sunGlow = pow(saturate(dot(localUp, sunDir)), _SunGlowIntensity);
                scatter += _SunsetColor.rgb * sunGlow * 0.5;

                // --- Fog / atmospheric haze ---
                float dist = length(i.worldPos - _WorldSpaceCameraPos);
                scatter = lerp(scatter, _FogColor.rgb, 1.0 - exp(-_FogDensity * dist));

                // --- Alpha: fade out at night for stars ---
                float alpha = saturate(cosTheta + 0.1);

                return fixed4(scatter, alpha);
            }
            ENDCG
        }
    }
}
