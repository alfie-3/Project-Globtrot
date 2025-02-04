float3 LightingSpecular(float3 lightColor, float3 lightDir, float3 normal, float3 viewDir, float4 specular, float smoothness)
{
    float3 halfVec = normalize(float3(lightDir) + float3(viewDir)); //SafeNormalize
    float NdotH = saturate(dot(normal, halfVec));
    float modifier = pow(NdotH, smoothness);
    float3 specularReflection = specular.rgb * modifier;
    return lightColor * specularReflection;
}

void DirectSpecular_float(float3 Specular, float Smoothness, float3 Direction, float3 Color, float3 WorldNormal, float3 WorldView, out float3 Out)
{
#if SHADERGRAPH_PREVIEW
		Out = 0;
#else
    Smoothness = exp2(10 * Smoothness + 1);
    WorldNormal = normalize(WorldNormal);
    WorldView = normalize(WorldView); //SafeNormalize
    Out = LightingSpecular(Color, Direction, WorldNormal, WorldView, float4(Specular, 0), Smoothness);
#endif
}