Shader "Custom/HalfLambert"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalRenderPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex MainVertex
            #pragma fragment MainFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPreMaterial)
            half4 _Diffuse;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
            };

            Varyings MainVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 MainFragment(Varyings input) : SV_Target
            {
                Light light = GetMainLight();
                half3 normalWS = normalize(input.normalWS);
                half3 lightDir = normalize(light.direction);

                half3 ambient = _GlossyEnvironmentColor.rgb;

                half halfLambert = dot(normalWS, lightDir) * 0.5 + 0.5;
                half3 diffuse = light.color.rgb * _Diffuse.rgb * saturate(halfLambert);

                return half4(ambient + diffuse, 1);
            }
            ENDHLSL
        }
    }
}