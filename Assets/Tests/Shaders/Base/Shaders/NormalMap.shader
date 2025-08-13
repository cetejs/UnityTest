Shader "Custom/URP/NormalMap"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "write" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Float) = 1
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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            CBUFFER_START(UnityPreMaterial)
            half4 _Color;
            float4 _MainTex_ST;
            float4 _BumpMap_ST;
            float _BumpScale;
            half4 _Specular;
            half _Gloss;
            CBUFFER_END

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 TtoW0 : TEXCOORD1;
                float4 TtoW1 : TEXCOORD2;
                float4 TtoW2 : TEXCOORD3;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy +_MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy +_BumpMap_ST.zw;

                half3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                half3 worldNormal = TransformObjectToWorldNormal(v.normal);
                half3 worldTangent = TransformObjectToWorldDir(v.tangent.xyz);
                half3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
                // half3 bump = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv.zw));
                // bump.xy *= _BumpScale;
                // bump.z = sqrt(1 - saturate(dot( bump.xy,  bump.xy)));
                half3 bump = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv.zw), _BumpScale);
                half3 worldNormal = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
                
                Light light = GetMainLight();;
                half3 worldLightDir = normalize(TransformObjectToWorldDir(light.direction));

                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy).rgb * _Color.rgb;

                // half3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                half3 ambient = _GlossyEnvironmentColor.rgb * albedo;

                // half3 diffuse = light.color.rgb * _Diffuse * saturate(dot(worldLightDir, worldNormal)) * albedo;
                half3 diffuse = LightingLambert(light.color.rgb, worldLightDir, worldNormal) * albedo;
                
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
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