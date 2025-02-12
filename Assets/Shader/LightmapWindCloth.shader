Shader "Corona/Lightmap/[WindCloth]" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _WindAmount ("Wind Amount", Float) = 0.08
 _NormalDirectionScale ("Normal Direction Scale", Float) = 1
 _WindDirectionScale ("Wind Direction Scale", Float) = 0
 _SpatialScale ("Spatial Scale", Float) = 2
 _Snap ("Snap", Float) = 1
 _HighFrequencyAmount ("High Frequency Amount", Float) = 1
 _WindSpeed0Scale ("Wind Speed 0 Scale", Float) = 1
 _WindSpeed1Scale ("Wind Speed 1 Scale", Float) = 0
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp vec4 _FogRange;
uniform highp vec3 _FogParams;
uniform highp vec4 unity_LightmapST;
uniform highp float _WindSpeed1Scale;
uniform highp float _WindSpeed0Scale;
uniform highp float _HighFrequencyAmount;
uniform highp float _Snap;
uniform highp float _SpatialScale;
uniform highp float _WindDirectionScale;
uniform highp float _NormalDirectionScale;
uniform highp float _WindAmount;
uniform highp vec4 g_globalWindDir2;
uniform highp vec4 g_globalWindDir;
uniform highp vec4 g_globalWindData;
uniform highp vec3 _DepthBand;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _Time;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 fog_1;
  mediump vec2 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = _glesVertex;
  highp vec3 tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _glesVertex.xyz;
  tmpvar_6 = (_Object2World * tmpvar_7).xyz;
  highp float tmpvar_8;
  tmpvar_8 = (_NormalDirectionScale * _WindAmount);
  highp vec4 tmpvar_9;
  tmpvar_9.w = 0.0;
  tmpvar_9.xyz = (g_globalWindDir.xyz * (_WindDirectionScale * _WindAmount));
  highp vec3 tmpvar_10;
  tmpvar_10 = ((normalize(_glesNormal) * ((_glesColor.x * (2.0 * tmpvar_8)) - tmpvar_8)) + (_World2Object * tmpvar_9).xyz);
  highp vec2 tmpvar_11;
  tmpvar_11.x = _WindSpeed0Scale;
  tmpvar_11.y = _WindSpeed1Scale;
  highp vec2 tmpvar_12;
  tmpvar_12.x = (dot (tmpvar_6, (g_globalWindDir.xyz * _SpatialScale)) - (dot (g_globalWindData.xy, tmpvar_11) * 20.0));
  tmpvar_12.y = (dot (tmpvar_6, (g_globalWindDir2.xyz * 4.0)) - (_Time.y * 40.0));
  highp vec2 tmpvar_13;
  tmpvar_13 = cos(tmpvar_12);
  highp float tmpvar_14;
  tmpvar_14 = ((g_globalWindData.w * 0.5) + 0.5);
  highp vec2 tmpvar_15;
  tmpvar_15 = (1.0 - pow ((1.0 - abs(tmpvar_13)), vec2(mix (1.0, ((g_globalWindData.w * vec2(2.0, 1.0)) + vec2(2.0, 1.0)).x, _Snap))));
  bvec2 tmpvar_16;
  tmpvar_16 = lessThan (tmpvar_13, vec2(0.0, 0.0));
  highp vec2 b_17;
  b_17 = -(tmpvar_15);
  highp float tmpvar_18;
  if (tmpvar_16.x) {
    tmpvar_18 = b_17.x;
  } else {
    tmpvar_18 = tmpvar_15.x;
  };
  highp float tmpvar_19;
  if (tmpvar_16.y) {
    tmpvar_19 = b_17.y;
  } else {
    tmpvar_19 = tmpvar_15.y;
  };
  highp float tmpvar_20;
  tmpvar_20 = (g_globalWindData.w * _HighFrequencyAmount);
  tmpvar_5.xyz = (_glesVertex.xyz + (tmpvar_10 * (((((1.0 - (0.5 * tmpvar_20)) * tmpvar_14) + (tmpvar_19 * ((0.5 * tmpvar_20) * tmpvar_14))) * tmpvar_18) * _glesColor.w)));
  highp vec4 tmpvar_21;
  tmpvar_21.w = 1.0;
  tmpvar_21.xyz = tmpvar_5.xyz;
  highp vec4 tmpvar_22;
  tmpvar_22 = (glstate_matrix_mvp * tmpvar_21);
  highp vec4 tmpvar_23;
  tmpvar_23.x = tmpvar_22.x;
  tmpvar_23.y = tmpvar_22.y;
  tmpvar_23.z = ((tmpvar_22.z * _DepthBand.z) + (tmpvar_22.w * _DepthBand.y));
  tmpvar_23.w = tmpvar_22.w;
  highp vec2 tmpvar_24;
  tmpvar_24 = _glesMultiTexCoord0.xy;
  tmpvar_2 = tmpvar_24;
  highp vec2 tmpvar_25;
  tmpvar_25 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  tmpvar_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = clamp (((tmpvar_23.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
  highp vec4 tmpvar_27;
  tmpvar_27.xyz = (_FogParams - (_FogParams * tmpvar_26));
  tmpvar_27.w = tmpvar_26;
  fog_1.xyz = tmpvar_27.xyz;
  fog_1.w = (tmpvar_26 * 2.0);
  tmpvar_4 = fog_1;
  gl_Position = tmpvar_23;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD5 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = (((texture2D (_MainTex, xlv_TEXCOORD0).xyz * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz) * xlv_TEXCOORD5.w) + xlv_TEXCOORD5.xyz);
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX

#define gl_Vertex _glesVertex
in vec4 _glesVertex;
#define gl_Color _glesColor
in vec4 _glesColor;
#define gl_Normal (normalize(_glesNormal))
in vec3 _glesNormal;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
in vec4 _glesMultiTexCoord0;
#define gl_MultiTexCoord1 _glesMultiTexCoord1
in vec4 _glesMultiTexCoord1;
#define TANGENT vec4(normalize(_glesTANGENT.xyz), _glesTANGENT.w)
in vec4 _glesTANGENT;
vec2 xll_vecTSel_vb2_vf2_vf2 (bvec2 a, vec2 b, vec2 c) {
  return vec2 (a.x ? b.x : c.x, a.y ? b.y : c.y);
}
vec3 xll_vecTSel_vb3_vf3_vf3 (bvec3 a, vec3 b, vec3 c) {
  return vec3 (a.x ? b.x : c.x, a.y ? b.y : c.y, a.z ? b.z : c.z);
}
vec4 xll_vecTSel_vb4_vf4_vf4 (bvec4 a, vec4 b, vec4 c) {
  return vec4 (a.x ? b.x : c.x, a.y ? b.y : c.y, a.z ? b.z : c.z, a.w ? b.w : c.w);
}
#line 151
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 187
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 181
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 67
struct appdata_full {
    highp vec4 vertex;
    highp vec4 tangent;
    highp vec3 normal;
    highp vec4 texcoord;
    highp vec4 texcoord1;
    lowp vec4 color;
};
#line 430
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    mediump vec2 lmUV;
    lowp vec4 fog;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_LightPosition[8];
uniform highp vec4 unity_LightAtten[8];
#line 19
uniform highp vec4 unity_SpotDirection[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
#line 23
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
#line 27
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
#line 31
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
#line 35
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
#line 39
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
#line 43
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
#line 47
uniform highp mat4 glstate_matrix_projection;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
#line 51
uniform lowp vec4 unity_ColorSpaceGrey;
#line 77
#line 82
#line 87
#line 91
#line 96
#line 120
#line 137
#line 158
#line 166
#line 193
#line 206
#line 215
#line 220
#line 229
#line 234
#line 243
#line 260
#line 265
#line 291
#line 299
#line 307
#line 311
#line 315
uniform lowp vec3 _SpecDir;
uniform mediump float _SpecPower;
uniform highp vec3 _DepthBand;
#line 348
#line 352
#line 377
#line 382
uniform highp vec4 g_globalWindData;
uniform highp vec4 g_globalWindDir;
uniform highp vec4 g_globalWindDir2;
uniform highp float _WindAmount;
#line 386
uniform highp float _NormalDirectionScale;
uniform highp float _WindDirectionScale;
uniform highp float _SpatialScale;
uniform highp float _Snap;
#line 390
uniform highp float _HighFrequencyAmount;
uniform highp float _WindSpeed0Scale;
uniform highp float _WindSpeed1Scale;
uniform sampler2D _MainTex;
uniform sampler2D unity_Lightmap;
#line 427
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 438
#line 474
#line 337
highp vec3 ObjectToWorldPos( in highp vec3 pos ) {
    #line 339
    return (_Object2World * vec4( pos, 1.0)).xyz;
}
#line 323
highp vec3 WorldToObjectDir( in highp vec3 dir ) {
    #line 325
    return (_World2Object * vec4( dir, 0.0)).xyz;
}
#line 393
void ApplyWindCloth( inout appdata_full v ) {
    #line 395
    highp vec3 highFrequencyDir = g_globalWindDir.xyz;
    highp float highFrequencyScale = _SpatialScale;
    highp float highFrequencySpeed = 20.0;
    highp vec3 highFrequency2Dir = g_globalWindDir2.xyz;
    #line 399
    highp float highFrequency2Scale = 4.0;
    highp float highFrequency2Speed = 40.0;
    highp vec3 worldPos = ObjectToWorldPos( v.vertex.xyz);
    highp float normalDirScale = (_NormalDirectionScale * _WindAmount);
    #line 403
    highp float windDirScale = (_WindDirectionScale * _WindAmount);
    normalDirScale = ((v.color.x * (2.0 * normalDirScale)) - normalDirScale);
    highp vec3 windDirOS = ((v.normal * normalDirScale) + WorldToObjectDir( (g_globalWindDir.xyz * windDirScale)));
    highp float time = _Time.y;
    #line 407
    highp float windSpeed = dot( g_globalWindData.xy, vec2( _WindSpeed0Scale, _WindSpeed1Scale));
    highp float highFrequencyTimeOffset = (windSpeed * 20.0);
    highp vec3 spatialEnvelope = ((worldPos * vec3( 0.37, 0.2, 0.246)) - (vec3( 0.5, 0.4, 0.9) * time));
    highp float highFrequencyEnvelope = (dot( worldPos, (highFrequencyDir * highFrequencyScale)) - highFrequencyTimeOffset);
    #line 411
    highp float highFrequencyEnvelope2 = (dot( worldPos, (highFrequency2Dir * highFrequency2Scale)) - (time * highFrequency2Speed));
    highp vec2 angles = vec2( highFrequencyEnvelope, highFrequencyEnvelope2);
    highp vec2 cosAngles = cos(angles);
    highp float lowEnvelopeBase = g_globalWindData.w;
    #line 415
    highp float lowEnvelope = ((lowEnvelopeBase * 0.5) + 0.5);
    highp vec2 snapPower = vec2( mix( 1.0, float( ((lowEnvelopeBase * vec2( 2.0, 1.0)) + vec2( 2.0, 1.0))), _Snap));
    highp vec2 cosValue = (1.0 - pow( (1.0 - abs(cosAngles)), snapPower));
    cosValue = xll_vecTSel_vb2_vf2_vf2 (lessThan( cosAngles, vec2( 0.0 )), (-cosValue), cosValue);
    #line 419
    highp float highEnvelopeLerp = (lowEnvelopeBase * _HighFrequencyAmount);
    highp float highEnvelopeScale = (0.5 * highEnvelopeLerp);
    highp float highEnvelopeOffset = (1.0 - (0.5 * highEnvelopeLerp));
    highp float windScale = ((((highEnvelopeOffset * lowEnvelope) + (cosValue.y * (highEnvelopeScale * lowEnvelope))) * cosValue.x) * v.color.w);
    #line 423
    v.vertex.xyz += (windDirOS * windScale);
}
#line 318
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 320
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 332
highp vec3 ObjectToWorldDir( in highp vec3 dir ) {
    #line 334
    dir = (_Object2World * vec4( dir, 0.0)).xyz;
    return dir;
}
#line 327
highp vec3 WorldToObjectPos( in highp vec3 pos ) {
    #line 329
    pos = (_World2Object * vec4( pos, 1.0)).xyz;
    return pos;
}
#line 438
v2f vert( in appdata_full v ) {
    v2f o;
    #line 443
    ApplyWindCloth( v);
    o.pos = ObjectToClipPos( v.vertex);
    #line 447
    o.uv1 = v.texcoord.xy;
    o.lmUV = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    #line 452
    highp vec3 lightDir = WorldToObjectDir( _SpecDir.xyz);
    highp vec3 camPos = WorldToObjectPos( _WorldSpaceCameraPos.xyz);
    highp vec3 camDir = (camPos - v.vertex.xyz);
    highp vec3 normalWS = normalize(ObjectToWorldDir( v.normal));
    #line 456
    highp vec3 reflectDirWS = (_Object2World * vec4( reflect( (-camDir), v.normal.xyz), 0.0)).xyz;
    highp float dist = o.pos.z;
    #line 460
    highp float visibility;
    visibility = clamp( ((dist * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
    #line 465
    highp vec4 fog = vec4( (_FogParams.xyz - (_FogParams.xyz * visibility)), visibility);
    fog.w *= 2.0;
    #line 470
    o.fog = fog;
    return o;
}

out mediump vec2 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out lowp vec4 xlv_TEXCOORD5;
void main() {
    v2f xl_retval;
    appdata_full xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.tangent = vec4(TANGENT);
    xlt_v.normal = vec3(gl_Normal);
    xlt_v.texcoord = vec4(gl_MultiTexCoord0);
    xlt_v.texcoord1 = vec4(gl_MultiTexCoord1);
    xlt_v.color = vec4(gl_Color);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec2(xl_retval.uv1);
    xlv_TEXCOORD1 = vec2(xl_retval.lmUV);
    xlv_TEXCOORD5 = vec4(xl_retval.fog);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];

#line 151
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 187
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 181
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 67
struct appdata_full {
    highp vec4 vertex;
    highp vec4 tangent;
    highp vec3 normal;
    highp vec4 texcoord;
    highp vec4 texcoord1;
    lowp vec4 color;
};
#line 430
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    mediump vec2 lmUV;
    lowp vec4 fog;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_LightPosition[8];
uniform highp vec4 unity_LightAtten[8];
#line 19
uniform highp vec4 unity_SpotDirection[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
#line 23
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
#line 27
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
#line 31
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
#line 35
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
#line 39
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
#line 43
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
#line 47
uniform highp mat4 glstate_matrix_projection;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
#line 51
uniform lowp vec4 unity_ColorSpaceGrey;
#line 77
#line 82
#line 87
#line 91
#line 96
#line 120
#line 137
#line 158
#line 166
#line 193
#line 206
#line 215
#line 220
#line 229
#line 234
#line 243
#line 260
#line 265
#line 291
#line 299
#line 307
#line 311
#line 315
uniform lowp vec3 _SpecDir;
uniform mediump float _SpecPower;
uniform highp vec3 _DepthBand;
#line 348
#line 352
#line 377
#line 382
uniform highp vec4 g_globalWindData;
uniform highp vec4 g_globalWindDir;
uniform highp vec4 g_globalWindDir2;
uniform highp float _WindAmount;
#line 386
uniform highp float _NormalDirectionScale;
uniform highp float _WindDirectionScale;
uniform highp float _SpatialScale;
uniform highp float _Snap;
#line 390
uniform highp float _HighFrequencyAmount;
uniform highp float _WindSpeed0Scale;
uniform highp float _WindSpeed1Scale;
uniform sampler2D _MainTex;
uniform sampler2D unity_Lightmap;
#line 427
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 438
#line 474
#line 348
lowp vec3 DecodeLightmapHalf( in lowp vec4 color ) {
    return ((4.0 * color.w) * color.xyz);
}
#line 474
lowp vec4 frag( in v2f i ) {
    lowp vec4 diffusemap = texture( _MainTex, i.uv1);
    lowp vec4 specmap = vec4( 1.0);
    #line 478
    lowp vec3 envmap = vec3( 0.0);
    lowp vec3 specular = vec3( 0.0);
    lowp vec3 diffuse = vec3( 0.0);
    lowp vec3 emissive = vec3( 0.0);
    #line 484
    diffuse = DecodeLightmapHalf( texture( unity_Lightmap, i.lmUV));
    lowp vec3 colour = (((vec3( diffusemap) + (specular * specmap.xyz)) * diffuse) + (envmap * vec3( specmap)));
    #line 489
    colour = ((colour * i.fog.w) + i.fog.xyz);
    colour += emissive;
    #line 494
    return vec4( colour, 1.0);
}
in mediump vec2 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in lowp vec4 xlv_TEXCOORD5;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD0);
    xlt_i.lmUV = vec2(xlv_TEXCOORD1);
    xlt_i.fog = vec4(xlv_TEXCOORD5);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4(xl_retval);
}


#endif"
}
}
Program "fp" {
SubProgram "gles " {
"!!GLES"
}
SubProgram "gles3 " {
"!!GLES3"
}
}
 }
}
Fallback Off
}