Shader "Custom/CircleShader"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        _CircleColor ("Circle Color", Color) = (0, 1, 0, 1)
        _CircleRadius ("Radius", Float) = 0.01
        _CircleScaling ("Scaling", Vector) = (0, 0, 0, 0)
        _ScrollSpeed ("ScrollSpeed", Vector) = (0, 0, 0, 0)
        _SpacingBetweenCircles ("Spacing", Float) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Name "SpriteLit"
            Tags { "LightMode"="Universal2D" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _CircleColor;
            float4 _CircleScaling;
            float4 _ScrollSpeed;
            float _CircleRadius;
            float _SpacingBetweenCircles;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float4 texColor = tex2D(_MainTex, uv) * IN.color;
                
                // Find the closest circle center.
                float2 uvValue = uv + _ScrollSpeed * _Time.y;
                uvValue = fmod(uvValue, 1);
                float2 closestCircle = floor(uvValue / (_SpacingBetweenCircles + _CircleRadius)) + 0.5;
                closestCircle = closestCircle * (_SpacingBetweenCircles + _CircleRadius);
                float dist = length(uv - closestCircle);
                
                // Color based on the distance from the closest circle.
                float circleMask = 1 - step(_CircleRadius * lerp(_CircleScaling.x, _CircleScaling.y, uv.x), dist);
                float4 circleColor = _CircleColor * circleMask;

                // Additively blend wave on top of sprite
                float4 finalColor = float4(uvValue, 0, 1);

                // Preserve sprite alpha
                finalColor.a = texColor.a;

                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}