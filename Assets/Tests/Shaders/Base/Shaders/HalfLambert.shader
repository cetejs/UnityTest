Shader "Custom/URP/HalfLambert"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
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
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
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
                half3 diffuse = light.color.rgb * _Diffuse.rgb * halfLambert;

                return half4(ambient + diffuse, 1);
            }
            ENDHLSL
        }
    }

    FallBack "UniversalRenderPipeline/Lit"
}