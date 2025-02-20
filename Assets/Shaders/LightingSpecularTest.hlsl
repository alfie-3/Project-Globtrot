float3 LightingSpecular(float3 lightColor, float3 lightDir, float3 normal, float3 viewDir, float4 specular, float smoothness)
{
    float3 halfVec = normalize(float3(lightDir) + float3(viewDir)); //SafeNormalize
    float NdotH = saturate(dot(normal, halfVec));
    float modifier = pow(NdotH, smoothness);
    float3 specularReflection = specular.rgb * modifier;
    return lightColor * specularReflection;
}

void DirectSpecular_half(half3 Specular, half Smoothness, half3 Direction, half3 Color, half3 WorldNormal, half3 WorldView, out half3 Out)
{
#if SHADERGRAPH_PREVIEW
		Out = 0;
#else
    Smoothness = exp2(10 * Smoothness + 1);
    WorldNormal = normalize(WorldNormal);
    WorldView = normalize(WorldView); //SafeNormalize
    Out = LightingSpecular(Color, Direction, WorldNormal, WorldView, half4(Specular, 0), Smoothness);
#endif
}