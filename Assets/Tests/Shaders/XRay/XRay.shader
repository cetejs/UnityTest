Shader "Custom/XRay"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.1,0.6,1,0.6)
        _FresnelPower ("Fresnel Power", Range(0.5,8)) = 2
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
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

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            float4 _BaseColor;
            float _FresnelPower;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.worldPos = mul(unity_ObjectToWorld, v.positionOS).xyz;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS.xyz);
                o.uv = v.texcoord;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float fresnel = pow(1.0 - saturate(dot(viewDir, normalize(i.normalWS))), _FresnelPower);

                half4 outColor = _BaseColor;
                outColor.rgb = _BaseColor.rgb * (0.5 + 0.5 * fresnel);
                return outColor;
            }
            ENDHLSL
        }
    }
    FallBack Off
}