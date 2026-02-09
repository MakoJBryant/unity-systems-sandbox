Shader "Custom/SpaceSkybox_FixedStars_Colorful"
{
    Properties
    {
        _SkyColor ("Sky Color", Color) = (0.01, 0.01, 0.03, 1)
        _StarBrightness ("Base Star Brightness", Range(0,10)) = 2
        _StarCount ("Number of Stars", Range(1,500)) = 200
        _StarSize ("Star Size", Range(0.001,0.05)) = 0.02
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            float4 _SkyColor;
            float _StarBrightness;
            float _StarSize;
            int _StarCount;

            // Integer-based deterministic hash
            float hash(int n)
            {
                n = (n << 13) ^ n;
                return frac(1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
            }

            // Star direction on unit sphere
            float3 starDirection(int index)
            {
                float u = hash(index * 1) * 2.0 - 1.0;
                float theta = hash(index * 2) * 6.2831853;
                float r = sqrt(1.0 - u * u);
                return float3(r * cos(theta), r * sin(theta), u);
            }

            // Star brightness multiplier
            float starBrightness(int index)
            {
                // random multiplier between 0.5 and 1.0
                return 0.5 + 0.5 * hash(index * 3);
            }

            // Star color: mostly white, some rare blue/red
            float3 starColor(int index)
            {
                float colorSeed = hash(index * 4);

                if (colorSeed < 0.05) return float3(0.6, 0.7, 1.0); // blue star ~5%
                if (colorSeed > 0.95) return float3(1.0, 0.6, 0.6); // red star ~5%
                return float3(1.0, 1.0, 1.0); // normal white
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.viewDir = normalize(worldPos - _WorldSpaceCameraPos);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 dir = normalize(input.viewDir);
                float3 color = float3(0,0,0);

                for (int i = 0; i < _StarCount; i++)
                {
                    float3 starDir = normalize(starDirection(i));
                    float cosAngle = dot(dir, starDir);
                    float angle = acos(saturate(cosAngle));

                    float intensity = smoothstep(_StarSize, 0.0, angle) * starBrightness(i);
                    color += intensity * starColor(i);
                }

                return half4(_SkyColor.rgb + color * _StarBrightness, 1.0);
            }
            ENDHLSL
        }
    }
}
