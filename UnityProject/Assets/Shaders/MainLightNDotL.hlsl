float MainLightNDotL(float3 NormalWS)
{
    #if defined(SHADERGRAPH_PREVIEW)
        return 1.0;
    #else
        Light l = GetMainLight();
        float3 L = normalize(-l.direction);
        return saturate(dot(normalize(NormalWS), L));
    #endif
}