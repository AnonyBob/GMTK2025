Shader "Custom/CircleShader"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        _TransitionTex ("Transition Texture", 2D) = "white" {}
        _TransitionColor ("Transition Color", Color) = (1, 1, 1, 1)
        _TransitionWidth ("Effect Width", Float) = 0.01
        _TransitionSpeed ("Speed", Float) = 0.02
        _TransitionRange ("Range", Vector) = (0, 1, 0, 0)
        _TransitionRate ("Rate", Float) = 0.02
        _TransitionRotation ("Rotation", Float) = 0.0
        _Scaling ("Scaling", Float) = 1
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

            sampler2D _TransitionTex;
            float4 _TransitionTex_ST;
            float4 _TransitionColor;
            float4 _TransitionRange;
            float _TransitionWidth;
            float _TransitionSpeed;
            float _TransitionRate;
            float _TransitionRotation;
            float _Scaling;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            float transition_rate()
            {
                // Frac will keep things between 0 and 1 and cleanly wrap around.
                return frac(_TransitionSpeed * _Time.y + _TransitionRate * 0.9999);
            }

            float inv_lerp(const float from, const float to, const float value)
            {
                return saturate(max(0, value - from) / max(0.001, to - from));
            }

            half4 apply_transition_color_filter(const half4 inColor, const half4 factor, const float intensity)
            {
                half4 color = inColor;
                color.rgb = color.rgb + factor.rgb;
                color = lerp(inColor, color, intensity);
                return color;
            }

            float2 rotate(float2 uv, float angle, float2 pivot)
            {
                float s = sin(angle);
                float c = cos(angle);

                // Translate to pivot
                uv -= pivot;

                // Apply 2D rotation matrix
                float2 rotated;
                rotated.x = uv.x * c - uv.y * s;
                rotated.y = uv.x * s + uv.y * c;

                // Translate back
                rotated += pivot;

                return rotated;
            }
            
            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float4 texColor = tex2D(_MainTex, uv) * IN.color;

                // Rotates the UV coordinates based on the center of the texture.
                float2 localUv = rotate(uv, radians(_TransitionRotation), float2(0.5, 0.5));

                // Adjust the tiling factor of the transition texture. If we have a width of zero then the tiling factor is
                // 100 so it's very small and with a transition width of 1 it is the normal tiling factor of 1.
                const half scale = lerp(100, 1, _TransitionWidth); 
                const half2 time = half2(transition_rate() * 2, 0);
                float2 finalUv = localUv * _TransitionTex_ST.xy * scale + time;
                const float alpha = tex2D(_TransitionTex, finalUv).a;

                const half4 patternColor = apply_transition_color_filter(half4(texColor.rgb, 1), half4(_TransitionColor.rgb * texColor.a, 1), _TransitionColor.a);
                float isPattern = inv_lerp(_TransitionRange.x, _TransitionRange.y, localUv.x) < alpha;
                half4 finalColor = texColor;
                finalColor.rgb = lerp(texColor.rgb, patternColor.rgb, isPattern);
                
                return finalColor;

                // Mix line color with sprite color
                //half4 finalColor = lerp(texColor, _CircleColor, linePos);
                
                // // Find the closest circle center.
                // float2 closestCircle = floor(uv / (_SpacingBetweenCircles + _CircleRadius)) + 0.5;
                // closestCircle = closestCircle * (_SpacingBetweenCircles + _CircleRadius);
                // float dist = length(uv - closestCircle);
                //
                // // Color based on the distance from the closest circle.
                // float circleMask = 1 - step(_CircleRadius * lerp(_CircleScaling.x, _CircleScaling.y, uv.x), dist);
                // float4 circleColor = _CircleColor * circleMask;
                //
                // // Additively blend wave on top of sprite
                // float4 finalColor = circleColor + texColor;
                //
                // // Preserve sprite alpha
                // finalColor.a = texColor.a;
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}