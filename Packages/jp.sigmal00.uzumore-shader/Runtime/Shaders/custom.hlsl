//----------------------------------------------------------------------------------------------------------------------
// Macro

// Custom variables
#define LIL_CUSTOM_PROPERTIES \
    float _UzumoreAmount; \
    float4 _UzumoreColor; \
    float _UzumoreEdgeFadeWidth; \
    int _UzumoreCorrectNormals; \
    float _UzumoreBias;

// Custom textures
#define LIL_CUSTOM_TEXTURES \
    TEXTURE2D(_UzumoreMask); \
    SAMPLER(sampler_UzumoreMask);

// Add vertex shader input
//#define LIL_REQUIRE_APP_POSITION
//#define LIL_REQUIRE_APP_TEXCOORD0
//#define LIL_REQUIRE_APP_TEXCOORD1
//#define LIL_REQUIRE_APP_TEXCOORD2
//#define LIL_REQUIRE_APP_TEXCOORD3
//#define LIL_REQUIRE_APP_TEXCOORD4
//#define LIL_REQUIRE_APP_TEXCOORD5
//#define LIL_REQUIRE_APP_TEXCOORD6
//#define LIL_REQUIRE_APP_TEXCOORD7
//#define LIL_REQUIRE_APP_COLOR
//#define LIL_REQUIRE_APP_NORMAL
//#define LIL_REQUIRE_APP_TANGENT
//#define LIL_REQUIRE_APP_VERTEXID

// Add vertex shader output
//#define LIL_V2F_FORCE_TEXCOORD0
//#define LIL_V2F_FORCE_TEXCOORD1
//#define LIL_V2F_FORCE_POSITION_OS
//#define LIL_V2F_FORCE_POSITION_WS
//#define LIL_V2F_FORCE_POSITION_SS
//#define LIL_V2F_FORCE_NORMAL
//#define LIL_V2F_FORCE_TANGENT
//#define LIL_V2F_FORCE_BITANGENT

#if defined(UNITY_PASS_SHADOWCASTER)
// Don't use id0 because of following bug.
//   https://github.com/lilxyzw/lilToon/blob/1.4.0/Assets/lilToon/Shader/Includes/lil_pass_shadowcaster.hlsl#L44-L47
#define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7) \
    float3 uzumoreOrgPosition : TEXCOORD ## id1; \
    float3 uzumoreOrgNormal   : TEXCOORD ## id2;
#else
// uzumoreOrgPosition : 押し戻し前の頂点位置（オブジェクト空間）
// uzumoreOrgNormal   : 頂点法線（オブジェクト空間）
#define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7) \
    float3 uzumoreOrgPosition : TEXCOORD ## id0; \
    float3 uzumoreOrgNormal   : TEXCOORD ## id1;
#endif


// Add vertex copy
// ファーのパス（lil_common_vert_fur.hlsl）では LIL_V2F_OUT_BASE が定義されず、出力も v2g なので、
// 書き込み先をダミーにして何もしないようにする。
// 通常のパスでは lil_common_vert.hlsl が #undef してから定義し直すので、こちらの定義は影響しない。
#define LIL_V2F_OUT_BASE g_uzumoreDummyVertCopy

// LIL_CUSTOM_VERTEX_OS で static 変数に書いた色を v2f に詰める（VERTEX_OS の後に実行される）
// 1パスアウトラインでは v2f が output.base になるので LIL_V2F_OUT_BASE を使う
#define LIL_CUSTOM_VERT_COPY \
    LIL_V2F_OUT_BASE.uzumoreOrgPosition = g_uzumoreOrgPositionOS; \
    LIL_V2F_OUT_BASE.uzumoreOrgNormal   = g_uzumoreOrgNormalOS;

// Inserting a process into the vertex shader
#define LIL_CUSTOM_VERTEX_OS \
    Uzumore_ApplyVertex(positionOS.xyz, input.normalOS, uvMain);

// Inserting a process into pixel shader
#define BEFORE_AUDIOLINK \
    Uzumore_ApplyFragmentNormal(fd, Uzumore_MakeFragInput(input.uzumoreOrgPosition, input.uzumoreOrgNormal), UZUMORE_HOOK_AUDIOLINK);
#define BEFORE_MATCAP \
    Uzumore_ApplyFragmentNormal(fd, Uzumore_MakeFragInput(input.uzumoreOrgPosition, input.uzumoreOrgNormal), UZUMORE_HOOK_MATCAP); \
    Uzumore_ApplyFragmentColor(fd, Uzumore_MakeFragInput(input.uzumoreOrgPosition, input.uzumoreOrgNormal), UZUMORE_HOOK_MATCAP);
