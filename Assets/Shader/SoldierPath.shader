Shader "Corona/Effects/SoldierPath"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZTest Greater
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "CoronaLegacy.cginc"
            float3 _DepthBand;
            float _StartProp;
            float _EndProp;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };
            v2f vert(appdata_t v)
            {
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
                  o.texcoord0 = v.texcoord0.xy;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                  float2 tmpvar_1;
                  tmpvar_1.y = i.texcoord0.y;
                  float4 tx_2;
                  tmpvar_1.x = min (0.5, min ((i.texcoord0.x * _StartProp), ((1.0 - i.texcoord0.x) * _EndProp)));
                  if ((i.texcoord0.x > 0.5)) {
                  tmpvar_1.x = (1.0 - tmpvar_1.x);
                  };
                  float4 tmpvar_3;
                  tmpvar_3 = tex2D (_MainTex, tmpvar_1);
                  tx_2 = tmpvar_3;
                  return ((i.color * tx_2) * 0.2);
            }
            ENDCG
        }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZTest Less
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "CoronaLegacy.cginc"
            float3 _DepthBand;
            float _StartProp;
            float _EndProp;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };
            v2f vert(appdata_t v)
            {
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
                  o.texcoord0 = v.texcoord0.xy;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                  float2 tmpvar_1;
                  tmpvar_1.y = i.texcoord0.y;
                  float4 tx_2;
                  tmpvar_1.x = min (0.5, min ((i.texcoord0.x * _StartProp), ((1.0 - i.texcoord0.x) * _EndProp)));
                  if ((i.texcoord0.x > 0.5)) {
                  tmpvar_1.x = (1.0 - tmpvar_1.x);
                  };
                  float4 tmpvar_3;
                  tmpvar_3 = tex2D (_MainTex, tmpvar_1);
                  tx_2 = tmpvar_3;
                  return (i.color * tx_2);
            }
            ENDCG
        }
    }
    Fallback Off
}
