ÐVShader "HeatDistort" {
Properties {
 _BumpAmt ("Distortion", Range(0,128)) = 10
 _MainTex ("Tint Color (RGB)", 2D) = "white" {}
 _BumpMap ("Normalmap", 2D) = "bump" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+100" "RenderType"="Opaque" }
 GrabPass {
  Name "BASE"
  Tags { "LIGHTMODE"="Always" }
 }
 Pass {
  Name "BASE"
  Tags { "LIGHTMODE"="Always" "QUEUE"="Transparent+100" "RenderType"="Opaque" }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

varying highp vec2 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  highp vec4 tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_2.xy = ((tmpvar_3.xy + tmpvar_3.w) * 0.5);
  tmpvar_2.zw = tmpvar_3.zw;
  highp vec4 tmpvar_4;
  tmpvar_4.zw = vec2(0.0, 0.0);
  tmpvar_4.x = tmpvar_1.x;
  tmpvar_4.y = tmpvar_1.y;
  highp vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.x = tmpvar_1.x;
  tmpvar_5.y = tmpvar_1.y;
  gl_Position = tmpvar_3;
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = (glstate_matrix_texture1 * tmpvar_4).xy;
  xlv_TEXCOORD2 = (glstate_matrix_texture2 * tmpvar_5).xy;
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD2;
varying highp vec2 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD0;
uniform highp float _BumpAmt;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
uniform highp vec4 _GrabTexture_TexelSize;
uniform sampler2D _GrabTexture;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1.zw = xlv_TEXCOORD0.zw;
  mediump vec4 tint_2;
  mediump vec4 col_3;
  mediump vec2 bump_4;
  lowp vec2 tmpvar_5;
  tmpvar_5 = ((texture2D (_BumpMap, xlv_TEXCOORD1).xyz * 2.0) - 1.0).xy;
  bump_4 = tmpvar_5;
  tmpvar_1.xy = ((((bump_4 * _BumpAmt) * _GrabTexture_TexelSize.xy) * xlv_TEXCOORD0.z) + xlv_TEXCOORD0.xy);
  highp vec4 tmpvar_6;
  tmpvar_6.w = 0.0;
  tmpvar_6.xyz = tmpvar_1.xyw;
  lowp vec4 tmpvar_7;
  tmpvar_7 = texture2DProj (_GrabTexture, tmpvar_6);
  col_3 = tmpvar_7;
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, xlv_TEXCOORD2);
  tint_2 = tmpvar_8;
  gl_FragData[0] = (col_3 * tint_2);
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
#line 319
struct v2f {
    highp vec4 vertex;
    highp vec4 uvgrab;
    highp vec2 uvbump;
    highp vec2 uvmain;
};
#line 337
struct appdata_t {
    highp vec4 vertex;
    highp vec2 texcoord;
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
uniform sampler2D _GrabTexture;
uniform highp vec4 _GrabTexture_TexelSize;
uniform sampler2D _BumpMap;
uniform sampler2D _MainTex;
#line 327
uniform highp float _BumpAmt;
#line 343
#line 193
highp vec2 MultiplyUV( in highp mat4 mat, in highp vec2 inUV ) {
    highp vec4 temp = vec4( inUV.x, inUV.y, 0.0, 0.0);
    temp = (mat * temp);
    #line 197
    return temp.xy;
}
#line 343
v2f vert( in appdata_t v ) {
    v2f o;
    o.vertex = (glstate_matrix_mvp * v.vertex);
    #line 347
    highp float scale = 1.0;
    o.uvgrab.xy = ((vec2( o.vertex.x, (o.vertex.y * scale)) + o.vertex.w) * 0.5);
    o.uvgrab.zw = o.vertex.zw;
    o.uvbump = MultiplyUV( glstate_matrix_texture1, v.texcoord);
    #line 351
    o.uvmain = MultiplyUV( glstate_matrix_texture2, v.texcoord);
    return o;
}
out highp vec4 xlv_TEXCOORD0;
out highp vec2 xlv_TEXCOORD1;
out highp vec2 xlv_TEXCOORD2;
void main() {
    v2f xl_retval;
    appdata_t xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.texcoord = vec2(gl_MultiTexCoord0);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.vertex);
    xlv_TEXCOORD0 = vec4(xl_retval.uvgrab);
    xlv_TEXCOORD1 = vec2(xl_retval.uvbump);
    xlv_TEXCOORD2 = vec2(xl_retval.uvmain);
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
#line 319
struct v2f {
    highp vec4 vertex;
    highp vec4 uvgrab;
    highp vec2 uvbump;
    highp vec2 uvmain;
};
#line 337
struct appdata_t {
    highp vec4 vertex;
    highp vec2 texcoord;
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
uniform sampler2D _GrabTexture;
uniform highp vec4 _GrabTexture_TexelSize;
uniform sampler2D _BumpMap;
uniform sampler2D _MainTex;
#line 327
uniform highp float _BumpAmt;
#line 343
#line 272
lowp vec3 UnpackNormal( in lowp vec4 packednormal ) {
    #line 274
    return ((packednormal.xyz * 2.0) - 1.0);
}
#line 328
mediump vec4 frag( in v2f i ) {
    mediump vec2 bump = UnpackNormal( texture( _BumpMap, i.uvbump)).xy;
    #line 331
    highp vec2 offset = ((bump * _BumpAmt) * _GrabTexture_TexelSize.xy);
    i.uvgrab.xy = ((offset * i.uvgrab.z) + i.uvgrab.xy);
    mediump vec4 col = textureProj( _GrabTexture, vec4( i.uvgrab.xyw, 0.0));
    mediump vec4 tint = texture( _MainTex, i.uvmain);
    #line 335
    return (col * tint);
}
in highp vec4 xlv_TEXCOORD0;
in highp vec2 xlv_TEXCOORD1;
in highp vec2 xlv_TEXCOORD2;
void main() {
    mediump vec4 xl_retval;
    v2f xlt_i;
    xlt_i.vertex = vec4(0.0);
    xlt_i.uvgrab = vec4(xlv_TEXCOORD0);
    xlt_i.uvbump = vec2(xlv_TEXCOORD1);
    xlt_i.uvmain = vec2(xlv_TEXCOORD2);
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
SubShader { 
 Tags { "QUEUE"="Transparent+100" "RenderType"="Opaque" }
 Pass {
  Name "BASE"
  Tags { "QUEUE"="Transparent+100" "RenderType"="Opaque" }
  Blend DstColor Zero
  SetTexture [_MainTex] { combine texture }
 }
}
}