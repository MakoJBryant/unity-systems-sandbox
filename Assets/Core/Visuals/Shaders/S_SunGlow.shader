Shader "Custom/S_SunGlow"
{
    Properties
    {
        _MainTex ("Sun Texture (Optional)", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,0.6,0.0,1)
        _GlowColor ("Glow Color", Color) = (1,0.8,0,1)
        _GlowIntensity ("Glow Intensity", Float) = 3.0
        _GlowSize ("Glow Size", Float) = 1.5
        _NoiseScale ("Noise Scale", Float) = 5.0
        _NoiseSpeed ("Noise Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One One // additive blending

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _BaseColor;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowSize;
            float _NoiseScale;
            float _NoiseSpeed;
            float _TimeY;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Simplex or Perlin noise functions would be ideal, but here's a simple hash-based noise:
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(a, b, u.x) + (c - a)* u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;

                // Add animated noise distortion to UV for turbulence effect
                float n = noise(uv * _NoiseScale + _Time.y * _NoiseSpeed);

                uv += (n - 0.5) * 0.05;

                float dist = length(uv);

                // Core brightness: sharp bright center fading quickly
                float core = smoothstep(0.0, 0.3, 0.3 - dist);

                // Outer glow: smooth falloff
                float glow = saturate(1.0 - dist * _GlowSize);

                // Base fiery color modulated by core brightness
                fixed4 baseCol = _BaseColor * core;

                // Add glow color multiplied by intensity and glow factor
                fixed4 glowCol = _GlowColor * glow * _GlowIntensity;

                // Combine base and glow, ensure core is opaque enough
                fixed4 col = baseCol + glowCol;

                col.a = saturate(core + glow * 0.5);

                return col;
            }
            ENDCG
        }
    }
}
