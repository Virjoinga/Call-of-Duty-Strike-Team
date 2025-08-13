ӘShader "Hidden/Probe/[Normal] [Spec] [SpecLookup] [Env] [NoFog]" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
 _BumpMap ("Normalmap", 2D) = "bump" {}
 _SpecLookup ("Specular Lookup", 2D) = "black" {}
 _Reflectivity ("Reflectivity", Range(0,1)) = 0.2
}
SubShader { 
 Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
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
  mediump vec3 tmpvar_7;
  lowp vec4 tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_10;
  tmpvar_10 = (glstate_matrix_mvp * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11.x = tmpvar_10.x;
  tmpvar_11.y = tmpvar_10.y;
  tmpvar_11.z = ((tmpvar_10.z * _DepthBand.z) + (tmpvar_10.w * _DepthBand.y));
  tmpvar_11.w = tmpvar_10.w;
  highp vec2 tmpvar_12;
  tmpvar_12 = _glesMultiTexCoord0.xy;
  tmpvar_3 = tmpvar_12;
  highp vec3 tmpvar_13;
  highp vec3 dir_14;
  dir_14 = _SpecDir;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 0.0;
  tmpvar_15.xyz = dir_14;
  tmpvar_13 = (_World2Object * tmpvar_15).xyz;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_17;
  tmpvar_17 = ((_World2Object * tmpvar_16).xyz - _glesVertex.xyz);
  highp vec4 tmpvar_18;
  tmpvar_18.w = 0.0;
  tmpvar_18.xyz = tmpvar_2;
  highp vec3 tmpvar_19;
  tmpvar_19 = normalize((_Object2World * tmpvar_18).xyz);
  highp vec3 i_20;
  i_20 = -(tmpvar_17);
  highp vec4 tmpvar_21;
  tmpvar_21.w = 0.0;
  tmpvar_21.xyz = (i_20 - (2.0 * (dot (tmpvar_2, i_20) * tmpvar_2)));
  highp vec3 tmpvar_22;
  tmpvar_22 = (_Object2World * tmpvar_21).xyz;
  highp vec3 tmpvar_23;
  tmpvar_23 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * _glesTANGENT.w);
  highp vec3 tmpvar_24;
  tmpvar_24.x = dot (tmpvar_17, tmpvar_1.xyz);
  tmpvar_24.y = dot (tmpvar_17, tmpvar_23);
  tmpvar_24.z = dot (tmpvar_17, tmpvar_2);
  highp vec3 tmpvar_25;
  tmpvar_25 = normalize(tmpvar_24);
  tmpvar_5 = tmpvar_25;
  highp vec3 tmpvar_26;
  tmpvar_26.x = dot (tmpvar_13, tmpvar_1.xyz);
  tmpvar_26.y = dot (tmpvar_13, tmpvar_23);
  tmpvar_26.z = dot (tmpvar_13, tmpvar_2);
  tmpvar_6 = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = max (0.0, dot (tmpvar_19, _SpecDir));
  tmpvar_4.w = tmpvar_27;
  highp vec4 tmpvar_28;
  tmpvar_28.w = 1.0;
  tmpvar_28.xyz = tmpvar_19;
  highp vec3 tmpvar_29;
  highp vec3 x2_30;
  highp vec3 x1_31;
  highp vec4 tmpvar_32;
  tmpvar_32.xyz = vec3(0.0, 0.0, 0.0);
  tmpvar_32.w = _AmbientLight.x;
  x1_31.x = dot ((cAr + tmpvar_32), tmpvar_28);
  highp vec4 tmpvar_33;
  tmpvar_33.xyz = vec3(0.0, 0.0, 0.0);
  tmpvar_33.w = _AmbientLight.y;
  x1_31.y = dot ((cAg + tmpvar_33), tmpvar_28);
  highp vec4 tmpvar_34;
  tmpvar_34.xyz = vec3(0.0, 0.0, 0.0);
  tmpvar_34.w = _AmbientLight.z;
  x1_31.z = dot ((cAb + tmpvar_34), tmpvar_28);
  highp vec4 tmpvar_35;
  tmpvar_35 = (tmpvar_19.xyzz * tmpvar_19.yzzx);
  x2_30.x = dot (cBr, tmpvar_35);
  x2_30.y = dot (cBg, tmpvar_35);
  x2_30.z = dot (cBb, tmpvar_35);
  tmpvar_29 = ((x1_31 + x2_30) + (cC.xyz * ((tmpvar_19.x * tmpvar_19.x) - (tmpvar_19.y * tmpvar_19.y))));
  tmpvar_4.xyz = tmpvar_29;
  tmpvar_7 = tmpvar_22;
  highp float tmpvar_36;
  tmpvar_36 = clamp (((tmpvar_11.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
  highp vec4 tmpvar_37;
  tmpvar_37.xyz = (_FogParams - (_FogParams * tmpvar_36));
  tmpvar_37.w = tmpvar_36;
  tmpvar_8 = tmpvar_37;
  gl_Position = tmpvar_11;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_5;
  xlv_TEXCOORD3 = tmpvar_6;
  xlv_TEXCOORD4 = tmpvar_7;
  xlv_TEXCOORD5 = tmpvar_8;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying lowp vec4 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform lowp float _Reflectivity;
uniform samplerCube _ThemedCube;
uniform sampler2D _SpecMap;
uniform sampler2D _BumpMap;
uniform sampler2D _MainTex;
uniform mediump float _SpecPower;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_SpecMap, xlv_TEXCOORD0);
  lowp float tmpvar_2;
  mediump float spec_3;
  lowp float tmpvar_4;
  tmpvar_4 = max (0.0, dot (normalize((normalize(xlv_TEXCOORD3) + normalize(xlv_TEXCOORD2))), normalize((texture2D (_BumpMap, xlv_TEXCOORD0).xyz - 0.5))));
  spec_3 = tmpvar_4;
  mediump float tmpvar_5;
  tmpvar_5 = pow (spec_3, _SpecPower);
  spec_3 = tmpvar_5;
  tmpvar_2 = tmpvar_5;
  lowp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = (((((texture2D (_MainTex, xlv_TEXCOORD0).xyz + (vec3(tmpvar_2) * tmpvar_1.xyz)) * xlv_TEXCOORD1.xyz) + ((textureCube (_ThemedCube, xlv_TEXCOORD4).xyz * _Reflectivity) * tmpvar_1.xyz)) * xlv_TEXCOORD5.w) + xlv_TEXCOORD5.xyz);
  gl_FragData[0] = tmpvar_6;
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
#line 397
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    lowp vec4 diffuse;
    lowp vec3 camDirTS;
    lowp vec3 lightDirTS;
    mediump vec3 reflectDirWS;
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
uniform samplerCube _ThemedCube;
#line 394
uniform lowp float _Reflectivity;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 408
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
#line 408
v2f vert( in appdata_full v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 412
    o.uv1 = v.texcoord.xy;
    highp vec3 lightDir = WorldToObjectDir( _SpecDir.xyz);
    highp vec3 camPos = WorldToObjectPos( _WorldSpaceCameraPos.xyz);
    highp vec3 camDir = (camPos - v.vertex.xyz);
    #line 416
    highp vec3 normalWS = normalize(ObjectToWorldDir( v.normal));
    highp vec3 reflectDirWS = (_Object2World * vec4( reflect( (-camDir), v.normal.xyz), 0.0)).xyz;
    #line 422
    highp vec3 binormal = (cross( v.normal.xyz, v.tangent.xyz) * v.tangent.w);
    o.camDirTS = normalize(vec3( dot( camDir, v.tangent.xyz), dot( camDir, binormal), dot( camDir, v.normal.xyz)));
    #line 426
    o.lightDirTS = vec3( dot( lightDir, v.tangent.xyz), dot( lightDir, binormal), dot( lightDir, v.normal.xyz));
    o.diffuse.w = max( 0.0, dot( normalWS, _SpecDir));
    #line 436
    o.diffuse.xyz = ShadeSH9_Float( vec4( normalWS, 1.0));
    #line 440
    o.reflectDirWS = reflectDirWS;
    #line 444
    highp float dist = o.pos.z;
    highp float visibility;
    #line 448
    visibility = clamp( ((dist * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
    highp vec4 fog = vec4( (_FogParams.xyz - (_FogParams.xyz * visibility)), visibility);
    o.fog = fog;
    #line 453
    return o;
}

out mediump vec2 xlv_TEXCOORD0;
out lowp vec4 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
out lowp vec3 xlv_TEXCOORD3;
out mediump vec3 xlv_TEXCOORD4;
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
    xlv_TEXCOORD4 = vec3(xl_retval.reflectDirWS);
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
#line 397
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    lowp vec4 diffuse;
    lowp vec3 camDirTS;
    lowp vec3 lightDirTS;
    mediump vec3 reflectDirWS;
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
uniform samplerCube _ThemedCube;
#line 394
uniform lowp float _Reflectivity;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 408
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
#line 455
lowp vec4 frag( in v2f i ) {
    #line 457
    lowp vec4 diffusemap = texture( _MainTex, i.uv1);
    lowp vec4 specmap = vec4( 1.0);
    lowp vec3 envmap = vec3( 0.0);
    lowp vec3 specular = vec3( 0.0);
    #line 461
    lowp vec3 diffuse = vec3( 0.0);
    lowp vec3 emissive = vec3( 0.0);
    #line 465
    diffuse = i.diffuse.xyz;
    #line 469
    lowp float envMapScale = _Reflectivity;
    envmap = (texture( _ThemedCube, i.reflectDirWS).xyz * envMapScale);
    #line 474
    specmap = texture( _SpecMap, i.uv1);
    #line 480
    lowp vec3 normalmap = UnpackNormalHQ( texture( _BumpMap, i.uv1));
    lowp vec3 lightDirTS = normalize(i.lightDirTS);
    #line 484
    specular = vec3( DirectionalSpecular( normalize(i.camDirTS), lightDirTS, normalmap));
    #line 488
    lowp vec3 colour = (((vec3( diffusemap) + (specular * specmap.xyz)) * diffuse) + (envmap * vec3( specmap)));
    colour = ((colour * i.fog.w) + i.fog.xyz);
    #line 493
    colour += emissive;
    return vec4( colour, 1.0);
}
in mediump vec2 xlv_TEXCOORD0;
in lowp vec4 xlv_TEXCOORD1;
in lowp vec3 xlv_TEXCOORD2;
in lowp vec3 xlv_TEXCOORD3;
in mediump vec3 xlv_TEXCOORD4;
in lowp vec4 xlv_TEXCOORD5;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD0);
    xlt_i.diffuse = vec4(xlv_TEXCOORD1);
    xlt_i.camDirTS = vec3(xlv_TEXCOORD2);
    xlt_i.lightDirTS = vec3(xlv_TEXCOORD3);
    xlt_i.reflectDirWS = vec3(xlv_TEXCOORD4);
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