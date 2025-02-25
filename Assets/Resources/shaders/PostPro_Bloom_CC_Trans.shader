Shader "Corona/PostProcess/Copy_Bloom_CC_Trans" {
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
            float _TimeSin;
            float _YonTrans;
            sampler2D _TranRampTex;
            sampler2D _TranTex;
            sampler2D _RampTex;
            sampler2D _BloomTex;
            sampler2D _MainTex;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
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
            half4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                float v_2;
                float4 col_3;
                float tmpvar_4;
                tmpvar_4 = (_YonTrans + (0.033 * i.uv.y));
                v_2 = tmpvar_4;
                float2 tmpvar_5;
                tmpvar_5.x = i.uv.x;
                tmpvar_5.y = v_2;
                float tmpvar_6;
                tmpvar_6 = (0.2 * (tex2D (_TranRampTex, tmpvar_5).x - 0.5));
                float2 tmpvar_7;
                tmpvar_7.x = (i.uv.x + tmpvar_6);
                tmpvar_7.y = i.uv.y;
                float4 tmpvar_8;
                tmpvar_8 = tex2D (_MainTex, tmpvar_7);
                float2 tmpvar_9;
                tmpvar_9.x = (i.uv.x + tmpvar_6);
                tmpvar_9.y = v_2;
                float4 tmpvar_10;
                tmpvar_10 = tex2D (_TranTex, tmpvar_9);
                float4 tmpvar_11;
                tmpvar_11 = ((tmpvar_8 * 0.5) + (0.5 * (((1.0 - _TimeSin) * tmpvar_8) + (_TimeSin * tmpvar_10))));
                col_3 = tmpvar_11;
                col_3.x = (tex2D (_RampTex, col_3.xx).x + 1e-05);
                col_3.y = (tex2D (_RampTex, col_3.yy).y + 2e-05);
                col_3.z = (tex2D (_RampTex, col_3.zz).z + 3e-05);
                float4 tmpvar_12;
                tmpvar_12 = tex2D (_BloomTex, i.uv);
                float3 tmpvar_13;
                tmpvar_13 = (col_3.xyz + (tmpvar_12.w * _Bloomification));
                col_3.xyz = tmpvar_13;
                tmpvar_1 = col_3;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}