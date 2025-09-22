Shader "Custom/MaskTexture"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "write" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Float) = 1
        _SpecularMask ("Specular Mask", 2D) = "write" {}
        _SpecularScale ("Specular Scale", Float) = 1
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
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            TEXTURE2D(_SpecularMask);
            SAMPLER(sampler_SpecularMask);

            CBUFFER_START(UnityPreMaterial)
            half4 _Color;
            float4 _MainTex_ST;
            float4 _BumpMap_ST;
            float _BumpScale;
            float _SpecularScale;
            half4 _Specular;
            half _Gloss;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 TtoW0 : TEXCOORD1;
                float4 TtoW1 : TEXCOORD2;
                float4 TtoW2 : TEXCOORD3;
            };

            Varyings MainVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);

                half3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                half3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                half3 tangentWS = TransformObjectToWorldDir(input.tangentOS.xyz);
                half3 binormalWS = cross(normalWS, tangentWS) * input.tangentOS.w;

                output.TtoW0 = float4(tangentWS.x, binormalWS.x, normalWS.x, positionWS.x);
                output.TtoW1 = float4(tangentWS.y, binormalWS.y, normalWS.y, positionWS.y);
                output.TtoW2 = float4(tangentWS.z, binormalWS.z, normalWS.z, positionWS.z);
                return output;
            }

            half4 MainFragment(Varyings input) : SV_Target
            {
                float3 positionWS = float3(input.TtoW0.w, input.TtoW1.w, input.TtoW2.w);
                half3 bump = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv), _BumpScale);
                half3 normalWS = normalize(half3(dot(input.TtoW0.xyz, bump), dot(input.TtoW1.xyz, bump), dot(input.TtoW2.xyz, bump)));
                
                Light light = GetMainLight();
                half3 lightDir = normalize(light.direction);

                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb * _Color.rgb;

                half3 ambient = _GlossyEnvironmentColor.rgb * albedo;

                half3 diffuse = light.color.rgb * _Color.rgb * saturate(dot(lightDir, normalWS)) * albedo;
                
                half3 viewDir = GetWorldSpaceViewDir(positionWS);
                half3 halfDir = normalize(lightDir + viewDir);
                half specularMask = SAMPLE_TEXTURE2D(_SpecularMask, sampler_SpecularMask, input.uv).r * _SpecularScale;
                half3 specular = light.color.rgb * _Specular.rgb * pow(saturate(dot(halfDir, normalWS)), _Gloss) * specularMask;

                return half4(ambient + diffuse + specular, 1);
            }
            ENDHLSL
        }
    }
}