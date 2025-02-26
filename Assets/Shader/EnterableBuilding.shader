#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Corona/Effects/Enterable Building" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _EdgeTex ("Edge Texture", 2D) = "white" {}
        _BlendTex ("Blend Texture", 2D) = "white" {}
        _FadeHeight ("Fade Height", Float) = 0
        _FadeOrigin ("Fade Origin", Vector) = (0,0,0,0)
    }
    SubShader { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Tags { "RenderType"="Opaque" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float3 _DepthBand;

            float4 _FadeOrigin;
            float _FadeHeight;
            float4 _Color;
            sampler2D _BlendTex;
            sampler2D _EdgeTex;
            sampler2D _MainTex;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float4 tmpvar_1;
                float3 tmpvar_2;
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
                float4 tmpvar_8;
                tmpvar_8.w = 0.0;
                tmpvar_8.xyz = (normalize(v.normal) * 1.0);
                float3 tmpvar_9;
                tmpvar_9 = mul(unity_ObjectToWorld, tmpvar_8).xyz;
                tmpvar_2 = tmpvar_9;
                float4 tmpvar_10;
                tmpvar_10 = mul(unity_ObjectToWorld, v.vertex);
                tmpvar_1 = tmpvar_10;
                float2 tmpvar_11;
                tmpvar_11 = v.uv.xy;
                tmpvar_3 = tmpvar_11;
                float2 tmpvar_12;
                tmpvar_12 = ((v.uv1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_4 = tmpvar_12;
                o.pos = tmpvar_7;
                o.uv = tmpvar_1;
                o.uv1 = tmpvar_2;
                o.uv2 = tmpvar_3;
                o.uv3 = tmpvar_4;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                float4 blendSample_2;
                float3 normalView_3;
                float3 strategyView_4;
                float fade_5;
                float4 fadeOffset_6;
                float4 tmpvar_7;
                tmpvar_7 = (i.uv - _FadeOrigin);
                fadeOffset_6 = tmpvar_7;
                float tmpvar_8;
                tmpvar_8 = sqrt(dot (fadeOffset_6, fadeOffset_6));
                fade_5 = (1.0 - (_FadeHeight - tmpvar_8));
                float tmpvar_9;
                float x_10;
                x_10 = normalize(fadeOffset_6).y;
                tmpvar_9 = (1.5708 - (sign(x_10) * (1.5708 - (sqrt((1.0 - abs(x_10))) * (1.5708 + (abs(x_10) * (-0.214602 + (abs(x_10) * (0.0865667 + (abs(x_10) * -0.0310296))))))))));
                float tmpvar_11;
                tmpvar_11 = (fade_5 + (0.1 * cos(((_Time * 100.0) + (10.0 * tmpvar_9)))).x);
                fade_5 = tmpvar_11;
                if ((tmpvar_11 < 0.0)) {
                    discard;
                };
                float3 tmpvar_12;
                tmpvar_12 = (2.0 * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.uv3).xyz);
                float4 tmpvar_13;
                tmpvar_13 = tex2D (_MainTex, i.uv2);
                float3 tmpvar_14;
                tmpvar_14 = ((_Color.xyz * tmpvar_12) * tmpvar_13.xyz);
                strategyView_4 = tmpvar_14;
                float2 tmpvar_15;
                tmpvar_15.y = 0.0;
                tmpvar_15.x = tmpvar_11;
                float4 tmpvar_16;
                tmpvar_16 = tex2D (_EdgeTex, tmpvar_15);
                float3 tmpvar_17;
                tmpvar_17 = (_Color * tmpvar_16).xyz;
                normalView_3 = tmpvar_17;
                float2 tmpvar_18;
                tmpvar_18.y = 0.0;
                tmpvar_18.x = tmpvar_11;
                float4 tmpvar_19;
                tmpvar_19 = tex2D (_BlendTex, tmpvar_18);
                blendSample_2 = tmpvar_19;
                float4 tmpvar_20;
                tmpvar_20.w = 1.0;
                tmpvar_20.xyz = ((blendSample_2.x * normalView_3) + (blendSample_2.y * strategyView_4));
                tmpvar_1 = tmpvar_20;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}