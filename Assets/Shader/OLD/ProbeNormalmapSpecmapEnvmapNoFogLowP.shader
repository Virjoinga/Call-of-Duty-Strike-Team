Shader "Corona/Probe/[Normal] [Spec] [Env] [NoFog] [LowP]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
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
            float3 _SpecDir;
            
            float _Reflectivity;
            samplerCUBE _ThemedCube;
            sampler2D _SpecMap;
            sampler2D _BumpMap;
            sampler2D _MainTex;
            float _SpecPower;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
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
                
                float4 tmpvar_1;
                tmpvar_1.xyz = normalize(v.tangent.xyz);
                tmpvar_1.w = v.tangent.w;
                float3 tmpvar_2;
                tmpvar_2 = normalize(v.normal);
                float2 tmpvar_3;
                float4 tmpvar_4;
                float3 tmpvar_5;
                float3 tmpvar_6;
                float3 tmpvar_7;
                float4 tmpvar_8;
                float4 tmpvar_9;
                tmpvar_9.w = 1.0;
                tmpvar_9.xyz = v.vertex.xyz;
                float4 tmpvar_10;
                tmpvar_10 = (UnityObjectToClipPos(tmpvar_9));
                float4 tmpvar_11;
                tmpvar_11.x = tmpvar_10.x;
                tmpvar_11.y = tmpvar_10.y;
                tmpvar_11.z = ((tmpvar_10.z * _DepthBand.z) + (tmpvar_10.w * _DepthBand.y));
                tmpvar_11.w = tmpvar_10.w;
                float2 tmpvar_12;
                tmpvar_12 = v.uv.xy;
                tmpvar_3 = tmpvar_12;
                float3 tmpvar_13;
                float3 dir_14;
                dir_14 = _SpecDir;
                float4 tmpvar_15;
                tmpvar_15.w = 0.0;
                tmpvar_15.xyz = dir_14;
                tmpvar_13 = mul(unity_WorldToObject, tmpvar_15).xyz;
                float4 tmpvar_16;
                tmpvar_16.w = 1.0;
                tmpvar_16.xyz = _WorldSpaceCameraPos;
                float3 tmpvar_17;
                tmpvar_17 = (mul(unity_WorldToObject, tmpvar_16).xyz - v.vertex.xyz);
                float4 tmpvar_18;
                tmpvar_18.w = 0.0;
                tmpvar_18.xyz = tmpvar_2;
                float3 tmpvar_19;
                tmpvar_19 = normalize(mul(unity_ObjectToWorld, tmpvar_18).xyz);
                float3 i_20;
                i_20 = -(tmpvar_17);
                float4 tmpvar_21;
                tmpvar_21.w = 0.0;
                tmpvar_21.xyz = (i_20 - (2.0 * (dot (tmpvar_2, i_20) * tmpvar_2)));
                float3 tmpvar_22;
                tmpvar_22 = mul(unity_ObjectToWorld, tmpvar_21).xyz;
                float3 tmpvar_23;
                tmpvar_23 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * v.tangent.w);
                float3 tmpvar_24;
                tmpvar_24.x = dot (tmpvar_17, tmpvar_1.xyz);
                tmpvar_24.y = dot (tmpvar_17, tmpvar_23);
                tmpvar_24.z = dot (tmpvar_17, tmpvar_2);
                float3 tmpvar_25;
                tmpvar_25 = normalize(tmpvar_24);
                tmpvar_5 = tmpvar_25;
                float3 tmpvar_26;
                tmpvar_26.x = dot (tmpvar_13, tmpvar_1.xyz);
                tmpvar_26.y = dot (tmpvar_13, tmpvar_23);
                tmpvar_26.z = dot (tmpvar_13, tmpvar_2);
                tmpvar_6 = tmpvar_26;
                float tmpvar_27;
                tmpvar_27 = max (0.0, dot (tmpvar_19, _SpecDir));
                tmpvar_4.w = tmpvar_27;
                float4 tmpvar_28;
                tmpvar_28.w = 1.0;
                tmpvar_28.xyz = tmpvar_19;
                float3 tmpvar_29;
                float3 x2_30;
                float3 x1_31;
                float4 tmpvar_32;
                tmpvar_32.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_32.w = _AmbientLight.x;
                x1_31.x = dot ((cAr + tmpvar_32), tmpvar_28);
                float4 tmpvar_33;
                tmpvar_33.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_33.w = _AmbientLight.y;
                x1_31.y = dot ((cAg + tmpvar_33), tmpvar_28);
                float4 tmpvar_34;
                tmpvar_34.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_34.w = _AmbientLight.z;
                x1_31.z = dot ((cAb + tmpvar_34), tmpvar_28);
                float4 tmpvar_35;
                tmpvar_35 = (tmpvar_19.xyzz * tmpvar_19.yzzx);
                x2_30.x = dot (cBr, tmpvar_35);
                x2_30.y = dot (cBg, tmpvar_35);
                x2_30.z = dot (cBb, tmpvar_35);
                tmpvar_29 = ((x1_31 + x2_30) + (cC.xyz * ((tmpvar_19.x * tmpvar_19.x) - (tmpvar_19.y * tmpvar_19.y))));
                tmpvar_4.xyz = tmpvar_29;
                tmpvar_7 = tmpvar_22;
                float tmpvar_36;
                tmpvar_36 = clamp (((tmpvar_11.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_37;
                tmpvar_37.xyz = (_FogParams - (_FogParams * tmpvar_36));
                tmpvar_37.w = tmpvar_36;
                tmpvar_8 = tmpvar_37;
                o.pos = tmpvar_11;
                o.uv = tmpvar_3;
                o.uv1 = tmpvar_4;
                o.uv2 = tmpvar_5;
                o.uv3 = tmpvar_6;
                o.uv4 = tmpvar_7;
                o.uv5 = tmpvar_8;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                tmpvar_1 = tex2D (_SpecMap, i.uv);
                float tmpvar_2;
                float spec_3;
                float tmpvar_4;
                tmpvar_4 = max (0.0, dot (normalize((normalize(i.uv3) + normalize(i.uv2))), normalize((tex2D (_BumpMap, i.uv).xyz - 0.5))));
                spec_3 = tmpvar_4;
                float tmpvar_5;
                tmpvar_5 = pow (spec_3, _SpecPower);
                spec_3 = tmpvar_5;
                tmpvar_2 = tmpvar_5;
                float4 tmpvar_6;
                tmpvar_6.w = 1.0;
                tmpvar_6.xyz = (((((tex2D (_MainTex, i.uv).xyz + (tmpvar_2.xxx * tmpvar_1.xyz)) * i.uv1.xyz) + ((texCUBE (_ThemedCube, i.uv4).xyz * _Reflectivity) * tmpvar_1.xyz)) * i.uv5.w) + i.uv5.xyz);
                return tmpvar_6;
            }
            ENDCG
        }
    }
}