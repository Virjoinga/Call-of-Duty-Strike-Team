Shader "Hidden/ProbeSpecmapFading" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _SpecPower ("Specular Power", Range(0,50)) = 10
        _Opacity ("Opacity", Range(0,1)) = 1
    }
    SubShader { 
        Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass {
            Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

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

            float _Opacity;
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
                float3 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float4 uv5 : TEXCOORD5;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float3 tmpvar_1;
                tmpvar_1 = normalize(v.normal);
                float2 tmpvar_2;
                float3 tmpvar_3;
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
                float4 tmpvar_10;
                tmpvar_10.w = 1.0;
                tmpvar_10.xyz = _WorldSpaceCameraPos;
                float4 tmpvar_11;
                tmpvar_11.w = 0.0;
                tmpvar_11.xyz = tmpvar_1;
                float3 tmpvar_12;
                tmpvar_12 = normalize(mul(unity_ObjectToWorld, tmpvar_11).xyz);
                float3 tmpvar_13;
                tmpvar_13 = normalize((mul(unity_WorldToObject, tmpvar_10).xyz - v.vertex.xyz));
                tmpvar_4 = tmpvar_13;
                float4 tmpvar_14;
                tmpvar_14.w = 1.0;
                tmpvar_14.xyz = tmpvar_12;
                float3 tmpvar_15;
                float3 x2_16;
                float3 x1_17;
                float4 tmpvar_18;
                tmpvar_18.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_18.w = _AmbientLight.x;
                x1_17.x = dot ((cAr + tmpvar_18), tmpvar_14);
                float4 tmpvar_19;
                tmpvar_19.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_19.w = _AmbientLight.y;
                x1_17.y = dot ((cAg + tmpvar_19), tmpvar_14);
                float4 tmpvar_20;
                tmpvar_20.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_20.w = _AmbientLight.z;
                x1_17.z = dot ((cAb + tmpvar_20), tmpvar_14);
                float4 tmpvar_21;
                tmpvar_21 = (tmpvar_12.xyzz * tmpvar_12.yzzx);
                x2_16.x = dot (cBr, tmpvar_21);
                x2_16.y = dot (cBg, tmpvar_21);
                x2_16.z = dot (cBb, tmpvar_21);
                tmpvar_15 = ((x1_17 + x2_16) + (cC.xyz * ((tmpvar_12.x * tmpvar_12.x) - (tmpvar_12.y * tmpvar_12.y))));
                tmpvar_3 = tmpvar_15;
                float tmpvar_22;
                tmpvar_22 = clamp (((tmpvar_8.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_23;
                tmpvar_23.xyz = (_FogParams - (_FogParams * tmpvar_22));
                tmpvar_23.w = tmpvar_22;
                tmpvar_5 = tmpvar_23;
                o.pos = tmpvar_8;
                o.uv = tmpvar_2;
                o.uv1 = tmpvar_3;
                o.uv2 = tmpvar_4;
                o.uv3 = tmpvar_1;
                o.uv5 = tmpvar_5;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float3 lightDir_1;
                float3 tmpvar_2;
                float3 dir_3;
                dir_3 = _SpecDir;
                float4 tmpvar_4;
                tmpvar_4.w = 0.0;
                tmpvar_4.xyz = dir_3;
                tmpvar_2 = mul(unity_WorldToObject, tmpvar_4).xyz;
                lightDir_1 = tmpvar_2;
                float tmpvar_5;
                float spec_6;
                float tmpvar_7;
                tmpvar_7 = max (0.0, dot (normalize((lightDir_1 + normalize(i.uv2))), normalize(i.uv3)));
                spec_6 = tmpvar_7;
                float tmpvar_8;
                tmpvar_8 = pow (spec_6, _SpecPower);
                spec_6 = tmpvar_8;
                tmpvar_5 = tmpvar_8;
                float4 tmpvar_9;
                tmpvar_9.xyz = ((((tex2D (_MainTex, i.uv).xyz + (tmpvar_5.xxx * tex2D (_SpecMap, i.uv).xyz)) * i.uv1) * i.uv5.w) + i.uv5.xyz);
                tmpvar_9.w = _Opacity;
                return tmpvar_9;
            }
            ENDCG
        }
    }
}