„^Shader "Corona/PostProcess/GausBlur" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "black" {}
 _BlurWidth ("Blur Width", Vector) = (0.002,0.004,0,0)
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp vec2 xlv_TEXCOORD7;
varying lowp vec2 xlv_TEXCOORD6;
varying lowp vec2 xlv_TEXCOORD5;
varying lowp vec2 xlv_TEXCOORD4;
varying lowp vec2 xlv_TEXCOORD3;
varying lowp vec2 xlv_TEXCOORD2;
varying lowp vec2 xlv_TEXCOORD1;
varying lowp vec2 xlv_TEXCOORD0;
uniform highp vec4 _BlurWidth;
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec2 tmpvar_2;
  lowp vec2 tmpvar_3;
  lowp vec2 tmpvar_4;
  lowp vec2 tmpvar_5;
  lowp vec2 tmpvar_6;
  lowp vec2 tmpvar_7;
  lowp vec2 tmpvar_8;
  lowp vec2 tmpvar_9;
  highp vec2 tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.zw = vec2(0.0, 0.0);
  tmpvar_11.x = tmpvar_1.x;
  tmpvar_11.y = tmpvar_1.y;
  tmpvar_10 = (glstate_matrix_texture0 * tmpvar_11).xy;
  tmpvar_2 = tmpvar_10;
  highp vec2 tmpvar_12;
  tmpvar_12 = (tmpvar_2 + (_BlurWidth.xy * 2.0));
  tmpvar_3 = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = (tmpvar_2 + (_BlurWidth.zw * 3.0));
  tmpvar_4 = tmpvar_13;
  highp vec2 tmpvar_14;
  tmpvar_14 = (tmpvar_2 + (_BlurWidth.xy * 4.0));
  tmpvar_7 = tmpvar_14;
  highp vec2 tmpvar_15;
  tmpvar_15 = (tmpvar_2 + _BlurWidth.zw);
  tmpvar_8 = tmpvar_15;
  highp vec2 tmpvar_16;
  tmpvar_16 = (tmpvar_2 - (_BlurWidth.xy * 2.0));
  tmpvar_9 = tmpvar_16;
  highp vec2 tmpvar_17;
  tmpvar_17 = (tmpvar_2 - (_BlurWidth.xy * 4.0));
  tmpvar_5 = tmpvar_17;
  highp vec2 tmpvar_18;
  tmpvar_18 = (tmpvar_2 - _BlurWidth.zw);
  tmpvar_6 = tmpvar_18;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
  xlv_TEXCOORD4 = tmpvar_6;
  xlv_TEXCOORD5 = tmpvar_7;
  xlv_TEXCOORD6 = tmpvar_8;
  xlv_TEXCOORD7 = tmpvar_9;
}



#endif
#ifdef FRAGMENT

