Shader "Corona/Effects/CloseSnowVolume" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Size ("Size", Float) = 0.01
        _Alpha ("Alpha", Float) = 1
        _Color ("Main Color", Color) = (1,1,1,1)
        _CentreFadeSpeed ("CentreFadeSpeed", Float) = 0.25
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

            float _CentreFadeSpeed;
            float4 _Color;
            float _Alpha;
            float _Size;
            float4 g_closeSnowWindData;
            float4 g_closeSnowFade;
            float4 g_closeSnowOffset;
            float4 g_closeSnowData2;
            float4 g_closeSnowData;
            float4 g_cameraForward;
            float4 g_cameraUp;
            float4 g_cameraRight;
            float4 g_cameraPos;
            float3 _DepthBand;
            float3 _SpecDir;

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
                float4 tmpvar_3;
                float3 lightDir_4;
                float3 pos_5;
                float3 tmpvar_6;
                tmpvar_6.x = g_closeSnowData.w;
                tmpvar_6.y = g_closeSnowData2.x;
                tmpvar_6.z = g_closeSnowData.w;
                float3 tmpvar_7;
                tmpvar_7.x = g_closeSnowOffset.w;
                tmpvar_7.y = g_closeSnowData2.y;
                tmpvar_7.z = g_closeSnowOffset.w;
                float3 tmpvar_8;
                tmpvar_8 = ((frac((((v.vertex.xyz + (tmpvar_1 * g_closeSnowWindData.x).xyz) * tmpvar_6) + g_closeSnowData.xyz)) * tmpvar_7) + g_closeSnowOffset.xyz);
                float3 tmpvar_9;
                tmpvar_9 = (tmpvar_8 - g_cameraPos.xyz);
                float tmpvar_10;
                tmpvar_10 = sqrt(dot (tmpvar_9, tmpvar_9));
                float tmpvar_11;
                tmpvar_11 = (clamp (((tmpvar_10 * (_CentreFadeSpeed * 2.0)) - ((6.0 * _CentreFadeSpeed) + 1.0)), 0.0, 1.0) * clamp ((10.0 - (sqrt(dot (tmpvar_9.xz, tmpvar_9.xz)) * (g_closeSnowData.w * 20.0))), 0.0, 1.0));
                float tmpvar_12;
                tmpvar_12 = min ((_Size * clamp (((tmpvar_10 * (_CentreFadeSpeed * 4.0)) - (12.0 * _CentreFadeSpeed)), 0.0, 1.0)), (dot (tmpvar_9, g_cameraForward.xyz) * 0.3));
                float tmpvar_13;
                tmpvar_13 = ((v.color.x * (4.0 * _Time.y)) - (2.0 * _Time.y));
                float tmpvar_14;
                tmpvar_14 = sin(tmpvar_13);
                float tmpvar_15;
                tmpvar_15 = cos(tmpvar_13);
                float2 tmpvar_16;
                tmpvar_16 = ((v.uv.xy * (2.0 * tmpvar_12)) - tmpvar_12);
                pos_5 = (tmpvar_8 + ((((tmpvar_16.x * tmpvar_15) - (tmpvar_16.y * tmpvar_14)) * g_cameraRight.xyz) + (((tmpvar_16.y * tmpvar_15) + (tmpvar_16.x * tmpvar_14)) * g_cameraUp.xyz)));
                float tmpvar_17;
                tmpvar_17 = ((g_closeSnowFade.x * clamp (((tmpvar_10 * g_closeSnowFade.w) + g_closeSnowFade.z), 0.0, 1.0)) + g_closeSnowFade.y);
                float tmpvar_18;
                if ((tmpvar_17 < 0.25)) {
                    tmpvar_18 = tmpvar_17;
                } else {
                    tmpvar_18 = 1.0;
                };
                float3 tmpvar_19;
                tmpvar_19.y = 0.0;
                tmpvar_19.x = _SpecDir.x;
                tmpvar_19.z = _SpecDir.z;
                float3 tmpvar_20;
                tmpvar_20 = normalize(tmpvar_19);
                lightDir_4 = tmpvar_20;
                float tmpvar_21;
                tmpvar_21 = ((tmpvar_18 * tmpvar_11) * (_Alpha - ((_Alpha * 0.25) * max (0.0, dot (lightDir_4, normalize((pos_5 - g_cameraPos.xyz)))))));
                float4 tmpvar_22;
                tmpvar_22.w = 1.0;
                tmpvar_22.xyz = pos_5;
                float4 tmpvar_23;
                tmpvar_23.w = 1.0;
                tmpvar_23.xyz = tmpvar_22.xyz;
                float4 tmpvar_24;
                tmpvar_24 = (UnityObjectToClipPos(tmpvar_23));
                float4 tmpvar_25;
                tmpvar_25.x = tmpvar_24.x;
                tmpvar_25.y = tmpvar_24.y;
                tmpvar_25.z = ((tmpvar_24.z * _DepthBand.z) + (tmpvar_24.w * _DepthBand.y));
                tmpvar_25.w = tmpvar_24.w;
                tmpvar_3.xyz = _Color.xyz;
                tmpvar_3.w = tmpvar_21;
                o.pos = tmpvar_25;
                o.uv = tmpvar_2;
                o.color = tmpvar_3;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                return (tex2D (_MainTex, i.uv) * i.color);
            }
            ENDCG
        }
    }
}