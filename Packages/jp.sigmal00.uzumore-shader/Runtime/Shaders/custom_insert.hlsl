inline float Uzumore_CalcDistanceFromPlane( float3 p, float3 n, float h )
{
    return dot(p, normalize(n)) + h;
}

float3 Uzumore_Slerp(float3 a, float3 b, float t)
{
    float d = clamp(dot(a, b), -1.0, 1.0);

    // b から a 方向の成分を除いた、a に直交する成分（長さは sinθ）
    float3 c = b - a * d;
    float lenSq = dot(c, c);

    // a と b が正反対だと c が 0 になり、回る向きが決まらない。その場合は a に直交する適当な軸を使う
    float3 helper = abs(a.y) < 0.99 ? float3(0, 1, 0) : float3(1, 0, 0);
    float3 perp = normalize(cross(a, helper));
    c = lenSq > 1e-10 ? c * rsqrt(lenSq) : perp;

    float theta = acos(d) * t;
    return a * cos(theta) + c * sin(theta);
}

inline float Uzumore_CalcMaxPushAmount(float2 uv)
{
    return _UzumoreAmount * LIL_SAMPLE_2D_LOD(_UzumoreMask, sampler_UzumoreMask, uv, 0).r;
}

inline float Uzumore_GetMaxBias()
{
    return _UzumoreBias;
}

float Uzumore_CalcPushAmount(float3 pos, float3 camPos, float3 camDir, float maxPushAmount)
{
    const float near = _ProjectionParams.y;
    const float maxBias = Uzumore_GetMaxBias();
    const float d = Uzumore_CalcDistanceFromPlane(pos - camPos, -camDir, near + maxBias);
    const float pushAmount = clamp(d, 0, maxPushAmount);
    return pushAmount;
}

float3 Uzumore_CalcPushedPosition(float3 positionOS, float3 normalOS, float2 uv)
{
    const float3 camDir = lilTransformDirWStoOS(-LIL_MATRIX_V._m20_m21_m22, true);

    if (dot(camDir, normalOS) >= -0.2)
    {
        return positionOS;
    }

    const float maxPushAmount = Uzumore_CalcMaxPushAmount(uv);

    const float pushAmount = Uzumore_CalcPushAmount
        (
            positionOS,
            lilTransformWStoOS(_WorldSpaceCameraPos),
            camDir,
            maxPushAmount
        );

    if (pushAmount <= 0.0f)
    {
        return positionOS;
    }

    const float maxBias = Uzumore_GetMaxBias();
    const float biasRate = min(1.0f, pushAmount/max(maxPushAmount, 0.00001));
    const float bias = maxBias*biasRate;

    const float3 extrude = (pushAmount - bias)*camDir;
    return positionOS + extrude;
}

// ファーのパスで LIL_CUSTOM_VERT_COPY の書き込み先にするダミー（custom.hlsl の LIL_V2F_OUT_BASE 参照）
struct UzumoreDummyVertCopy
{
    float3 uzumoreOrgPosition;
    float3 uzumoreOrgNormal;
};
static UzumoreDummyVertCopy g_uzumoreDummyVertCopy;

// LIL_CUSTOM_VERTEX_OS で書いて LIL_CUSTOM_VERT_COPY で v2f に詰める
static float3 g_uzumoreOrgPositionOS = float3(0,0,0);
static float3 g_uzumoreOrgNormalOS = float3(0,0,0);

inline void Uzumore_ApplyVertex(inout float3 positionOS, float3 normalOS, float2 uv)
{
    g_uzumoreOrgPositionOS = positionOS;
    g_uzumoreOrgNormalOS = normalOS;
    positionOS = Uzumore_CalcPushedPosition(positionOS, normalOS, uv);
}

//----------------------------------------------------------------------------------------------------------------------
// Fragment

// ピクセルシェーダーに渡ってくる埋もれ情報（v2f の補間値）
struct UzumoreFragInput
{
    float3 orgPositionOS;   // 押し戻し前の位置（オブジェクト空間）
    float3 orgNormalOS;     // 頂点法線（オブジェクト空間、補間後なので未正規化）
};

inline UzumoreFragInput Uzumore_MakeFragInput(float3 orgPositionOS, float3 orgNormalOS)
{
    UzumoreFragInput uz;
    uz.orgPositionOS = orgPositionOS;
    uz.orgNormalOS = orgNormalOS;
    return uz;
}

// 法線・色フックをどこで実行するか（custom.hlsl の BEFORE_xx 参照）
// LIL_LITE / LIL_GEM はここ（SubShader の HLSLINCLUDE より後）なら判定できる
#define UZUMORE_HOOK_NONE       0
#define UZUMORE_HOOK_AUDIOLINK  1
#define UZUMORE_HOOK_MATCAP     2
#define UZUMORE_HOOK_ANISOTROPY 3
#define UZUMORE_HOOK_FOG        4

