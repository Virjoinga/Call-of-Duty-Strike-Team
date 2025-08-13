Shader "Corona/Probe/[Normal] [Spec] [NoFog] [ViewModel]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _SpecPower ("Specular Power", Range(0,50)) = 10
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
                float3 uv2 : TEXCOORD2;
                float3 uv3 : TEXCOORD3;
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
                float4 tmpvar_7;
                tmpvar_7.w = 1.0;
                tmpvar_7.xyz = v.vertex.xyz;
                float4 tmpvar_8;
                tmpvar_8 = (UnityObjectToClipPos(tmpvar_7));
                float4 tmpvar_9;
                tmpvar_9.x = tmpvar_8.x;
                tmpvar_9.y = tmpvar_8.y;
                tmpvar_9.z = (tmpvar_8.z);
                tmpvar_9.w = tmpvar_8.w;
                float2 tmpvar_10;
                tmpvar_10 = v.uv.xy;
                tmpvar_3 = tmpvar_10;
                float3 tmpvar_11;
                float3 dir_12;
                dir_12 = _SpecDir;
                float4 tmpvar_13;
                tmpvar_13.w = 0.0;
                tmpvar_13.xyz = dir_12;
                tmpvar_11 = mul(unity_WorldToObject, tmpvar_13).xyz;
                float4 tmpvar_14;
                tmpvar_14.w = 1.0;
                tmpvar_14.xyz = _WorldSpaceCameraPos;
                float3 tmpvar_15;
                tmpvar_15 = (mul(unity_WorldToObject, tmpvar_14).xyz - v.vertex.xyz);
                float4 tmpvar_16;
                tmpvar_16.w = 0.0;
                tmpvar_16.xyz = tmpvar_2;
                float3 tmpvar_17;
                tmpvar_17 = normalize(mul(unity_ObjectToWorld, tmpvar_16).xyz);
                float3 tmpvar_18;
                tmpvar_18 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * v.tangent.w);
                float3 tmpvar_19;
                tmpvar_19.x = dot (tmpvar_15, tmpvar_1.xyz);
                tmpvar_19.y = dot (tmpvar_15, tmpvar_18);
                tmpvar_19.z = dot (tmpvar_15, tmpvar_2);
                float3 tmpvar_20;
                tmpvar_20 = normalize(tmpvar_19);
                tmpvar_5 = tmpvar_20;
                float3 tmpvar_21;
                tmpvar_21.x = dot (tmpvar_11, tmpvar_1.xyz);
                tmpvar_21.y = dot (tmpvar_11, tmpvar_18);
                tmpvar_21.z = dot (tmpvar_11, tmpvar_2);
                tmpvar_6 = tmpvar_21;
                float tmpvar_22;
                tmpvar_22 = max (0.0, dot (tmpvar_17, _SpecDir));
                tmpvar_4.w = tmpvar_22;
                float4 tmpvar_23;
                tmpvar_23.w = 1.0;
                tmpvar_23.xyz = tmpvar_17;
                float3 tmpvar_24;
                float3 x2_25;
                float3 x1_26;
                float4 tmpvar_27;
                tmpvar_27.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_27.w = _AmbientLight.x;
                x1_26.x = dot ((cAr + tmpvar_27), tmpvar_23);
                float4 tmpvar_28;
                tmpvar_28.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_28.w = _AmbientLight.y;
                x1_26.y = dot ((cAg + tmpvar_28), tmpvar_23);
                float4 tmpvar_29;
                tmpvar_29.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_29.w = _AmbientLight.z;
                x1_26.z = dot ((cAb + tmpvar_29), tmpvar_23);
                float4 tmpvar_30;
                tmpvar_30 = (tmpvar_17.xyzz * tmpvar_17.yzzx);
                x2_25.x = dot (cBr, tmpvar_30);
                x2_25.y = dot (cBg, tmpvar_30);
                x2_25.z = dot (cBb, tmpvar_30);
                tmpvar_24 = ((x1_26 + x2_25) + (cC.xyz * ((tmpvar_17.x * tmpvar_17.x) - (tmpvar_17.y * tmpvar_17.y))));
                tmpvar_4.xyz = tmpvar_24;
                o.pos = tmpvar_9;
                o.uv = tmpvar_3;
                o.uv1 = tmpvar_4;
                o.uv2 = tmpvar_5;
                o.uv3 = tmpvar_6;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float tmpvar_1;
                float spec_2;
                float tmpvar_3;
                tmpvar_3 = max (0.0, dot (normalize((normalize(i.uv3) + normalize(i.uv2))), normalize((tex2D (_BumpMap, i.uv).xyz - 0.5))));
                spec_2 = tmpvar_3;
                float tmpvar_4;
                tmpvar_4 = pow (spec_2, _SpecPower);
                spec_2 = tmpvar_4;
                tmpvar_1 = tmpvar_4;
                float4 tmpvar_5;
                tmpvar_5.w = 1.0;
                tmpvar_5.xyz = ((tex2D (_MainTex, i.uv).xyz + (tmpvar_1.xxx * tex2D (_SpecMap, i.uv).xyz)) * i.uv1.xyz);
                return tmpvar_5;
            }
            ENDCG
        }
    }
}