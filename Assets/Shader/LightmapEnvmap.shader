φέShader "Corona/Lightmap/[Env]" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
 _Reflectivity ("Reflectivity", Range(0,1)) = 0.2
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
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp vec4 _FogRange;
uniform highp vec3 _FogParams;
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _DepthBand;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _WorldSpaceCameraPos;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  highp vec4 fog_2;
  mediump vec2 tmpvar_3;
  mediump vec2 tmpvar_4;
  mediump vec3 tmpvar_5;
  lowp vec4 tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_8;
  tmpvar_8 = (glstate_matrix_mvp * tmpvar_7);
  highp vec4 tmpvar_9;
  tmpvar_9.x = tmpvar_8.x;
  tmpvar_9.y = tmpvar_8.y;
  tmpvar_9.z = ((tmpvar_8.z * _DepthBand.z) + (tmpvar_8.w * _DepthBand.y));
  tmpvar_9.w = tmpvar_8.w;
  highp vec2 tmpvar_10;
  tmpvar_10 = _glesMultiTexCoord0.xy;
  tmpvar_3 = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  tmpvar_4 = tmpvar_11;
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
  tmpvar_5 = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = clamp (((tmpvar_9.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
  highp vec4 tmpvar_17;
  tmpvar_17.xyz = (_FogParams - (_FogParams * tmpvar_16));
  tmpvar_17.w = tmpvar_16;
  fog_2.xyz = tmpvar_17.xyz;
  fog_2.w = (tmpvar_16 * 2.0);
  tmpvar_6 = fog_2;
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = tmpvar_6;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp float _Reflectivity;
uniform samplerCube _ThemedCube;
uniform sampler2D _SpecMap;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = ((((texture2D (_MainTex, xlv_TEXCOORD0).xyz * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz) + ((textureCube (_ThemedCube, xlv_TEXCOORD4).xyz * (_Reflectivity * 0.5)) * texture2D (_SpecMap, xlv_TEXCOORD0).xyz)) * xlv_TEXCOORD5.w) + xlv_TEXCOORD5.xyz);
  gl_FragData[0] = tmpvar_1;
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
#line 390
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    mediump vec2 lmUV;
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
#line 377
#line 382
uniform sampler2D _MainTex;
uniform sampler2D _SpecMap;
uniform samplerCube _ThemedCube;
uniform lowp float _Reflectivity;
#line 386
uniform sampler2D unity_Lightmap;
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 399
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
#line 399
v2f vert( in appdata_full v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 403
    o.uv1 = v.texcoord.xy;
    o.lmUV = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
    #line 408
    highp vec3 lightDir = WorldToObjectDir( _SpecDir.xyz);
    highp vec3 camPos = WorldToObjectPos( _WorldSpaceCameraPos.xyz);
    highp vec3 camDir = (camPos - v.vertex.xyz);
    highp vec3 normalWS = normalize(ObjectToWorldDir( v.normal));
    #line 412
    highp vec3 reflectDirWS = (_Object2World * vec4( reflect( (-camDir), v.normal.xyz), 0.0)).xyz;
    o.reflectDirWS = reflectDirWS;
    #line 419
    highp float dist = o.pos.z;
    highp float visibility;
    #line 423
    visibility = clamp( ((dist * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
    highp vec4 fog = vec4( (_FogParams.xyz - (_FogParams.xyz * visibility)), visibility);
    #line 428
    fog.w *= 2.0;
    o.fog = fog;
    #line 432
    return o;
}

out mediump vec2 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
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
    xlv_TEXCOORD1 = vec2(xl_retval.lmUV);
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
#line 390
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    mediump vec2 lmUV;
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
#line 377
#line 382
uniform sampler2D _MainTex;
uniform sampler2D _SpecMap;
uniform samplerCube _ThemedCube;
uniform lowp float _Reflectivity;
#line 386
uniform sampler2D unity_Lightmap;
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 399
#line 348
lowp vec3 DecodeLightmapHalf( in lowp vec4 color ) {
    return ((4.0 * color.w) * color.xyz);
}
#line 434
lowp vec4 frag( in v2f i ) {
    #line 436
    lowp vec4 diffusemap = texture( _MainTex, i.uv1);
    lowp vec4 specmap = vec4( 1.0);
    lowp vec3 envmap = vec3( 0.0);
    lowp vec3 specular = vec3( 0.0);
    #line 440
    lowp vec3 diffuse = vec3( 0.0);
    lowp vec3 emissive = vec3( 0.0);
    #line 444
    diffuse = DecodeLightmapHalf( texture( unity_Lightmap, i.lmUV));
    #line 448
    lowp float envMapScale = _Reflectivity;
    envMapScale *= 0.5;
    #line 453
    envmap = (texture( _ThemedCube, i.reflectDirWS).xyz * envMapScale);
    #line 457
    specmap = texture( _SpecMap, i.uv1);
    lowp vec3 colour = (((vec3( diffusemap) + (specular * specmap.xyz)) * diffuse) + (envmap * vec3( specmap)));
    #line 462
    colour = ((colour * i.fog.w) + i.fog.xyz);
    colour += emissive;
    #line 467
    return vec4( colour, 1.0);
}
in mediump vec2 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec3 xlv_TEXCOORD4;
in lowp vec4 xlv_TEXCOORD5;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD0);
    xlt_i.lmUV = vec2(xlv_TEXCOORD1);
    xlt_i.reflectDirWS = vec3(xlv_TEXCOORD4);
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
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp vec4 _FogRange;
uniform highp vec3 _FogParams;
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _DepthBand;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _WorldSpaceCameraPos;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  highp vec4 fog_2;
  mediump vec2 tmpvar_3;
  mediump vec2 tmpvar_4;
  mediump vec3 tmpvar_5;
  lowp vec4 tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7.w = 1.0;
  tmpvar_7.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_8;
  tmpvar_8 = (glstate_matrix_mvp * tmpvar_7);
  highp vec4 tmpvar_9;
  tmpvar_9.x = tmpvar_8.x;
  tmpvar_9.y = tmpvar_8.y;
  tmpvar_9.z = ((tmpvar_8.z * _DepthBand.z) + (tmpvar_8.w * _DepthBand.y));
  tmpvar_9.w = tmpvar_8.w;
  highp vec2 tmpvar_10;
  tmpvar_10 = _glesMultiTexCoord0.xy;
  tmpvar_3 = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  tmpvar_4 = tmpvar_11;
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
  tmpvar_5 = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = clamp (((tmpvar_9.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
  highp vec4 tmpvar_17;
  tmpvar_17.xyz = (_FogParams - (_FogParams * tmpvar_16));
  tmpvar_17.w = tmpvar_16;
  fog_2.xyz = tmpvar_17.xyz;
  fog_2.w = (tmpvar_16 * 2.0);
  tmpvar_6 = fog_2;
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD4 = tmpvar_5;
  xlv_TEXCOORD5 = tmpvar_6;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec3 xlv_TEXCOORD4;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D unity_Lightmap;
uniform lowp float _Reflectivity;
uniform samplerCube _ThemedCube;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = ((((texture2D (_MainTex, xlv_TEXCOORD0).xyz * texture2D (unity_Lightmap, xlv_TEXCOORD1).xyz) + (textureCube (_ThemedCube, xlv_TEXCOORD4).xyz * (_Reflectivity * 0.5))) * xlv_TEXCOORD5.w) + xlv_TEXCOORD5.xyz);
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
#line 389
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    mediump vec2 lmUV;
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
#line 377
#line 382
uniform sampler2D _MainTex;
uniform samplerCube _ThemedCube;
uniform lowp float _Reflectivity;
uniform sampler2D unity_Lightmap;
#line 386
uniform highp vec4 unity_LightmapST;
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
    o.reflectDirWS = reflectDirWS;
    #line 418
    highp float dist = o.pos.z;
    highp float visibility;
    #line 422
    visibility = clamp( ((dist * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
    highp vec4 fog = vec4( (_FogParams.xyz - (_FogParams.xyz * visibility)), visibility);
    #line 427
    fog.w *= 2.0;
    o.fog = fog;
    #line 431
    return o;
}

out mediump vec2 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
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
    xlv_TEXCOORD1 = vec2(xl_retval.lmUV);
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
#line 389
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    mediump vec2 lmUV;
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
#line 377
#line 382
uniform sampler2D _MainTex;
uniform samplerCube _ThemedCube;
uniform lowp float _Reflectivity;
uniform sampler2D unity_Lightmap;
#line 386
uniform highp vec4 unity_LightmapST;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 398
#line 348
lowp vec3 DecodeLightmapHalf( in lowp vec4 color ) {
    return ((4.0 * color.w) * color.xyz);
}
#line 433
lowp vec4 frag( in v2f i ) {
    #line 435
    lowp vec4 diffusemap = texture( _MainTex, i.uv1);
    lowp vec4 specmap = vec4( 1.0);
    lowp vec3 envmap = vec3( 0.0);
    lowp vec3 specular = vec3( 0.0);
    #line 439
    lowp vec3 diffuse = vec3( 0.0);
    lowp vec3 emissive = vec3( 0.0);
    #line 443
    diffuse = DecodeLightmapHalf( texture( unity_Lightmap, i.lmUV));
    #line 447
    lowp float envMapScale = _Reflectivity;
    envMapScale *= 0.5;
    #line 452
    envmap = (texture( _ThemedCube, i.reflectDirWS).xyz * envMapScale);
    lowp vec3 colour = (((vec3( diffusemap) + (specular * specmap.xyz)) * diffuse) + (envmap * vec3( specmap)));
    #line 457
    colour = ((colour * i.fog.w) + i.fog.xyz);
    colour += emissive;
    #line 462
    return vec4( colour, 1.0);
}
in mediump vec2 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec3 xlv_TEXCOORD4;
in lowp vec4 xlv_TEXCOORD5;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD0);
    xlt_i.lmUV = vec2(xlv_TEXCOORD1);
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