Shader "Skybox/ProceduralStars"
{
    Properties
    {
        _StarDensity ("Star Density", Range(100,5000)) = 1500
        _StarBrightness ("Star Brightness", Range(0,5)) = 1.5
        _StarSize ("Star Size", Range(0.0005,0.02)) = 0.003
        _StarTint ("Star Tint", Color) = (1,1,1,1)
        _SkyColor ("Sky Color", Color) = (0.01, 0.01, 0.02, 1)
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _StarDensity;
            float _StarBrightness;
            float _StarSize;
            float4 _StarTint;
            float4 _SkyColor;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // Worldspace direction (safe for skybox)
                o.dir = normalize(mul(unity_ObjectToWorld, v.vertex).xyz);
                return o;
            }

            // 2D hash (stable)
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 78.233);
                return frac(p.x * p.y);
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 d = normalize(i.dir);

                // Convert direction -> spherical UV
                float2 uv;
                uv.x = atan2(d.z, d.x) / (2 * UNITY_PI) + 0.5;
                uv.y = asin(d.y) / UNITY_PI + 0.5;

                // Star grid
                float2 cell = floor(uv * _StarDensity);

                float rnd = hash21(cell);

                // Star threshold
                float star = step(1.0 - _StarSize, rnd);

                float3 color =
                    _SkyColor.rgb +
                    _StarTint.rgb * star * _StarBrightness;

                return float4(color, 1);
            }
            ENDCG
        }
    }
}
