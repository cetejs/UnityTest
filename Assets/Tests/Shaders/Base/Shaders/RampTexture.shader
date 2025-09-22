Shader "Custom/RampTexture"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
        _RampTex ("Ramp Tex", 2D) = "write" {}
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

            TEXTURE2D(_RampTex);
            SAMPLER(sampler_RampTex);

            CBUFFER_START(UnityPreMaterial)
            half4 _Diffuse;
            float4 _RampTex_ST;
            half4 _Specular;
            half _Gloss;
            CBUFFER_END

            struct Attributes
            {
                float4 position : POSITION;
                float3 normalOS : NORMAL;
                float3 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            Varyings MainVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.position.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionWS = TransformObjectToWorld(input.position.xyz);
                output.uv = TRANSFORM_TEX(input.texcoord, _RampTex);
                return output;
            }

            half4 MainFragment(Varyings input) : SV_Target
            {
                Light light = GetMainLight();
                half3 normalWS = normalize(input.normalWS);
                half3 lightDir = normalize(light.direction);

                half3 ambient = _GlossyEnvironmentColor.rgb;

                half halfLambert = dot(lightDir, normalWS) * 0.5 + 0.5;
                half3 ramp = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, half2(halfLambert, halfLambert)).rgb;
                half3 diffuse = light.color.rgb * _Diffuse.rgb * ramp;
                
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - input.positionWS);
                half3 halfDir = normalize(lightDir + viewDir);
                half3 specular = light.color.rgb * _Specular.rgb * pow(saturate(dot(halfDir, normalWS)), _Gloss);

                return half4(ambient + diffuse + specular, 1);
            }
            ENDHLSL
        }
    }
}