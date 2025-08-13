Shader "Custom/URP/RampTexture"
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
            "RenderPipeline" = "UniversalRenderPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord, _RampTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                Light light = GetMainLight();
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldLightDir = normalize(TransformObjectToWorldDir(light.direction));

                // half3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                half3 ambient = _GlossyEnvironmentColor.rgb;

                half halfLambert = dot(worldLightDir, worldNormal) * 0.5 + 0.5;
                // half3 diffuse = light.color.rgb * _Diffuse.rgb * saturate(dot(worldLightDir, worldNormal));
                half3 diffuse = light.color.rgb * _Diffuse.rgb * SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, half2(halfLambert, halfLambert)).rgb;
                
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                // half3 halfViewDir = normalize(worldLightDir + viewDir);
                // half3 specular = light.color.rgb * _Specular.rgb * pow(saturate(dot(halfViewDir, worldNormal)), _Gloss);
                half3 specular = LightingSpecular(light.color.rgb, worldLightDir, worldNormal, viewDir, _Specular, _Gloss);

                return half4(ambient + diffuse + specular, 1);
            }
            ENDHLSL
        }
    }

    FallBack "UniversalRenderPipeline/Lit"
}