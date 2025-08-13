τShader "Corona/Effects/CloseSnowVolume" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Size ("Size", Float) = 0.01
 _Alpha ("Alpha", Float) = 1
 _Color ("Main Color", Color) = (1,1,1,1)
 _CentreFadeSpeed ("CentreFadeSpeed", Float) = 0.25
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

varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp float _CentreFadeSpeed;
uniform lowp vec4 _Color;
uniform highp float _Alpha;
uniform highp float _Size;
uniform highp vec4 g_closeSnowWindData;
uniform highp vec4 g_closeSnowFade;
uniform highp vec4 g_closeSnowOffset;
uniform highp vec4 g_closeSnowData2;
uniform highp vec4 g_closeSnowData;
uniform highp vec4 g_cameraForward;
uniform highp vec4 g_cameraUp;
uniform highp vec4 g_cameraRight;
uniform highp vec4 g_cameraPos;
uniform highp vec3 _DepthBand;
uniform lowp vec3 _SpecDir;
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
  lowp vec4 tmpvar_3;
  highp vec3 lightDir_4;
  highp vec3 pos_5;
  highp vec3 tmpvar_6;
  tmpvar_6.x = g_closeSnowData.w;
  tmpvar_6.y = g_closeSnowData2.x;
  tmpvar_6.z = g_closeSnowData.w;
  highp vec3 tmpvar_7;
  tmpvar_7.x = g_closeSnowOffset.w;
  tmpvar_7.y = g_closeSnowData2.y;
  tmpvar_7.z = g_closeSnowOffset.w;
  highp vec3 tmpvar_8;
  tmpvar_8 = ((fract((((_glesVertex.xyz + (tmpvar_1 * g_closeSnowWindData.x).xyz) * tmpvar_6) + g_closeSnowData.xyz)) * tmpvar_7) + g_closeSnowOffset.xyz);
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 - g_cameraPos.xyz);
  highp float tmpvar_10;
  tmpvar_10 = sqrt(dot (tmpvar_9, tmpvar_9));
  highp float tmpvar_11;
  tmpvar_11 = (clamp (((tmpvar_10 * (_CentreFadeSpeed * 2.0)) - ((6.0 * _CentreFadeSpeed) + 1.0)), 0.0, 1.0) * clamp ((10.0 - (sqrt(dot (tmpvar_9.xz, tmpvar_9.xz)) * (g_closeSnowData.w * 20.0))), 0.0, 1.0));
  highp float tmpvar_12;
  tmpvar_12 = min ((_Size * clamp (((tmpvar_10 * (_CentreFadeSpeed * 4.0)) - (12.0 * _CentreFadeSpeed)), 0.0, 1.0)), (dot (tmpvar_9, g_cameraForward.xyz) * 0.3));
  highp float tmpvar_13;
  tmpvar_13 = ((_glesColor.x * (4.0 * _Time.y)) - (2.0 * _Time.y));
  highp float tmpvar_14;
  tmpvar_14 = sin(tmpvar_13);
  highp float tmpvar_15;
  tmpvar_15 = cos(tmpvar_13);
  highp vec2 tmpvar_16;
  tmpvar_16 = ((_glesMultiTexCoord0.xy * (2.0 * tmpvar_12)) - tmpvar_12);
  pos_5 = (tmpvar_8 + ((((tmpvar_16.x * tmpvar_15) - (tmpvar_16.y * tmpvar_14)) * g_cameraRight.xyz) + (((tmpvar_16.y * tmpvar_15) + (tmpvar_16.x * tmpvar_14)) * g_cameraUp.xyz)));
  highp float tmpvar_17;
  tmpvar_17 = ((g_closeSnowFade.x * clamp (((tmpvar_10 * g_closeSnowFade.w) + g_closeSnowFade.z), 0.0, 1.0)) + g_closeSnowFade.y);
  highp float tmpvar_18;
  if ((tmpvar_17 < 0.25)) {
    tmpvar_18 = tmpvar_17;
  } else {
    tmpvar_18 = 1.0;
  };
  lowp vec3 tmpvar_19;
  tmpvar_19.y = 0.0;
  tmpvar_19.x = _SpecDir.x;
  tmpvar_19.z = _SpecDir.z;
  lowp vec3 tmpvar_20;
  tmpvar_20 = normalize(tmpvar_19);
  lightDir_4 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = ((tmpvar_18 * tmpvar_11) * (_Alpha - ((_Alpha * 0.25) * max (0.0, dot (lightDir_4, normalize((pos_5 - g_cameraPos.xyz)))))));
  highp vec4 tmpvar_22;
  tmpvar_22.w = 1.0;
  tmpvar_22.xyz = pos_5;
  highp vec4 tmpvar_23;
  tmpvar_23.w = 1.0;
  tmpvar_23.xyz = tmpvar_22.xyz;
  highp vec4 tmpvar_24;
  tmpvar_24 = (glstate_matrix_mvp * tmpvar_23);
  highp vec4 tmpvar_25;
  tmpvar_25.x = tmpvar_24.x;
  tmpvar_25.y = tmpvar_24.y;
  tmpvar_25.z = ((tmpvar_24.z * _DepthBand.z) + (tmpvar_24.w * _DepthBand.y));
  tmpvar_25.w = tmpvar_24.w;
  tmpvar_3.xyz = _Color.xyz;
  tmpvar_3.w = tmpvar_21;
  gl_Position = tmpvar_25;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_COLOR = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  gl_FragData[0] = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
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
    lowp vec4 color;
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
uniform highp vec4 g_closeSnowData;
uniform highp vec4 g_closeSnowData2;
uniform highp vec4 g_closeSnowOffset;
uniform highp vec4 g_closeSnowFade;
#line 390
uniform highp vec4 g_closeSnowWindData;
uniform highp float _Size;
uniform highp float _Alpha;
uniform lowp vec4 _Color;
#line 394
uniform highp float _CentreFadeSpeed;
uniform sampler2D _MainTex;
#line 411
#line 452
#line 318
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 320
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 411
v2f vert( in appdata_t v ) {
    highp vec3 pos = v.vertex.xyz;
    highp float time = _Time.y;
    #line 415
    highp float windDirTime = g_closeSnowWindData.x;
    pos += vec3( (v.normal * windDirTime));
    pos = fract(((pos * vec3( g_closeSnowData.w, g_closeSnowData2.x, g_closeSnowData.w)) + g_closeSnowData.xyz));
    pos = ((pos * vec3( g_closeSnowOffset.w, g_closeSnowData2.y, g_closeSnowOffset.w)) + g_closeSnowOffset.xyz);
    #line 419
    highp vec3 camDelta = (pos - vec3( g_cameraPos));
    highp float camDist = length(camDelta);
    highp float camDir = float( (camDelta / camDist));
    highp float fadeAlpha = xll_saturate_f(((camDist * (_CentreFadeSpeed * 2.0)) - ((6.0 * _CentreFadeSpeed) + 1.0)));
    #line 423
    highp float fadeSize = xll_saturate_f(((camDist * (_CentreFadeSpeed * 4.0)) - (12.0 * _CentreFadeSpeed)));
    highp float edgeFade = xll_saturate_f((10.0 - (length(camDelta.xz) * (g_closeSnowData.w * 20.0))));
    fadeAlpha *= edgeFade;
    highp float perpDist = dot( camDelta, vec3( g_cameraForward));
    #line 427
    highp float size = min( (_Size * fadeSize), (perpDist * 0.3));
    highp float spin = ((v.color.x * (4.0 * time)) - (2.0 * time));
    highp float sinX;
    highp float cosX;
    #line 431
    xll_sincos_f_f_f( spin, sinX, cosX);
    highp vec2 quadOffset = ((v.texCoord * (2.0 * size)) - size);
    highp float right = ((quadOffset.x * cosX) - (quadOffset.y * sinX));
    highp float up = ((quadOffset.y * cosX) + (quadOffset.x * sinX));
    #line 435
    highp vec3 billboardOffset = ((right * g_cameraRight.xyz) + (up * g_cameraUp.xyz));
    pos.xyz += billboardOffset;
    highp float alphaInterp = xll_saturate_f(((camDist * g_closeSnowFade.w) + g_closeSnowFade.z));
    highp float alpha = ((g_closeSnowFade.x * alphaInterp) + g_closeSnowFade.y);
    #line 439
    alpha = (( (alpha < 0.25) ) ? ( alpha ) : ( 1.0 ));
    alpha *= fadeAlpha;
    highp vec3 camDir2 = normalize((pos - vec3( g_cameraPos)));
    highp vec3 lightDir = normalize(vec3( _SpecDir.x, 0.0, _SpecDir.z));
    #line 443
    highp float tintLerp = max( 0.0, dot( lightDir, camDir2));
    alpha *= (_Alpha - ((_Alpha * 0.25) * tintLerp));
    v2f o;
    o.vertex = ObjectToClipPos( vec4( pos, 1.0));
    #line 447
    o.texCoord = v.texCoord;
    o.color.xyz = _Color.xyz;
    o.color.w = alpha;
    return o;
}
out mediump vec2 xlv_TEXCOORD0;
out lowp vec4 xlv_COLOR;
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
    xlv_COLOR = vec4(xl_retval.color);
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
    lowp vec4 color;
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
uniform highp vec4 g_closeSnowData;
uniform highp vec4 g_closeSnowData2;
uniform highp vec4 g_closeSnowOffset;
uniform highp vec4 g_closeSnowFade;
#line 390
uniform highp vec4 g_closeSnowWindData;
uniform highp float _Size;
uniform highp float _Alpha;
uniform lowp vec4 _Color;
#line 394
uniform highp float _CentreFadeSpeed;
uniform sampler2D _MainTex;
#line 411
#line 452
#line 452
lowp vec4 frag( in v2f i ) {
    return (texture( _MainTex, i.texCoord) * i.color);
}
in mediump vec2 xlv_TEXCOORD0;
in lowp vec4 xlv_COLOR;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.vertex = vec4(0.0);
    xlt_i.texCoord = vec2(xlv_TEXCOORD0);
    xlt_i.color = vec4(xlv_COLOR);
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