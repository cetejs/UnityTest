Shader "Custom/URP/BlinnPhone"
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
            "RenderPipeline" = "UniversalRenderPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPreMaterial)
            half4 _Diffuse;
            half4 _Specular;
            half _Gloss;
            CBUFFER_END

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                Light light = GetMainLight();
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldLightDir = normalize(TransformObjectToWorldDir(light.direction));

                // half3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                half3 ambient = _GlossyEnvironmentColor.rgb;

                // half3 diffuse = light.color.rgb * _Diffuse.rgb * saturate(dot(worldLightDir, worldNormal));
                half3 diffuse = LightingLambert(light.color.rgb, worldLightDir, worldNormal) * _Diffuse.rgb;
                
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