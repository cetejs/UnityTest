Shader "Custom/URP/AlphaBlend"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "write" {}
        _AlphaScale ("Alpha Scale", Range(0 ,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPreMaterial)
            half4 _Color;
            float4 _MainTex_ST;
            half _AlphaScale;
            CBUFFER_END

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                Light light = GetMainLight();
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldLightDir = normalize(TransformObjectToWorldDir(light.direction));

                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half3 albedo = texColor.rgb * _Color.rgb;

                half3 ambient = _GlossyEnvironmentColor * albedo;

                half3 diffuse = light.color * albedo * saturate(dot(worldLightDir, worldNormal));

                return half4(ambient + diffuse, texColor.a * _AlphaScale);
            }
            ENDHLSL
        }
    }
}