#if defined(LIL_LITE)
    #define UZUMORE_COLOR_HOOK UZUMORE_HOOK_MATCAP
#else
    #define UZUMORE_COLOR_HOOK UZUMORE_HOOK_FOG
#endif

#if defined(LIL_GEM)
    // Gem は法線から作るローカル変数が多く、後から差し替えると整合が取れないので無効
    #define UZUMORE_NORMAL_HOOK UZUMORE_HOOK_NONE
#elif defined(LIL_LITE)
    #define UZUMORE_NORMAL_HOOK UZUMORE_HOOK_MATCAP
#else
    #define UZUMORE_NORMAL_HOOK UZUMORE_HOOK_AUDIOLINK
#endif

// 法線を書き換える
//   normalWS : ワールド空間・正規化済み・裏面反転済みの法線（ノーマルマップ適用後）。書き換えた値は正規化して使われる
//   fd.origN : ノーマルマップ適用前の法線
//   fd.positionWS / fd.V / fd.uvMain なども参照できる
inline void ApplyFragmentNormal_Internal(inout float3 normalWS, lilFragData fd, UzumoreFragInput uz)
{
    const float2 uv = fd.uv0;
    const float3 positionOS = uz.orgPositionOS;

    const float3 camDir = lilTransformDirWStoOS(-LIL_MATRIX_V._m20_m21_m22, true);
    const float3 camPos = lilTransformWStoOS(_WorldSpaceCameraPos);
    const float maxPushAmount = Uzumore_CalcMaxPushAmount(uv);
    const float pushAmount = Uzumore_CalcPushAmount
    (
        positionOS,
        camPos,
        camDir,
        maxPushAmount
    );

    if (pushAmount <= 0.0f)
    {
        return;
    }

    normalWS = normalize(Uzumore_Slerp(normalWS, LIL_MATRIX_V._m20_m21_m22, saturate(pushAmount/max(_UzumoreEdgeFadeWidth, 0.00001))));
}

// 色を書き換える
//   呼ばれる位置で color（fd.col.rgb）の中身が違う
//     Lite    : BEFORE_MATCAP。ライティング前・MatCap 前の色（この後 fd.albedo にコピーされる）
//     それ以外: BEFORE_FOG。ライティング・距離フェード後の最終色。アウトラインもここを通る
//               ForwardAdd パスでは追加ライトの加算分なので、塗りつぶす場合は注意
inline void ApplyFragmentColor_Internal(inout float3 color, lilFragData fd, UzumoreFragInput uz)
{
    const float2 uv = fd.uv0;
    const float3 positionOS = uz.orgPositionOS;

    const float3 camDir = lilTransformDirWStoOS(-LIL_MATRIX_V._m20_m21_m22, true);
    const float3 camPos = lilTransformWStoOS(_WorldSpaceCameraPos);
    const float maxPushAmount = Uzumore_CalcMaxPushAmount(uv);
    const float pushAmount = Uzumore_CalcPushAmount
    (
        positionOS,
        camPos,
        camDir,
        maxPushAmount
    );

    if (pushAmount <= 0.0f)
    {
        return;
    }

    color *= lerp(1.0, _UzumoreColor.rgb, saturate(pushAmount/max(_UzumoreEdgeFadeWidth, 0.0001)));
}

inline void Uzumore_ApplyFragmentNormal(inout lilFragData fd, UzumoreFragInput uz, int hook)
{
    // hook も UZUMORE_NORMAL_HOOK も定数なので、一致しない方はコンパイル時に消える
    if(hook != UZUMORE_NORMAL_HOOK) return;

    float3 N = fd.N;
    if(_UzumoreCorrectNormals)
    {
       ApplyFragmentNormal_Internal(N, fd, uz);
    }
    fd.N = N;

    // lilToon が法線から作っている値を作り直す（lil_pass_forward_normal.hlsl の Normal 節と同じ）
    fd.ln = dot(fd.L, fd.N);
    fd.nv = saturate(dot(fd.N, fd.V));
    fd.nvabs = abs(dot(fd.N, fd.V));
    fd.uvRim = float2(fd.nvabs, fd.nvabs);
    #if !defined(LIL_LITE)
        // Lite の MatCap UV は頂点シェーダーで計算済みなので触らない
        fd.uvMat = mul(fd.cameraMatrix, fd.N).xy * 0.5 + 0.5;
    #endif
    fd.reflectionN = fd.N;
    fd.matcapN = fd.N;
    fd.matcap2ndN = fd.N;
}

inline void Uzumore_ApplyFragmentColor(inout lilFragData fd, UzumoreFragInput uz, int hook)
{
    // 法線と同じく、一致しない方はコンパイル時に消える
    if(hook != UZUMORE_COLOR_HOOK) return;

    ApplyFragmentColor_Internal(fd.col.rgb, fd, uz);
}