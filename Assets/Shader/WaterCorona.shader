žVShader "Corona/Water/Corona" {
Properties {
 _ColorControl ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
 _BumpMap ("Waves Normalmap ", 2D) = "" {}
 _WaveScale ("Wave scale", Range(0.02,0.15)) = 0.063
 WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
}
SubShader { 
 Tags { "QUEUE"="Geometry+997" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Geometry+997" "RenderType"="Opaque" }
  Fog { Mode Off }
  Blend One SrcAlpha
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform highp vec4 _WaveOffset;
uniform highp vec4 _WaveScale4;
uniform highp vec3 _DepthBand;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _WorldSpaceCameraPos;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec3 tmpvar_1;
  highp vec4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_3;
  tmpvar_3 = (glstate_matrix_mvp * tmpvar_2);
  highp vec4 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = tmpvar_3.y;
  tmpvar_4.z = ((tmpvar_3.z * _DepthBand.z) + (tmpvar_3.w * _DepthBand.y));
  tmpvar_4.w = tmpvar_3.w;
  highp vec3 tmpvar_5;
  tmpvar_5 = (_Object2World * _glesVertex).xyz;
  highp vec3 tmpvar_6;
  tmpvar_6 = (_WorldSpaceCameraPos - tmpvar_5).xzy;
  tmpvar_1 = tmpvar_6;
  gl_Position = tmpvar_4;
  xlv_TEXCOORD0 = ((tmpvar_5.xz * _WaveScale4.xy) + _WaveOffset.xy);
  xlv_TEXCOORD1 = ((tmpvar_5.xz * _WaveScale4.zw) + _WaveOffset.zw);
  xlv_TEXCOORD2 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _ColorControl;
uniform sampler2D _BumpMap;
void main ()
{
  lowp float fresnel_1;
  mediump vec3 viewDir_2;
  lowp vec3 tmpvar_3;
  tmpvar_3 = normalize(xlv_TEXCOORD2);
  viewDir_2 = tmpvar_3;
  lowp vec3 tmpvar_4;
  tmpvar_4 = ((texture2D (_BumpMap, xlv_TEXCOORD0).xyz + texture2D (_BumpMap, xlv_TEXCOORD1).xyz) - 1.0);
  mediump float tmpvar_5;
  tmpvar_5 = dot (viewDir_2, tmpvar_4);
  fresnel_1 = tmpvar_5;
  gl_FragData[0] = texture2D (_ColorControl, vec2(fresnel_1));
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX

#define gl_Vertex _glesVertex
in vec4 _glesVertex;
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
#line 390
struct v2f {
    highp vec4 pos;
    highp vec2 bumpuv0;
    highp vec2 bumpuv1;
    lowp vec3 viewDir;
};
#line 384
struct appdata {
    highp vec4 vertex;
    highp vec3 normal;
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
uniform highp vec4 _WaveScale4;
uniform highp vec4 _WaveOffset;
#line 398
uniform sampler2D _BumpMap;
#line 410
uniform sampler2D _ColorControl;
#line 318
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 320
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 398
v2f vert( in appdata v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 402
    highp vec3 pos = (_Object2World * v.vertex).xyz;
    highp vec3 camPos = _WorldSpaceCameraPos;
    o.bumpuv0 = ((pos.xz * _WaveScale4.xy) + _WaveOffset.xy);
    o.bumpuv1 = ((pos.xz * _WaveScale4.zw) + _WaveOffset.zw);
    #line 406
    o.viewDir = (camPos - pos).xzy;
    return o;
}
out highp vec2 xlv_TEXCOORD0;
out highp vec2 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
void main() {
    v2f xl_retval;
    appdata xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.normal = vec3(gl_Normal);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec2(xl_retval.bumpuv0);
    xlv_TEXCOORD1 = vec2(xl_retval.bumpuv1);
    xlv_TEXCOORD2 = vec3(xl_retval.viewDir);
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
#line 390
struct v2f {
    highp vec4 pos;
    highp vec2 bumpuv0;
    highp vec2 bumpuv1;
    lowp vec3 viewDir;
};
#line 384
struct appdata {
    highp vec4 vertex;
    highp vec3 normal;
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
uniform highp vec4 _WaveScale4;
uniform highp vec4 _WaveOffset;
#line 398
uniform sampler2D _BumpMap;
#line 410
uniform sampler2D _ColorControl;
#line 341
lowp vec3 UnpackNormalHQ( in lowp vec4 packednormal ) {
    #line 343
    lowp vec3 normal;
    normal.xy = ((packednormal.wy * 2.0) - 1.0);
    normal.z = sqrt(((1.0 - (normal.x * normal.x)) - (normal.y * normal.y)));
    return normal;
}
#line 411
lowp vec4 frag( in v2f i ) {
    mediump vec3 viewDir = normalize(i.viewDir);
    #line 414
    lowp vec3 bump1 = UnpackNormalHQ( texture( _BumpMap, i.bumpuv0)).xyz;
    lowp vec3 bump2 = UnpackNormalHQ( texture( _BumpMap, i.bumpuv1)).xyz;
    lowp vec3 bump = ((bump1 + bump2) * 0.5);
    lowp float fresnel = dot( viewDir, bump);
    #line 418
    lowp vec4 col = texture( _ColorControl, vec2( fresnel, fresnel));
    return col;
}
in highp vec2 xlv_TEXCOORD0;
in highp vec2 xlv_TEXCOORD1;
in lowp vec3 xlv_TEXCOORD2;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.bumpuv0 = vec2(xlv_TEXCOORD0);
    xlt_i.bumpuv1 = vec2(xlv_TEXCOORD1);
    xlt_i.viewDir = vec3(xlv_TEXCOORD2);
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