Shader "Corona/Effects/LaserScanLineEdge"
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
        _DistanceMap ("DistMap", 2D) = "white" {}
        _UVInc ("uvinc", Range(0,1)) = 10
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            Cull Off
            Fog {
            Mode Exp2
            }
            Blend SrcAlpha One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "../../Shader/CoronaLegacy.cginc"
            float _Scroll;
            float3 _DepthBand;
            // float4x4 _Object2World;
            float _UVInc;
            float _ScanScale;
            sampler2D _DistanceMap;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 color : COLOR;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                  float2 tmpvar_1;
                  float2 tmpvar_2;
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
                  float2 tmpvar_6;
                  tmpvar_6 = (mul(unity_ObjectToWorld, v.vertex).xz * 0.1);
                  tmpvar_1 = tmpvar_6;
                  tmpvar_1.y = (tmpvar_1.y - _Scroll);
                  float2 tmpvar_7;
                  tmpvar_7 = v.texcoord0.xy;
                  tmpvar_2 = tmpvar_7;
                  o.vertex = CoronaClipToNative(tmpvar_5);
                  o.texcoord0 = tmpvar_1;
                  o.texcoord1 = tmpvar_2;
                  o.color = v.color;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                  float2 uv_1;
                  float4 col_2;
                  col_2 = (tex2D (_MainTex, i.texcoord0) * i.color);
                  uv_1.y = 0.0;
                  uv_1.x = (i.texcoord1.x / lerp (_ScanScale, 1.0, i.texcoord1.y));
                  uv_1.x = (uv_1.x + _UVInc);
                  float4 tmpvar_3;
                  tmpvar_3 = tex2D (_DistanceMap, uv_1);
                  if ((tmpvar_3.w < i.texcoord1.y)) {
                  col_2.w = 0.0;
                  };
                  return col_2;
            }
            ENDCG
        }
    }
    Fallback Off
}