varying lowp vec2 xlv_TEXCOORD7;
varying lowp vec2 xlv_TEXCOORD6;
varying lowp vec2 xlv_TEXCOORD5;
varying lowp vec2 xlv_TEXCOORD4;
varying lowp vec2 xlv_TEXCOORD3;
varying lowp vec2 xlv_TEXCOORD2;
varying lowp vec2 xlv_TEXCOORD1;
varying lowp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
void main ()
{
  highp vec4 tmpvar_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = ((((((((texture2D (_MainTex, xlv_TEXCOORD0) * 0.19) + (texture2D (_MainTex, xlv_TEXCOORD1) * 0.12)) + (texture2D (_MainTex, xlv_TEXCOORD2) * 0.09)) + (texture2D (_MainTex, xlv_TEXCOORD3) * 0.05)) + (texture2D (_MainTex, xlv_TEXCOORD4) * 0.16)) + (texture2D (_MainTex, xlv_TEXCOORD5) * 0.05)) + (texture2D (_MainTex, xlv_TEXCOORD6) * 0.16)) + (texture2D (_MainTex, xlv_TEXCOORD7) * 0.12));
  tmpvar_1 = tmpvar_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX

#define gl_Vertex _glesVertex
in vec4 _glesVertex;
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
#line 317
struct v2f {
    highp vec4 pos;
    lowp vec2 uv;
    lowp vec2 uv1;
    lowp vec2 uv2;
    lowp vec2 uv3;
    lowp vec2 uv4;
    lowp vec2 uv5;
    lowp vec2 uv6;
    lowp vec2 uv7;
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
uniform sampler2D _MainTex;
uniform highp vec4 _BlurWidth;
#line 330
#line 351
#line 193
highp vec2 MultiplyUV( in highp mat4 mat, in highp vec2 inUV ) {
    highp vec4 temp = vec4( inUV.x, inUV.y, 0.0, 0.0);
    temp = (mat * temp);
    #line 197
    return temp.xy;
}
#line 330
v2f vert( in appdata_img v ) {
    v2f o;
    o.pos = (glstate_matrix_mvp * v.vertex);
    #line 334
    o.uv = MultiplyUV( glstate_matrix_texture0, v.texcoord);
    o.uv1 = o.uv;
    o.uv2 = o.uv;
    o.uv3 = o.uv;
    #line 338
    o.uv4 = o.uv;
    o.uv5 = o.uv;
    o.uv6 = o.uv;
    o.uv7 = o.uv;
    #line 342
    o.uv1 += (_BlurWidth.xy * 2.0);
    o.uv2 += (_BlurWidth.zw * 3.0);
    o.uv5 += (_BlurWidth.xy * 4.0);
    o.uv6 += _BlurWidth.zw;
    #line 346
    o.uv7 -= (_BlurWidth.xy * 2.0);
    o.uv3 -= (_BlurWidth.xy * 4.0);
    o.uv4 -= _BlurWidth.zw;
    return o;
}
out lowp vec2 xlv_TEXCOORD0;
out lowp vec2 xlv_TEXCOORD1;
out lowp vec2 xlv_TEXCOORD2;
out lowp vec2 xlv_TEXCOORD3;
out lowp vec2 xlv_TEXCOORD4;
out lowp vec2 xlv_TEXCOORD5;
out lowp vec2 xlv_TEXCOORD6;
out lowp vec2 xlv_TEXCOORD7;
void main() {
    v2f xl_retval;
    appdata_img xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.texcoord = vec2(gl_MultiTexCoord0);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec2(xl_retval.uv);
    xlv_TEXCOORD1 = vec2(xl_retval.uv1);
    xlv_TEXCOORD2 = vec2(xl_retval.uv2);
    xlv_TEXCOORD3 = vec2(xl_retval.uv3);
    xlv_TEXCOORD4 = vec2(xl_retval.uv4);
    xlv_TEXCOORD5 = vec2(xl_retval.uv5);
    xlv_TEXCOORD6 = vec2(xl_retval.uv6);
    xlv_TEXCOORD7 = vec2(xl_retval.uv7);
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
#line 317
struct v2f {
    highp vec4 pos;
    lowp vec2 uv;
    lowp vec2 uv1;
    lowp vec2 uv2;
    lowp vec2 uv3;
    lowp vec2 uv4;
    lowp vec2 uv5;
    lowp vec2 uv6;
    lowp vec2 uv7;
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
uniform sampler2D _MainTex;
uniform highp vec4 _BlurWidth;
#line 330
#line 351
#line 351
highp vec4 frag( in v2f i ) {
    lowp vec4 col = (texture( _MainTex, i.uv) * 0.19);
    col += (texture( _MainTex, i.uv1) * 0.12);
    #line 355
    col += (texture( _MainTex, i.uv2) * 0.09);
    col += (texture( _MainTex, i.uv3) * 0.05);
    col += (texture( _MainTex, i.uv4) * 0.16);
    col += (texture( _MainTex, i.uv5) * 0.05);
    #line 359
    col += (texture( _MainTex, i.uv6) * 0.16);
    col += (texture( _MainTex, i.uv7) * 0.12);
    return col;
}
in lowp vec2 xlv_TEXCOORD0;
in lowp vec2 xlv_TEXCOORD1;
in lowp vec2 xlv_TEXCOORD2;
in lowp vec2 xlv_TEXCOORD3;
in lowp vec2 xlv_TEXCOORD4;
in lowp vec2 xlv_TEXCOORD5;
in lowp vec2 xlv_TEXCOORD6;
in lowp vec2 xlv_TEXCOORD7;
void main() {
    highp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv = vec2(xlv_TEXCOORD0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD1);
    xlt_i.uv2 = vec2(xlv_TEXCOORD2);
    xlt_i.uv3 = vec2(xlv_TEXCOORD3);
    xlt_i.uv4 = vec2(xlv_TEXCOORD4);
    xlt_i.uv5 = vec2(xlv_TEXCOORD5);
    xlt_i.uv6 = vec2(xlv_TEXCOORD6);
    xlt_i.uv7 = vec2(xlv_TEXCOORD7);
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