Shader "Custom/SpaceSkybox_FixedStars"
{
    Properties
    {
        _SkyColor ("Sky Color", Color) = (0.01, 0.01, 0.03, 1)
        _StarBrightness ("Star Brightness", Range(0,10)) = 2
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

            // Integer-based pseudo-random generator
            float hash(int n)
            {
                n = (n << 13) ^ n;
                return frac(1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
            }

            // Generate deterministic star direction based on index
            float3 starDirection(int index)
            {
                float u = hash(index * 1) * 2.0 - 1.0;
                float theta = hash(index * 2) * 6.2831853;
                float r = sqrt(1.0 - u * u);
                return float3(r * cos(theta), r * sin(theta), u);
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
                float star = 0;

                for (int i = 0; i < _StarCount; i++)
                {
                    float3 starDir = normalize(starDirection(i));

                    // Use dot product to compute angular distance
                    float cosAngle = dot(dir, starDir);
                    float angle = acos(saturate(cosAngle));

                    star += smoothstep(_StarSize, 0.0, angle);
                }

                return half4(_SkyColor.rgb + star * _StarBrightness, 1.0);
            }

            ENDHLSL
        }
    }
}
