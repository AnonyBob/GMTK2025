Shader "Custom/URP/MovingCircleWithTexture"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _CircleColor("Circle Color", Color) = (1, 0, 0, 1)
        _Speed("Speed", Float) = 1.0
        _Radius("Radius", Float) = 0.2
    }

    SubShader
    {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _CircleColor;
            float _Speed;
            float _Radius;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // Sample base texture
                float4 baseColor = tex2D(_MainTex, uv);

                // Time-driven X position for circle center (loops 0–1)
                float xCenter = frac(_Time.y * _Speed);
                float2 center = float2(xCenter, 0.5);

                // Distance to circle center
                float dist = distance(uv, center);

                // Circle alpha mask (smooth edge)
                float circleMask = smoothstep(_Radius, _Radius - 0.01, dist);

                // Circle overlay color (fades out at edges)
                float4 circleOverlay = lerp(_CircleColor, float4(0, 0, 0, 0), circleMask);

                // Blend circle on top of base texture
                float4 finalColor = baseColor + circleOverlay * circleOverlay.a;

                // Preserve overall transparency
                finalColor.a = max(baseColor.a, circleOverlay.a);

                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}
