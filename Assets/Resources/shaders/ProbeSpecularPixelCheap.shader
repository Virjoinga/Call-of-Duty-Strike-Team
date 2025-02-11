‹lShader "Hidden/ProbeSpecularPixelCheap" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 cAr ("Light Probe Ar", Color) = (1,1,1,1)
 cAg ("Light Probe Ag", Color) = (1,1,1,1)
 cAb ("Light Probe Ab", Color) = (1,1,1,1)
 cBr ("Light Probe Br", Color) = (1,1,1,1)
 cBg ("Light Probe Bg", Color) = (1,1,1,1)
 cBb ("Light Probe Bb", Color) = (1,1,1,1)
 cC ("Light Probe C", Color) = (1,1,1,1)
 _SpecDir ("Specular Dir", Vector) = (-0.66,0.333,0.66,1)
}
SubShader { 
 LOD 200
 Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
  Fog {
   Mode Linear
  }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying lowp vec2 xlv_TEXCOORD0;
uniform highp vec4 cC;
uniform highp vec4 cBb;
uniform highp vec4 cBg;
uniform highp vec4 cBr;
uniform highp vec4 cAb;
uniform highp vec4 cAg;
uniform highp vec4 cAr;
uniform highp vec3 _DepthBand;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _WorldSpaceCameraPos;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec2 tmpvar_1;
  lowp vec4 tmpvar_2;
  lowp vec3 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_5;
  tmpvar_5 = (glstate_matrix_mvp * tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6.x = tmpvar_5.x;
  tmpvar_6.y = tmpvar_5.y;
  tmpvar_6.z = ((tmpvar_5.z * _DepthBand.z) + (tmpvar_5.w * _DepthBand.y));
  tmpvar_6.w = tmpvar_5.w;
  highp vec2 tmpvar_7;
  tmpvar_7 = _glesMultiTexCoord0.xy;
  tmpvar_1 = tmpvar_7;
  mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * normalize(_glesNormal));
  highp vec4 tmpvar_10;
  tmpvar_10.w = 1.0;
  tmpvar_10.xyz = tmpvar_9;
  mediump vec3 tmpvar_11;
  mediump vec4 normal_12;
  normal_12 = tmpvar_10;
  highp float vC_13;
  mediump vec3 x3_14;
  mediump vec3 x2_15;
  mediump vec3 x1_16;
  highp float tmpvar_17;
  tmpvar_17 = dot (cAr, normal_12);
  x1_16.x = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = dot (cAg, normal_12);
  x1_16.y = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = dot (cAb, normal_12);
  x1_16.z = tmpvar_19;
  mediump vec4 tmpvar_20;
  tmpvar_20 = (normal_12.xyzz * normal_12.yzzx);
  highp float tmpvar_21;
  tmpvar_21 = dot (cBr, tmpvar_20);
  x2_15.x = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = dot (cBg, tmpvar_20);
  x2_15.y = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (cBb, tmpvar_20);
  x2_15.z = tmpvar_23;
  mediump float tmpvar_24;
  tmpvar_24 = ((normal_12.x * normal_12.x) - (normal_12.y * normal_12.y));
  vC_13 = tmpvar_24;
  highp vec3 tmpvar_25;
  tmpvar_25 = (cC.xyz * vC_13);
  x3_14 = tmpvar_25;
  tmpvar_11 = ((x1_16 + x2_15) + x3_14);
  highp vec3 tmpvar_26;
  tmpvar_26 = (tmpvar_11 + (2.0 * glstate_lightmodel_ambient).xyz);
  tmpvar_2.xyz = tmpvar_26;
  tmpvar_2.w = ((tmpvar_2.x + tmpvar_2.y) + tmpvar_2.z);
  highp vec3 tmpvar_27;
  tmpvar_27 = ((_Object2World * _glesVertex).xyz - _WorldSpaceCameraPos);
  highp vec3 tmpvar_28;
  tmpvar_28 = (tmpvar_27 - (tmpvar_9 * (2.0 * dot (tmpvar_27, tmpvar_9))));
  tmpvar_3 = tmpvar_28;
  gl_Position = tmpvar_6;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying lowp vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp vec3 _SpecDir;
void main ()
{
  lowp vec4 col_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  col_1.xyz = (tmpvar_2.xyz * xlv_TEXCOORD1.xyz);
  col_1.w = (tmpvar_2.w * clamp ((xlv_TEXCOORD1.w - 1.0), 0.0, 1.0));
  col_1.xyz = (col_1.xyz + (pow (max (dot (normalize(xlv_TEXCOORD2), _SpecDir), 0.0), 10.0) * col_1.w));
  gl_FragData[0] = col_1;
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
struct Input {
    highp vec4 worldPosition;
    lowp vec2 uv_MainTex;
    lowp vec4 probeColor;
    lowp vec3 worldSpec;
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
uniform sampler2D _MainTex;
uniform highp vec4 cAr;
uniform highp vec4 cAg;
uniform highp vec4 cAb;
#line 386
uniform highp vec4 cBr;
uniform highp vec4 cBg;
uniform highp vec4 cBb;
uniform highp vec4 cC;
#line 398
#line 427
#line 318
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 320
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 398
mediump vec3 ShadeSH99( in mediump vec4 normal ) {
    mediump vec3 x1;
    mediump vec3 x2;
    mediump vec3 x3;
    x1.x = dot( cAr, normal);
    #line 402
    x1.y = dot( cAg, normal);
    x1.z = dot( cAb, normal);
    mediump vec4 vB = (normal.xyzz * normal.yzzx);
    x2.x = dot( cBr, vB);
    #line 406
    x2.y = dot( cBg, vB);
    x2.z = dot( cBb, vB);
    highp float vC = ((normal.x * normal.x) - (normal.y * normal.y));
    x3 = (cC.xyz * vC);
    #line 410
    return ((x1 + x2) + x3);
}
#line 412
Input vert( in appdata_base v ) {
    #line 414
    Input o;
    o.worldPosition = ObjectToClipPos( v.vertex);
    highp vec3 worldPos = vec3( (_Object2World * v.vertex));
    #line 418
    o.uv_MainTex = v.texcoord.xy;
    highp mat3 rotonly = mat3( _Object2World);
    highp vec3 worldNormal = (rotonly * v.normal);
    o.probeColor.xyz = (ShadeSH99( vec4( worldNormal.xyz, 1.0)) + vec3( (2.0 * glstate_lightmodel_ambient)));
    #line 422
    o.probeColor.w = ((o.probeColor.x + o.probeColor.y) + o.probeColor.z);
    highp vec3 eyevec = (worldPos - _WorldSpaceCameraPos);
    o.worldSpec = (eyevec - (worldNormal * (2.0 * dot( eyevec, worldNormal))));
    return o;
}
out lowp vec2 xlv_TEXCOORD0;
out lowp vec4 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
void main() {
    Input xl_retval;
    appdata_base xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.normal = vec3(gl_Normal);
    xlt_v.texcoord = vec4(gl_MultiTexCoord0);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.worldPosition);
    xlv_TEXCOORD0 = vec2(xl_retval.uv_MainTex);
    xlv_TEXCOORD1 = vec4(xl_retval.probeColor);
    xlv_TEXCOORD2 = vec3(xl_retval.worldSpec);
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
struct Input {
    highp vec4 worldPosition;
    lowp vec2 uv_MainTex;
    lowp vec4 probeColor;
    lowp vec3 worldSpec;
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
uniform sampler2D _MainTex;
uniform highp vec4 cAr;
uniform highp vec4 cAg;
uniform highp vec4 cAb;
#line 386
uniform highp vec4 cBr;
uniform highp vec4 cBg;
uniform highp vec4 cBb;
uniform highp vec4 cC;
#line 398
#line 427
#line 427
lowp vec4 frag( in Input IN ) {
    lowp vec4 col = texture( _MainTex, IN.uv_MainTex.xy);
    col.xyz *= vec3( IN.probeColor);
    #line 431
    lowp float lightProbeSpecMod = clamp( (IN.probeColor.w - 1.0), 0.0, 1.0);
    col.w *= lightProbeSpecMod;
    lowp float s = dot( normalize(IN.worldSpec), _SpecDir);
    col.xyz += (pow( max( s, 0.0), 10.0) * col.w);
    #line 435
    return col;
}
in lowp vec2 xlv_TEXCOORD0;
in lowp vec4 xlv_TEXCOORD1;
in lowp vec3 xlv_TEXCOORD2;
void main() {
    lowp vec4 xl_retval;
    Input xlt_IN;
    xlt_IN.worldPosition = vec4(0.0);
    xlt_IN.uv_MainTex = vec2(xlv_TEXCOORD0);
    xlt_IN.probeColor = vec4(xlv_TEXCOORD1);
    xlt_IN.worldSpec = vec3(xlv_TEXCOORD2);
    xl_retval = frag( xlt_IN);
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
Fallback "Diffuse"
}