Shader "Hidden/MinimalLightmapReflective" {
    Properties {
        _Color ("Light Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _RefTex ("Reflection (RGB)", CUBE) = "white" {}
    }
    SubShader { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Tags { "RenderType"="Opaque" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float3 _DepthBand;

            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 uv2 : TEXCOORD2;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                float2 tmpvar_2;
                float3 tmpvar_3;
                float4 tmpvar_4;
                tmpvar_4.w = 1.0;
                tmpvar_4.xyz = v.vertex.xyz;
                float4 tmpvar_5;
                tmpvar_5 = (UnityObjectToClipPos(tmpvar_4));
                float4 tmpvar_6;
                tmpvar_6.x = tmpvar_5.x;
                tmpvar_6.y = tmpvar_5.y;
                tmpvar_6.z = ((tmpvar_5.z * _DepthBand.z) + (tmpvar_5.w * _DepthBand.y));
                tmpvar_6.w = tmpvar_5.w;
                float3x3 tmpvar_7;
                tmpvar_7[0] = unity_ObjectToWorld[0].xyz;
                tmpvar_7[1] = unity_ObjectToWorld[1].xyz;
                tmpvar_7[2] = unity_ObjectToWorld[2].xyz;
                float3 tmpvar_8;
                tmpvar_8 = mul(tmpvar_7, normalize(v.normal));
                float2 tmpvar_9;
                tmpvar_9 = v.uv.xy;
                tmpvar_1 = tmpvar_9;
                float2 tmpvar_10;
                tmpvar_10 = ((v.uv1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_2 = tmpvar_10;
                tmpvar_3 = tmpvar_8;
                o.pos = tmpvar_6;
                o.uv = tmpvar_1;
                o.uv1 = tmpvar_2;
                o.uv2 = tmpvar_3;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2.w = 1.0;
                tmpvar_2.xyz = (normalize(i.uv2) * 10.0);
                tmpvar_1 = tmpvar_2;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}