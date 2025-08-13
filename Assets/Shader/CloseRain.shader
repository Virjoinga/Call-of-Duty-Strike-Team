ÔrShader "Corona/Effects/CloseRain" {
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
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp float tmpvar_2;
  highp vec3 pos_3;
  highp vec3 tmpvar_4;
  tmpvar_4 = ((fract((((_glesVertex.xyz + (_glesNormal * g_closeSnowWindData.x)) * g_closeSnowData.w) + g_closeSnowData.xyz)) * (g_closeSnowOffset.w * _PosScale)) + ((g_closeSnowOffset.xyz * _PosScale) + (g_cameraPos * (1.0 - _PosScale)).xyz));
  highp vec3 tmpvar_5;
  tmpvar_5 = normalize((tmpvar_4 - g_cameraPos.xyz));
  highp vec3 tmpvar_6;
  tmpvar_6 = ((g_cameraVel * _CameraMotionBlur).xyz - ((_glesNormal * (0.015 * g_closeSnowWindData.y)) * _PosScale));
  highp vec3 tmpvar_7;
  tmpvar_7 = ((tmpvar_6.yzx * tmpvar_5.zxy) - (tmpvar_6.zxy * tmpvar_5.yzx));
  highp vec3 tmpvar_8;
  tmpvar_8 = ((tmpvar_5.yzx * tmpvar_7.zxy) - (tmpvar_5.zxy * tmpvar_7.yzx));
  highp float tmpvar_9;
  tmpvar_9 = max (1.0, (_Size / sqrt(dot (tmpvar_6, tmpvar_6))));
  highp float tmpvar_10;
  tmpvar_10 = inversesqrt(dot (tmpvar_7, tmpvar_7));
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * 2.0) - 1.0);
  pos_3 = (tmpvar_4 + (tmpvar_8 * ((max ((_Size - ((dot (tmpvar_6, tmpvar_8) * tmpvar_9) * tmpvar_10)), 0.0) * tmpvar_10) * tmpvar_11.x)));
  pos_3 = (pos_3 + (tmpvar_6 * (tmpvar_11.x * tmpvar_9)));
  pos_3 = (pos_3 + (tmpvar_7 * ((tmpvar_11.y * _Size) * tmpvar_10)));
  highp float tmpvar_12;
  tmpvar_12 = ((g_closeSnowFade.x * clamp (((tmpvar_5 * g_closeSnowFade.w) + g_closeSnowFade.z), 0.0, 1.0).x) + g_closeSnowFade.y);
  highp float tmpvar_13;
  if ((tmpvar_12 < 0.25)) {
    tmpvar_13 = tmpvar_12;
  } else {
    tmpvar_13 = 1.0;
  };
  highp vec4 tmpvar_14;
  tmpvar_14.w = 1.0;
  tmpvar_14.xyz = pos_3;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = tmpvar_14.xyz;
  highp vec4 tmpvar_16;
  tmpvar_16 = (glstate_matrix_mvp * tmpvar_15);
  highp vec4 tmpvar_17;
  tmpvar_17.x = tmpvar_16.x;
  tmpvar_17.y = tmpvar_16.y;
  tmpvar_17.z = ((tmpvar_16.z * _DepthBand.z) + (tmpvar_16.w * _DepthBand.y));
  tmpvar_17.w = tmpvar_16.w;
  highp float tmpvar_18;
  tmpvar_18 = (tmpvar_13 * _Alpha);
  tmpvar_2 = tmpvar_18;
  gl_Position = tmpvar_17;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_COLOR = tmpvar_2;
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
    vel = ((vel * _PosScale) - vec3( (g_cameraVel * _CameraMotionBlur)));
    pos = fract(((pos * g_closeSnowData.w) + g_closeSnowData.xyz));
    pos = ((pos * (g_closeSnowOffset.w * _PosScale)) + ((g_closeSnowOffset.xyz * _PosScale) + vec3( (g_cameraPos * (1.0 - _PosScale)))));
    highp vec3 camDir = normalize((pos - vec3( g_cameraPos)));
    #line 423
    highp vec3 xDir = (-vel);
    highp vec3 yDir = cross( xDir, camDir);
    highp vec3 screenXDir = cross( camDir, yDir);
    highp float xScale = max( 1.0, (_Size / length(xDir)));
    #line 427
    highp float yScale = (1.0 / length(yDir));
    highp vec2 quadOffset = ((v.texCoord.xy * 2.0) - 1.0);
    pos.xyz += (screenXDir * ((max( (_Size - ((dot( xDir, screenXDir) * xScale) * yScale)), 0.0) * yScale) * quadOffset.x));
    pos.xyz += (xDir * (quadOffset.x * xScale));
    #line 431
    pos.xyz += (yDir * ((quadOffset.y * _Size) * yScale));
    highp float alphaInterp = float( xll_saturate_vf3(((camDir * g_closeSnowFade.w) + g_closeSnowFade.z)));
    highp float alpha = ((g_closeSnowFade.x * alphaInterp) + g_closeSnowFade.y);
    alpha = (( (alpha < 0.25) ) ? ( alpha ) : ( 1.0 ));
    #line 435
    v2f o;
    o.vertex = ObjectToClipPos( vec4( pos, 1.0));
    o.texCoord = v.texCoord;
    o.alpha = (alpha * _Alpha);
    #line 439
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
#line 441
lowp vec4 frag( in v2f i ) {
    #line 443
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