#define BEFORE_FOG \
    Uzumore_ApplyFragmentColor(fd, Uzumore_MakeFragInput(input.uzumoreOrgPosition, input.uzumoreOrgNormal), UZUMORE_HOOK_FOG);
//#define OVERRIDE_xx

//----------------------------------------------------------------------------------------------------------------------
// Information about variables
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
// Vertex shader inputs (appdata structure)
//
// Type     Name                    Description
// -------- ----------------------- --------------------------------------------------------------------
// float4   input.positionOS        POSITION
// float2   input.uv0               TEXCOORD0
// float2   input.uv1               TEXCOORD1
// float2   input.uv2               TEXCOORD2
// float2   input.uv3               TEXCOORD3
// float2   input.uv4               TEXCOORD4
// float2   input.uv5               TEXCOORD5
// float2   input.uv6               TEXCOORD6
// float2   input.uv7               TEXCOORD7
// float4   input.color             COLOR
// float3   input.normalOS          NORMAL
// float4   input.tangentOS         TANGENT
// uint     vertexID                SV_VertexID

//----------------------------------------------------------------------------------------------------------------------
// Vertex shader outputs or pixel shader inputs (v2f structure)
//
// The structure depends on the pass.
// Please check lil_pass_xx.hlsl for details.
//
// Type     Name                    Description
// -------- ----------------------- --------------------------------------------------------------------
// float4   output.positionCS       SV_POSITION
// float2   output.uv01             TEXCOORD0 TEXCOORD1
// float2   output.uv23             TEXCOORD2 TEXCOORD3
// float3   output.positionOS       object space position
// float3   output.positionWS       world space position
// float3   output.normalWS         world space normal
// float4   output.tangentWS        world space tangent

//----------------------------------------------------------------------------------------------------------------------
// Variables commonly used in the forward pass
//
// These are members of `lilFragData fd`
//
// Type     Name                    Description
// -------- ----------------------- --------------------------------------------------------------------
// float4   col                     lit color
// float3   albedo                  unlit color
// float3   emissionColor           color of emission
// -------- ----------------------- --------------------------------------------------------------------
// float3   lightColor              color of light
// float3   indLightColor           color of indirectional light
// float3   addLightColor           color of additional light
// float    attenuation             attenuation of light
// float3   invLighting             saturate((1.0 - lightColor) * sqrt(lightColor));
// -------- ----------------------- --------------------------------------------------------------------
// float2   uv0                     TEXCOORD0
// float2   uv1                     TEXCOORD1
// float2   uv2                     TEXCOORD2
// float2   uv3                     TEXCOORD3
// float2   uvMain                  Main UV
// float2   uvMat                   MatCap UV
// float2   uvRim                   Rim Light UV
// float2   uvPanorama              Panorama UV
// float2   uvScn                   Screen UV
// bool     isRightHand             input.tangentWS.w > 0.0;
// -------- ----------------------- --------------------------------------------------------------------
// float3   positionOS              object space position
// float3   positionWS              world space position
// float4   positionCS              clip space position
// float4   positionSS              screen space position
// float    depth                   distance from camera
// -------- ----------------------- --------------------------------------------------------------------
// float3x3 TBN                     tangent / bitangent / normal matrix
// float3   T                       tangent direction
// float3   B                       bitangent direction
// float3   N                       normal direction
// float3   V                       view direction
// float3   L                       light direction
// float3   origN                   normal direction without normal map
// float3   origL                   light direction without sh light
// float3   headV                   middle view direction of 2 cameras
// float3   reflectionN             normal direction for reflection
// float3   matcapN                 normal direction for reflection for MatCap
// float3   matcap2ndN              normal direction for reflection for MatCap 2nd
// float    facing                  VFACE
// -------- ----------------------- --------------------------------------------------------------------
// float    vl                      dot(viewDirection, lightDirection);
// float    hl                      dot(headDirection, lightDirection);
// float    ln                      dot(lightDirection, normalDirection);
// float    nv                      saturate(dot(normalDirection, viewDirection));
// float    nvabs                   abs(dot(normalDirection, viewDirection));
// -------- ----------------------- --------------------------------------------------------------------
// float4   triMask                 TriMask (for lite version)
// float3   parallaxViewDirection   mul(tbnWS, viewDirection);
// float2   parallaxOffset          parallaxViewDirection.xy / (parallaxViewDirection.z+0.5);
// float    anisotropy              strength of anisotropy
// float    smoothness              smoothness
// float    roughness               roughness
// float    perceptualRoughness     perceptual roughness
// float    shadowmix               this variable is 0 in the shadow area
// float    audioLinkValue          volume acquired by AudioLink
// -------- ----------------------- --------------------------------------------------------------------
// uint     renderingLayers         light layer of object (for URP / HDRP)
// uint     featureFlags            feature flags (for HDRP)
// uint2    tileIndex               tile index (for HDRP)