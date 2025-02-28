Shader "Corona/Probe/[Normal] [Spec] [Env] [NoFog] [ViewModel]" {
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
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float3 uv4 : TEXCOORD4;
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
                tmpvar_8.w = 1.0;
                tmpvar_8.xyz = v.vertex.xyz;
                float4 tmpvar_9;
                tmpvar_9 = (UnityObjectToClipPos(tmpvar_8));
                float4 tmpvar_10;
                tmpvar_10.x = tmpvar_9.x;
                tmpvar_10.y = tmpvar_9.y;
                tmpvar_10.z = (tmpvar_9.z);
                tmpvar_10.w = tmpvar_9.w;
                float2 tmpvar_11;
                tmpvar_11 = v.uv.xy;
                tmpvar_3 = tmpvar_11;
                float3 tmpvar_12;
                float3 dir_13;
                dir_13 = _SpecDir;
                float4 tmpvar_14;
                tmpvar_14.w = 0.0;
                tmpvar_14.xyz = dir_13;
                tmpvar_12 = mul(unity_WorldToObject, tmpvar_14).xyz;
                float4 tmpvar_15;
                tmpvar_15.w = 1.0;
                tmpvar_15.xyz = _WorldSpaceCameraPos;
                float3 tmpvar_16;
                tmpvar_16 = (mul(unity_WorldToObject, tmpvar_15).xyz - v.vertex.xyz);
                float4 tmpvar_17;
                tmpvar_17.w = 0.0;
                tmpvar_17.xyz = tmpvar_2;
                float3 tmpvar_18;
                tmpvar_18 = normalize(mul(unity_ObjectToWorld, tmpvar_17).xyz);
                float3 i_19;
                i_19 = -(tmpvar_16);
                float4 tmpvar_20;
                tmpvar_20.w = 0.0;
                tmpvar_20.xyz = (i_19 - (2.0 * (dot (tmpvar_2, i_19) * tmpvar_2)));
                float3 tmpvar_21;
                tmpvar_21 = mul(unity_ObjectToWorld, tmpvar_20).xyz;
                float3 tmpvar_22;
                tmpvar_22 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * v.tangent.w);
                float3 tmpvar_23;
                tmpvar_23.x = dot (tmpvar_16, tmpvar_1.xyz);
                tmpvar_23.y = dot (tmpvar_16, tmpvar_22);
                tmpvar_23.z = dot (tmpvar_16, tmpvar_2);
                float3 tmpvar_24;
                tmpvar_24 = normalize(tmpvar_23);
                tmpvar_5 = tmpvar_24;
                float3 tmpvar_25;
                tmpvar_25.x = dot (tmpvar_12, tmpvar_1.xyz);
                tmpvar_25.y = dot (tmpvar_12, tmpvar_22);
                tmpvar_25.z = dot (tmpvar_12, tmpvar_2);
                tmpvar_6 = tmpvar_25;
                float tmpvar_26;
                tmpvar_26 = max (0.0, dot (tmpvar_18, _SpecDir));
                tmpvar_4.w = tmpvar_26;
                float4 tmpvar_27;
                tmpvar_27.w = 1.0;
                tmpvar_27.xyz = tmpvar_18;
                float3 tmpvar_28;
                float3 x2_29;
                float3 x1_30;
                float4 tmpvar_31;
                tmpvar_31.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_31.w = _AmbientLight.x;
                x1_30.x = dot ((cAr + tmpvar_31), tmpvar_27);
                float4 tmpvar_32;
                tmpvar_32.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_32.w = _AmbientLight.y;
                x1_30.y = dot ((cAg + tmpvar_32), tmpvar_27);
                float4 tmpvar_33;
                tmpvar_33.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_33.w = _AmbientLight.z;
                x1_30.z = dot ((cAb + tmpvar_33), tmpvar_27);
                float4 tmpvar_34;
                tmpvar_34 = (tmpvar_18.xyzz * tmpvar_18.yzzx);
                x2_29.x = dot (cBr, tmpvar_34);
                x2_29.y = dot (cBg, tmpvar_34);
                x2_29.z = dot (cBb, tmpvar_34);
                tmpvar_28 = ((x1_30 + x2_29) + (cC.xyz * ((tmpvar_18.x * tmpvar_18.x) - (tmpvar_18.y * tmpvar_18.y))));
                tmpvar_4.xyz = tmpvar_28;
                tmpvar_7 = tmpvar_21;
                o.pos = tmpvar_10;
                o.uv = tmpvar_3;
                o.uv1 = tmpvar_4;
                o.uv2 = tmpvar_5;
                o.uv3 = tmpvar_6;
                o.uv4 = tmpvar_7;
                
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
                tmpvar_6.xyz = (((tex2D (_MainTex, i.uv).xyz + (tmpvar_2.xxx * tmpvar_1.xyz)) * i.uv1.xyz) + ((texCUBE (_ThemedCube, i.uv4).xyz * _Reflectivity) * tmpvar_1.xyz));
                return tmpvar_6;
            }
            ENDCG
        }
    }
}