Shader "Custom/AtmosphereGradient"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.1,0.2,0.5,1)
        _HorizonColor ("Horizon Color", Color) = (0.8,0.6,0.4,1)
        _SunDirection ("Sun Direction", Vector) = (0,1,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
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
                float3 worldNormal : TEXCOORD0;
            };

            float4 _TopColor;
            float4 _HorizonColor;
            float3 _SunDirection;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate sun height based on normal vs sun direction
                float sunHeight = saturate(dot(i.worldNormal, normalize(_SunDirection)));

                // Blend horizon and top color based on sun height
                float gradient = i.worldNormal.y * 0.5 + 0.5; // simple vertical gradient
                gradient = lerp(gradient, sunHeight, 0.5);    // mix in sun influence

                fixed4 color = lerp(_HorizonColor, _TopColor, gradient);
                color.a = 1.0; // fully opaque
                return color;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}
