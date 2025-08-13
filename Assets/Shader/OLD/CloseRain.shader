Shader "Corona/Effects/CloseRain" {
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
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                tmpvar_1 = v.uv.xy;
                float tmpvar_2;
                float3 pos_3;
                float3 tmpvar_4;
                tmpvar_4 = ((frac((((v.vertex.xyz + (v.normal * g_closeSnowWindData.x)) * g_closeSnowData.w) + g_closeSnowData.xyz)) * (g_closeSnowOffset.w * _PosScale)) + ((g_closeSnowOffset.xyz * _PosScale) + (g_cameraPos * (1.0 - _PosScale)).xyz));
                float3 tmpvar_5;
                tmpvar_5 = normalize((tmpvar_4 - g_cameraPos.xyz));
                float3 tmpvar_6;
                tmpvar_6 = ((g_cameraVel * _CameraMotionBlur).xyz - ((v.normal * (0.015 * g_closeSnowWindData.y)) * _PosScale));
                float3 tmpvar_7;
                tmpvar_7 = ((tmpvar_6.yzx * tmpvar_5.zxy) - (tmpvar_6.zxy * tmpvar_5.yzx));
                float3 tmpvar_8;
                tmpvar_8 = ((tmpvar_5.yzx * tmpvar_7.zxy) - (tmpvar_5.zxy * tmpvar_7.yzx));
                float tmpvar_9;
                tmpvar_9 = max (1.0, (_Size / sqrt(dot (tmpvar_6, tmpvar_6))));
                float tmpvar_10;
                tmpvar_10 = rsqrt(dot (tmpvar_7, tmpvar_7));
                float2 tmpvar_11;
                tmpvar_11 = ((v.uv.xy * 2.0) - 1.0);
                pos_3 = (tmpvar_4 + (tmpvar_8 * ((max ((_Size - ((dot (tmpvar_6, tmpvar_8) * tmpvar_9) * tmpvar_10)), 0.0) * tmpvar_10) * tmpvar_11.x)));
                pos_3 = (pos_3 + (tmpvar_6 * (tmpvar_11.x * tmpvar_9)));
                pos_3 = (pos_3 + (tmpvar_7 * ((tmpvar_11.y * _Size) * tmpvar_10)));
                float tmpvar_12;
                tmpvar_12 = ((g_closeSnowFade.x * clamp (((tmpvar_5 * g_closeSnowFade.w) + g_closeSnowFade.z), 0.0, 1.0).x) + g_closeSnowFade.y);
                float tmpvar_13;
                if ((tmpvar_12 < 0.25)) {
                    tmpvar_13 = tmpvar_12;
                } else {
                    tmpvar_13 = 1.0;
                };
                float4 tmpvar_14;
                tmpvar_14.w = 1.0;
                tmpvar_14.xyz = pos_3;
                float4 tmpvar_15;
                tmpvar_15.w = 1.0;
                tmpvar_15.xyz = tmpvar_14.xyz;
                float4 tmpvar_16;
                tmpvar_16 = (UnityObjectToClipPos(tmpvar_15));
                float4 tmpvar_17;
                tmpvar_17.x = tmpvar_16.x;
                tmpvar_17.y = tmpvar_16.y;
                tmpvar_17.z = ((tmpvar_16.z * _DepthBand.z) + (tmpvar_16.w * _DepthBand.y));
                tmpvar_17.w = tmpvar_16.w;
                float tmpvar_18;
                tmpvar_18 = (tmpvar_13 * _Alpha);
                tmpvar_2 = tmpvar_18;
                o.pos = tmpvar_17;
                o.uv = tmpvar_1;
                o.color = tmpvar_2;
                
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