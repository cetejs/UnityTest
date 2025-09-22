Shader "Custom/2D/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineSize ("Outline Size", Range(0, 10)) = 1
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

            CBUFFER_START(UnityPreMaterial)
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineSize;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

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
                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                float outline = 0.0; // 计算四个个方向的采样值，剔除周围完全透明的像素
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(-offset.x, 0)).a;
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(offset.x, 0)).a;
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(0, -offset.y)).a;
                outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(0, offset.y)).a;

                float isOutline = step(0.05, outline) * (1 - col.a); // 如果周围有透明像素，则认为是轮廓，并且自身透明度越小越明显
                col = lerp(col, _OutlineColor, isOutline);
                return col;
            }
            
            ENDHLSL
        }
    }
}