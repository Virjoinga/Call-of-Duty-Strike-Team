Shader "Corona/PostProcess/Copy_Bloom_CC" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BloomTex ("bloom tex", 2D) = "black" {}
        _RampTex ("ramp tex", 2D) = "grayscaleRamp" {}
    }
    SubShader { 
        Pass {
            ZTest Always
            ZWrite Off
            Cull Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _Bloomification;
            sampler2D _RampTex;
            sampler2D _BloomTex;
            sampler2D _MainTex;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                tmpvar_1 = v.uv.xy;
                float2 tmpvar_2;
                float2 tmpvar_3;
                float4 tmpvar_4;
                tmpvar_4.zw = float2(0.0, 0.0);
                tmpvar_4.x = tmpvar_1.x;
                tmpvar_4.y = tmpvar_1.y;
                tmpvar_3 = mul(UNITY_MATRIX_TEXTURE0, tmpvar_4).xy;
                tmpvar_2 = tmpvar_3;
                o.pos = (UnityObjectToClipPos(v.vertex));
                o.uv = tmpvar_2;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                float4 col_2;
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_MainTex, i.uv);
                col_2.w = tmpvar_3.w;
                float4 tmpvar_4;
                tmpvar_4 = tex2D (_BloomTex, i.uv);
                col_2.x = (tex2D (_RampTex, tmpvar_3.xx).x + 1e-05);
                col_2.y = (tex2D (_RampTex, tmpvar_3.yy).y + 2e-05);
                col_2.z = (tex2D (_RampTex, tmpvar_3.zz).z + 3e-05);
                float3 tmpvar_5;
                tmpvar_5 = (col_2.xyz + (tmpvar_4.w * _Bloomification));
                col_2.xyz = tmpvar_5;
                tmpvar_1 = col_2;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}