Shader "Corona/Water/Arctic" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SunVec ("SunVector", Vector) = (-0.6,0.3,0.75,1)
 _Ripple ("Ripple1", Vector) = (0.05,0.1,0.1,0.1)
 _Ripple2 ("Ripple2", Vector) = (0.065,0.11,0.054,0.11)
 baseWaterCol ("Base Water Col", Color) = (0.7,0.9,1,1)
 subWaterCol ("SubWaterCol", Color) = (0.6,0.6,0.8,1)
 _MainTex ("NormalMap", 2D) = "white" {}
 RippleAnim ("RippleAnim", Vector) = (1000,1000,1000,1000)
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

#ifndef SHADER_API_GLES
    #define SHADER_API_GLES 1
#endif
#ifndef SHADER_API_MOBILE
    #define SHADER_API_MOBILE 1
#endif
#define gl_Vertex _glesVertex
attribute vec4 _glesVertex;
#define gl_Normal (normalize(_glesNormal))
attribute vec3 _glesNormal;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
attribute vec4 _glesMultiTexCoord0;

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
#line 387
struct v2f {
    highp vec4 pos;
    highp vec4 ripples;
    highp vec4 pcopy;
};
#line 52
struct appdata_base {
    highp vec4 vertex;
    highp vec3 normal;
    highp vec4 texcoord;
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
#line 363
uniform highp vec4 RippleAnim;
#line 380
uniform highp vec4 _Ripple;
uniform highp vec4 _Ripple2;
uniform highp vec4 _SunVec;
uniform highp vec4 baseWaterCol;
#line 384
uniform highp vec4 subWaterCol;
uniform sampler _MainTex;
uniform highp vec4 _Color;
#line 394
#line 407
#line 318
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 320
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 394
v2f vert( in appdata_base v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 398
    o.pcopy = (_Object2World * v.vertex);
    o.pcopy.w = (_Time.y * 0.2);
    o.ripples.x = dot( o.pcopy.xz, _Ripple.xy);
    o.ripples.y = dot( o.pcopy.xz, _Ripple.zw);
    #line 402
    o.ripples.z = dot( o.pcopy.xz, _Ripple2.xy);
    o.ripples.w = dot( o.pcopy.xz, _Ripple2.zw);
    o.ripples += ((vec4( 0.1, 0.11, 0.02, 0.025) * _Time.w) * 2.0);
    return o;
}
varying highp vec4 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main() {
    v2f xl_retval;
    appdata_base xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.normal = vec3(gl_Normal);
    xlt_v.texcoord = vec4(gl_MultiTexCoord0);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec4(xl_retval.ripples);
    xlv_TEXCOORD1 = vec4(xl_retval.pcopy);
}
/* NOTE: GLSL optimization failed
0:386(17): error: syntax error, unexpected NEW_IDENTIFIER, expecting '{'
*/


#endif
#ifdef FRAGMENT

#ifndef SHADER_API_GLES
    #define SHADER_API_GLES 1
#endif
#ifndef SHADER_API_MOBILE
    #define SHADER_API_MOBILE 1
#endif
float xll_mod_f_f( float x, float y ) {
  float d = x / y;
  float f = fract (abs(d)) * y;
  return d >= 0.0 ? f : -f;
}
vec2 xll_mod_vf2_vf2( vec2 x, vec2 y ) {
  vec2 d = x / y;
  vec2 f = fract (abs(d)) * y;
  return vec2 (d.x >= 0.0 ? f.x : -f.x, d.y >= 0.0 ? f.y : -f.y);
}
vec3 xll_mod_vf3_vf3( vec3 x, vec3 y ) {
  vec3 d = x / y;
  vec3 f = fract (abs(d)) * y;
  return vec3 (d.x >= 0.0 ? f.x : -f.x, d.y >= 0.0 ? f.y : -f.y, d.z >= 0.0 ? f.z : -f.z);
}
vec4 xll_mod_vf4_vf4( vec4 x, vec4 y ) {
  vec4 d = x / y;
  vec4 f = fract (abs(d)) * y;
  return vec4 (d.x >= 0.0 ? f.x : -f.x, d.y >= 0.0 ? f.y : -f.y, d.z >= 0.0 ? f.z : -f.z, d.w >= 0.0 ? f.w : -f.w);
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
#line 387
struct v2f {
    highp vec4 pos;
    highp vec4 ripples;
    highp vec4 pcopy;
};
#line 52
struct appdata_base {
    highp vec4 vertex;
    highp vec3 normal;
    highp vec4 texcoord;
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
#line 363
uniform highp vec4 RippleAnim;
#line 380
uniform highp vec4 _Ripple;
uniform highp vec4 _Ripple2;
uniform highp vec4 _SunVec;
uniform highp vec4 baseWaterCol;
#line 384
uniform highp vec4 subWaterCol;
uniform sampler _MainTex;
uniform highp vec4 _Color;
#line 394
#line 407
#line 407
lowp vec4 frag( in v2f i ) {
    highp vec4 modrip = xll_mod_vf4_vf4( i.ripples, vec4( 1.0));
    highp vec4 remrip = (i.ripples - modrip);
    #line 411
    highp vec4 mdsqr = (modrip * modrip);
    highp vec4 mdcube = (mdsqr * modrip);
    highp vec4 dmd = (((modrip * 2.0) + (mdcube * 4.0)) - (mdsqr * 6.0));
    highp vec3 relpos = (vec3( i.pcopy) - _WorldSpaceCameraPos);
    #line 415
    highp vec3 eyepos = normalize(relpos);
    highp vec2 uv = (i.pcopy.xz + (eyepos.zx * dmd.x));
    uv += i.pcopy.ww;
    highp vec2 uv2 = (i.pcopy.xz + (eyepos.zx * dmd.x));
    #line 419
    uv2 += (i.pcopy.ww * 0.9);
    highp vec3 norm = ((((texture2D( _MainTex, ((remrip.xx + uv) * 0.1)).xzy - 0.5) * dmd.x) * 5.0) + (((texture2D( _MainTex, ((remrip.ww + uv2) * 0.052)).yzx - 0.5) * dmd.w) * 5.0));
    norm.y = 1.0;
    norm.xz += ((((_Ripple.xy * dmd.x) + (_Ripple.zw * dmd.y)) + (_Ripple2.xy * dmd.z)) + (_Ripple2.zw * dmd.w));
    #line 423
    norm = normalize(norm);
    highp vec3 ref = (eyepos - ((norm * 2.0) * dot( eyepos, norm)));
    highp float b = (-1.0 - dot( eyepos, norm));
    b *= (b * b);
    #line 427
    b = (-1.0 - b);
    highp vec3 c = (baseWaterCol.xyz + (subWaterCol.xyz * min( 0.0, b)));
    highp float s = dot( _SunVec.xyz, ref);
    s = max( 0.0, ((s * 200.0) - 198.0));
    #line 431
    c += vec3( (_Color * s));
    return vec4( c, 1.0);
}
varying highp vec4 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.ripples = vec4(xlv_TEXCOORD0);
    xlt_i.pcopy = vec4(xlv_TEXCOORD1);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4(xl_retval);
}
/* NOTE: GLSL optimization failed
0:386(17): error: syntax error, unexpected NEW_IDENTIFIER, expecting '{'
*/


#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX

#define gl_Vertex _glesVertex
in vec4 _glesVertex;
#define gl_Normal (normalize(_glesNormal))
in vec3 _glesNormal;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
in vec4 _glesMultiTexCoord0;

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
#line 390
struct v2f {
    highp vec4 pos;
    highp vec4 ripples;
    highp vec4 pcopy;
};
#line 52
struct appdata_base {
    highp vec4 vertex;
    highp vec3 normal;
    highp vec4 texcoord;
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
uniform highp vec4 RippleAnim;
uniform highp vec4 _Ripple = vec4( 0.05, 0.1, 0.1, 0.1);
uniform highp vec4 _Ripple2 = vec4( 0.065, 0.11, 0.054, 0.11);
uniform highp vec4 _SunVec = vec4( -0.6, 0.3, 0.75, 1.0);
#line 386
uniform highp vec4 baseWaterCol = vec4( 0.7, 0.9, 1.0, 1.0);
uniform highp vec4 subWaterCol = vec4( 0.6, 0.6, 0.8, 1.0);
uniform sampler _MainTex;
uniform highp vec4 _Color;
#line 397
#line 410
#line 318
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 320
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 397
v2f vert( in appdata_base v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 401
    o.pcopy = (_Object2World * v.vertex);
    o.pcopy.w = (_Time.y * 0.2);
    o.ripples.x = dot( o.pcopy.xz, _Ripple.xy);
    o.ripples.y = dot( o.pcopy.xz, _Ripple.zw);
    #line 405
    o.ripples.z = dot( o.pcopy.xz, _Ripple2.xy);
    o.ripples.w = dot( o.pcopy.xz, _Ripple2.zw);
    o.ripples += ((vec4( 0.1, 0.11, 0.02, 0.025) * _Time.w) * 2.0);
    return o;
}
out highp vec4 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
void main() {
    v2f xl_retval;
    appdata_base xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.normal = vec3(gl_Normal);
    xlt_v.texcoord = vec4(gl_MultiTexCoord0);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec4(xl_retval.ripples);
    xlv_TEXCOORD1 = vec4(xl_retval.pcopy);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];
float xll_mod_f_f( float x, float y ) {
  float d = x / y;
  float f = fract (abs(d)) * y;
  return d >= 0.0 ? f : -f;
}
vec2 xll_mod_vf2_vf2( vec2 x, vec2 y ) {
  vec2 d = x / y;
  vec2 f = fract (abs(d)) * y;
  return vec2 (d.x >= 0.0 ? f.x : -f.x, d.y >= 0.0 ? f.y : -f.y);
}
vec3 xll_mod_vf3_vf3( vec3 x, vec3 y ) {
  vec3 d = x / y;
  vec3 f = fract (abs(d)) * y;
  return vec3 (d.x >= 0.0 ? f.x : -f.x, d.y >= 0.0 ? f.y : -f.y, d.z >= 0.0 ? f.z : -f.z);
}
vec4 xll_mod_vf4_vf4( vec4 x, vec4 y ) {
  vec4 d = x / y;
  vec4 f = fract (abs(d)) * y;
  return vec4 (d.x >= 0.0 ? f.x : -f.x, d.y >= 0.0 ? f.y : -f.y, d.z >= 0.0 ? f.z : -f.z, d.w >= 0.0 ? f.w : -f.w);
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
#line 390
struct v2f {
    highp vec4 pos;
    highp vec4 ripples;
    highp vec4 pcopy;
};
#line 52
struct appdata_base {
    highp vec4 vertex;
    highp vec3 normal;
    highp vec4 texcoord;
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
uniform highp vec4 RippleAnim;
uniform highp vec4 _Ripple = vec4( 0.05, 0.1, 0.1, 0.1);
uniform highp vec4 _Ripple2 = vec4( 0.065, 0.11, 0.054, 0.11);
uniform highp vec4 _SunVec = vec4( -0.6, 0.3, 0.75, 1.0);
#line 386
uniform highp vec4 baseWaterCol = vec4( 0.7, 0.9, 1.0, 1.0);
uniform highp vec4 subWaterCol = vec4( 0.6, 0.6, 0.8, 1.0);
uniform sampler _MainTex;
uniform highp vec4 _Color;
#line 397
#line 410
#line 410
lowp vec4 frag( in v2f i ) {
    highp vec4 modrip = xll_mod_vf4_vf4( i.ripples, vec4( 1.0));
    highp vec4 remrip = (i.ripples - modrip);
    #line 414
    highp vec4 mdsqr = (modrip * modrip);
    highp vec4 mdcube = (mdsqr * modrip);
    highp vec4 dmd = (((modrip * 2.0) + (mdcube * 4.0)) - (mdsqr * 6.0));
    highp vec3 relpos = (vec3( i.pcopy) - _WorldSpaceCameraPos);
    #line 418
    highp vec3 eyepos = normalize(relpos);
    highp vec2 uv = (i.pcopy.xz + (eyepos.zx * dmd.x));
    uv += i.pcopy.ww;
    highp vec2 uv2 = (i.pcopy.xz + (eyepos.zx * dmd.x));
    #line 422
    uv2 += (i.pcopy.ww * 0.9);
    highp vec3 norm = ((((texture( _MainTex, ((remrip.xx + uv) * 0.1)).xzy - 0.5) * dmd.x) * 5.0) + (((texture( _MainTex, ((remrip.ww + uv2) * 0.052)).yzx - 0.5) * dmd.w) * 5.0));
    norm.y = 1.0;
    norm.xz += ((((_Ripple.xy * dmd.x) + (_Ripple.zw * dmd.y)) + (_Ripple2.xy * dmd.z)) + (_Ripple2.zw * dmd.w));
    #line 426
    norm = normalize(norm);
    highp vec3 ref = (eyepos - ((norm * 2.0) * dot( eyepos, norm)));
    highp float b = (-1.0 - dot( eyepos, norm));
    b *= (b * b);
    #line 430
    b = (-1.0 - b);
    highp vec3 c = (baseWaterCol.xyz + (subWaterCol.xyz * min( 0.0, b)));
    highp float s = dot( _SunVec.xyz, ref);
    s = max( 0.0, ((s * 200.0) - 198.0));
    #line 434
    c += vec3( (_Color * s));
    return vec4( c, 1.0);
}
in highp vec4 xlv_TEXCOORD0;
in highp vec4 xlv_TEXCOORD1;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.ripples = vec4(xlv_TEXCOORD0);
    xlt_i.pcopy = vec4(xlv_TEXCOORD1);
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