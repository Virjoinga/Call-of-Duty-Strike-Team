’lShader "Corona/Effects/MonitorScreen" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _NoiseTex ("Noise Texture", 2D) = "white" {}
 _Brightness ("Brightness", Range(0,2)) = 2
 _PhaseLineTex ("Phase Line Texture", 2D) = "black" {}
 _PhaseLineFrequency ("Phase line frequency", Range(0,1)) = 0.1
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp vec2 xlv_TEXCOORD6;
varying mediump vec2 xlv_TEXCOORD7;
varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec2 xlv_TEXCOORD0;
uniform highp vec4 _FogRange;
uniform highp vec3 _FogParams;
uniform highp vec3 _DepthBand;
uniform highp float _PhaseLineFrequency;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _Time;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  mediump vec2 tmpvar_1;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec2 tmpvar_4;
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
  tmpvar_1 = tmpvar_8;
  highp float tmpvar_9;
  tmpvar_9 = clamp (((tmpvar_7.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
  highp vec4 tmpvar_10;
  tmpvar_10.xyz = (_FogParams - (_FogParams * tmpvar_9));
  tmpvar_10.w = tmpvar_9;
  tmpvar_2 = tmpvar_10;
  highp float tmpvar_11;
  tmpvar_11 = _glesMultiTexCoord0.x;
  tmpvar_3.x = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = (_glesMultiTexCoord0.y + fract((135.914 * _Time.y)));
  tmpvar_3.y = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = vec2(((_glesMultiTexCoord0.y + fract((_Time.y * _PhaseLineFrequency))) * 4.0));
  tmpvar_4 = tmpvar_13;
  gl_Position = tmpvar_7;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD5 = tmpvar_2;
  xlv_TEXCOORD7 = tmpvar_3;
  xlv_TEXCOORD6 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

varying lowp vec2 xlv_TEXCOORD6;
varying mediump vec2 xlv_TEXCOORD7;
varying lowp vec4 xlv_TEXCOORD5;
varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D _MainTex;
uniform lowp float _Brightness;
uniform sampler2D _PhaseLineTex;
uniform sampler2D _NoiseTex;
void main ()
{
  lowp vec4 diffusemap_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  diffusemap_1.w = tmpvar_2.w;
  diffusemap_1.xyz = (tmpvar_2.xyz + texture2D (_PhaseLineTex, xlv_TEXCOORD6).xyz);
  diffusemap_1.xyz = (diffusemap_1.xyz * (texture2D (_NoiseTex, xlv_TEXCOORD7).xyz * _Brightness));
  lowp vec4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = ((diffusemap_1.xyz * xlv_TEXCOORD5.w) + xlv_TEXCOORD5.xyz);
  gl_FragData[0] = tmpvar_3;
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

#line 155
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 191
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 185
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 389
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    lowp vec4 fog;
    mediump vec2 uv2;
    lowp vec2 phaseLine;
};
#line 71
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
uniform lowp float _Brightness;
#line 55
uniform lowp vec4 unity_ColorSpaceGrey;
#line 81
#line 86
#line 91
#line 95
#line 100
#line 124
#line 141
#line 162
#line 170
#line 197
#line 210
#line 219
#line 224
#line 233
#line 238
#line 247
#line 264
#line 269
#line 295
#line 303
#line 311
#line 315
#line 319
uniform lowp vec3 _SpecDir;
uniform mediump float _SpecPower;
uniform highp vec3 _DepthBand;
#line 352
#line 356
#line 381
#line 386
uniform sampler2D _MainTex;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 398
#line 322
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 324
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 336
highp vec3 ObjectToWorldDir( in highp vec3 dir ) {
    #line 338
    dir = (_Object2World * vec4( dir, 0.0)).xyz;
    return dir;
}
#line 327
highp vec3 WorldToObjectDir( in highp vec3 dir ) {
    #line 329
    return (_World2Object * vec4( dir, 0.0)).xyz;
}
#line 331
highp vec3 WorldToObjectPos( in highp vec3 pos ) {
    #line 333
    pos = (_World2Object * vec4( pos, 1.0)).xyz;
    return pos;
}
#line 398
v2f vert( in appdata_full v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 402
    o.uv1 = v.texcoord.xy;
    highp vec3 lightDir = WorldToObjectDir( _SpecDir.xyz);
    highp vec3 camPos = WorldToObjectPos( _WorldSpaceCameraPos.xyz);
    highp vec3 camDir = (camPos - v.vertex.xyz);
    #line 406
    highp vec3 normalWS = normalize(ObjectToWorldDir( v.normal));
    highp vec3 reflectDirWS = (_Object2World * vec4( reflect( (-camDir), v.normal.xyz), 0.0)).xyz;
    #line 410
    highp float dist = o.pos.z;
    highp float visibility;
    #line 414
    visibility = clamp( ((dist * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
    highp vec4 fog = vec4( (_FogParams.xyz - (_FogParams.xyz * visibility)), visibility);
    o.fog = fog;
    #line 421
    o.uv2.x = v.texcoord.x;
    o.uv2.y = (v.texcoord.y + fract((135.914 * _Time.y)));
    o.phaseLine = vec2( ((v.texcoord.y + fract((_Time.y * _PhaseLineFrequency))) * 4.0));
    #line 426
    return o;
}

out mediump vec2 xlv_TEXCOORD0;
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
    xlv_TEXCOORD5 = vec4(xl_retval.fog);
    xlv_TEXCOORD7 = vec2(xl_retval.uv2);
    xlv_TEXCOORD6 = vec2(xl_retval.phaseLine);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];

#line 155
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 191
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 185
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 389
struct v2f {
    highp vec4 pos;
    mediump vec2 uv1;
    lowp vec4 fog;
    mediump vec2 uv2;
    lowp vec2 phaseLine;
};
#line 71
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
uniform lowp float _Brightness;
#line 55
uniform lowp vec4 unity_ColorSpaceGrey;
#line 81
#line 86
#line 91
#line 95
#line 100
#line 124
#line 141
#line 162
#line 170
#line 197
#line 210
#line 219
#line 224
#line 233
#line 238
#line 247
#line 264
#line 269
#line 295
#line 303
#line 311
#line 315
#line 319
uniform lowp vec3 _SpecDir;
uniform mediump float _SpecPower;
uniform highp vec3 _DepthBand;
#line 352
#line 356
#line 381
#line 386
uniform sampler2D _MainTex;
uniform highp vec3 _FogParams;
uniform highp vec4 _FogRange;
#line 398
#line 428
lowp vec4 frag( in v2f i ) {
    #line 430
    lowp vec4 diffusemap = texture( _MainTex, i.uv1);
    lowp vec4 specmap = vec4( 1.0);
    lowp vec3 envmap = vec3( 0.0);
    lowp vec3 specular = vec3( 0.0);
    #line 434
    lowp vec3 diffuse = vec3( 0.0);
    lowp vec3 emissive = vec3( 0.0);
    #line 438
    diffuse = vec3( 1.0);
    #line 442
    diffusemap.xyz += texture( _PhaseLineTex, i.phaseLine).xyz;
    diffusemap.xyz *= (texture( _NoiseTex, i.uv2).xyz * _Brightness);
    diffuse = vec3( 1.0);
    #line 447
    lowp vec3 colour = (((vec3( diffusemap) + (specular * specmap.xyz)) * diffuse) + (envmap * vec3( specmap)));
    colour = ((colour * i.fog.w) + i.fog.xyz);
    #line 452
    colour += emissive;
    return vec4( colour, 1.0);
}
in mediump vec2 xlv_TEXCOORD0;
in lowp vec4 xlv_TEXCOORD5;
in mediump vec2 xlv_TEXCOORD7;
in lowp vec2 xlv_TEXCOORD6;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD0);
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