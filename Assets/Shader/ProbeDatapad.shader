°’Shader "Corona/Effects/Datapad [ViewModel]" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
 _NoiseTex ("Noise Texture", 2D) = "white" {}
 _PhaseLineTex ("Phase Line Texture", 2D) = "black" {}
 _Reflectivity ("Reflectivity", Range(0,1)) = 0.5
 _BroadSpecular ("Broad Specular", Range(0,1)) = 0.25
 _PhaseLineFrequency ("Phase line frequency", Range(0,5)) = 0.5
 _PhaseLineOverscan ("Phase line overscan", Range(1,2)) = 1.2
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

varying lowp vec2 xlv_TEXCOORD6;
varying mediump vec2 xlv_TEXCOORD7;
varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp vec4 _FogRange;
uniform highp vec3 _FogParams;
uniform highp float _BroadSpecular;
uniform highp vec3 _AmbientLight;
uniform highp vec4 cC;
uniform highp vec4 cBb;
uniform highp vec4 cBg;
uniform highp vec4 cBr;
uniform highp vec4 cAb;
uniform highp vec4 cAg;
uniform highp vec4 cAr;
uniform highp vec3 _DepthBand;
uniform highp float _PhaseLineOverscan;
uniform highp float _PhaseLineFrequency;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _Time;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  mediump vec2 tmpvar_2;
  lowp vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  lowp vec4 tmpvar_5;
  mediump vec2 tmpvar_6;
  lowp vec2 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * tmpvar_8);
  highp vec4 tmpvar_10;
  tmpvar_10.x = tmpvar_9.x;
  tmpvar_10.y = tmpvar_9.y;
  tmpvar_10.z = (tmpvar_9.z * _DepthBand.x);
  tmpvar_10.w = tmpvar_9.w;
  highp vec2 tmpvar_11;
  tmpvar_11 = _glesMultiTexCoord0.xy;
  tmpvar_2 = tmpvar_11;
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = _WorldSpaceCameraPos;
  highp vec3 i_13;
  i_13 = (_glesVertex.xyz - (_World2Object * tmpvar_12).xyz);
  highp vec4 tmpvar_14;
  tmpvar_14.w = 0.0;
  tmpvar_14.xyz = (i_13 - (2.0 * (dot (tmpvar_1, i_13) * tmpvar_1)));
  highp vec3 tmpvar_15;
  tmpvar_15 = (_Object2World * tmpvar_14).xyz;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  highp vec3 x2_17;
  highp vec3 x1_18;
  highp vec4 tmpvar_19;
  tmpvar_19.xyz = vec3(0.0, 0.0, 0.0);
  tmpvar_19.w = _AmbientLight.x;
  x1_18.x = dot ((cAr + tmpvar_19), tmpvar_16);
  highp vec4 tmpvar_20;
  tmpvar_20.xyz = vec3(0.0, 0.0, 0.0);
  tmpvar_20.w = _AmbientLight.y;
  x1_18.y = dot ((cAg + tmpvar_20), tmpvar_16);
  highp vec4 tmpvar_21;
  tmpvar_21.xyz = vec3(0.0, 0.0, 0.0);
  tmpvar_21.w = _AmbientLight.z;
  x1_18.z = dot ((cAb + tmpvar_21), tmpvar_16);
  highp vec4 tmpvar_22;
  tmpvar_22 = (tmpvar_15.xyzz * tmpvar_15.yzzx);
  x2_17.x = dot (cBr, tmpvar_22);
  x2_17.y = dot (cBg, tmpvar_22);
  x2_17.z = dot (cBb, tmpvar_22);
  highp vec3 tmpvar_23;
  tmpvar_23 = (((x1_18 + x2_17) + (cC.xyz * ((tmpvar_15.x * tmpvar_15.x) - (tmpvar_15.y * tmpvar_15.y)))) * _BroadSpecular);
  tmpvar_3 = tmpvar_23;
  tmpvar_4 = tmpvar_15;
  highp float tmpvar_24;
  tmpvar_24 = clamp (((tmpvar_10.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
  highp vec4 tmpvar_25;
  tmpvar_25.xyz = (_FogParams - (_FogParams * tmpvar_24));
  tmpvar_25.w = tmpvar_24;
  tmpvar_5 = tmpvar_25;
  highp vec2 tmpvar_26;
  tmpvar_26.x = fract((157.079 * _Time.y));
  tmpvar_26.y = fract((135.914 * _Time.y));
  highp vec2 tmpvar_27;
  tmpvar_27 = ((_glesMultiTexCoord0.xy * 2.0) + tmpvar_26);
  tmpvar_6 = tmpvar_27;
  highp vec2 tmpvar_28;
  tmpvar_28 = vec2(((((_glesMultiTexCoord0.y * 1024.0) / 245.0) - 4.17959) + (fract((_Time.y * _PhaseLineFrequency)) * _PhaseLineOverscan)));
  tmpvar_7 = tmpvar_28;
  gl_Position = tmpvar_10;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
  xlv_TEXCOORD7 = tmpvar_6;
  xlv_TEXCOORD6 = tmpvar_7;
}



#endif
#ifdef FRAGMENT

varying lowp vec2 xlv_TEXCOORD6;
varying mediump vec2 xlv_TEXCOORD7;
varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec2 xlv_TEXCOORD0;
uniform lowp float _Reflectivity;
uniform samplerCube _ThemedCube;
uniform sampler2D _SpecMap;
uniform sampler2D _MainTex;
uniform lowp float g_datapadBrightness;
uniform sampler2D _PhaseLineTex;
uniform sampler2D _NoiseTex;
void main ()
{
  lowp vec4 diffusemap_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  diffusemap_1.w = tmpvar_2.w;
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_SpecMap, xlv_TEXCOORD0);
  diffusemap_1.xyz = (tmpvar_2.xyz + texture2D (_PhaseLineTex, xlv_TEXCOORD6).xyz);
  diffusemap_1.xyz = (diffusemap_1.xyz * (texture2D (_NoiseTex, xlv_TEXCOORD7).xyz * g_datapadBrightness));
  lowp vec4 tmpvar_4;
  tmpvar_4.w = 1.0;
  tmpvar_4.xyz = ((((diffusemap_1.xyz + (xlv_TEXCOORD2 * tmpvar_3.xyz)) + ((textureCube (_ThemedCube, xlv_TEXCOORD4).xyz * _Reflectivity) * tmpvar_3.xyz)) * xlv_TEXCOORD5.w) + xlv_TEXCOORD5.xyz);
  gl_FragData[0] = tmpvar_4;
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

#line 156
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 192
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 186
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 402
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    lowp vec3 specular;
    mediump vec3 reflectDirWS;
    lowp vec4 fog;
    mediump vec2 uv2;
    lowp vec2 phaseLine;
};
#line 72
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
uniform sampler2D _NoiseTex;
uniform sampler2D _PhaseLineTex;
uniform highp float _PhaseLineFrequency;
uniform highp float _PhaseLineOverscan;
#line 55
uniform lowp float g_datapadBrightness;
uniform lowp vec4 unity_ColorSpaceGrey;
#line 82
#line 87
#line 92
#line 96
#line 101
#line 125
#line 142
#line 163
#line 171
#line 198
#line 211
#line 220
#line 225
#line 234
#line 239
#line 248
#line 265
#line 270
#line 296
#line 304
#line 312
#line 316
#line 320
uniform lowp vec3 _SpecDir;
uniform mediump float _SpecPower;
uniform highp vec3 _DepthBand;
#line 353
#line 357
uniform highp vec4 cAr;
uniform highp vec4 cAg;
uniform highp vec4 cAb;
uniform highp vec4 cBr;
#line 361
uniform highp vec4 cBg;
uniform highp vec4 cBb;
uniform highp vec4 cC;
uniform highp vec3 _AmbientLight;
#line 365
#line 390
#line 395
uniform sampler2D _MainTex;
uniform sampler2D _SpecMap;
uniform samplerCube _ThemedCube;
uniform lowp float _Reflectivity;
#line 399
uniform highp float _BroadSpecular;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 413
#line 323
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 325
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, (pos.z * _DepthBand.x), pos.w);
}
#line 337
highp vec3 ObjectToWorldDir( in highp vec3 dir ) {
    #line 339
    dir = (_Object2World * vec4( dir, 0.0)).xyz;
    return dir;
}
#line 365
highp vec3 ShadeSH9_Float( in highp vec4 normal ) {
    highp vec3 x1;
    highp vec3 x2;
    highp vec3 x3;
    x1.x = dot( (cAr + vec4( 0.0, 0.0, 0.0, _AmbientLight.x)), normal);
    #line 369
    x1.y = dot( (cAg + vec4( 0.0, 0.0, 0.0, _AmbientLight.y)), normal);
    x1.z = dot( (cAb + vec4( 0.0, 0.0, 0.0, _AmbientLight.z)), normal);
    highp vec4 vB = (normal.xyzz * normal.yzzx);
    x2.x = dot( cBr, vB);
    #line 373
    x2.y = dot( cBg, vB);
    x2.z = dot( cBb, vB);
    highp float vC = ((normal.x * normal.x) - (normal.y * normal.y));
    x3 = (cC.xyz * vC);
    #line 377
    return ((x1 + x2) + x3);
}
#line 328
highp vec3 WorldToObjectDir( in highp vec3 dir ) {
    #line 330
    return (_World2Object * vec4( dir, 0.0)).xyz;
}
#line 332
highp vec3 WorldToObjectPos( in highp vec3 pos ) {
    #line 334
    pos = (_World2Object * vec4( pos, 1.0)).xyz;
    return pos;
}
#line 413
v2f vert( in appdata_full v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 417
    o.uv1 = v.texcoord.xy;
    highp vec3 lightDir = WorldToObjectDir( _SpecDir.xyz);
    highp vec3 camPos = WorldToObjectPos( _WorldSpaceCameraPos.xyz);
    highp vec3 camDir = (camPos - v.vertex.xyz);
    #line 421
    highp vec3 normalWS = normalize(ObjectToWorldDir( v.normal));
    highp vec3 reflectDirWS = (_Object2World * vec4( reflect( (-camDir), v.normal.xyz), 0.0)).xyz;
    #line 425
    o.specular = (ShadeSH9_Float( vec4( reflectDirWS, 1.0)) * _BroadSpecular);
    #line 429
    o.reflectDirWS = reflectDirWS;
    #line 433
    highp float dist = o.pos.z;
    highp float visibility;
    #line 437
    visibility = clamp( ((dist * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
    highp vec4 fog = vec4( (_FogParams.xyz - (_FogParams.xyz * visibility)), visibility);
    o.fog = fog;
    #line 444
    highp float uoffset = fract(((_Time.y * 3.14159) * 50.0));
    highp float voffset = fract(((_Time.y * 2.71828) * 50.0));
    o.uv2.xy = ((v.texcoord.xy * 2.0) + vec2( uoffset, voffset));
    o.phaseLine = vec2( ((((v.texcoord.y * 1024.0) / 245.0) - 4.17959) + (fract((_Time.y * _PhaseLineFrequency)) * _PhaseLineOverscan)));
    #line 450
    return o;
}

out mediump vec2 xlv_TEXCOORD0;
out lowp vec3 xlv_TEXCOORD2;
out mediump vec3 xlv_TEXCOORD4;
out lowp vec4 xlv_TEXCOORD5;
out mediump vec2 xlv_TEXCOORD7;
out lowp vec2 xlv_TEXCOORD6;
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
    xlv_TEXCOORD2 = vec3(xl_retval.specular);
    xlv_TEXCOORD4 = vec3(xl_retval.reflectDirWS);
    xlv_TEXCOORD5 = vec4(xl_retval.fog);
    xlv_TEXCOORD7 = vec2(xl_retval.uv2);
    xlv_TEXCOORD6 = vec2(xl_retval.phaseLine);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];

#line 156
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 192
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 186
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 402
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    lowp vec3 specular;
    mediump vec3 reflectDirWS;
    lowp vec4 fog;
    mediump vec2 uv2;
    lowp vec2 phaseLine;
};
#line 72
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
uniform sampler2D _NoiseTex;
uniform sampler2D _PhaseLineTex;
uniform highp float _PhaseLineFrequency;
uniform highp float _PhaseLineOverscan;
#line 55
uniform lowp float g_datapadBrightness;
uniform lowp vec4 unity_ColorSpaceGrey;
#line 82
#line 87
#line 92
#line 96
#line 101
#line 125
#line 142
#line 163
#line 171
#line 198
#line 211
#line 220
#line 225
#line 234
#line 239
#line 248
#line 265
#line 270
#line 296
#line 304
#line 312
#line 316
#line 320
uniform lowp vec3 _SpecDir;
uniform mediump float _SpecPower;
uniform highp vec3 _DepthBand;
#line 353
#line 357
uniform highp vec4 cAr;
uniform highp vec4 cAg;
uniform highp vec4 cAb;
uniform highp vec4 cBr;
#line 361
uniform highp vec4 cBg;
uniform highp vec4 cBb;
uniform highp vec4 cC;
uniform highp vec3 _AmbientLight;
#line 365
#line 390
#line 395
uniform sampler2D _MainTex;
uniform sampler2D _SpecMap;
uniform samplerCube _ThemedCube;
uniform lowp float _Reflectivity;
#line 399
uniform highp float _BroadSpecular;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 413
#line 452
lowp vec4 frag( in v2f i ) {
    #line 454
    lowp vec4 diffusemap = texture( _MainTex, i.uv1);
    lowp vec4 specmap = vec4( 1.0);
    lowp vec3 envmap = vec3( 0.0);
    lowp vec3 specular = vec3( 0.0);
    #line 458
    lowp vec3 diffuse = vec3( 0.0);
    lowp vec3 emissive = vec3( 0.0);
    #line 462
    diffuse = vec3( 1.0);
    #line 466
    lowp float envMapScale = _Reflectivity;
    envmap = (texture( _ThemedCube, i.reflectDirWS).xyz * envMapScale);
    #line 471
    specmap = texture( _SpecMap, i.uv1);
    #line 475
    specular = i.specular;
    #line 479
    diffusemap.xyz += texture( _PhaseLineTex, i.phaseLine).xyz;
    diffusemap.xyz *= (texture( _NoiseTex, i.uv2).xyz * g_datapadBrightness);
    #line 483
    lowp vec3 colour = (((vec3( diffusemap) + (specular * specmap.xyz)) * diffuse) + (envmap * vec3( specmap)));
    colour = ((colour * i.fog.w) + i.fog.xyz);
    #line 488
    colour += emissive;
    return vec4( colour, 1.0);
}
in mediump vec2 xlv_TEXCOORD0;
in lowp vec3 xlv_TEXCOORD2;
in mediump vec3 xlv_TEXCOORD4;
in lowp vec4 xlv_TEXCOORD5;
in mediump vec2 xlv_TEXCOORD7;
in lowp vec2 xlv_TEXCOORD6;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD0);
    xlt_i.specular = vec3(xlv_TEXCOORD2);
    xlt_i.reflectDirWS = vec3(xlv_TEXCOORD4);
    xlt_i.fog = vec4(xlv_TEXCOORD5);
    xlt_i.uv2 = vec2(xlv_TEXCOORD7);
    xlt_i.phaseLine = vec2(xlv_TEXCOORD6);
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