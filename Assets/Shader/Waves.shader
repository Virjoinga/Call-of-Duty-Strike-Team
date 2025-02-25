Shader "Corona/Water/Waves" {
    Properties {
        _MainTex1 ("Texture 1", 2D) = "white" {}
        _MainTex2 ("Texture 2", 2D) = "white" {}
        _Waves1 ("Wave distortion 1", 2D) = "" {}
        _Waves2 ("Wave distortion 2", 2D) = "" {}
        _TintColor ("TintColor", Color) = (1,1,1,1)
    }
    SubShader { 
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _Waves2_ST;
            float4 _Waves1_ST;
            float4 _MainTex2_ST;
            float4 _MainTex1_ST;
            float3 _DepthBand;

            float4 _TintColor;
            sampler2D _Waves2;
            sampler2D _Waves1;
            sampler2D _MainTex2;
            sampler2D _MainTex1;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                float2 tmpvar_2;
                float4 tmpvar_3;
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
                tmpvar_7 = ((v.uv.xy * _Waves1_ST.xy) + _Waves1_ST.zw);
                tmpvar_1 = tmpvar_7;
                float2 tmpvar_8;
                tmpvar_8 = ((v.uv.xy * _Waves2_ST.xy) + _Waves2_ST.zw);
                tmpvar_2 = tmpvar_8;
                float2 tmpvar_9;
                tmpvar_9 = ((v.uv.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
                tmpvar_3.xy = tmpvar_9;
                float2 tmpvar_10;
                tmpvar_10 = ((v.uv.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
                tmpvar_3.zw = tmpvar_10;
                tmpvar_3.yw = (tmpvar_3.yw - 0.5);
                o.pos = tmpvar_6;
                o.uv = tmpvar_1;
                o.uv1 = tmpvar_2;
                o.uv2 = tmpvar_3;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 mainUVs_1;
                float2 offsets_2;
                offsets_2.x = tex2D (_Waves1, i.uv).w;
                offsets_2.y = tex2D (_Waves2, i.uv1).w;
                mainUVs_1.xz = i.uv2.xz;
                mainUVs_1.yw = (i.uv2.yw + offsets_2);
                return (max (tex2D (_MainTex1, mainUVs_1.xy), tex2D (_MainTex2, mainUVs_1.zw)) * _TintColor);
            }
            ENDCG
        }
    }
}