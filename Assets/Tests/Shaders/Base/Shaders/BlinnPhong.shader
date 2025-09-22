Shader "Custom/BlinnPhong"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
        _Specular ("Specular", Color) = (1, 1, 1, 1)
        _Gloss ("Gloss", Range(8, 256)) = 20
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
            half4 _Specular;
            half _Gloss;
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
                float3 positionWS : TEXCOORD1;
            };

            Varyings MainVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                return output;
            }

            half4 MainFragment(Varyings input) : SV_Target
            {

                Light light = GetMainLight();
                half3 normalWS = normalize(input.normalWS);
                half3 lightDir = normalize(light.direction);

                half3 ambient = _GlossyEnvironmentColor.rgb;

                half3 diffuse = light.color.rgb * _Diffuse.rgb * saturate(dot(normalWS, lightDir));

                half3 viewDir = normalize(GetWorldSpaceViewDir(input.positionWS));
                half3 halfDir = normalize(lightDir + viewDir);
                half3 specular = light.color.rgb * _Specular.rgb * pow(saturate(dot(halfDir, normalWS)), _Gloss);

                return half4(ambient + diffuse + specular, 1);
            }
            ENDHLSL
        }
    }

    FallBack Off
}