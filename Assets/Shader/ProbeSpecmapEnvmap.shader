Shader "Corona/Probe/[Spec] [Env]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _SpecPower ("Specular Power", Range(0,50)) = 10
        _Reflectivity ("Reflectivity", Range(0,1)) = 0.2
    }
    SubShader { 
        Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
        Pass {
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _FogRange;
            float3 _FogParams;
            float3 _AmbientLight;
            float4 cC;
            float4 cBb;
            float4 cBg;
            float4 cBr;
            float4 cAb;
            float4 cAg;
            float4 cAr;
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
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 uv1 : TEXCOORD1;
                float3 uv2 : TEXCOORD2;
                float3 uv3 : TEXCOORD3;
                float3 uv4 : TEXCOORD4;
                float4 uv5 : TEXCOORD5;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float3 tmpvar_1;
                tmpvar_1 = normalize(v.normal);
                float2 tmpvar_2;
                float3 tmpvar_3;
                float3 tmpvar_4;
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
                tmpvar_2 = tmpvar_10;
                float4 tmpvar_11;
                tmpvar_11.w = 1.0;
                tmpvar_11.xyz = _WorldSpaceCameraPos;
                float3 tmpvar_12;
                tmpvar_12 = (mul(unity_WorldToObject, tmpvar_11).xyz - v.vertex.xyz);
                float4 tmpvar_13;
                tmpvar_13.w = 0.0;
                tmpvar_13.xyz = tmpvar_1;
                float3 tmpvar_14;
                tmpvar_14 = normalize(mul(unity_ObjectToWorld, tmpvar_13).xyz);
                float3 i_15;
                i_15 = -(tmpvar_12);
                float4 tmpvar_16;
                tmpvar_16.w = 0.0;
                tmpvar_16.xyz = (i_15 - (2.0 * (dot (tmpvar_1, i_15) * tmpvar_1)));
                float3 tmpvar_17;
                tmpvar_17 = mul(unity_ObjectToWorld, tmpvar_16).xyz;
                float3 tmpvar_18;
                tmpvar_18 = normalize(tmpvar_12);
                tmpvar_4 = tmpvar_18;
                float4 tmpvar_19;
                tmpvar_19.w = 1.0;
                tmpvar_19.xyz = tmpvar_14;
                float3 tmpvar_20;
                float3 x2_21;
                float3 x1_22;
                float4 tmpvar_23;
                tmpvar_23.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_23.w = _AmbientLight.x;
                x1_22.x = dot ((cAr + tmpvar_23), tmpvar_19);
                float4 tmpvar_24;
                tmpvar_24.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_24.w = _AmbientLight.y;
                x1_22.y = dot ((cAg + tmpvar_24), tmpvar_19);
                float4 tmpvar_25;
                tmpvar_25.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_25.w = _AmbientLight.z;
                x1_22.z = dot ((cAb + tmpvar_25), tmpvar_19);
                float4 tmpvar_26;
                tmpvar_26 = (tmpvar_14.xyzz * tmpvar_14.yzzx);
                x2_21.x = dot (cBr, tmpvar_26);
                x2_21.y = dot (cBg, tmpvar_26);
                x2_21.z = dot (cBb, tmpvar_26);
                tmpvar_20 = ((x1_22 + x2_21) + (cC.xyz * ((tmpvar_14.x * tmpvar_14.x) - (tmpvar_14.y * tmpvar_14.y))));
                tmpvar_3 = tmpvar_20;
                tmpvar_5 = tmpvar_17;
                float tmpvar_27;
                tmpvar_27 = clamp (((tmpvar_9.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_28;
                tmpvar_28.xyz = (_FogParams - (_FogParams * tmpvar_27));
                tmpvar_28.w = tmpvar_27;
                tmpvar_6 = tmpvar_28;
                o.pos = tmpvar_9;
                o.uv = tmpvar_2;
                o.uv1 = tmpvar_3;
                o.uv2 = tmpvar_4;
                o.uv3 = tmpvar_1;
                o.uv4 = tmpvar_5;
                o.uv5 = tmpvar_6;
                
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
                tmpvar_10.xyz = (((((tex2D (_MainTex, i.uv).xyz + (tmpvar_6.xxx * tmpvar_1.xyz)) * i.uv1) + ((texCUBE (_ThemedCube, i.uv4).xyz * _Reflectivity) * tmpvar_1.xyz)) * i.uv5.w) + i.uv5.xyz);
                return tmpvar_10;
            }
            ENDCG
        }
    }
}