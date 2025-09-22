Shader "Custom/2D/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]
        _DissolveTex ("Dissolve Noise", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0.5
        _EdgeColor ("Edge Color", Color) = (1, 1, 1, 1)
        _EdgeWidth ("Edge Width", Range(0, 1)) = 0.1
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
            TEXTURE2D(_DissolveTex);
            SAMPLER(sampler_DissolveTex);

            CBUFFER_START(UnityPreMaterial)
            float4 _MainTex_ST;
            float _DissolveAmount;
            float4 _EdgeColor;
            float _EdgeWidth;
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
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half dissolve = SAMPLE_TEXTURE2D(_DissolveTex, sampler_DissolveTex, input.uv).r; // 采样噪声贴图
                float edge = smoothstep(_DissolveAmount, _DissolveAmount + _EdgeWidth, dissolve); // 计算边缘效果 smoothstep(a, b, x)， x = [a, b] => [0, 1]
                float alpha = step(_DissolveAmount, dissolve); // 判断是否在消失范围内 step(a, b) b >= a ? 1 : 0
                col.a *= alpha;
                col.rgb = lerp(_EdgeColor.rgb, col.rgb, edge);
                return col;
            }
            ENDHLSL
        }
    }
}