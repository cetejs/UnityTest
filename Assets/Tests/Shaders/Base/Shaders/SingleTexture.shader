Shader "Custom/SingleTexture"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "write" {}
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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPreMaterial)
            half4 _Color;
            float4 _MainTex_ST;
            half4 _Specular;
            half _Gloss;
            CBUFFER_END

          struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
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
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
                return output;
            }

            half4 MainFragment(Varyings input) : SV_Target
            {
               
                Light light = GetMainLight();
                half3 normalWS = normalize(input.normalWS);
                half3 lightDir = normalize(light.direction);
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb * _Color.rgb;

                half3 ambient = _GlossyEnvironmentColor.rgb * albedo;

                half3 diffuse = light.color.rgb * _Color.rgb * saturate(dot(normalWS, lightDir)) * albedo;
   
                half3 viewDir = normalize(GetWorldSpaceViewDir(input.positionWS));
                half3 halfDir = normalize(lightDir + viewDir);
                half3 specular = light.color.rgb * _Specular.rgb * pow(saturate(dot(halfDir, normalWS)), _Gloss);

                return half4(ambient + diffuse + specular, 1);
            }
            ENDHLSL
        }
    }

    FallBack "UniversalRenderPipeline/Lit"
}