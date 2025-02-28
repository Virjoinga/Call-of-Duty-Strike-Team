Shader "Hidden/MinimalLightmapVertexAlpha" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader { 
        Tags { "QUEUE"="Transparent-1" "RenderType"="Transparent" }
        Pass {
            Tags { "QUEUE"="Transparent-1" "RenderType"="Transparent" }
            Fog {
                Mode Exp2
            }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float3 _DepthBand;

            sampler2D _MainTex;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 color : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                float2 tmpvar_2;
                float tmpvar_3;
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
                float2 tmpvar_7;
                tmpvar_7 = v.uv.xy;
                tmpvar_1 = tmpvar_7;
                float2 tmpvar_8;
                tmpvar_8 = ((v.uv1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_2 = tmpvar_8;
                float tmpvar_9;
                tmpvar_9 = v.color.w;
                tmpvar_3 = tmpvar_9;
                o.pos = tmpvar_6;
                o.uv = tmpvar_1;
                o.uv1 = tmpvar_2;
                o.uv2 = tmpvar_3;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                float3 tmpvar_2;
                tmpvar_2 = (2.0 * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.uv1).xyz);
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_MainTex, i.uv);
                float4 tmpvar_4;
                tmpvar_4.xyz = (tmpvar_3.xyz * tmpvar_2);
                tmpvar_4.w = i.uv2;
                tmpvar_1 = tmpvar_4;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}