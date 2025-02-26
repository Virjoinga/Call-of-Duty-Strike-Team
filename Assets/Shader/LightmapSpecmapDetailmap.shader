// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Corona/Lightmap/[Spec] [Detail]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _SpecPower ("Specular Power", Float) = 10
        _DetailTex ("DetailMap(RGB)", 2D) = "white" {}
        _DetailPower ("Detail Power", Float) = 0.5
    }
    SubShader { 
        LOD 200
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

            float _DetailPower;
            sampler2D _DetailTex;
            sampler2D _SpecMap;
            sampler2D _MainTex;
            float _SpecPower;
            float3 _SpecDir;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float4 uv5 : TEXCOORD5;
                float2 uv6 : TEXCOORD6;
            };

            v2f vert(appdata_t v)
            {
                v2f o;

                float4 fog_1;
                float2 tmpvar_2;
                float2 tmpvar_3;
                float3 tmpvar_4;
                float4 tmpvar_5;
                float4 tmpvar_6;
                tmpvar_6.w = 1.0;
                tmpvar_6.xyz = v.vertex.xyz;
                float4 tmpvar_7;
                tmpvar_7 = (UnityObjectToClipPos(tmpvar_6));
                float4 tmpvar_8;
                tmpvar_8.x = tmpvar_7.x;
                tmpvar_8.y = tmpvar_7.y;
                tmpvar_8.z = ((tmpvar_7.z * _DepthBand.z) + (tmpvar_7.w * _DepthBand.y));
                tmpvar_8.w = tmpvar_7.w;
                float2 tmpvar_9;
                tmpvar_9 = v.uv.xy;
                tmpvar_2 = tmpvar_9;
                float2 tmpvar_10;
                tmpvar_10 = ((v.uv1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_3 = tmpvar_10;
                float4 tmpvar_11;
                tmpvar_11.w = 1.0;
                tmpvar_11.xyz = _WorldSpaceCameraPos;
                float3 tmpvar_12;
                tmpvar_12 = normalize((mul(unity_WorldToObject, tmpvar_11).xyz - v.vertex.xyz));
                tmpvar_4 = tmpvar_12;
                float tmpvar_13;
                tmpvar_13 = clamp (((tmpvar_8.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_14;
                tmpvar_14.xyz = (_FogParams - (_FogParams * tmpvar_13));
                tmpvar_14.w = tmpvar_13;
                fog_1.xyz = tmpvar_14.xyz;
                fog_1.w = (tmpvar_13 * 2.0);
                tmpvar_5 = fog_1;
                o.pos = tmpvar_8;
                o.uv = tmpvar_2;
                o.uv1 = tmpvar_3;
                o.uv2 = tmpvar_4;
                o.uv3 = normalize(v.normal);
                o.uv5 = tmpvar_5;
                o.uv6 = (v.uv.xy * 20.0);

                return o;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                float detailPower_1;
                float3 detailCol_2;
                float4 diffusemap_3;
                float4 tmpvar_4;
                tmpvar_4 = tex2D (_MainTex, i.uv);
                diffusemap_3.w = tmpvar_4.w;
                float4 tmpvar_5;
                tmpvar_5 = tex2D (_SpecMap, i.uv);
                float3 lightDir_6;
                float3 tmpvar_7;
                float3 dir_8;
                dir_8 = _SpecDir;
                float4 tmpvar_9;
                tmpvar_9.w = 0.0;
                tmpvar_9.xyz = dir_8;
                tmpvar_7 = (mul(unity_WorldToObject, tmpvar_9)).xyz;
                lightDir_6 = tmpvar_7;
                float tmpvar_10;
                float spec_11;
                float tmpvar_12;
                tmpvar_12 = max (0.0, dot (normalize((lightDir_6 + normalize(i.uv2))), normalize(i.uv3)));
                spec_11 = tmpvar_12;
                float tmpvar_13;
                tmpvar_13 = pow (spec_11, _SpecPower);
                spec_11 = tmpvar_13;
                tmpvar_10 = tmpvar_13;
                float3 tmpvar_14;
                tmpvar_14 = tmpvar_5.www;
                detailCol_2 = tmpvar_14;
                float4 tmpvar_15;
                float2 P_16;
                P_16 = (i.uv6 - floor(i.uv6));
                tmpvar_15 = tex2D (_DetailTex, P_16);
                float3 tmpvar_17;
                tmpvar_17 = clamp ((detailCol_2 + tmpvar_15.xyz), 0.0, 1.0);
                detailCol_2 = tmpvar_17;
                float tmpvar_18;
                tmpvar_18 = (1.0 + (((1.0 - tmpvar_5.w) * (1.0 - tmpvar_5.w)) * _DetailPower));
                detailPower_1 = tmpvar_18;
                float3 tmpvar_19;
                tmpvar_19 = clamp (((tmpvar_4.xyz * tmpvar_17) * detailPower_1), 0.0, 1.0);
                diffusemap_3.xyz = tmpvar_19;
                float4 tmpvar_20;
                tmpvar_20.w = 1.0;
                tmpvar_20.xyz = ((((diffusemap_3.xyz + (tmpvar_10.xxx * tmpvar_5.xyz)) * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.uv1).xyz) * i.uv5.w) + i.uv5.xyz);
                return tmpvar_20;
            }
            ENDCG
        }
    }
}
