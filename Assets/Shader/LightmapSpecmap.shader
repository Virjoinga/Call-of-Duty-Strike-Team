узShader "Corona/Lightmap/[Spec]" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
 _SpecPower ("Specular Power", Range(0,50)) = 10
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
Keywords { "ENABLE_HIGH_LOD" }
"!!GLES


#ifdef VERTEX

varying lowp vec4 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp vec4 _FogRange;
uniform highp vec3 _FogParams;
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _DepthBand;
uniform highp mat4 _World2Object;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _WorldSpaceCameraPos;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 fog_1;
  mediump vec2 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec4 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_7;
  tmpvar_7 = (glstate_matrix_mvp * tmpvar_6);
  highp vec4 tmpvar_8;
  tmpvar_8.x = tmpvar_7.x;
  tmpvar_8.y = tmpvar_7.y;
  tmpvar_8.z = ((tmpvar_7.z * _DepthBand.z) + (tmpvar_7.w * _DepthBand.y));
  tmpvar_8.w = tmpvar_7.w;
  highp vec2 tmpvar_9;
  tmpvar_9 = _glesMultiTexCoord0.xy;
  tmpvar_2 = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  tmpvar_3 = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = _WorldSpaceCameraPos;
  highp vec3 tmpvar_12;
  tmpvar_12 = normalize(((_World2Object * tmpvar_11).xyz - _glesVertex.xyz));
  tmpvar_4 = tmpvar_12;
  highp float tmpvar_13;
  tmpvar_13 = clamp (((tmpvar_8.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
  highp vec4 tmpvar_14;
  tmpvar_14.xyz = (_FogParams - (_FogParams * tmpvar_13));
  tmpvar_14.w = tmpvar_13;
  fog_1.xyz = tmpvar_14.xyz;
  fog_1.w = (tmpvar_13 * 2.0);
  tmpvar_5 = fog_1;
  gl_Position = tmpvar_8;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = normalize(_glesNormal);
  xlv_TEXCOORD5 = tmpvar_5;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD5;
varying lowp vec3 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _SpecMap;
uniform sampler2D _MainTex;
uniform mediump float _SpecPower;
uniform lowp vec3 _SpecDir;
uniform highp mat4 _World2Object;
void main ()
{
  lowp vec3 lightDir_1;
  highp vec3 tmpvar_2;
  highp vec3 dir_3;
  dir_3 = _SpecDir;
  highp vec4 tmpvar_4;
  tmpvar_4.w = 0.0;
  tmpvar_4.xyz = dir_3;
  tmpvar_2 = (_World2Object * tmpvar_4).xyz;
  lightDir_1 = tmpvar_2;
  lowp float tmpvar_5;
  mediump float spec_6;
  lowp float tmpvar_7;
  tmpvar_7 = max (0.0, dot (normalize((lightDir_1 + normalize(xlv_TEXCOORD2))), normalize(xlv_TEXCOORD3)));
  spec_6 = tmpvar_7;
  mediump float tmpvar_8;
  tmpvar_8 = pow (spec_6, _SpecPower);
  spec_6 = tmpvar_8;
  tmpvar_5 = tmpvar_8;
  lowp vec4 tmpvar_9;
  tmpvar_9.w = 1.0;
  tmpvar_9.xyz = ((((texture2D (_MainTex, xlv_TEXCOORD0).xyz + (vec3(tmpvar_5) * texture2D (_SpecMap, xlv_TEXCOORD0).xyz)) * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz) * xlv_TEXCOORD5.w) + xlv_TEXCOORD5.xyz);
  gl_FragData[0] = tmpvar_9;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "ENABLE_HIGH_LOD" }
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
#line 388
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    mediump vec2 lmUV;
    lowp vec3 camDir;
    lowp vec3 normal;
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
#line 377
#line 382
uniform sampler2D _MainTex;
uniform sampler2D _SpecMap;
uniform sampler2D unity_Lightmap;
uniform highp vec4 unity_LightmapST;
#line 386
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 398
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
#line 398
v2f vert( in appdata_full v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 402
    o.uv1 = v.texcoord.xy;
    o.lmUV = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    #line 407
    highp vec3 lightDir = WorldToObjectDir( _SpecDir.xyz);
    highp vec3 camPos = WorldToObjectPos( _WorldSpaceCameraPos.xyz);
    highp vec3 camDir = (camPos - v.vertex.xyz);
    highp vec3 normalWS = normalize(ObjectToWorldDir( v.normal));
    #line 411
    highp vec3 reflectDirWS = (_Object2World * vec4( reflect( (-camDir), v.normal.xyz), 0.0)).xyz;
    #line 416
    o.camDir = normalize(camDir);
    o.normal = v.normal;
    #line 422
    highp float dist = o.pos.z;
    highp float visibility;
    #line 426
    visibility = clamp( ((dist * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
    highp vec4 fog = vec4( (_FogParams.xyz - (_FogParams.xyz * visibility)), visibility);
    #line 431
    fog.w *= 2.0;
    o.fog = fog;
    #line 435
    return o;
}

out mediump vec2 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
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
    xlv_TEXCOORD1 = vec2(xl_retval.lmUV);
    xlv_TEXCOORD2 = vec3(xl_retval.camDir);
    xlv_TEXCOORD3 = vec3(xl_retval.normal);
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
#line 388
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    mediump vec2 lmUV;
    lowp vec3 camDir;
    lowp vec3 normal;
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
#line 377
#line 382
uniform sampler2D _MainTex;
uniform sampler2D _SpecMap;
uniform sampler2D unity_Lightmap;
uniform highp vec4 unity_LightmapST;
#line 386
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 398
#line 348
lowp vec3 DecodeLightmapHalf( in lowp vec4 color ) {
    return ((4.0 * color.w) * color.xyz);
}
#line 366
lowp float DirectionalSpecular( in lowp vec3 camDir, in lowp vec3 lightDir, in lowp vec3 normal ) {
    #line 368
    mediump float spec;
    lowp vec3 halfVector = normalize((lightDir + camDir));
    #line 372
    spec = max( 0.0, dot( halfVector, normal));
    spec = pow( spec, _SpecPower);
    return spec;
}
#line 323
highp vec3 WorldToObjectDir( in highp vec3 dir ) {
    #line 325
    return (_World2Object * vec4( dir, 0.0)).xyz;
}
#line 377
lowp float DirectionalSpecularOS( in lowp vec3 camDir, in lowp vec3 normal ) {
    lowp vec3 lightDir = WorldToObjectDir( _SpecDir);
    return DirectionalSpecular( camDir, lightDir, normal);
}
#line 437
lowp vec4 frag( in v2f i ) {
    #line 439
    lowp vec4 diffusemap = texture( _MainTex, i.uv1);
    lowp vec4 specmap = vec4( 1.0);
    lowp vec3 envmap = vec3( 0.0);
    lowp vec3 specular = vec3( 0.0);
    #line 443
    lowp vec3 diffuse = vec3( 0.0);
    lowp vec3 emissive = vec3( 0.0);
    #line 447
    diffuse = DecodeLightmapHalf( texture( unity_Lightmap, i.lmUV));
    #line 451
    specmap = texture( _SpecMap, i.uv1);
    #line 457
    specular = vec3( DirectionalSpecularOS( normalize(i.camDir), normalize(i.normal)));
    lowp vec3 colour = (((vec3( diffusemap) + (specular * specmap.xyz)) * diffuse) + (envmap * vec3( specmap)));
    #line 463
    colour = ((colour * i.fog.w) + i.fog.xyz);
    colour += emissive;
    #line 468
    return vec4( colour, 1.0);
}
in mediump vec2 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in lowp vec3 xlv_TEXCOORD2;
in lowp vec3 xlv_TEXCOORD3;
in lowp vec4 xlv_TEXCOORD5;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD0);
    xlt_i.lmUV = vec2(xlv_TEXCOORD1);
    xlt_i.camDir = vec3(xlv_TEXCOORD2);
    xlt_i.normal = vec3(xlv_TEXCOORD3);
    xlt_i.fog = vec4(xlv_TEXCOORD5);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4(xl_retval);
}


#endif"
}
SubProgram "gles " {
Keywords { "ENABLE_LOW_LOD" }
"!!GLES


#ifdef VERTEX

varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp vec4 _FogRange;
uniform highp vec3 _FogParams;
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _DepthBand;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  highp vec4 fog_1;
  mediump vec2 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec4 tmpvar_4;
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
  highp vec2 tmpvar_8;
  tmpvar_8 = _glesMultiTexCoord0.xy;
  tmpvar_2 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  tmpvar_3 = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = clamp (((tmpvar_7.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
  highp vec4 tmpvar_11;
  tmpvar_11.xyz = (_FogParams - (_FogParams * tmpvar_10));
  tmpvar_11.w = tmpvar_10;
  fog_1.xyz = tmpvar_11.xyz;
  fog_1.w = (tmpvar_10 * 2.0);
  tmpvar_4 = fog_1;
  gl_Position = tmpvar_7;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD5 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = (((texture2D (_MainTex, xlv_TEXCOORD0).xyz * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz) * xlv_TEXCOORD5.w) + xlv_TEXCOORD5.xyz);
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "ENABLE_LOW_LOD" }
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
#line 387
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    mediump vec2 lmUV;
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
#line 377
#line 382
uniform sampler2D _MainTex;
uniform sampler2D unity_Lightmap;
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _FogParams;
#line 386
uniform highp vec4 _FogRange;
#line 395
#line 426
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
#line 395
v2f vert( in appdata_full v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 399
    o.uv1 = v.texcoord.xy;
    o.lmUV = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    #line 404
    highp vec3 lightDir = WorldToObjectDir( _SpecDir.xyz);
    highp vec3 camPos = WorldToObjectPos( _WorldSpaceCameraPos.xyz);
    highp vec3 camDir = (camPos - v.vertex.xyz);
    highp vec3 normalWS = normalize(ObjectToWorldDir( v.normal));
    #line 408
    highp vec3 reflectDirWS = (_Object2World * vec4( reflect( (-camDir), v.normal.xyz), 0.0)).xyz;
    highp float dist = o.pos.z;
    #line 412
    highp float visibility;
    visibility = clamp( ((dist * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
    #line 417
    highp vec4 fog = vec4( (_FogParams.xyz - (_FogParams.xyz * visibility)), visibility);
    fog.w *= 2.0;
    #line 422
    o.fog = fog;
    return o;
}

out mediump vec2 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
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
    xlv_TEXCOORD1 = vec2(xl_retval.lmUV);
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
#line 387
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    mediump vec2 lmUV;
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
#line 377
#line 382
uniform sampler2D _MainTex;
uniform sampler2D unity_Lightmap;
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _FogParams;
#line 386
uniform highp vec4 _FogRange;
#line 395
#line 426
#line 348
lowp vec3 DecodeLightmapHalf( in lowp vec4 color ) {
    return ((4.0 * color.w) * color.xyz);
}
#line 426
lowp vec4 frag( in v2f i ) {
    lowp vec4 diffusemap = texture( _MainTex, i.uv1);
    lowp vec4 specmap = vec4( 1.0);
    #line 430
    lowp vec3 envmap = vec3( 0.0);
    lowp vec3 specular = vec3( 0.0);
    lowp vec3 diffuse = vec3( 0.0);
    lowp vec3 emissive = vec3( 0.0);
    #line 436
    diffuse = DecodeLightmapHalf( texture( unity_Lightmap, i.lmUV));
    lowp vec3 colour = (((vec3( diffusemap) + (specular * specmap.xyz)) * diffuse) + (envmap * vec3( specmap)));
    #line 441
    colour = ((colour * i.fog.w) + i.fog.xyz);
    colour += emissive;
    #line 446
    return vec4( colour, 1.0);
}
in mediump vec2 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in lowp vec4 xlv_TEXCOORD5;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD0);
    xlt_i.lmUV = vec2(xlv_TEXCOORD1);
    xlt_i.fog = vec4(xlv_TEXCOORD5);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4(xl_retval);
}


#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "ENABLE_HIGH_LOD" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "ENABLE_HIGH_LOD" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "ENABLE_LOW_LOD" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "ENABLE_LOW_LOD" }
"!!GLES3"
}
}
 }
}
Fallback Off
}