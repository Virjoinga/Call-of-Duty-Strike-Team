̋Shader "Corona/Effects/CloseSnow2" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Size ("Size", Float) = 0.01
 _Alpha ("Alpha", Float) = 1
 _CameraMotionBlur ("CameraMotionBlur", Float) = 0.3
 _PosScale ("PosScale", Float) = 1
}
SubShader { 
 Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp float xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp float _PosScale;
uniform highp float _CameraMotionBlur;
uniform highp float _Alpha;
uniform highp float _Size;
uniform highp vec4 g_closeSnowWindData;
uniform highp vec4 g_closeSnowFade;
uniform highp vec4 g_closeSnowOffset;
uniform highp vec4 g_closeSnowData;
uniform highp vec4 g_cameraVel;
uniform highp vec4 g_cameraPos;
uniform highp vec3 _DepthBand;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _Time;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.w = 0.0;
  tmpvar_1.xyz = _glesNormal;
  vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  lowp float tmpvar_3;
  highp vec2 sinCos_4;
  highp vec3 pos_5;
  highp float tmpvar_6;
  tmpvar_6 = dot (_glesNormal, _glesNormal);
  highp vec3 tmpvar_7;
  tmpvar_7.y = 0.0;
  tmpvar_7.x = -(_glesNormal.z);
  tmpvar_7.z = tmpvar_1.x;
  highp vec3 tmpvar_8;
  tmpvar_8.xz = vec2(0.0, 0.0);
  tmpvar_8.y = tmpvar_6;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 - (_glesNormal.y * _glesNormal));
  highp float tmpvar_10;
  tmpvar_10 = (1.0/(max (1e-05, sqrt(dot (_glesNormal.xz, _glesNormal.xz)))));
  highp float tmpvar_11;
  tmpvar_11 = (sqrt(tmpvar_6) * tmpvar_10);
  highp float tmpvar_12;
  tmpvar_12 = ((_glesColor.y * 2.5) + 2.0);
  highp float x_13;
  x_13 = ((_Time.y + _glesColor.x) * tmpvar_12);
  sinCos_4.x = sin(x_13);
  sinCos_4.y = cos(x_13);
  highp vec2 tmpvar_14;
  tmpvar_14 = (sinCos_4 * 0.15);
  highp vec2 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (tmpvar_12 * 0.015));
  highp vec3 tmpvar_16;
  tmpvar_16 = ((fract(((((_glesVertex.xyz + (_glesNormal * g_closeSnowWindData.x)) + ((tmpvar_7 * (tmpvar_11 * tmpvar_14.y)) + (tmpvar_9 * (tmpvar_10 * tmpvar_14.x)))) * g_closeSnowData.w) + g_closeSnowData.xyz)) * (g_closeSnowOffset.w * _PosScale)) + ((g_closeSnowOffset.xyz * _PosScale) + (g_cameraPos * (1.0 - _PosScale)).xyz));
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize((tmpvar_16 - g_cameraPos.xyz));
  highp vec3 tmpvar_18;
  tmpvar_18 = ((g_cameraVel * _CameraMotionBlur).xyz - (((_glesNormal * (0.015 * g_closeSnowWindData.y)) + ((tmpvar_9 * (tmpvar_10 * tmpvar_15.y)) - (tmpvar_7 * (tmpvar_11 * tmpvar_15.x)))) * _PosScale));
  highp vec3 tmpvar_19;
  tmpvar_19 = ((tmpvar_18.yzx * tmpvar_17.zxy) - (tmpvar_18.zxy * tmpvar_17.yzx));
  highp vec3 tmpvar_20;
  tmpvar_20 = ((tmpvar_17.yzx * tmpvar_19.zxy) - (tmpvar_17.zxy * tmpvar_19.yzx));
  highp float tmpvar_21;
  tmpvar_21 = max (1.0, (_Size / sqrt(dot (tmpvar_18, tmpvar_18))));
  highp float tmpvar_22;
  tmpvar_22 = inversesqrt(dot (tmpvar_19, tmpvar_19));
  highp vec2 tmpvar_23;
  tmpvar_23 = ((_glesMultiTexCoord0.xy * 2.0) - 1.0);
  pos_5 = (tmpvar_16 + (tmpvar_20 * ((max ((_Size - ((dot (tmpvar_18, tmpvar_20) * tmpvar_21) * tmpvar_22)), 0.0) * tmpvar_22) * tmpvar_23.x)));
  pos_5 = (pos_5 + (tmpvar_18 * (tmpvar_23.x * tmpvar_21)));
  pos_5 = (pos_5 + (tmpvar_19 * ((tmpvar_23.y * _Size) * tmpvar_22)));
  highp float tmpvar_24;
  tmpvar_24 = ((g_closeSnowFade.x * clamp (((tmpvar_17 * g_closeSnowFade.w) + g_closeSnowFade.z), 0.0, 1.0).x) + g_closeSnowFade.y);
  highp float tmpvar_25;
  if ((tmpvar_24 < 0.25)) {
    tmpvar_25 = tmpvar_24;
  } else {
    tmpvar_25 = 1.0;
  };
  highp vec4 tmpvar_26;
  tmpvar_26.w = 1.0;
  tmpvar_26.xyz = pos_5;
  highp vec4 tmpvar_27;
  tmpvar_27.w = 1.0;
  tmpvar_27.xyz = tmpvar_26.xyz;
  highp vec4 tmpvar_28;
  tmpvar_28 = (glstate_matrix_mvp * tmpvar_27);
  highp vec4 tmpvar_29;
  tmpvar_29.x = tmpvar_28.x;
  tmpvar_29.y = tmpvar_28.y;
  tmpvar_29.z = ((tmpvar_28.z * _DepthBand.z) + (tmpvar_28.w * _DepthBand.y));
  tmpvar_29.w = tmpvar_28.w;
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_25 * _Alpha);
  tmpvar_3 = tmpvar_30;
  gl_Position = tmpvar_29;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_COLOR = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying lowp float xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1.xyz = vec3(1.0, 1.0, 1.0);
  tmpvar_1.w = (texture2D (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR);
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
#define gl_Normal _glesNormal
in vec3 _glesNormal;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
in vec4 _glesMultiTexCoord0;
void xll_sincos_f_f_f( float x, out float s, out float c) {
  s = sin(x); 
  c = cos(x); 
}
void xll_sincos_vf2_vf2_vf2( vec2 x, out vec2 s, out vec2 c) {
  s = sin(x); 
  c = cos(x); 
}
void xll_sincos_vf3_vf3_vf3( vec3 x, out vec3 s, out vec3 c) {
  s = sin(x); 
  c = cos(x); 
}
void xll_sincos_vf4_vf4_vf4( vec4 x, out vec4 s, out vec4 c) {
  s = sin(x); 
  c = cos(x); 
}
void xll_sincos_mf2x2_mf2x2_mf2x2( mat2 x, out mat2 s, out mat2 c) {
  s = mat2( sin ( x[0] ), sin ( x[1] ) ); 
  c = mat2( cos ( x[0] ), cos ( x[1] ) ); 
}
void xll_sincos_mf3x3_mf3x3_mf3x3( mat3 x, out mat3 s, out mat3 c) {
  s = mat3( sin ( x[0] ), sin ( x[1] ), sin ( x[2] ) ); 
  c = mat3( cos ( x[0] ), cos ( x[1] ), cos ( x[2] ) ); 
}
void xll_sincos_mf4x4_mf4x4_mf4x4( mat4 x, out mat4 s, out mat4 c) {
  s = mat4( sin ( x[0] ), sin ( x[1] ), sin ( x[2] ), sin ( x[3] ) ); 
  c = mat4( cos ( x[0] ), cos ( x[1] ), cos ( x[2] ), cos ( x[3] ) ); 
}
float xll_saturate_f( float x) {
  return clamp( x, 0.0, 1.0);
}
vec2 xll_saturate_vf2( vec2 x) {
  return clamp( x, 0.0, 1.0);
}
vec3 xll_saturate_vf3( vec3 x) {
  return clamp( x, 0.0, 1.0);
}
vec4 xll_saturate_vf4( vec4 x) {
  return clamp( x, 0.0, 1.0);
}
mat2 xll_saturate_mf2x2(mat2 m) {
  return mat2( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0));
}
mat3 xll_saturate_mf3x3(mat3 m) {
  return mat3( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0), clamp(m[2], 0.0, 1.0));
}
mat4 xll_saturate_mf4x4(mat4 m) {
  return mat4( clamp(m[0], 0.0, 1.0), clamp(m[1], 0.0, 1.0), clamp(m[2], 0.0, 1.0), clamp(m[3], 0.0, 1.0));
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
#line 404
struct v2f {
    highp vec4 vertex;
    mediump vec2 texCoord;
    lowp float alpha;
};
#line 396
struct appdata_t {
    highp vec4 vertex;
    highp vec4 normal;
    highp vec4 color;
    highp vec2 texCoord;
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
uniform highp vec4 g_cameraPos;
uniform highp vec4 g_cameraRight;
uniform highp vec4 g_cameraUp;
uniform highp vec4 g_cameraForward;
#line 386
uniform highp vec4 g_cameraVel;
uniform highp vec4 g_closeSnowData;
uniform highp vec4 g_closeSnowOffset;
uniform highp vec4 g_closeSnowFade;
#line 390
uniform highp vec4 g_closeSnowWindData;
uniform highp float _Size;
uniform highp float _Alpha;
uniform highp float _CameraMotionBlur;
#line 394
uniform highp float _PosScale;
uniform sampler2D _MainTex;
#line 411
#line 318
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 320
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 411
v2f vert( in appdata_t v ) {
    highp float time = _Time.y;
    highp float windDirTime = g_closeSnowWindData.x;
    #line 415
    const highp float c_timeInterval = 0.015;
    highp vec3 normal = vec3( v.normal);
    highp vec3 pos = (v.vertex.xyz + (normal * windDirTime));
    highp vec3 vel = (normal * (0.015 * g_closeSnowWindData.y));
    #line 419
    highp float normalLengthSq = dot( normal, normal);
    highp float normalLength = sqrt(normalLengthSq);
    highp vec3 spiralX = vec3( (-normal.z), 0.0, normal.x);
    highp vec3 spiralY = (vec3( 0.0, normalLengthSq, 0.0) - (normal.y * normal));
    #line 423
    highp float rcpNormalXZLength = (1.0 / max( 1e-05, length(normal.xz)));
    highp float spiralXScale = (normalLength * rcpNormalXZLength);
    highp float spiralYScale = rcpNormalXZLength;
    highp float spiralTime = (time + (v.color.x * 1.0));
    #line 427
    highp float spiralTimeScale = ((v.color.y * 2.5) + 2.0);
    highp vec2 sinCos;
    xll_sincos_f_f_f( (spiralTime * spiralTimeScale), sinCos.x, sinCos.y);
    highp vec2 sinCosP = (sinCos * 0.15);
    #line 431
    highp vec2 sinCosV = (sinCosP * (spiralTimeScale * 0.015));
    highp vec3 spiralPos = ((spiralX * (spiralXScale * sinCosP.y)) + (spiralY * (spiralYScale * sinCosP.x)));
    highp vec3 spiralVel = ((spiralY * (spiralYScale * sinCosV.y)) - (spiralX * (spiralXScale * sinCosV.x)));
    pos += spiralPos;
    #line 435
    vel += spiralVel;
    vel = ((vel * _PosScale) - vec3( (g_cameraVel * _CameraMotionBlur)));
    pos = fract(((pos * g_closeSnowData.w) + g_closeSnowData.xyz));
    pos = ((pos * (g_closeSnowOffset.w * _PosScale)) + ((g_closeSnowOffset.xyz * _PosScale) + vec3( (g_cameraPos * (1.0 - _PosScale)))));
    #line 439
    highp vec3 camDir = normalize((pos - vec3( g_cameraPos)));
    highp vec3 xDir = (-vel);
    highp vec3 yDir = cross( xDir, camDir);
    highp vec3 screenXDir = cross( camDir, yDir);
    #line 443
    highp float xScale = max( 1.0, (_Size / length(xDir)));
    highp float yScale = (1.0 / length(yDir));
    highp vec2 quadOffset = ((v.texCoord.xy * 2.0) - 1.0);
    pos.xyz += (screenXDir * ((max( (_Size - ((dot( xDir, screenXDir) * xScale) * yScale)), 0.0) * yScale) * quadOffset.x));
    #line 447
    pos.xyz += (xDir * (quadOffset.x * xScale));
    pos.xyz += (yDir * ((quadOffset.y * _Size) * yScale));
    highp float alphaInterp = float( xll_saturate_vf3(((camDir * g_closeSnowFade.w) + g_closeSnowFade.z)));
    highp float alpha = ((g_closeSnowFade.x * alphaInterp) + g_closeSnowFade.y);
    #line 451
    alpha = (( (alpha < 0.25) ) ? ( alpha ) : ( 1.0 ));
    v2f o;
    o.vertex = ObjectToClipPos( vec4( pos, 1.0));
    o.texCoord = v.texCoord;
    #line 455
    o.alpha = (alpha * _Alpha);
    return o;
}
out mediump vec2 xlv_TEXCOORD0;
out lowp float xlv_COLOR;
void main() {
    v2f xl_retval;
    appdata_t xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.normal = vec4(gl_Normal, 0.0);
    xlt_v.color = vec4(gl_Color);
    xlt_v.texCoord = vec2(gl_MultiTexCoord0);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.vertex);
    xlv_TEXCOORD0 = vec2(xl_retval.texCoord);
    xlv_COLOR = float(xl_retval.alpha);
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
#line 404
struct v2f {
    highp vec4 vertex;
    mediump vec2 texCoord;
    lowp float alpha;
};
#line 396
struct appdata_t {
    highp vec4 vertex;
    highp vec4 normal;
    highp vec4 color;
    highp vec2 texCoord;
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
uniform highp vec4 g_cameraPos;
uniform highp vec4 g_cameraRight;
uniform highp vec4 g_cameraUp;
uniform highp vec4 g_cameraForward;
#line 386
uniform highp vec4 g_cameraVel;
uniform highp vec4 g_closeSnowData;
uniform highp vec4 g_closeSnowOffset;
uniform highp vec4 g_closeSnowFade;
#line 390
uniform highp vec4 g_closeSnowWindData;
uniform highp float _Size;
uniform highp float _Alpha;
uniform highp float _CameraMotionBlur;
#line 394
uniform highp float _PosScale;
uniform sampler2D _MainTex;
#line 411
#line 458
lowp vec4 frag( in v2f i ) {
    #line 460
    lowp float texAlpha = texture( _MainTex, i.texCoord).w;
    lowp vec4 colour = vec4( 1.0, 1.0, 1.0, (texAlpha * i.alpha));
    return colour;
}
in mediump vec2 xlv_TEXCOORD0;
in lowp float xlv_COLOR;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.vertex = vec4(0.0);
    xlt_i.texCoord = vec2(xlv_TEXCOORD0);
    xlt_i.alpha = float(xlv_COLOR);
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
}