Shader "Hidden/ProbeNormalmapSpecmap NoFog LowPFading" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
 _BumpMap ("Normalmap", 2D) = "bump" {}
 _SpecPower ("Specular Power", Range(0,50)) = 10
 _Opacity ("Opacity", Range(0,1)) = 1
}
SubShader { 
 Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp vec4 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp vec4 _FogRange;
uniform highp vec3 _FogParams;
uniform highp vec3 _AmbientLight;
uniform highp vec4 cC;
uniform highp vec4 cBb;
uniform highp vec4 cBg;
uniform highp vec4 cBr;
uniform highp vec4 cAb;
uniform highp vec4 cAg;
uniform highp vec4 cAr;
uniform highp vec3 _DepthBand;
uniform lowp vec3 _SpecDir;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _WorldSpaceCameraPos;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  vec3 tmpvar_2;
  tmpvar_2 = normalize(_glesNormal);
  mediump vec2 tmpvar_3;
  lowp vec4 tmpvar_4;
  lowp vec3 tmpvar_5;
  lowp vec3 tmpvar_6;
  lowp vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * tmpvar_8);
  highp vec4 tmpvar_10;
  tmpvar_10.x = tmpvar_9.x;
  tmpvar_10.y = tmpvar_9.y;
  tmpvar_10.z = ((tmpvar_9.z * _DepthBand.z) + (tmpvar_9.w * _DepthBand.y));
  tmpvar_10.w = tmpvar_9.w;
  highp vec2 tmpvar_11;
  tmpvar_11 = _glesMultiTexCoord0.xy;
  tmpvar_3 = tmpvar_11;
  highp vec3 tmpvar_12;
  highp vec3 dir_13;
  dir_13 = _SpecDir;
  highp vec4 tmpvar_14;
  tmpvar_14.w = 0.0;
  tmpvar_14.xyz = dir_13;
  tmpvar_12 = (_World2Object * tmpvar_14).xyz;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_16;
  tmpvar_16 = ((_World2Object * tmpvar_15).xyz - _glesVertex.xyz);
  highp vec4 tmpvar_17;
  tmpvar_17.w = 0.0;
  tmpvar_17.xyz = tmpvar_2;
  highp vec3 tmpvar_18;
  tmpvar_18 = normalize((_Object2World * tmpvar_17).xyz);
  highp vec3 tmpvar_19;
  tmpvar_19 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp vec3 tmpvar_20;
  tmpvar_20.x = dot (tmpvar_16, tmpvar_1.xyz);
  tmpvar_20.y = dot (tmpvar_16, tmpvar_19);
  tmpvar_20.z = dot (tmpvar_16, tmpvar_2);
  highp vec3 tmpvar_21;
  tmpvar_21 = normalize(tmpvar_20);
  tmpvar_5 = tmpvar_21;
  highp vec3 tmpvar_22;
  tmpvar_22.x = dot (tmpvar_12, tmpvar_1.xyz);
  tmpvar_22.y = dot (tmpvar_12, tmpvar_19);
  tmpvar_22.z = dot (tmpvar_12, tmpvar_2);
  tmpvar_6 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = max (0.0, dot (tmpvar_18, _SpecDir));
  tmpvar_4.w = tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_18;
  highp vec3 tmpvar_25;
  highp vec3 x2_26;
  highp vec3 x1_27;
  highp vec4 tmpvar_28;
  tmpvar_28.xyz = vec3(0.0, 0.0, 0.0);
  tmpvar_28.w = _AmbientLight.x;
  x1_27.x = dot ((cAr + tmpvar_28), tmpvar_24);
  highp vec4 tmpvar_29;
  tmpvar_29.xyz = vec3(0.0, 0.0, 0.0);
  tmpvar_29.w = _AmbientLight.y;
  x1_27.y = dot ((cAg + tmpvar_29), tmpvar_24);
  highp vec4 tmpvar_30;
  tmpvar_30.xyz = vec3(0.0, 0.0, 0.0);
  tmpvar_30.w = _AmbientLight.z;
  x1_27.z = dot ((cAb + tmpvar_30), tmpvar_24);
  highp vec4 tmpvar_31;
  tmpvar_31 = (tmpvar_18.xyzz * tmpvar_18.yzzx);
  x2_26.x = dot (cBr, tmpvar_31);
  x2_26.y = dot (cBg, tmpvar_31);
  x2_26.z = dot (cBb, tmpvar_31);
  tmpvar_25 = ((x1_27 + x2_26) + (cC.xyz * ((tmpvar_18.x * tmpvar_18.x) - (tmpvar_18.y * tmpvar_18.y))));
  tmpvar_4.xyz = tmpvar_25;
  highp float tmpvar_32;
  tmpvar_32 = clamp (((tmpvar_10.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
  highp vec4 tmpvar_33;
  tmpvar_33.xyz = (_FogParams - (_FogParams * tmpvar_32));
  tmpvar_33.w = tmpvar_32;
  tmpvar_7 = tmpvar_33;
  gl_Position = tmpvar_10;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD5 = tmpvar_7;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform lowp float _Opacity;
uniform sampler2D _SpecMap;
uniform sampler2D _BumpMap;
uniform sampler2D _MainTex;
uniform mediump float _SpecPower;
void main ()
{
  lowp float tmpvar_1;
  mediump float spec_2;
  lowp float tmpvar_3;
  tmpvar_3 = max (0.0, dot (normalize((normalize(xlv_TEXCOORD3) + normalize(xlv_TEXCOORD2))), normalize((texture2D (_BumpMap, xlv_TEXCOORD0).xyz - 0.5))));
  spec_2 = tmpvar_3;
  mediump float tmpvar_4;
  tmpvar_4 = pow (spec_2, _SpecPower);
  spec_2 = tmpvar_4;
  tmpvar_1 = tmpvar_4;
  lowp vec4 tmpvar_5;
  tmpvar_5.xyz = ((((texture2D (_MainTex, xlv_TEXCOORD0).xyz + (vec3(tmpvar_1) * texture2D (_SpecMap, xlv_TEXCOORD0).xyz)) * xlv_TEXCOORD1.xyz) * xlv_TEXCOORD5.w) + xlv_TEXCOORD5.xyz);
  tmpvar_5.w = _Opacity;
  gl_FragData[0] = tmpvar_5;
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
#define gl_MultiTexCoord0 _glesMultiTexCoord0
in vec4 _glesMultiTexCoord0;
#define gl_MultiTexCoord1 _glesMultiTexCoord1
in vec4 _glesMultiTexCoord1;
#define TANGENT vec4(normalize(_glesTANGENT.xyz), _glesTANGENT.w)
in vec4 _glesTANGENT;

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
    mediump vec2 uv1;
    lowp vec4 diffuse;
    lowp vec3 camDirTS;
    lowp vec3 lightDirTS;
    lowp vec4 fog;
};
#line 67
struct appdata_full {
    highp vec4 vertex;
    highp vec4 tangent;
    highp vec3 normal;
    highp vec4 texcoord;
    highp vec4 texcoord1;
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
uniform highp vec4 cAr;
uniform highp vec4 cAg;
uniform highp vec4 cAb;
uniform highp vec4 cBr;
#line 356
uniform highp vec4 cBg;
uniform highp vec4 cBb;
uniform highp vec4 cC;
uniform highp vec3 _AmbientLight;
#line 360
#line 385
#line 390
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform sampler2D _SpecMap;
uniform highp vec3 _FogParams;
#line 394
uniform highp vec4 _FogRange;
uniform lowp float _Opacity;
#line 406
#line 318
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 320
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 332
highp vec3 ObjectToWorldDir( in highp vec3 dir ) {
    #line 334
    dir = (_Object2World * vec4( dir, 0.0)).xyz;
    return dir;
}
#line 360
highp vec3 ShadeSH9_Float( in highp vec4 normal ) {
    highp vec3 x1;
    highp vec3 x2;
    highp vec3 x3;
    x1.x = dot( (cAr + vec4( 0.0, 0.0, 0.0, _AmbientLight.x)), normal);
    #line 364
    x1.y = dot( (cAg + vec4( 0.0, 0.0, 0.0, _AmbientLight.y)), normal);
    x1.z = dot( (cAb + vec4( 0.0, 0.0, 0.0, _AmbientLight.z)), normal);
    highp vec4 vB = (normal.xyzz * normal.yzzx);
    x2.x = dot( cBr, vB);
    #line 368
    x2.y = dot( cBg, vB);
    x2.z = dot( cBb, vB);
    highp float vC = ((normal.x * normal.x) - (normal.y * normal.y));
    x3 = (cC.xyz * vC);
    #line 372
    return ((x1 + x2) + x3);
}
#line 323
highp vec3 WorldToObjectDir( in highp vec3 dir ) {
    #line 325
    return (_World2Object * vec4( dir, 0.0)).xyz;
}
#line 327
highp vec3 WorldToObjectPos( in highp vec3 pos ) {
    #line 329
    pos = (_World2Object * vec4( pos, 1.0)).xyz;
    return pos;
}
#line 406
v2f vert( in appdata_full v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 410
    o.uv1 = v.texcoord.xy;
    highp vec3 lightDir = WorldToObjectDir( _SpecDir.xyz);
    highp vec3 camPos = WorldToObjectPos( _WorldSpaceCameraPos.xyz);
    highp vec3 camDir = (camPos - v.vertex.xyz);
    #line 414
    highp vec3 normalWS = normalize(ObjectToWorldDir( v.normal));
    highp vec3 reflectDirWS = (_Object2World * vec4( reflect( (-camDir), v.normal.xyz), 0.0)).xyz;
    #line 420
    highp vec3 binormal = (cross( v.normal.xyz, v.tangent.xyz) * v.tangent.w);
    o.camDirTS = normalize(vec3( dot( camDir, v.tangent.xyz), dot( camDir, binormal), dot( camDir, v.normal.xyz)));
    #line 424
    o.lightDirTS = vec3( dot( lightDir, v.tangent.xyz), dot( lightDir, binormal), dot( lightDir, v.normal.xyz));
    o.diffuse.w = max( 0.0, dot( normalWS, _SpecDir));
    #line 434
    o.diffuse.xyz = ShadeSH9_Float( vec4( normalWS, 1.0));
    #line 438
    highp float dist = o.pos.z;
    highp float visibility;
    #line 442
    visibility = clamp( ((dist * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
    highp vec4 fog = vec4( (_FogParams.xyz - (_FogParams.xyz * visibility)), visibility);
    o.fog = fog;
    #line 447
    return o;
}

out mediump vec2 xlv_TEXCOORD0;
out lowp vec4 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
out lowp vec3 xlv_TEXCOORD3;
out lowp vec4 xlv_TEXCOORD5;
void main() {
    v2f xl_retval;
    appdata_full xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.tangent = vec4(TANGENT);
    xlt_v.normal = vec3(gl_Normal);
    xlt_v.texcoord = vec4(gl_MultiTexCoord0);
    xlt_v.texcoord1 = vec4(gl_MultiTexCoord1);
    xlt_v.color = vec4(gl_Color);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_TEXCOORD0 = vec2(xl_retval.uv1);
    xlv_TEXCOORD1 = vec4(xl_retval.diffuse);
    xlv_TEXCOORD2 = vec3(xl_retval.camDirTS);
    xlv_TEXCOORD3 = vec3(xl_retval.lightDirTS);
    xlv_TEXCOORD5 = vec4(xl_retval.fog);
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
#line 396
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    lowp vec4 diffuse;
    lowp vec3 camDirTS;
    lowp vec3 lightDirTS;
    lowp vec4 fog;
};
#line 67
struct appdata_full {
    highp vec4 vertex;
    highp vec4 tangent;
    highp vec3 normal;
    highp vec4 texcoord;
    highp vec4 texcoord1;
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
uniform highp vec4 cAr;
uniform highp vec4 cAg;
uniform highp vec4 cAb;
uniform highp vec4 cBr;
#line 356
uniform highp vec4 cBg;
uniform highp vec4 cBb;
uniform highp vec4 cC;
uniform highp vec3 _AmbientLight;
#line 360
#line 385
#line 390
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform sampler2D _SpecMap;
uniform highp vec3 _FogParams;
#line 394
uniform highp vec4 _FogRange;
uniform lowp float _Opacity;
#line 406
#line 374
lowp float DirectionalSpecular( in lowp vec3 camDir, in lowp vec3 lightDir, in lowp vec3 normal ) {
    #line 376
    mediump float spec;
    lowp vec3 halfVector = normalize((lightDir + camDir));
    #line 380
    spec = max( 0.0, dot( halfVector, normal));
    spec = pow( spec, _SpecPower);
    return spec;
}
#line 341
lowp vec3 UnpackNormalHQ( in lowp vec4 packednormal ) {
    #line 343
    lowp vec3 normal;
    normal.xy = ((packednormal.wy * 2.0) - 1.0);
    normal.z = sqrt(((1.0 - (normal.x * normal.x)) - (normal.y * normal.y)));
    return normal;
}
#line 449
lowp vec4 frag( in v2f i ) {
    #line 451
    lowp vec4 diffusemap = texture( _MainTex, i.uv1);
    lowp vec4 specmap = vec4( 1.0);
    lowp vec3 envmap = vec3( 0.0);
    lowp vec3 specular = vec3( 0.0);
    #line 455
    lowp vec3 diffuse = vec3( 0.0);
    lowp vec3 emissive = vec3( 0.0);
    #line 459
    diffuse = i.diffuse.xyz;
    #line 463
    specmap = texture( _SpecMap, i.uv1);
    #line 469
    lowp vec3 normalmap = UnpackNormalHQ( texture( _BumpMap, i.uv1));
    lowp vec3 lightDirTS = normalize(i.lightDirTS);
    #line 473
    specular = vec3( DirectionalSpecular( normalize(i.camDirTS), lightDirTS, normalmap));
    #line 477
    lowp vec3 colour = (((vec3( diffusemap) + (specular * specmap.xyz)) * diffuse) + (envmap * vec3( specmap)));
    colour = ((colour * i.fog.w) + i.fog.xyz);
    #line 482
    colour += emissive;
    return vec4( colour, _Opacity);
}
in mediump vec2 xlv_TEXCOORD0;
in lowp vec4 xlv_TEXCOORD1;
in lowp vec3 xlv_TEXCOORD2;
in lowp vec3 xlv_TEXCOORD3;
in lowp vec4 xlv_TEXCOORD5;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD0);
    xlt_i.diffuse = vec4(xlv_TEXCOORD1);
    xlt_i.camDirTS = vec3(xlv_TEXCOORD2);
    xlt_i.lightDirTS = vec3(xlv_TEXCOORD3);
    xlt_i.fog = vec4(xlv_TEXCOORD5);
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