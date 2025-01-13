inline float sdPlane( float3 p, float3 n, float h )
{
    return dot(p, normalize(n)) + h;
}

SamplerState SmpClampPoint;

float3 calcIntrudeOffset(float3 pos, float2 uv)
{
    const float3 wpos = lilTransformOStoWS(pos);
    const float3 camDir = -UNITY_MATRIX_V._m20_m21_m22;
    const float3 camPos = _WorldSpaceCameraPos;

    const float near = _ProjectionParams.y;
    const float bias = _UzumoreBias;

    const float d = sdPlane(wpos - camPos, camDir, -(near+bias));
    float3 intrude = float3(0,0,0);

    const float amount = _UzumoreAmount * LIL_SAMPLE_2D_LOD(_UzumoreMask, SmpClampPoint, uv, 0).r;
    if(d <= 0.0f)
    {
        intrude = min(-d, amount)*camDir;
    }
    return lilTransformWStoOS(wpos + intrude);
}