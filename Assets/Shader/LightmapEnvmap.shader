Shader "Corona/Lightmap/[Env]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
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
                float4 tmpvar_6;
                float4 tmpvar_7;
                tmpvar_7.w = 1.0;
                tmpvar_7.xyz = v.vertex.xyz;
                float4 tmpvar_8;
                tmpvar_8 = (UnityObjectToClipPos(tmpvar_7));
                float4 tmpvar_9;
                tmpvar_9.x = tmpvar_8.x;
                tmpvar_9.y = tmpvar_8.y;
                tmpvar_9.z = ((tmpvar_8.z * _DepthBand.z) + (tmpvar_8.w * _DepthBand.y));
                tmpvar_9.w = tmpvar_8.w;
                float2 tmpvar_10;
                tmpvar_10 = v.uv.xy;
                tmpvar_3 = tmpvar_10;
                float2 tmpvar_11;
                tmpvar_11 = ((v.uv1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_4 = tmpvar_11;
                float4 tmpvar_12;
                tmpvar_12.w = 1.0;
                tmpvar_12.xyz = _WorldSpaceCameraPos;
                float3 i_13;
                i_13 = (v.vertex.xyz - mul(unity_WorldToObject, tmpvar_12).xyz);
                float4 tmpvar_14;
                tmpvar_14.w = 0.0;
                tmpvar_14.xyz = (i_13 - (2.0 * (dot (tmpvar_1, i_13) * tmpvar_1)));
                float3 tmpvar_15;
                tmpvar_15 = mul(unity_ObjectToWorld, tmpvar_14).xyz;
                tmpvar_5 = tmpvar_15;
                float tmpvar_16;
                tmpvar_16 = clamp (((tmpvar_9.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_17;
                tmpvar_17.xyz = (_FogParams - (_FogParams * tmpvar_16));
                tmpvar_17.w = tmpvar_16;
                fog_2.xyz = tmpvar_17.xyz;
                fog_2.w = (tmpvar_16 * 2.0);
                tmpvar_6 = fog_2;
                o.pos = tmpvar_9;
                o.uv = tmpvar_3;
                o.uv1 = tmpvar_4;
                o.uv4 = tmpvar_5;
                o.uv5 = tmpvar_6;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = ((((tex2D (_MainTex, i.uv).xyz * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.uv1).xyz) + ((texCUBE (_ThemedCube, i.uv4).xyz * (_Reflectivity * 0.5)) * tex2D (_SpecMap, i.uv).xyz)) * i.uv5.w) + i.uv5.xyz);
                return tmpvar_1;
            }
            ENDCG
        }
    }
}