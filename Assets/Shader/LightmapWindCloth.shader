Shader "Corona/Lightmap/[WindCloth]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _WindAmount ("Wind Amount", Float) = 0.08
        _NormalDirectionScale ("Normal Direction Scale", Float) = 1
        _WindDirectionScale ("Wind Direction Scale", Float) = 0
        _SpatialScale ("Spatial Scale", Float) = 2
        _Snap ("Snap", Float) = 1
        _HighFrequencyAmount ("High Frequency Amount", Float) = 1
        _WindSpeed0Scale ("Wind Speed 0 Scale", Float) = 1
        _WindSpeed1Scale ("Wind Speed 1 Scale", Float) = 0
    }
    SubShader { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Tags { "RenderType"="Opaque" }
            Fog { Mode Off }

                        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _FogRange;
            float3 _FogParams;
            float _WindSpeed1Scale;
            float _WindSpeed0Scale;
            float _HighFrequencyAmount;
            float _Snap;
            float _SpatialScale;
            float _WindDirectionScale;
            float _NormalDirectionScale;
            float _WindAmount;
            float4 g_globalWindDir2;
            float4 g_globalWindDir;
            float4 g_globalWindData;
            float3 _DepthBand;

            sampler2D _MainTex;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 uv5 : TEXCOORD5;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float4 fog_1;
                float2 tmpvar_2;
                float2 tmpvar_3;
                float4 tmpvar_4;
                float4 tmpvar_5;
                tmpvar_5 = v.vertex;
                float3 tmpvar_6;
                float4 tmpvar_7;
                tmpvar_7.w = 1.0;
                tmpvar_7.xyz = v.vertex.xyz;
                tmpvar_6 = mul(unity_ObjectToWorld, tmpvar_7).xyz;
                float tmpvar_8;
                tmpvar_8 = (_NormalDirectionScale * _WindAmount);
                float4 tmpvar_9;
                tmpvar_9.w = 0.0;
                tmpvar_9.xyz = (g_globalWindDir.xyz * (_WindDirectionScale * _WindAmount));
                float3 tmpvar_10;
                tmpvar_10 = ((normalize(v.normal) * ((v.color.x * (2.0 * tmpvar_8)) - tmpvar_8)) + mul(unity_WorldToObject, tmpvar_9).xyz);
                float2 tmpvar_11;
                tmpvar_11.x = _WindSpeed0Scale;
                tmpvar_11.y = _WindSpeed1Scale;
                float2 tmpvar_12;
                tmpvar_12.x = (dot (tmpvar_6, (g_globalWindDir.xyz * _SpatialScale)) - (dot (g_globalWindData.xy, tmpvar_11) * 20.0));
                tmpvar_12.y = (dot (tmpvar_6, (g_globalWindDir2.xyz * 4.0)) - (_Time.y * 40.0));
                float2 tmpvar_13;
                tmpvar_13 = cos(tmpvar_12);
                float tmpvar_14;
                tmpvar_14 = ((g_globalWindData.w * 0.5) + 0.5);
                float2 tmpvar_15;
                tmpvar_15 = (1.0 - pow ((1.0 - abs(tmpvar_13)), lerp (1.0, ((g_globalWindData.w * float2(2.0, 1.0)) + float2(2.0, 1.0)).x, _Snap)));
                float2 tmpvar_16;
                tmpvar_16 = min (tmpvar_13, float2(0.0, 0.0));
                float2 b_17;
                b_17 = -(tmpvar_15);
                float tmpvar_18;
                if (tmpvar_16.x) {
                    tmpvar_18 = b_17.x;
                } else {
                    tmpvar_18 = tmpvar_15.x;
                };
                float tmpvar_19;
                if (tmpvar_16.y) {
                  tmpvar_19 = b_17.y;
                } else {
                  tmpvar_19 = tmpvar_15.y;
                };
                float tmpvar_20;
                tmpvar_20 = (g_globalWindData.w * _HighFrequencyAmount);
                tmpvar_5.xyz = (v.vertex.xyz + (tmpvar_10 * (((((1.0 - (0.5 * tmpvar_20)) * tmpvar_14) + (tmpvar_19 * ((0.5 * tmpvar_20) * tmpvar_14))) * tmpvar_18) * v.color.w)));
                float4 tmpvar_21;
                tmpvar_21.w = 1.0;
                tmpvar_21.xyz = tmpvar_5.xyz;
                float4 tmpvar_22;
                tmpvar_22 = (UnityObjectToClipPos(tmpvar_21));
                float4 tmpvar_23;
                tmpvar_23.x = tmpvar_22.x;
                tmpvar_23.y = tmpvar_22.y;
                tmpvar_23.z = ((tmpvar_22.z * _DepthBand.z) + (tmpvar_22.w * _DepthBand.y));
                tmpvar_23.w = tmpvar_22.w;
                float2 tmpvar_24;
                tmpvar_24 = v.uv.xy;
                tmpvar_2 = tmpvar_24;
                float2 tmpvar_25;
                tmpvar_25 = ((v.uv1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_3 = tmpvar_25;
                float tmpvar_26;
                tmpvar_26 = clamp (((tmpvar_23.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_27;
                tmpvar_27.xyz = (_FogParams - (_FogParams * tmpvar_26));
                tmpvar_27.w = tmpvar_26;
                fog_1.xyz = tmpvar_27.xyz;
                fog_1.w = (tmpvar_26 * 2.0);
                tmpvar_4 = fog_1;
                o.pos = tmpvar_23;
                o.uv = tmpvar_2;
                o.uv1 = tmpvar_3;
                o.uv5 = tmpvar_4;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = (((tex2D (_MainTex, i.uv).xyz * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.uv1).xyz) * i.uv5.w) + i.uv5.xyz);
                return tmpvar_1;
            }
            ENDCG
        }
    }
}