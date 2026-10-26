Shader "Corona/Effects/LaserScanLine"
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZTest Always
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
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 color : COLOR;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 color : COLOR;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                  float2 tmpvar_1;
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
                  float2 tmpvar_5;
                  tmpvar_5 = (mul(unity_ObjectToWorld, v.vertex).xz * 0.1);
                  tmpvar_1 = tmpvar_5;
                  tmpvar_1.y = (tmpvar_1.y - _Scroll);
                  o.vertex = CoronaClipToNative(tmpvar_4);
                  o.texcoord0 = tmpvar_1;
                  o.color = v.color;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                  return (tex2D (_MainTex, i.texcoord0) * i.color);
            }
            ENDCG
        }
    }
    Fallback Off
}
