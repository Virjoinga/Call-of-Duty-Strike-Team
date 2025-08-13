Shader "Corona/Effects/CloseSnow2" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Size ("Size", Float) = 0.01
        _Alpha ("Alpha", Float) = 1
        _CameraMotionBlur ("CameraMotionBlur", Float) = 0.3
        _PosScale ("PosScale", Float) = 1
    }
    SubShader { 
        Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Pass {
            Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _PosScale;
            float _CameraMotionBlur;
            float _Alpha;
            float _Size;
            float4 g_closeSnowWindData;
            float4 g_closeSnowFade;
            float4 g_closeSnowOffset;
            float4 g_closeSnowData;
            float4 g_cameraVel;
            float4 g_cameraPos;
            float3 _DepthBand;

            sampler2D _MainTex;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float4 tmpvar_1;
                tmpvar_1.w = 0.0;
                tmpvar_1.xyz = v.normal;
                float2 tmpvar_2;
                tmpvar_2 = v.uv.xy;
                float tmpvar_3;
                float2 sinCos_4;
                float3 pos_5;
                float tmpvar_6;
                tmpvar_6 = dot (v.normal, v.normal);
                float3 tmpvar_7;
                tmpvar_7.y = 0.0;
                tmpvar_7.x = -(v.normal.z);
                tmpvar_7.z = tmpvar_1.x;
                float3 tmpvar_8;
                tmpvar_8.xz = float2(0.0, 0.0);
                tmpvar_8.y = tmpvar_6;
                float3 tmpvar_9;
                tmpvar_9 = (tmpvar_8 - (v.normal.y * v.normal));
                float tmpvar_10;
                tmpvar_10 = (1.0/(max (1e-05, sqrt(dot (v.normal.xz, v.normal.xz)))));
                float tmpvar_11;
                tmpvar_11 = (sqrt(tmpvar_6) * tmpvar_10);
                float tmpvar_12;
                tmpvar_12 = ((v.color.y * 2.5) + 2.0);
                float x_13;
                x_13 = ((_Time.y + v.color.x) * tmpvar_12);
                sinCos_4.x = sin(x_13);
                sinCos_4.y = cos(x_13);
                float2 tmpvar_14;
                tmpvar_14 = (sinCos_4 * 0.15);
                float2 tmpvar_15;
                tmpvar_15 = (tmpvar_14 * (tmpvar_12 * 0.015));
                float3 tmpvar_16;
                tmpvar_16 = ((frac(((((v.vertex.xyz + (v.normal * g_closeSnowWindData.x)) + ((tmpvar_7 * (tmpvar_11 * tmpvar_14.y)) + (tmpvar_9 * (tmpvar_10 * tmpvar_14.x)))) * g_closeSnowData.w) + g_closeSnowData.xyz)) * (g_closeSnowOffset.w * _PosScale)) + ((g_closeSnowOffset.xyz * _PosScale) + (g_cameraPos * (1.0 - _PosScale)).xyz));
                float3 tmpvar_17;
                tmpvar_17 = normalize((tmpvar_16 - g_cameraPos.xyz));
                float3 tmpvar_18;
                tmpvar_18 = ((g_cameraVel * _CameraMotionBlur).xyz - (((v.normal * (0.015 * g_closeSnowWindData.y)) + ((tmpvar_9 * (tmpvar_10 * tmpvar_15.y)) - (tmpvar_7 * (tmpvar_11 * tmpvar_15.x)))) * _PosScale));
                float3 tmpvar_19;
                tmpvar_19 = ((tmpvar_18.yzx * tmpvar_17.zxy) - (tmpvar_18.zxy * tmpvar_17.yzx));
                float3 tmpvar_20;
                tmpvar_20 = ((tmpvar_17.yzx * tmpvar_19.zxy) - (tmpvar_17.zxy * tmpvar_19.yzx));
                float tmpvar_21;
                tmpvar_21 = max (1.0, (_Size / sqrt(dot (tmpvar_18, tmpvar_18))));
                float tmpvar_22;
                tmpvar_22 = rsqrt(dot (tmpvar_19, tmpvar_19));
                float2 tmpvar_23;
                tmpvar_23 = ((v.uv.xy * 2.0) - 1.0);
                pos_5 = (tmpvar_16 + (tmpvar_20 * ((max ((_Size - ((dot (tmpvar_18, tmpvar_20) * tmpvar_21) * tmpvar_22)), 0.0) * tmpvar_22) * tmpvar_23.x)));
                pos_5 = (pos_5 + (tmpvar_18 * (tmpvar_23.x * tmpvar_21)));
                pos_5 = (pos_5 + (tmpvar_19 * ((tmpvar_23.y * _Size) * tmpvar_22)));
                float tmpvar_24;
                tmpvar_24 = ((g_closeSnowFade.x * clamp (((tmpvar_17 * g_closeSnowFade.w) + g_closeSnowFade.z), 0.0, 1.0).x) + g_closeSnowFade.y);
                float tmpvar_25;
                if ((tmpvar_24 < 0.25)) {
                    tmpvar_25 = tmpvar_24;
                } else {
                    tmpvar_25 = 1.0;
                };
                float4 tmpvar_26;
                tmpvar_26.w = 1.0;
                tmpvar_26.xyz = pos_5;
                float4 tmpvar_27;
                tmpvar_27.w = 1.0;
                tmpvar_27.xyz = tmpvar_26.xyz;
                float4 tmpvar_28;
                tmpvar_28 = (UnityObjectToClipPos(tmpvar_27));
                float4 tmpvar_29;
                tmpvar_29.x = tmpvar_28.x;
                tmpvar_29.y = tmpvar_28.y;
                tmpvar_29.z = ((tmpvar_28.z * _DepthBand.z) + (tmpvar_28.w * _DepthBand.y));
                tmpvar_29.w = tmpvar_28.w;
                float tmpvar_30;
                tmpvar_30 = (tmpvar_25 * _Alpha);
                tmpvar_3 = tmpvar_30;
                o.pos = tmpvar_29;
                o.uv = tmpvar_2;
                o.color = tmpvar_3;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                tmpvar_1.xyz = float3(1.0, 1.0, 1.0);
                tmpvar_1.w = (tex2D (_MainTex, i.uv).w * i.color);
                return tmpvar_1;
            }
            ENDCG
        }
    }
}