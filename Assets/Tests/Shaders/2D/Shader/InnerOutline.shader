Shader "Custom/2D/InnerOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineThickness ("Outline Thickness", Range(0, 10)) = 1
        _OutlineAlpha ("Outline Alpha", Range(0, 1)) = 0.05
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

            sampler2D _MainTex;

            CBUFFER_START(UnityPreMaterial)
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _OutlineAlpha;
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

            half3 Tex2DWithOffset(float2 offset, float2 uv, sampler2D tex)
            {
                return tex2D(tex, uv + offset * _MainTex_TexelSize.xy).rgb;
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
                half4 col = tex2D(_MainTex, input.uv);
                // 计算内描边的强度，上下左右差值越大越强
                half3 innert = abs(Tex2DWithOffset(float2(_OutlineThickness, 0), input.uv, _MainTex) - Tex2DWithOffset(float2(-_OutlineThickness, 0), input.uv, _MainTex)) +
                                abs(Tex2DWithOffset(float2(0, _OutlineThickness), input.uv, _MainTex) - Tex2DWithOffset(float2(0, -_OutlineThickness), input.uv, _MainTex));
                innert *= col.a * _OutlineAlpha; // 剔除透明像素点
                col.rgb += length(innert) * _OutlineColor.rgb;
                return col;
            }
            ENDHLSL
        }
    }
}