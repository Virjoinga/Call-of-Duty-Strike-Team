ÏRShader "Corona/PostProcess/Copy_Bloom_CC_Trans" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _BloomTex ("bloom tex", 2D) = "black" {}
 _RampTex ("ramp tex", 2D) = "grayscaleRamp" {}
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

varying mediump vec2 xlv_TEXCOORD0;
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  mediump vec2 tmpvar_2;
  highp vec2 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.zw = vec2(0.0, 0.0);
  tmpvar_4.x = tmpvar_1.x;
  tmpvar_4.y = tmpvar_1.y;
  tmpvar_3 = (glstate_matrix_texture0 * tmpvar_4).xy;
  tmpvar_2 = tmpvar_3;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying lowp vec2 xlv_TEXCOORD0;
uniform highp float _Bloomification;
uniform highp float _TimeSin;
uniform highp float _YonTrans;
uniform sampler2D _TranRampTex;
uniform sampler2D _TranTex;
uniform sampler2D _RampTex;
uniform sampler2D _BloomTex;
uniform sampler2D _MainTex;
void main ()
{
  highp vec4 tmpvar_1;
  lowp float v_2;
  lowp vec4 col_3;
  highp float tmpvar_4;
  tmpvar_4 = (_YonTrans + (0.033 * xlv_TEXCOORD0.y));
  v_2 = tmpvar_4;
  lowp vec2 tmpvar_5;
  tmpvar_5.x = xlv_TEXCOORD0.x;
  tmpvar_5.y = v_2;
  lowp float tmpvar_6;
  tmpvar_6 = (0.2 * (texture2D (_TranRampTex, tmpvar_5).x - 0.5));
  lowp vec2 tmpvar_7;
  tmpvar_7.x = (xlv_TEXCOORD0.x + tmpvar_6);
  tmpvar_7.y = xlv_TEXCOORD0.y;
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, tmpvar_7);
  lowp vec2 tmpvar_9;
  tmpvar_9.x = (xlv_TEXCOORD0.x + tmpvar_6);
  tmpvar_9.y = v_2;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_TranTex, tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = ((tmpvar_8 * 0.5) + (0.5 * (((1.0 - _TimeSin) * tmpvar_8) + (_TimeSin * tmpvar_10))));
  col_3 = tmpvar_11;
  col_3.x = (texture2D (_RampTex, col_3.xx).x + 1e-05);
  col_3.y = (texture2D (_RampTex, col_3.yy).y + 2e-05);
  col_3.z = (texture2D (_RampTex, col_3.zz).z + 3e-05);
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2D (_BloomTex, xlv_TEXCOORD0);
  highp vec3 tmpvar_13;
  tmpvar_13 = (col_3.xyz + (tmpvar_12.w * _Bloomification));
  col_3.xyz = tmpvar_13;
  tmpvar_1 = col_3;
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
#line 323
struct v2f {
    highp vec4 pos;
    lowp vec2 uv;
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
uniform sampler2D _BloomTex;
uniform sampler2D _RampTex;
uniform sampler2D _TranTex;
#line 319
uniform sampler2D _TranRampTex;
uniform highp float _YonTrans;
uniform highp float _TimeSin;
uniform highp float _Bloomification;
#line 329
#line 193
highp vec2 MultiplyUV( in highp mat4 mat, in highp vec2 inUV ) {
    highp vec4 temp = vec4( inUV.x, inUV.y, 0.0, 0.0);
    temp = (mat * temp);
    #line 197
    return temp.xy;
}
#line 199
v2f_img vert_img( in appdata_img v ) {
    #line 201
    v2f_img o;
    o.pos = (glstate_matrix_mvp * v.vertex);
    o.uv = MultiplyUV( glstate_matrix_texture0, v.texcoord);
    return o;
}
out mediump vec2 xlv_TEXCOORD0;
void main() {
    v2f_img xl_retval;
    appdata_img xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.texcoord = vec2(gl_MultiTexCoord0);
    xl_retval = vert_img( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec2(xl_retval.uv);
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
#line 323
struct v2f {
    highp vec4 pos;
    lowp vec2 uv;
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
uniform sampler2D _BloomTex;
uniform sampler2D _RampTex;
uniform sampler2D _TranTex;
#line 319
uniform sampler2D _TranRampTex;
uniform highp float _YonTrans;
uniform highp float _TimeSin;
uniform highp float _Bloomification;
#line 329
#line 336
highp vec4 frag( in v2f i ) {
    #line 338
    lowp vec4 transData = vec4( 0.0);
    lowp vec4 col;
    lowp float v = (_YonTrans + (0.033 * i.uv.y));
    transData = texture( _TranRampTex, vec2( i.uv.x, v));
    #line 342
    lowp float u = ((transData.x - 0.5) * 2.0);
    u *= 0.1;
    lowp vec4 colScreen = texture( _MainTex, vec2( (i.uv.x + u), i.uv.y));
    transData = texture( _TranTex, vec2( (i.uv.x + u), v));
    #line 346
    col = ((colScreen * 0.5) + (0.5 * (((1.0 - _TimeSin) * colScreen) + (_TimeSin * transData))));
    col.x = (texture( _RampTex, col.xx).x + 1e-05);
    col.y = (texture( _RampTex, col.yy).y + 2e-05);
    col.z = (texture( _RampTex, col.zz).z + 3e-05);
    #line 350
    lowp vec4 bloom = texture( _BloomTex, i.uv);
    col.xyz += (bloom.w * _Bloomification);
    return col;
}
in lowp vec2 xlv_TEXCOORD0;
void main() {
    highp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv = vec2(xlv_TEXCOORD0);
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