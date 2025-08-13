Shader "Custom/URP/Toon Water"
{
    Properties
    {
        _DepthGradientShallow ("Depth Gradient Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
        _DepthGradientDeep ("Depth Gradient Deep", Color) = (0.086, 0.407, 1, 0.749)
        _DepthMaxDistance ("Depth Maximum Distance", Float) = 1
        _SurfaceNoise ("Surface Noise", 2D) = "write" {}
        _SurfaceNoiseCutoff ("Surface Noise Cutoff", Range(0, 1)) = 0.777
        _SurfaceNoiseScroll ("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
        _SurfaceDistortion ("Surface Distortion", 2D) = "write" {}
        _SurfaceDistortionAmount ("Surface Distortion", Range(0, 1)) = 0.27
        _FoamMaxDistance ("Foam Maximum Distance", Float) = 0.4
        _FoamMinDistance ("Foam Minimum Distance", Float) = 0.04
        _FormColor ("Form Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "UniversalPipeline"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #define SMOOTHSTEP_AA 0.01

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPreMaterial)
            float4 _DepthGradientShallow;
            float4 _DepthGradientDeep;
            float _DepthMaxDistance;
            float4 _SurfaceNoise_ST;
            float _SurfaceNoiseCutoff;
            float2 _SurfaceNoiseScroll;
            float4 _SurfaceDistortion_ST;
            float _SurfaceDistortionAmount;
            float _FoamMaxDistance;
            float _FoamMinDistance;
            float4 _FormColor;
            CBUFFER_END


            TEXTURE2D(_SurfaceNoise); SAMPLER(sampler_SurfaceNoise);
            TEXTURE2D(_SurfaceDistortion); SAMPLER(sampler_SurfaceDistortion);

            TEXTURE2D_X_FLOAT(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);
            TEXTURE2D_X_FLOAT(_CameraNormalsTexture); SAMPLER(sampler_CameraNormalsTexture);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalVS : NORMAL;
                float4 positionSS : TEXCOORD0;
                float2 noiseUV : TEXCOORD1;
                float2 distortUV : TEXCOORD2;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionSS = ComputeScreenPos(output.positionCS);
                output.noiseUV = TRANSFORM_TEX(input.uv, _SurfaceNoise);
                output.distortUV = TRANSFORM_TEX(input.uv, _SurfaceDistortion);
                output.normalVS = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, input.normalOS));
                return output;
            }

            float4 AlphaBlend(float4 top, float4 bottom)
            {
                float3 color = top.rgb * top.a + bottom.rgb * (1 - top.a);
                float alpha = top.a + bottom.a * (1 - top.a);
                return float4(color, alpha);
            }

            half4 frag(Varyings input) : SV_Target
            {
                float existingDepth01 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, input.positionSS.xy / input.positionSS.w).r;
                float existingDepthLinear = LinearEyeDepth(existingDepth01, _ZBufferParams);
                float depthDifference = existingDepthLinear - input.positionSS.w;
                float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
                float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);

                float3 existingNormal = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, input.positionSS.xy / input.positionSS.w);
                existingNormal = mul(GetWorldToViewMatrix(), existingNormal);
                float3 normalDot = saturate(dot(existingNormal, input.normalVS));
                float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);

                float foamDepthDifference01 = saturate(depthDifference / foamDistance);
                float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;

                float2 distortSample = (SAMPLE_TEXTURE2D(_SurfaceDistortion, sampler_SurfaceDistortion, input.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;

                float2 noiseUV = float2(input.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x + distortSample.x, input.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y + distortSample.y);
                float surfaceNoiseSample = SAMPLE_TEXTURE2D(_SurfaceNoise, sampler_SurfaceNoise, noiseUV).r;
                // float surfaceNoise = surfaceNoiseSample > surfaceNoiseCutoff ? 1 : 0;
                float surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);
                float4 surfaceNoiseColor = _FormColor;
                surfaceNoiseColor.a *= surfaceNoise;
                return AlphaBlend(surfaceNoiseColor, waterColor);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}