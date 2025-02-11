ÒVShader "Corona/Effects/LaserScanLineEdge" {
Properties {
 _MainTex ("Base", 2D) = "white" {}
 _DistanceMap ("DistMap", 2D) = "white" {}
 _UVInc ("uvinc", Range(0,1)) = 10
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
  Cull Off
  Fog {
   Mode Exp2
  }
  Blend SrcAlpha One
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying lowp vec4 xlv_COLOR;
varying lowp vec2 xlv_TEXCOORD1;
varying lowp vec2 xlv_TEXCOORD0;
uniform lowp float _Scroll;
uniform highp vec3 _DepthBand;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  lowp vec2 tmpvar_1;
  lowp vec2 tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = _glesVertex.xyz;
  highp vec4 tmpvar_4;
  tmpvar_4 = (glstate_matrix_mvp * tmpvar_3);
  highp vec4 tmpvar_5;
  tmpvar_5.x = tmpvar_4.x;
  tmpvar_5.y = tmpvar_4.y;
  tmpvar_5.z = ((tmpvar_4.z * _DepthBand.z) + (tmpvar_4.w * _DepthBand.y));
  tmpvar_5.w = tmpvar_4.w;
  highp vec2 tmpvar_6;
  tmpvar_6 = ((_Object2World * _glesVertex).xz * 0.1);
  tmpvar_1 = tmpvar_6;
  tmpvar_1.y = (tmpvar_1.y - _Scroll);
  highp vec2 tmpvar_7;
  tmpvar_7 = _glesMultiTexCoord0.xy;
  tmpvar_2 = tmpvar_7;
  gl_Position = tmpvar_5;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_COLOR = _glesColor;
}



#endif
#ifdef FRAGMENT

varying lowp vec4 xlv_COLOR;
varying lowp vec2 xlv_TEXCOORD1;
varying lowp vec2 xlv_TEXCOORD0;
uniform lowp float _UVInc;
uniform lowp float _ScanScale;
uniform sampler2D _DistanceMap;
uniform sampler2D _MainTex;
void main ()
{
  lowp vec2 uv_1;
  lowp vec4 col_2;
  col_2 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
  uv_1.y = 0.0;
  uv_1.x = (xlv_TEXCOORD1.x / mix (_ScanScale, 1.0, xlv_TEXCOORD1.y));
  uv_1.x = (uv_1.x + _UVInc);
  lowp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_DistanceMap, uv_1);
  if ((tmpvar_3.w < xlv_TEXCOORD1.y)) {
    col_2.w = 0.0;
  };
  gl_FragData[0] = col_2;
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
#line 387
struct v2f {
    highp vec4 pos;
    lowp vec2 uv1;
    lowp vec2 uv2;
    lowp vec4 col;
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
uniform sampler2D _DistanceMap;
uniform lowp float _Scroll;
uniform lowp float _ScanScale;
#line 386
uniform lowp float _UVInc;
#line 395
#line 318
highp vec4 ObjectToClipPos( in highp vec4 vertexPos ) {
    #line 320
    highp vec4 pos = (glstate_matrix_mvp * vec4( vertexPos.xyz, 1.0));
    return vec4( pos.x, pos.y, ((pos.z * _DepthBand.z) + (pos.w * _DepthBand.y)), pos.w);
}
#line 395
v2f vert( in appdata_full v ) {
    v2f o;
    o.pos = ObjectToClipPos( v.vertex);
    #line 399
    o.uv1.xy = ((_Object2World * v.vertex).xz * 0.1);
    o.uv1.y -= _Scroll;
    o.col = v.color;
    o.uv2 = v.texcoord.xy;
    #line 403
    return o;
}

out lowp vec2 xlv_TEXCOORD0;
out lowp vec2 xlv_TEXCOORD1;
out lowp vec4 xlv_COLOR;
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
    xlv_TEXCOORD1 = vec2(xl_retval.uv2);
    xlv_COLOR = vec4(xl_retval.col);
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
    lowp vec2 uv1;
    lowp vec2 uv2;
    lowp vec4 col;
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
uniform sampler2D _DistanceMap;
uniform lowp float _Scroll;
uniform lowp float _ScanScale;
#line 386
uniform lowp float _UVInc;
#line 395
#line 405
lowp vec4 frag( in v2f i ) {
    #line 407
    lowp vec4 col = (texture( _MainTex, i.uv1) * i.col);
    lowp vec2 uv = i.uv2;
    uv.y = 0.0;
    uv.x /= mix( _ScanScale, 1.0, i.uv2.y);
    #line 411
    uv.x += _UVInc;
    lowp vec4 dist = texture( _DistanceMap, uv);
    if ((dist.w < i.uv2.y)){
        #line 415
        col.w = 0.0;
    }
    return col;
}
in lowp vec2 xlv_TEXCOORD0;
in lowp vec2 xlv_TEXCOORD1;
in lowp vec4 xlv_COLOR;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.uv1 = vec2(xlv_TEXCOORD0);
    xlt_i.uv2 = vec2(xlv_TEXCOORD1);
    xlt_i.col = vec4(xlv_COLOR);
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