Shader "Custom/URP/SampleLit"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1, 1)
        [NoScaleOffset] _MetallicGlossMap ("Metallic Gloss Map", 2D) = "white" {}
        _MetallicGloss ("Metallic Gloss", Range(0, 1)) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 1
        [NoScaleOffset][Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _OPTIMIZING_PBR
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPreMaterial)
            float4 _BaseMap_ST;
            half4 _BaseColor;
            half _Smoothness;
            half _MetallicGloss;
            float _BumpScale;
            CBUFFER_END

            TEXTURE2D(_BaseMap);          SAMPLER(sampler_BaseMap);
            TEXTURE2D(_MetallicGlossMap); SAMPLER(sampler_MetallicGlossMap);
            TEXTURE2D(_BumpMap);          SAMPLER(sampler_BumpMap);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 texcoord : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 normalWS : TEXCOORD0;
                float4 tangentWS : TEXCOORD1;
                float4 bitangentWS : TEXCOORD2;
                float2 uv : TEXCOORD3;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 4);
            };

            real Pow5(real x)
            {
                return x * x * x * x * x;
            }

            half DistributionGGX(half NoH, half roughness2)
            {
                float d = NoH * NoH * (roughness2 - 1) + 1;
                return roughness2 / (d * d * PI);
            }

            half GeometrySchlickGGX(half NoV, half k)
            {
                return NoV / (NoV * (1 - k) + k);
            }

            half GeometrySmith(half NoV, half NoL, half perceptualRoughness)
            {
                half k = (perceptualRoughness + 1) * (perceptualRoughness + 1) / 8;
                half ggx1 = GeometrySchlickGGX(NoV, k);
                half ggx2 = GeometrySchlickGGX(NoL, k);
                return ggx1 * ggx2;
            }

            half3 FresnelSchlick(half3 F0, float cosA)
            {
                return F0 + (1 - F0) * Pow5(1 - cosA);
            }

            half3 FresnelLerp(half3 F0, half3 F90, half cosA)
            {
                return lerp(F0, F90, Pow5(1 - cosA));
            }

            half3 DirectPBR(Light light, half3 normalWS, half3 viewDirWS, half3 albedo, half perceptualRoughness, half reflectivity)
            {
                half roughness = max(perceptualRoughness * perceptualRoughness, HALF_MIN_SQRT);
                half roughness2 = max(roughness * roughness, HALF_MIN);
                half3 lightDirWS = normalize(light.direction);
                half3 lightColor = light.color;
                half lightAttenuation = light.distanceAttenuation * light.shadowAttenuation;
                half3 F0 = reflectivity * albedo;
                half3 halfDirWS = normalize(viewDirWS + lightDirWS);
                half3 diffuse = (1 - reflectivity) * albedo;

                #if defined(_OPTIMIZING_PBR)
                    // GGX Distribution multiplied by combined approximation of Visibility and Fresnel
                    // BRDFspec = (D * V * F) / 4.0
                    // D = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2
                    // V * F = 1.0 / ( LoH^2 * (roughness + 0.5) )
                    // See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
                    // https://community.arm.com/events/1155

                    // Final BRDFspec = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2 * (LoH^2 * (roughness + 0.5) * 4.0)
                    // We further optimize a few light invariant terms
                    // brdfData.normalizationTerm = (roughness + 0.5) * 4.0 rewritten as roughness * 4.0 + 2.0 to a fit a MAD.
                    half LoH = max(dot(lightDirWS, halfDirWS), HALF_MIN);
                    half NoH = max(dot(normalWS, halfDirWS), HALF_MIN);
                    half NoL = max(dot(normalWS, lightDirWS), HALF_MIN);
                    half d = NoH * NoH * (roughness2 - 1) + 1.00001f;
                    half specular = roughness2 / (d * d * max(0.1, LoH * LoH) * (roughness * 4 + 2));
                    half3 radiance = lightColor * NoL * lightAttenuation;
                    return (diffuse + F0 * specular) * radiance;
                #else
                    half NoH = max(dot(normalWS, halfDirWS), HALF_MIN);
                    half NoV = max(dot(normalWS, viewDirWS), HALF_MIN);
                    half NoL = max(dot(normalWS, lightDirWS), HALF_MIN);
                    half VoH = max(dot(viewDirWS, halfDirWS), HALF_MIN);
                    half D = DistributionGGX(NoH, roughness2);
                    half G = GeometrySmith(NoV, NoL, perceptualRoughness);
                    half3 F = FresnelSchlick(F0, VoH);
                    half3 radiance = lightColor * NoL * lightAttenuation;
                    half3 specular = D * G * F / (4 * NoV * NoL);
                    return (diffuse + specular) * radiance;
                #endif
            }

            half3 IndirectPBR(half3 normalWS, half3 viewDirWS, half3 albedo, half3 bakeGI, half smoothness, half perceptualRoughness, half reflectivity)
            {
                half roughness = max(perceptualRoughness * perceptualRoughness, HALF_MIN_SQRT);
                half roughness2 = max(roughness * roughness, HALF_MIN);
                half3 reflectVector = reflect(-viewDirWS, normalWS);
                half mip = perceptualRoughness * (1.7 - 0.7 * perceptualRoughness) * 6;
                half4 encodedIrradiance = half4(SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflectVector, mip));
                half3 irradiance = DecodeHDREnvironment(encodedIrradiance, unity_SpecCube0_HDR);
                half NoV = saturate(dot(normalWS, viewDirWS));
                half3 F0 = reflectivity * albedo;
                half grazingTerm = saturate(smoothness + reflectivity);
                half surfaceReduction = 1 / (roughness2 + 1);
                half3 specularTerm = surfaceReduction * FresnelLerp(F0, grazingTerm, NoV);
                half3 diffuse = (1 - reflectivity) * albedo;
                return bakeGI * diffuse + irradiance * specularTerm;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS.xyz = TransformObjectToWorldNormal(input.normalOS);
                output.tangentWS.xyz = TransformObjectToWorldDir(input.tangentOS.xyz);
                output.bitangentWS.xyz = cross(output.normalWS.xyz, output.tangentWS.xyz) * input.tangentOS.w;
                output.normalWS.w = positionWS.z;
                output.tangentWS.w = positionWS.x;
                output.bitangentWS.w = positionWS.y;
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, out.lightmapUV);
                output.vertexSH = SampleSHVertex(output.normalWS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 positionWS = float3(input.tangentWS.w, input.bitangentWS.w, input.normalWS.w);
                half3x3 tangentToWorld = half3x3(normalize(input.tangentWS.xyz), normalize(input.bitangentWS.xyz), normalize(input.normalWS.xyz));
                float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                float4 metallicColor = SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, input.uv);
                float4 bumpColor = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
                half4 shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);

                half3 normalTS = UnpackNormalScale(bumpColor, _BumpScale);
                half3 normalWS = normalize(TransformTangentToWorld(normalTS, tangentToWorld));
                half3 viewDirWS = normalize(GetWorldSpaceViewDir(positionWS));
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(positionWS), positionWS, shadowMask);

                half metalness = metallicColor.r * _MetallicGloss;
                half3 albedo = baseColor.rgb * _BaseColor.rgb;
                half smoothness = metallicColor.a * _Smoothness;
                half perceptualRoughness = 1 - smoothness;
                half reflectivity = lerp(0.04, 1, metalness);

                half3 directLightColor = DirectPBR(mainLight, normalWS, viewDirWS, albedo, perceptualRoughness, reflectivity);
                half3 bakeGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS);
                half3 indirectLightColor = IndirectPBR(normalWS, viewDirWS, albedo, bakeGI, smoothness, perceptualRoughness, reflectivity);

                #if defined(_ADDITIONAL_LIGHTS)
                    uint pixelLightCount = GetAdditionalLightsCount();
                    for (uint lightIndex = 0; lightIndex < pixelLightCount; ++lightIndex)
                    {
                        Light light = GetAdditionalLight(lightIndex, positionWS, shadowMask);
                        directLightColor += DirectPBR(light, normalWS, normalWS, albedo, perceptualRoughness, reflectivity);
                    }
                #endif

                return half4(directLightColor + indirectLightColor, 1);
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // -------------------------------------
            // Universal Pipeline keywords

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull Off

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}