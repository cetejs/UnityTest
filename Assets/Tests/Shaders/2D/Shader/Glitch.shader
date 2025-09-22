Shader "Custom/2D/Glitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _JitterDisplacement ("Jitter Displacement", Range(0, 1)) = 0.05
        _JitterThreshold ("Jitter Threshold", Range(0, 1)) = 0
        _VerticalJumpAmount ("Vertical Jump Amount", Range(0, 1)) = 0
        _VerticalJumpTime ("Vertical Jump Time", Range(0, 1)) = 0
        _HorizontalShake ("Horizontal Shake", Range(0, 1)) = 0.05
        _ColorDriftAmount ("Color Drift Amount", Range(0, 1)) = 0
        _ColorDriftTime ("Color Drift Time", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline"
            "Queue" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex MainVertex
            #pragma fragment MainFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPreMaterial)
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            float _JitterDisplacement;
            float _JitterThreshold;
            float _VerticalJumpAmount;
            float _VerticalJumpTime;
            float _HorizontalShake;
            float _ColorDriftAmount;
            float _ColorDriftTime;
            CBUFFER_END

         struct Attributes
            {
                float4 positionOS : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float rand(float2 value)
            {
                return frac(sin(dot(value, float2(12.9898, 78.233))) * 43758.5453);
            }

            Varyings MainVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
                return output;
            }

            half4 MainFragment(Varyings input) : SV_Target
            {
                float u = input.uv.x;
                float v = input.uv.y;

                // Scan line jitter
                float jitter = rand(float2(v, _Time.x)) * 2 - 1;
                jitter *= step(_JitterThreshold, abs(jitter)) * _JitterDisplacement;

                // Vertical jump
                float jump = lerp(v, frac(v + _VerticalJumpTime), _VerticalJumpAmount);

                // Horizontal shake
                float shake = (rand(float2(_Time.x, 2)) - 0.5) * _HorizontalShake;

                // Color drift
                float drift = sin(jump + _ColorDriftTime) * _ColorDriftAmount;

                half4 src1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, frac(float2(u + jitter + shake, jump)));
                half4 src2 =  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, frac(float2(u + jitter + shake + drift, jump)));

                return half4(src1.r, src2.g, src1.b, src1.a);
            }
            ENDHLSL
        }
    }
}