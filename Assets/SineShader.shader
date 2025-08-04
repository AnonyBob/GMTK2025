Shader "Custom/SpriteLitSineWave"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        _WaveColor ("Wave Color", Color) = (0, 1, 0, 1)
        _Frequency ("Wave Frequency", Float) = 20
        _Amplitude ("Wave Amplitude", Float) = 0.02
        _Speed ("Wave Speed", Float) = 2
        _Thickness ("Wave Thickness", Float) = 0.01
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

            float4 _WaveColor;
            float _Frequency;
            float _Amplitude;
            float _Speed;
            float _Thickness;

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

                // Sine wave overlay
                float time = _Time.y;
                float sineY = sin(uv.x * _Frequency + time * _Speed) * _Amplitude;
                float dist = abs(uv.y - 0.5 - sineY); // center wave on UV.y = 0.5
                float waveMask = smoothstep(_Thickness, 0.0, dist);

                float4 wave = _WaveColor * waveMask;

                // Additively blend wave on top of sprite
                float4 finalColor = texColor + wave;

                // Preserve sprite alpha
                finalColor.a = texColor.a;

                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}