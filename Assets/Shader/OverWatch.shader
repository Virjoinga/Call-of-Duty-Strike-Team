¯OShader "Corona/OverWatch" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _GrainTex ("Base (RGB)", 2D) = "gray" {}
}
SubShader { 
 Pass {
  ZTest False
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp vec2 xlv_TEXCOORD1;
varying lowp vec2 xlv_TEXCOORD0;
uniform mediump vec4 _GrainOffsetScale;
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
  highp vec2 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.x = tmpvar_1.x;
  tmpvar_5.y = tmpvar_1.y;
  tmpvar_4 = (glstate_matrix_texture0 * tmpvar_5).xy;
  tmpvar_2 = tmpvar_4;
  mediump vec2 tmpvar_6;
  tmpvar_6 = ((_glesMultiTexCoord0.xy * _GrainOffsetScale.zw) + _GrainOffsetScale.xy);
  tmpvar_3 = tmpvar_6;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying lowp vec2 xlv_TEXCOORD1;
varying lowp vec2 xlv_TEXCOORD0;
uniform lowp float _Fade;
uniform lowp float _Saturation;
uniform lowp float _IntensitySquare;
uniform lowp float _Intensity;
uniform sampler2D _GrainTex;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 col_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  col_1.w = tmpvar_2.w;
  col_1.xyz = vec3(mix (dot (tmpvar_2.xyz, vec3(0.3, 0.59, 0.11)), tmpvar_2.x, _Saturation));
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_GrainTex, xlv_TEXCOORD1);
  col_1.xyz = (col_1.xyz + (tmpvar_3.xyz * _IntensitySquare));
  lowp vec4 tmpvar_4;
  tmpvar_4 = (col_1 + (tmpvar_3.w * _Intensity));
  col_1.w = tmpvar_4.w;
  lowp vec2 tmpvar_5;
  tmpvar_5 = (vec2(0.5, 0.5) - xlv_TEXCOORD0);
  lowp float tmpvar_6;
  tmpvar_6 = dot (tmpvar_5, tmpvar_5);
  col_1.xyz = (tmpvar_4.xyz - ((tmpvar_6 * tmpvar_6) * _Fade));
  gl_FragData[0] = col_1;
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
#line 315
struct v2f {
    highp vec4 pos;
    lowp vec2 uv;
    lowp vec2 uvg;
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
#line 322
uniform sampler2D _MainTex;
uniform sampler2D _GrainTex;
uniform mediump vec4 _GrainOffsetScale;
uniform lowp float _Intensity;
#line 326
uniform lowp float _IntensitySquare;
uniform lowp float _Saturation;
uniform lowp float _Fade;
#line 193
highp vec2 MultiplyUV( in highp mat4 mat, in highp vec2 inUV ) {
    highp vec4 temp = vec4( inUV.x, inUV.y, 0.0, 0.0);
    temp = (mat * temp);
    #line 197
    return temp.xy;
}
#line 329
v2f vert( in appdata_img v ) {
    #line 331
    v2f o;
    o.pos = (glstate_matrix_mvp * v.vertex);
    o.uv = MultiplyUV( glstate_matrix_texture0, v.texcoord);
    o.uvg = ((v.texcoord.xy * _GrainOffsetScale.zw) + _GrainOffsetScale.xy);
    #line 335
    return o;
}
out lowp vec2 xlv_TEXCOORD0;
out lowp vec2 xlv_TEXCOORD1;
void main() {
    v2f xl_retval;
    appdata_img xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.texcoord = vec2(gl_MultiTexCoord0);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec2(xl_retval.uv);
    xlv_TEXCOORD1 = vec2(xl_retval.uvg);
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
#line 315
struct v2f {
    highp vec4 pos;
    lowp vec2 uv;
    lowp vec2 uvg;
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
#line 322
uniform sampler2D _MainTex;
uniform sampler2D _GrainTex;
uniform mediump vec4 _GrainOffsetScale;
uniform lowp float _Intensity;
#line 326
uniform lowp float _IntensitySquare;
uniform lowp float _Saturation;
uniform lowp float _Fade;
#line 337
lowp vec4 frag( in v2f i ) {
    #line 339
    lowp vec4 col = texture( _MainTex, i.uv);
    lowp vec3 luminanceWeights = vec3( 0.3, 0.59, 0.11);
    lowp float luminance = dot( col.xyz, luminanceWeights);
    col.xyz = vec3( mix( luminance, float( col.xyz), _Saturation));
    #line 343
    lowp vec4 grain = texture( _GrainTex, i.uvg);
    col.xyz += (grain.xyz * _IntensitySquare);
    col += (grain.w * _Intensity);
    lowp vec2 v2 = (vec2( 0.5, 0.5) - i.uv);
    #line 347
    lowp float d = dot( v2, v2);
    col.xyz -= ((d * d) * _Fade);
    return col;
}
in lowp vec2 xlv_TEXCOORD0;
in lowp vec2 xlv_TEXCOORD1;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv = vec2(xlv_TEXCOORD0);
    xlt_i.uvg = vec2(xlv_TEXCOORD1);
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