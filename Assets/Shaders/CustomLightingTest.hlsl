#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void MainLightTest_float(float3 WorldPos, float3 WorldNormal, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
    {
    #if SHADERGRAPH_PREVIEW
        Direction = float3(0.5, 0.5, -0.25);
        Color = 1;
        DistanceAtten = 1;
        ShadowAtten = 1;
    #else
        #if SHADOWS_SCREEN
            float4 clipPos = TransformWorldToHClip(WorldPos);
            float4 shadowCoord = ComputeScreenPos(clipPos);
        #else
            half cascadeIndex = ComputeCascadeIndex(WorldPos);
            float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(WorldPos, 1.0));
        #endif
            Light mainLight = GetMainLight(shadowCoord);
            Direction = mainLight.direction;
            Color = LightingLambert(mainLight.color, mainLight.direction, WorldNormal);
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

/*
- Handles additional lights (e.g. additional directional, point, spotlights)
- For custom lighting, you may want to duplicate this and swap the LightingLambert / LightingSpecular functions out. See Toon Example below!
- To work in the Unlit Graph, the following keywords must be defined in the blackboard :
	- Boolean Keyword, Global Multi-Compile "_ADDITIONAL_LIGHT_SHADOWS"
	- Boolean Keyword, Global Multi-Compile "_ADDITIONAL_LIGHTS"
- To support Forward+ path,
	- Boolean Keyword, Global Multi-Compile "_FORWARD_PLUS" (2022.2+)
*/
void AdditionalLightsTest_float(float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView, float4 Shadowmask,
							out float3 Diffuse, out float3 Specular, out float3 Direction, out float Attenuation) {
	float3 diffuseColor = 0;
	float3 specularColor = 0;
	float3 totalDirection = 0;
	float attenuation = 0;

#ifndef SHADERGRAPH_PREVIEW
	Smoothness = exp2(10 * Smoothness + 1);
	uint pixelLightCount = GetAdditionalLightsCount();
	uint meshRenderingLayers = GetMeshRenderingLayer();

	#if USE_FORWARD_PLUS
	for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++) {
		FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK
		Light light = GetAdditionalLight(lightIndex, WorldPosition, 1);
	#ifdef _LIGHT_LAYERS
		if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
	#endif
		{
			// Blinn-Phong
			float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
			diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
			specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldView, float4(SpecColor, 0), Smoothness);
		}
	}
	#endif


	// For Foward+ the LIGHT_LOOP_BEGIN macro will use inputData.normalizedScreenSpaceUV, inputData.positionWS, so create that:
	InputData inputData = (InputData)0;
	float4 screenPos = ComputeScreenPos(TransformWorldToHClip(WorldPosition));
	inputData.normalizedScreenSpaceUV = screenPos.xy / screenPos.w;
	inputData.positionWS = WorldPosition;

	LIGHT_LOOP_BEGIN(pixelLightCount)
		Light light = GetAdditionalLight(lightIndex, WorldPosition, 1);
	#ifdef _LIGHT_LAYERS
		if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
	#endif
		{
			// Blinn-Phong
			float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
			diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
			specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldView, float4(SpecColor, 0), Smoothness);
			attenuation += light.distanceAttenuation * light.shadowAttenuation;
			totalDirection += light.direction * (light.distanceAttenuation * light.shadowAttenuation);
		}
	LIGHT_LOOP_END
#endif

	Diffuse = diffuseColor;
	Specular = specularColor;
	Attenuation = attenuation;
	Direction = totalDirection;
}

#endif
