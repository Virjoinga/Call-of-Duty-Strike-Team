Shader "Corona/PostProcess/Start" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Strength ("Bloom Strength", Float) = 0.5
        _CutOff ("Cut Off", Float) = 0.8
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

            float _CutOff;
            float _Strength;
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
                float avgcol_2;
                float4 col_3;
                float4 tmpvar_4;
                tmpvar_4 = tex2D (_MainTex, i.uv);
                col_3 = tmpvar_4;
                float tmpvar_5;
                tmpvar_5 = (((tmpvar_4.x + tmpvar_4.y) + tmpvar_4.z) * 0.34);
                avgcol_2 = tmpvar_5;
                if (((tmpvar_5 - _CutOff) < 0.0)) {
                  avgcol_2 = 0.0;
                };
                col_3.w = (((avgcol_2 * avgcol_2) * avgcol_2) * _Strength);
                tmpvar_1 = col_3;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}