inline float sdPlane( float3 p, float3 n, float h )
{
    return dot(p, normalize(n)) + h;
}

float3 calcIntrudePos(float3 pos, float3 normalOS, float2 uv)
{
    const float3 wnormal = lilTransformDirOStoWS(normalOS, true);
    const float3 wpos = lilTransformOStoWS(pos);
    const float3 camDir = -LIL_MATRIX_V._m20_m21_m22;
    const float3 camPos = _WorldSpaceCameraPos;

    const float near = _ProjectionParams.y;
    const float bias = _UzumoreBias;

    const float d = sdPlane(wpos - camPos, camDir, -(near+bias));
    float3 intrude = float3(0,0,0);

    const float amount = _UzumoreAmount * LIL_SAMPLE_2D_LOD(_UzumoreMask, sampler_UzumoreMask, uv, 0).r;
    if(d <= 0.0f && dot(camDir, wnormal) < -0.2)
    {
        intrude = min(-d, amount)*camDir;
    }
    return lilTransformWStoOS(wpos + intrude);
}