Shader "Corona/Effects/Hologram Vertex Alpha" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _NoiseTex ("Noise Texture", 2D) = "white" {}
 _NoiseTexRate ("Noise Tex Rate", Range(0,4)) = 0.85
 _NoiseRate ("Noise Scroll Rate", Range(0,1)) = 0.5
 _ShimmerIntensity ("Shimmer Intensity", Range(0,0.2)) = 0.125
 _ShimmerRate ("Shimmer Rate", Range(0,400)) = 275
 _ZBias ("Z Bias", Float) = -0.0001
}
SubShader { 
 Tags { "QUEUE"="Transparent-1" }
 UsePass "Corona/Effects/HologramZFill/PASS"
 Pass {
  Tags { "QUEUE"="Transparent-1" }
  ZWrite Off
  Fog { Mode Off }
  Blend One OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp float xlv_TEXCOORD4;
varying lowp float xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp float _ShimmerRate;
uniform highp float _ShimmerIntensity;
uniform highp float _NoiseRate;
uniform highp float _NoiseTexRate;
uniform highp float _ZBias;
uniform highp vec3 _DepthBand;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _Time;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  lowp vec3 tmpvar_3;
  lowp float tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_6;
  tmpvar_6 = (glstate_matrix_mvp * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7.x = tmpvar_6.x;
  tmpvar_7.y = tmpvar_6.y;
  tmpvar_7.z = ((tmpvar_6.z * _DepthBand.z) + (tmpvar_6.w * _DepthBand.y));
  tmpvar_7.w = tmpvar_6.w;
  tmpvar_1.xyw = tmpvar_7.xyw;
  tmpvar_1.z = (tmpvar_7.z + _ZBias);
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_9;
  tmpvar_9 = normalize(((_World2Object * tmpvar_8).xyz - _glesVertex.xyz));
  tmpvar_3 = tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10.w = 0.0;
  tmpvar_10.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 0.0;
  tmpvar_11.xyz = _glesVertex.xyz;
  highp vec2 tmpvar_12;
  tmpvar_12.x = -((glstate_matrix_modelview0 * tmpvar_10).x);
  tmpvar_12.y = (_Object2World * tmpvar_11).y;
  highp vec2 tmpvar_13;
  tmpvar_13.x = 0.0;
  tmpvar_13.y = (-(fract(((_Time.y * _NoiseRate) * 0.015625))) * 64.0);
  highp vec2 tmpvar_14;
  tmpvar_14 = ((tmpvar_12 * _NoiseTexRate) + tmpvar_13);
  tmpvar_2 = tmpvar_14;
  highp float tmpvar_15;
  tmpvar_15 = (0.25 + ((((sin((_Time.y * (_ShimmerRate / 3.0))) + sin((_Time.y * (_ShimmerRate / 5.0)))) + sin((_Time.y * (_ShimmerRate / 7.0)))) * 0.333333) * _ShimmerIntensity));
  tmpvar_4 = tmpvar_15;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = normalize(_glesNormal);
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = _glesColor.w;
}



#endif
#ifdef FRAGMENT

varying lowp float xlv_TEXCOORD4;
varying lowp float xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec3 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform lowp vec4 _Color;
uniform sampler2D _NoiseTex;
void main ()
{
  lowp vec4 colour_1;
  lowp float tmpvar_2;
  tmpvar_2 = clamp ((1.0 - abs((xlv_TEXCOORD3 - dot (normalize(xlv_TEXCOORD2), xlv_TEXCOORD1)))), 0.0, 1.0);
  colour_1.w = (tmpvar_2 * ((tmpvar_2 * 0.75) + 0.25));
  colour_1.w = (colour_1.w * xlv_TEXCOORD4);
  colour_1.xyz = (((_Color.xyz * 2.0) * texture2D (_NoiseTex, xlv_TEXCOORD0).xyz) * colour_1.w);
  gl_FragData[0] = colour_1;
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
#line 396
struct v2f {
    highp vec4 pos;
    mediump vec2 noiseuv;
    lowp vec3 camDir;
    lowp vec3 normal;
    lowp float edgeCmp;
    lowp float alpha;
};
#line 389
struct appdata {
    highp vec4 vertex;
    highp vec3 normal;
    lowp vec4 color;
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
uniform highp float _ZBias;
uniform highp float _NoiseTexRate;
uniform highp float _NoiseRate;
uniform highp float _ShimmerIntensity;
#line 386
uniform highp float _ShimmerRate;
uniform sampler2D _NoiseTex;
uniform lowp vec4 _Color;
#line 406
#line 422
#line 318
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 320
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 327
highp vec3 WorldToObjectPos( in highp vec3 pos ) {
    #line 329
    pos = (_World2Object * vec4( pos, 1.0)).xyz;
    return pos;
}
#line 406
v2f vert( in appdata v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 410
    o.pos.z += _ZBias;
    o.camDir = normalize((WorldToObjectPos( _WorldSpaceCameraPos) - vec3( v.vertex)));
    o.normal = v.normal;
    highp vec2 noiseuv = vec2( (-(glstate_matrix_modelview0 * vec4( v.vertex.xyz, 0.0)).x), (_Object2World * vec4( v.vertex.xyz, 0.0)).y);
    #line 414
    highp float noiseOffset = ((-fract(((_Time.y * _NoiseRate) * 0.015625))) * 64.0);
    noiseuv = ((noiseuv * _NoiseTexRate) + vec2( 0.0, noiseOffset));
    o.noiseuv = noiseuv;
    highp float shimmer = (((sin((_Time.y * (_ShimmerRate / 3.0))) + sin((_Time.y * (_ShimmerRate / 5.0)))) + sin((_Time.y * (_ShimmerRate / 7.0)))) * 0.333333);
    #line 418
    o.edgeCmp = (0.25 + (shimmer * _ShimmerIntensity));
    o.alpha = v.color.w;
    return o;
}
out mediump vec2 xlv_TEXCOORD0;
out lowp vec3 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
out lowp float xlv_TEXCOORD3;
out lowp float xlv_TEXCOORD4;
void main() {
    v2f xl_retval;
    appdata xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.normal = vec3(gl_Normal);
    xlt_v.color = vec4(gl_Color);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec2(xl_retval.noiseuv);
    xlv_TEXCOORD1 = vec3(xl_retval.camDir);
    xlv_TEXCOORD2 = vec3(xl_retval.normal);
    xlv_TEXCOORD3 = float(xl_retval.edgeCmp);
    xlv_TEXCOORD4 = float(xl_retval.alpha);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];
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
#line 396
struct v2f {
    highp vec4 pos;
    mediump vec2 noiseuv;
    lowp vec3 camDir;
    lowp vec3 normal;
    lowp float edgeCmp;
    lowp float alpha;
};
#line 389
struct appdata {
    highp vec4 vertex;
    highp vec3 normal;
    lowp vec4 color;
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
uniform highp float _ZBias;
uniform highp float _NoiseTexRate;
uniform highp float _NoiseRate;
uniform highp float _ShimmerIntensity;
#line 386
uniform highp float _ShimmerRate;
uniform sampler2D _NoiseTex;
uniform lowp vec4 _Color;
#line 406
#line 422
#line 422
lowp vec4 frag( in v2f i ) {
    lowp float ndotl = dot( normalize(i.normal), i.camDir);
    lowp float edge = xll_saturate_f((1.0 - abs((i.edgeCmp - ndotl))));
    #line 426
    lowp vec3 noise = texture( _NoiseTex, i.noiseuv).xyz;
    lowp vec4 colour;
    colour.w = (edge * ((edge * 0.75) + 0.25));
    colour.w = (colour.w * i.alpha);
    #line 430
    colour.xyz = (((_Color.xyz * 2.0) * noise) * colour.w);
    return colour;
}
in mediump vec2 xlv_TEXCOORD0;
in lowp vec3 xlv_TEXCOORD1;
in lowp vec3 xlv_TEXCOORD2;
in lowp float xlv_TEXCOORD3;
in lowp float xlv_TEXCOORD4;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.noiseuv = vec2(xlv_TEXCOORD0);
    xlt_i.camDir = vec3(xlv_TEXCOORD1);
    xlt_i.normal = vec3(xlv_TEXCOORD2);
    xlt_i.edgeCmp = float(xlv_TEXCOORD3);
    xlt_i.alpha = float(xlv_TEXCOORD4);
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