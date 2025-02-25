Shader "Corona/PostProcess/GausBlur" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "black" {}
        _BlurWidth ("Blur Width", Vector) = (0.002,0.004,0,0)
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

            float4 _BlurWidth;

            sampler2D _MainTex;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float2 uv4 : TEXCOORD4;
                float2 uv5 : TEXCOORD5;
                float2 uv6 : TEXCOORD6;
                float2 uv7 : TEXCOORD7;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                tmpvar_1 = v.uv.xy;
                float2 tmpvar_2;
                float2 tmpvar_3;
                float2 tmpvar_4;
                float2 tmpvar_5;
                float2 tmpvar_6;
                float2 tmpvar_7;
                float2 tmpvar_8;
                float2 tmpvar_9;
                float2 tmpvar_10;
                float4 tmpvar_11;
                tmpvar_11.zw = float2(0.0, 0.0);
                tmpvar_11.x = tmpvar_1.x;
                tmpvar_11.y = tmpvar_1.y;
                tmpvar_10 = mul(UNITY_MATRIX_TEXTURE0, tmpvar_11).xy;
                tmpvar_2 = tmpvar_10;
                float2 tmpvar_12;
                tmpvar_12 = (tmpvar_2 + (_BlurWidth.xy * 2.0));
                tmpvar_3 = tmpvar_12;
                float2 tmpvar_13;
                tmpvar_13 = (tmpvar_2 + (_BlurWidth.zw * 3.0));
                tmpvar_4 = tmpvar_13;
                float2 tmpvar_14;
                tmpvar_14 = (tmpvar_2 + (_BlurWidth.xy * 4.0));
                tmpvar_7 = tmpvar_14;
                float2 tmpvar_15;
                tmpvar_15 = (tmpvar_2 + _BlurWidth.zw);
                tmpvar_8 = tmpvar_15;
                float2 tmpvar_16;
                tmpvar_16 = (tmpvar_2 - (_BlurWidth.xy * 2.0));
                tmpvar_9 = tmpvar_16;
                float2 tmpvar_17;
                tmpvar_17 = (tmpvar_2 - (_BlurWidth.xy * 4.0));
                tmpvar_5 = tmpvar_17;
                float2 tmpvar_18;
                tmpvar_18 = (tmpvar_2 - _BlurWidth.zw);
                tmpvar_6 = tmpvar_18;
                o.pos = (UnityObjectToClipPos(v.vertex));
                o.uv = tmpvar_2;
                o.uv1 = tmpvar_3;
                o.uv2 = tmpvar_4;
                o.uv3 = tmpvar_5;
                o.uv4 = tmpvar_6;
                o.uv5 = tmpvar_7;
                o.uv6 = tmpvar_8;
                o.uv7 = tmpvar_9;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2 = ((((((((tex2D (_MainTex, i.uv) * 0.19) + (tex2D (_MainTex, i.uv1) * 0.12)) + (tex2D (_MainTex, i.uv2) * 0.09)) + (tex2D (_MainTex, i.uv3) * 0.05)) + (tex2D (_MainTex, i.uv4) * 0.16)) + (tex2D (_MainTex, i.uv5) * 0.05)) + (tex2D (_MainTex, i.uv6) * 0.16)) + (tex2D (_MainTex, i.uv7) * 0.12));
                tmpvar_1 = tmpvar_2;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}