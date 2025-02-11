ÅShader "Color Space/YCrCbtoRGB" {
Properties {
 _YTex ("Y (RGB)", 2D) = "white" {}
 _CbTex ("Cb (RGB)", 2D) = "white" {}
 _CrTex ("Cr (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Fog {
   Color (0,0,0,0)
  }
  ColorMask RGB
Program "vp" {
SubProgram "gles " {
"!!GLES
#define SHADER_API_GLES 1
#define tex2D texture2D


#ifdef VERTEX
#define gl_ModelViewProjectionMatrix glstate_matrix_mvp
uniform mat4 glstate_matrix_mvp;

varying highp vec2 xlv_TEXCOORD0;

uniform highp vec4 _YTex_ST;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesVertex;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _YTex_ST.xy) + _YTex_ST.zw);
}



#endif
#ifdef FRAGMENT

varying highp vec2 xlv_TEXCOORD0;
uniform sampler2D _YTex;
uniform sampler2D _CrTex;
uniform sampler2D _CbTex;
void main ()
{
  mediump vec4 rgbVec;
  mediump vec4 yuvVec;
  lowp vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.x = texture2D (_YTex, xlv_TEXCOORD0).x;
  tmpvar_1.y = texture2D (_CbTex, xlv_TEXCOORD0).y;
  tmpvar_1.z = texture2D (_CrTex, xlv_TEXCOORD0).z;
  yuvVec = tmpvar_1;
  rgbVec.x = dot (vec4(1.16438, 0.0, 1.59603, -0.870785), yuvVec);
  rgbVec.y = dot (vec4(1.16438, -0.391762, -0.812969, 0.529594), yuvVec);
  rgbVec.z = dot (vec4(1.16438, 2.01723, 0.0, -1.08139), yuvVec);
  rgbVec.w = 1.0;
  gl_FragData[0] = rgbVec;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
"!!GLES"
}
}
 }
}
Fallback "VertexLit"
}