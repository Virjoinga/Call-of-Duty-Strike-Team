Shader "Custom/ParticleSmokeAlphaFog"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
        _FogRangeFog ("Fog Range", Range(0,1)) = 0.2
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            BindChannels {
            Bind "vertex", Vertex
            Bind "color", Color
            Bind "texcoord", TexCoord
            }
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "CoronaLegacy.cginc"

#pragma multi_compile_particles
#if defined(SOFTPARTICLES_ON)
float4 _MainTex_ST;
float _FogRangeFog;
float4 _FogRange;
float3 _FogParams;
float3 _DepthBand;
float _InvFade;
sampler2D _CameraDepthTexture;
float4 _TintColor;
sampler2D _MainTex;
struct appdata_t {
    float4 texcoord0 : TEXCOORD0;
    float4 color : COLOR;
    float4 vertex : POSITION;
};
struct v2f {
    float4 vertex : SV_POSITION;
    float4 texcoord2 : TEXCOORD2;
    float4 texcoord1 : TEXCOORD1;
    float2 texcoord0 : TEXCOORD0;
    float4 color : COLOR;
};
v2f vert(appdata_t v) {
    v2f o;

  float4 tmpvar_1;
  float4 tmpvar_2;
  float4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = v.vertex.xyz;
  float4 tmpvar_4;
  tmpvar_4 = CoronaObjectToClipPos(tmpvar_3);
  float4 tmpvar_5;
  tmpvar_5.x = tmpvar_4.x;
  tmpvar_5.y = tmpvar_4.y;
  tmpvar_5.z = ((tmpvar_4.z * _DepthBand.z) + (tmpvar_4.w * _DepthBand.y));
  tmpvar_5.w = tmpvar_4.w;
  float4 o_6;
  float4 tmpvar_7;
  tmpvar_7 = (tmpvar_5 * 0.5);
  float2 tmpvar_8;
  tmpvar_8.x = tmpvar_7.x;
  tmpvar_8.y = (tmpvar_7.y * _ProjectionParams.x);
  o_6.xy = (tmpvar_8 + tmpvar_7.w);
  o_6.zw = tmpvar_5.zw;
  tmpvar_1.xyw = o_6.xyw;
  tmpvar_1.z = -(mul(UNITY_MATRIX_MV, v.vertex).z);
  float tmpvar_9;
  tmpvar_9 = clamp (((tmpvar_5.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRangeFog, 1.0);
  float4 tmpvar_10;
  tmpvar_10.xyz = (_FogParams - (_FogParams * tmpvar_9));
  tmpvar_10.w = tmpvar_9;
  tmpvar_2 = tmpvar_10;
  o.vertex = CoronaClipToNative(tmpvar_5);
  o.color = v.color;
  o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  o.texcoord1 = tmpvar_1;
  o.texcoord2 = tmpvar_2;

    return o;
}
float4 frag(v2f i) : SV_Target {

  float4 tmpvar_1;
  tmpvar_1.xyz = i.color.xyz;
  float4 colour_2;
  float4 tmpvar_3;
  tmpvar_3 = tex2Dproj (_CameraDepthTexture, i.texcoord1);
  float z_4;
  z_4 = tmpvar_3.x;
  float tmpvar_5;
  tmpvar_5 = (i.color.w * clamp ((_InvFade * ((1.0/(((_ZBufferParams.z * z_4) + _ZBufferParams.w))) - i.texcoord1.z)), 0.0, 1.0));
  tmpvar_1.w = tmpvar_5;
  float4 tmpvar_6;
  tmpvar_6 = (((2.0 * tmpvar_1) * _TintColor) * tex2D (_MainTex, i.texcoord0));
  colour_2.w = tmpvar_6.w;
  colour_2.xyz = ((tmpvar_6.xyz * i.texcoord2.w) + i.texcoord2.xyz);
  return colour_2;

}
#else
float4 _MainTex_ST;
float _FogRangeFog;
float4 _FogRange;
float3 _FogParams;
float3 _DepthBand;
float4 _TintColor;
sampler2D _MainTex;
struct appdata_t {
    float4 texcoord0 : TEXCOORD0;
    float4 color : COLOR;
    float4 vertex : POSITION;
};
struct v2f {
    float4 vertex : SV_POSITION;
    float4 texcoord2 : TEXCOORD2;
    float2 texcoord0 : TEXCOORD0;
    float4 color : COLOR;
};
v2f vert(appdata_t v) {
    v2f o;

  float4 tmpvar_1;
  float4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = v.vertex.xyz;
  float4 tmpvar_3;
  tmpvar_3 = CoronaObjectToClipPos(tmpvar_2);
  float4 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = tmpvar_3.y;
  tmpvar_4.z = ((tmpvar_3.z * _DepthBand.z) + (tmpvar_3.w * _DepthBand.y));
  tmpvar_4.w = tmpvar_3.w;
  float tmpvar_5;
  tmpvar_5 = clamp (((tmpvar_4.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRangeFog, 1.0);
  float4 tmpvar_6;
  tmpvar_6.xyz = (_FogParams - (_FogParams * tmpvar_5));
  tmpvar_6.w = tmpvar_5;
  tmpvar_1 = tmpvar_6;
  o.vertex = CoronaClipToNative(tmpvar_4);
  o.color = v.color;
  o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  o.texcoord2 = tmpvar_1;

    return o;
}
float4 frag(v2f i) : SV_Target {

  float4 colour_1;
  float4 tmpvar_2;
  tmpvar_2 = (((2.0 * i.color) * _TintColor) * tex2D (_MainTex, i.texcoord0));
  colour_1.w = tmpvar_2.w;
  colour_1.xyz = ((tmpvar_2.xyz * i.texcoord2.w) + i.texcoord2.xyz);
  return colour_1;

}
#endif

ENDCG
        }
    }

}
