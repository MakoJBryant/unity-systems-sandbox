Shader "Sky/SunDisc"
{
    Properties
    {
        _SunColor ("Sun Color", Color) = (1,0.95,0.8,1)
        _SunIntensity ("Intensity", Range(0,10)) = 4
        _SunRadius ("Radius", Range(0.1,1)) = 0.5
        _SunSoftness ("Edge Softness", Range(0.001,0.5)) = 0.1
        _AtmosphereFade ("Atmosphere Fade", Range(0.1,8)) = 3
    }

    SubShader
    {
        Tags { "Queue"="Background+1" "RenderType"="Transparent" }
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _SunColor;
            float _SunIntensity;
            float _SunRadius;
            float _SunSoftness;
            float _AtmosphereFade;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Quad UV in -1..1
                o.uv = v.uv * 2.0 - 1.0;

                // World-space view direction
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(worldPos - _WorldSpaceCameraPos);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Disc shape
                float r = length(i.uv);
                float disc = smoothstep(
                    _SunRadius,
                    _SunRadius - _SunSoftness,
                    r
                );

                // Sun direction (directional light)
                float3 sunDir = normalize(_WorldSpaceLightPos0.xyz);

                // View vs sun alignment
                float alignment = saturate(dot(-i.viewDir, sunDir));

                // Atmospheric extinction
                float atmosphere = pow(alignment, _AtmosphereFade);

                float alpha = disc * atmosphere;

                float3 color = _SunColor.rgb * _SunIntensity * alpha;

                return float4(color, alpha);
            }
            ENDCG
        }
    }
}
