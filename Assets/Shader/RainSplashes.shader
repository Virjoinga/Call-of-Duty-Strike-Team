Shader "Corona/Effects/RainSplashes" {
    Properties {
        _MainTex ("Mask Texture", 2D) = "white" {}
        _Ripples ("Ripples", 2D) = "white" {}
        _Splashes ("Splashes", 2D) = "white" {}
        _RippleSpeed ("Ripple Speed", Vector) = (1,1,0,0)
    }
    SubShader { 
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Pass {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            ZWrite Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _Splashes_ST;
            float4 _MainTex_ST;
            float2 _RippleSpeed;
            float3 _DepthBand;

            sampler2D _Splashes;
            sampler2D _Ripples;
            sampler2D _MainTex;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                float2 tmpvar_2;
                float2 tmpvar_3;
                float2 tmpvar_4;
                float4 tmpvar_5;
                tmpvar_5.w = 1.0;
                tmpvar_5.xyz = v.vertex.xyz;
                float4 tmpvar_6;
                tmpvar_6 = (UnityObjectToClipPos(tmpvar_5));
                float4 tmpvar_7;
                tmpvar_7.x = tmpvar_6.x;
                tmpvar_7.y = tmpvar_6.y;
                tmpvar_7.z = ((tmpvar_6.z * _DepthBand.z) + (tmpvar_6.w * _DepthBand.y));
                tmpvar_7.w = tmpvar_6.w;
                float2 tmpvar_8;
                tmpvar_8 = ((v.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                tmpvar_1 = tmpvar_8;
                float2 tmpvar_9;
                tmpvar_9 = frac((_Time.y * _RippleSpeed));
                tmpvar_2 = tmpvar_9;
                float2 tmpvar_10;
                tmpvar_10 = ((v.uv.xy * _Splashes_ST.xy) + _Splashes_ST.zw);
                tmpvar_3 = tmpvar_10;
                float2 tmpvar_11;
                tmpvar_11 = ((v.uv1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_4 = tmpvar_11;
                o.pos = tmpvar_7;
                o.uv = tmpvar_1;
                o.uv1 = tmpvar_2;
                o.uv2 = tmpvar_3;
                o.uv3 = tmpvar_4;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                tmpvar_1 = tex2D (_Splashes, i.uv2);
                float2 P_2;
                P_2 = (i.uv1 + tmpvar_1.xy);
                float4 tmpvar_3;
                tmpvar_3.xyz = (2.0 * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.uv3).xyz);
                tmpvar_3.w = (tex2D (_MainTex, i.uv).x * dot (tex2D (_Ripples, P_2).xy, tmpvar_1.zw));
                return tmpvar_3;
            }
            ENDCG
        }
    }
}