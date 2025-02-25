// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Corona/Lightmap/[Spec] [Env]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _SpecPower ("Specular Power", Range(0,50)) = 10
        _Reflectivity ("Reflectivity", Range(0,1)) = 0.2
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
            float3 _DepthBand;

            float _Reflectivity;
            samplerCUBE _ThemedCube;
            sampler2D _SpecMap;
            sampler2D _MainTex;
            float _SpecPower;
            float3 _SpecDir;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 uv2 : TEXCOORD2;
                float3 uv3 : TEXCOORD3;
                float3 uv4 : TEXCOORD4;
                float4 uv5 : TEXCOORD5;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float3 tmpvar_1;
                tmpvar_1 = normalize(v.normal);
                float4 fog_2;
                float2 tmpvar_3;
                float2 tmpvar_4;
                float3 tmpvar_5;
                float3 tmpvar_6;
                float4 tmpvar_7;
                float4 tmpvar_8;
                tmpvar_8.w = 1.0;
                tmpvar_8.xyz = v.vertex.xyz;
                float4 tmpvar_9;
                tmpvar_9 = (UnityObjectToClipPos(tmpvar_8));
                float4 tmpvar_10;
                tmpvar_10.x = tmpvar_9.x;
                tmpvar_10.y = tmpvar_9.y;
                tmpvar_10.z = ((tmpvar_9.z * _DepthBand.z) + (tmpvar_9.w * _DepthBand.y));
                tmpvar_10.w = tmpvar_9.w;
                float2 tmpvar_11;
                tmpvar_11 = v.uv.xy;
                tmpvar_3 = tmpvar_11;
                float2 tmpvar_12;
                tmpvar_12 = ((v.uv1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_4 = tmpvar_12;
                float4 tmpvar_13;
                tmpvar_13.w = 1.0;
                tmpvar_13.xyz = _WorldSpaceCameraPos;
                float3 tmpvar_14;
                tmpvar_14 = (mul(unity_WorldToObject, tmpvar_13).xyz - v.vertex.xyz);
                float3 i_15;
                i_15 = -(tmpvar_14);
                float4 tmpvar_16;
                tmpvar_16.w = 0.0;
                tmpvar_16.xyz = (i_15 - (2.0 * (dot (tmpvar_1, i_15) * tmpvar_1)));
                float3 tmpvar_17;
                tmpvar_17 = mul(unity_ObjectToWorld, tmpvar_16).xyz;
                float3 tmpvar_18;
                tmpvar_18 = normalize(tmpvar_14);
                tmpvar_5 = tmpvar_18;
                tmpvar_6 = tmpvar_17;
                float tmpvar_19;
                tmpvar_19 = clamp (((tmpvar_10.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_20;
                tmpvar_20.xyz = (_FogParams - (_FogParams * tmpvar_19));
                tmpvar_20.w = tmpvar_19;
                fog_2.xyz = tmpvar_20.xyz;
                fog_2.w = (tmpvar_19 * 2.0);
                tmpvar_7 = fog_2;
                o.pos = tmpvar_10;
                o.uv = tmpvar_3;
                o.uv1 = tmpvar_4;
                o.uv2 = tmpvar_5;
                o.uv3 = tmpvar_1;
                o.uv4 = tmpvar_6;
                o.uv5 = tmpvar_7;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                tmpvar_1 = tex2D (_SpecMap, i.uv);
                float3 lightDir_2;
                float3 tmpvar_3;
                float3 dir_4;
                dir_4 = _SpecDir;
                float4 tmpvar_5;
                tmpvar_5.w = 0.0;
                tmpvar_5.xyz = dir_4;
                tmpvar_3 = mul(unity_WorldToObject, tmpvar_5).xyz;
                lightDir_2 = tmpvar_3;
                float tmpvar_6;
                float spec_7;
                float tmpvar_8;
                tmpvar_8 = max (0.0, dot (normalize((lightDir_2 + normalize(i.uv2))), normalize(i.uv3)));
                spec_7 = tmpvar_8;
                float tmpvar_9;
                tmpvar_9 = pow (spec_7, _SpecPower);
                spec_7 = tmpvar_9;
                tmpvar_6 = tmpvar_9;
                float4 tmpvar_10;
                tmpvar_10.w = 1.0;
                tmpvar_10.xyz = (((((tex2D (_MainTex, i.uv).xyz + (tmpvar_6.xxx * tmpvar_1.xyz)) * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.uv1).xyz) + ((texCUBE (_ThemedCube, i.uv4).xyz * (_Reflectivity * 0.5)) * tmpvar_1.xyz)) * i.uv5.w) + i.uv5.xyz);
                return tmpvar_10;
            }
            ENDCG
        }
    }
}