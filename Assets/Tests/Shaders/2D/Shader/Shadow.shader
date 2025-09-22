Shader "Custom/2D/Shadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _ShadowX ("Shadow X", Range(-0.5, 0.5)) = 0
        _ShadowY ("Shadow Y", Range(-0.5, 0.5)) = 0
        _ShadowAlpha("Shadow Alpha", Range(0, 1)) = 0.5
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
            float4 _Color;
            float _ShadowX;
            float _ShadowY;
            float _ShadowAlpha;
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

            Varyings MainVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
                return output;
            }

            half4 MainFragment(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Color;
                half4 shadow = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + half2(_ShadowX, _ShadowY));
                col.a = max(col.a, shadow.a * _ShadowAlpha * _Color.a);
                return col;
            }
            ENDHLSL
        }
    }
}
