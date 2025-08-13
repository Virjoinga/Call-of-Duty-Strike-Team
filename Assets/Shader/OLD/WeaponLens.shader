Shader "Corona/Effects/WeaponLens [ViewModel]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _Cube ("Reflection Cubemap", CUBE) = "_Skybox" {}
        _SpecPower ("Specular Power", Range(0,500)) = 10
        _InternalSpecTint ("Internal Specular Tint", Color) = (1,1,1,1)
        _Reflectivity ("Reflectivity", Range(0,1)) = 0.2
        _LensCurvature ("Lens Curvature", Range(0,2)) = 1
    }
    SubShader { 
        Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
        Pass {
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float3 _InternalSpecTint;
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
            samplerCUBE _Cube;
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
                float3 uv2 : TEXCOORD2;
                float3 uv3 : TEXCOORD3;
                float3 uv4 : TEXCOORD4;
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
                tmpvar_6.w = 1.0;
                tmpvar_6.xyz = v.vertex.xyz;
                float4 tmpvar_7;
                tmpvar_7 = (UnityObjectToClipPos(tmpvar_6));
                float4 tmpvar_8;
                tmpvar_8.x = tmpvar_7.x;
                tmpvar_8.y = tmpvar_7.y;
                tmpvar_8.z = (tmpvar_7.z * _DepthBand.x);
                tmpvar_8.w = tmpvar_7.w;
                float2 tmpvar_9;
                tmpvar_9 = v.uv.xy;
                tmpvar_2 = tmpvar_9;
                float4 tmpvar_10;
                tmpvar_10.w = 1.0;
                tmpvar_10.xyz = v.vertex.xyz;
                float4 tmpvar_11;
                tmpvar_11.w = 0.0;
                tmpvar_11.xyz = tmpvar_1;
                float3 tmpvar_12;
                tmpvar_12 = mul(unity_ObjectToWorld, tmpvar_11).xyz;
                float4 v_13;
                v_13.x = unity_WorldToObject[0].x;
                v_13.y = unity_WorldToObject[1].x;
                v_13.z = unity_WorldToObject[2].x;
                v_13.w = unity_WorldToObject[3].x;
                float3 tmpvar_14;
                float3 i_15;
                i_15 = -(tmpvar_12);
                tmpvar_14 = (i_15 - (2.0 * (dot (v_13.xyz, i_15) * v_13.xyz)));
                float3 tmpvar_16;
                tmpvar_16 = normalize((_WorldSpaceCameraPos - mul(unity_ObjectToWorld, tmpvar_10).xyz));
                float3 tmpvar_17;
                float3 i_18;
                i_18 = -(tmpvar_16);
                tmpvar_17 = (i_18 - (2.0 * (dot (tmpvar_12, i_18) * tmpvar_12)));
                float3 tmpvar_19;
                float3 i_20;
                i_20 = -(tmpvar_16);
                tmpvar_19 = (i_20 - (2.0 * (dot (tmpvar_14, i_20) * tmpvar_14)));
                tmpvar_3 = tmpvar_17;
                float4 tmpvar_21;
                tmpvar_21.w = 1.0;
                tmpvar_21.xyz = tmpvar_17;
                float3 tmpvar_22;
                float3 x2_23;
                float3 x1_24;
                float4 tmpvar_25;
                tmpvar_25.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_25.w = _AmbientLight.x;
                x1_24.x = dot ((cAr + tmpvar_25), tmpvar_21);
                float4 tmpvar_26;
                tmpvar_26.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_26.w = _AmbientLight.y;
                x1_24.y = dot ((cAg + tmpvar_26), tmpvar_21);
                float4 tmpvar_27;
                tmpvar_27.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_27.w = _AmbientLight.z;
                x1_24.z = dot ((cAb + tmpvar_27), tmpvar_21);
                float4 tmpvar_28;
                tmpvar_28 = (tmpvar_17.xyzz * tmpvar_17.yzzx);
                x2_23.x = dot (cBr, tmpvar_28);
                x2_23.y = dot (cBg, tmpvar_28);
                x2_23.z = dot (cBb, tmpvar_28);
                tmpvar_22 = ((x1_24 + x2_23) + (cC.xyz * ((tmpvar_17.x * tmpvar_17.x) - (tmpvar_17.y * tmpvar_17.y))));
                tmpvar_4 = tmpvar_22;
                float4 tmpvar_29;
                tmpvar_29.w = 1.0;
                tmpvar_29.xyz = tmpvar_19;
                float3 x2_30;
                float3 x1_31;
                float4 tmpvar_32;
                tmpvar_32.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_32.w = _AmbientLight.x;
                x1_31.x = dot ((cAr + tmpvar_32), tmpvar_29);
                float4 tmpvar_33;
                tmpvar_33.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_33.w = _AmbientLight.y;
                x1_31.y = dot ((cAg + tmpvar_33), tmpvar_29);
                float4 tmpvar_34;
                tmpvar_34.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_34.w = _AmbientLight.z;
                x1_31.z = dot ((cAb + tmpvar_34), tmpvar_29);
                float4 tmpvar_35;
                tmpvar_35 = (tmpvar_19.xyzz * tmpvar_19.yzzx);
                x2_30.x = dot (cBr, tmpvar_35);
                x2_30.y = dot (cBg, tmpvar_35);
                x2_30.z = dot (cBb, tmpvar_35);
                float3 tmpvar_36;
                tmpvar_36 = ((((x1_31 + x2_30) + (cC.xyz * ((tmpvar_19.x * tmpvar_19.x) - (tmpvar_19.y * tmpvar_19.y)))) * _InternalSpecTint) * sqrt((1.0 - (tmpvar_1.x * tmpvar_1.x))));
                tmpvar_5 = tmpvar_36;
                o.pos = tmpvar_8;
                o.uv = tmpvar_2;
                o.uv2 = tmpvar_3;
                o.uv3 = tmpvar_4;
                o.uv4 = tmpvar_5;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float specDotPow_1;
                float specDot_2;
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_SpecMap, i.uv);
                float tmpvar_4;
                tmpvar_4 = clamp (dot (normalize(i.uv2), _SpecDir), 0.0, 1.0);
                specDot_2 = tmpvar_4;
                float tmpvar_5;
                tmpvar_5 = pow (specDot_2, _SpecPower);
                specDotPow_1 = tmpvar_5;
                float4 tmpvar_6;
                tmpvar_6.w = 1.0;
                tmpvar_6.xyz = (tex2D (_MainTex, i.uv).xyz + (((specDotPow_1 * i.uv3) + i.uv4) + ((tmpvar_3.xyz * tmpvar_3.xyz) * ((texCUBE (_Cube, i.uv2) * _Reflectivity).xyz + i.uv3))));
                return tmpvar_6;
            }
            ENDCG
        }
    }
}