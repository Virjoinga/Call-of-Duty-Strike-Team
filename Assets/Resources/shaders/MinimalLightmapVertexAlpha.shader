Shader "Hidden/MinimalLightmapVertexAlpha"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent-1" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent-1" "RenderType"="Transparent" }
            Fog {
            Mode Exp2
            }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "../../Shader/CoronaLegacy.cginc"
            float3 _DepthBand;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float texcoord2 : TEXCOORD2;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                  float2 tmpvar_1;
                  float2 tmpvar_2;
                  float tmpvar_3;
                  float4 tmpvar_4;
                  tmpvar_4.w = 1.0;
                  tmpvar_4.xyz = v.vertex.xyz;
                  float4 tmpvar_5;
                  tmpvar_5 = CoronaObjectToClipPos(tmpvar_4);
                  float4 tmpvar_6;
                  tmpvar_6.x = tmpvar_5.x;
                  tmpvar_6.y = tmpvar_5.y;
                  tmpvar_6.z = ((tmpvar_5.z * _DepthBand.z) + (tmpvar_5.w * _DepthBand.y));
                  tmpvar_6.w = tmpvar_5.w;
                  float2 tmpvar_7;
                  tmpvar_7 = v.texcoord0.xy;
                  tmpvar_1 = tmpvar_7;
                  float2 tmpvar_8;
                  tmpvar_8 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                  tmpvar_2 = tmpvar_8;
                  float tmpvar_9;
                  tmpvar_9 = v.color.w;
                  tmpvar_3 = tmpvar_9;
                  o.vertex = CoronaClipToNative(tmpvar_6);
                  o.texcoord0 = tmpvar_1;
                  o.texcoord1 = tmpvar_2;
                  o.texcoord2 = tmpvar_3;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                  float4 tmpvar_1;
                  float3 tmpvar_2;
                  tmpvar_2 = (2.0 * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.texcoord1).xyz);
                  float4 tmpvar_3;
                  tmpvar_3 = tex2D (_MainTex, i.texcoord0);
                  float4 tmpvar_4;
                  tmpvar_4.xyz = (tmpvar_3.xyz * tmpvar_2);
                  tmpvar_4.w = i.texcoord2;
                  tmpvar_1 = tmpvar_4;
                  return tmpvar_1;
            }
            ENDCG
        }
    }
    Fallback Off
}
