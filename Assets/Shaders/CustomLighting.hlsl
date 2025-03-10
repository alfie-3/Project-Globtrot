void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
        {
        #if SHADERGRAPH_PREVIEW
                Direction = float3(0.5, 0.5, -0.25);
                Color = 1;
                DistanceAtten = 1;
                ShadowAtten = 1;
        #else
            #if SHADOWS_SCREEN
                float4 clipPos = TransformWorldToHCLip(WorldPos);
                float4 shadowCoord = ComputeScreenPos(clipPos);
            #else
                half cascadeIndex = ComputeCascadeIndex(WorldPos);
                float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(WorldPos, 1.0));
            #endif
                Light mainLight = GetMainLight(shadowCoord);
                Direction = mainLight.direction;
                Color = mainLight.color;
                DistanceAtten = mainLight.distanceAttenuation;
                ShadowAtten = mainLight.shadowAttenuation;
            #if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
                ShadowAtten = 1.0f;
            #endif

            #if SHADOWS_SCREEN
                ShadowAtten = SampleScreenSpaceShadowmap(shadowCoord);
            #else
                ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
                float shadowStrength = GetMainLightShadowStrength();
                ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);
            #endif
    
            
    
        #endif
        }

void AdditionalLights_float(float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView, out float3 Diffuse, out float3 Specular)
{
    float3 diffuseColor = 0;
    float3 specularColor = 0;

#ifndef SHADERGRAPH_PREVIEW
    Smoothness = exp2(10 * Smoothness + 1);
    WorldNormal = normalize(WorldNormal);
    WorldView = SafeNormalize(WorldView);
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, WorldPosition);
        half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
        diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
        specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldView, float4(SpecColor, 0), Smoothness);
    }
#endif

    Diffuse = diffuseColor;
    Specular = specularColor;
}

void Shadowmask_half(float2 lightmapUV, out half4 Shadowmask)
{
#ifdef SHADERGRAPH_PREVIEW
		Shadowmask = half4(1,1,1,1);
#else
    OUTPUT_LIGHTMAP_UV(lightmapUV, unity_LightmapST, lightmapUV);
    Shadowmask = SAMPLE_SHADOWMASK(lightmapUV);
#endif
}