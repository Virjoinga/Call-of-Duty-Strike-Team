Shader "Custom/ParticleSmokeAlpha"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
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
    float4 texcoord1 : TEXCOORD1;
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
  float4 o_5;
  float4 tmpvar_6;
  tmpvar_6 = (tmpvar_4 * 0.5);
  float2 tmpvar_7;
  tmpvar_7.x = tmpvar_6.x;
  tmpvar_7.y = (tmpvar_6.y * _ProjectionParams.x);
  o_5.xy = (tmpvar_7 + tmpvar_6.w);
  o_5.zw = tmpvar_4.zw;
  tmpvar_1.xyw = o_5.xyw;
  tmpvar_1.z = -(mul(UNITY_MATRIX_MV, v.vertex).z);
  o.vertex = CoronaClipToNative(tmpvar_4);
  o.color = v.color;
  o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  o.texcoord1 = tmpvar_1;

    return o;
}
float4 frag(v2f i) : SV_Target {

  float4 tmpvar_1;
  tmpvar_1.xyz = i.color.xyz;
  float4 tmpvar_2;
  tmpvar_2 = tex2Dproj (_CameraDepthTexture, i.texcoord1);
  float z_3;
  z_3 = tmpvar_2.x;
  float tmpvar_4;
  tmpvar_4 = (i.color.w * clamp ((_InvFade * ((1.0/(((_ZBufferParams.z * z_3) + _ZBufferParams.w))) - i.texcoord1.z)), 0.0, 1.0));
  tmpvar_1.w = tmpvar_4;
  return (((2.0 * tmpvar_1) * _TintColor) * tex2D (_MainTex, i.texcoord0));

}
#else
float4 _MainTex_ST;
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
    float2 texcoord0 : TEXCOORD0;
    float4 color : COLOR;
};
v2f vert(appdata_t v) {
    v2f o;

  float4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = v.vertex.xyz;
  float4 tmpvar_2;
  tmpvar_2 = CoronaObjectToClipPos(tmpvar_1);
  float4 tmpvar_3;
  tmpvar_3.x = tmpvar_2.x;
  tmpvar_3.y = tmpvar_2.y;
  tmpvar_3.z = ((tmpvar_2.z * _DepthBand.z) + (tmpvar_2.w * _DepthBand.y));
  tmpvar_3.w = tmpvar_2.w;
  o.vertex = CoronaClipToNative(tmpvar_3);
  o.color = v.color;
  o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);

    return o;
}
float4 frag(v2f i) : SV_Target {

  return (((2.0 * i.color) * _TintColor) * tex2D (_MainTex, i.texcoord0));

}
#endif

ENDCG
        }
    }

}
