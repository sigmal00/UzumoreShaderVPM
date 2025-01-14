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

    const float maxAmount = _UzumoreAmount * LIL_SAMPLE_2D_LOD(_UzumoreMask, sampler_UzumoreMask, uv, 0).r;
    const float maxBias = 0.0005;
    const float d = sdPlane(wpos - camPos, -camDir, (near+maxBias));
    const float intrudeAmount = clamp(d, 0, maxAmount);

    if(intrudeAmount > 0.0f && dot(camDir, wnormal) < -0.2)
    {
        const float biasRate = min(1.0f, intrudeAmount/max(maxAmount, 0.00001));
        const float bias = maxBias*biasRate;

        float3 extrude = (intrudeAmount-bias)*camDir;
        return lilTransformWStoOS(wpos + extrude);
    }

    return pos;
}