Shader "Corona/Effects/Datapad [ViewModel]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _PhaseLineTex ("Phase Line Texture", 2D) = "black" {}
        _Reflectivity ("Reflectivity", Range(0,1)) = 0.5
        _BroadSpecular ("Broad Specular", Range(0,1)) = 0.25
        _PhaseLineFrequency ("Phase line frequency", Range(0,5)) = 0.5
        _PhaseLineOverscan ("Phase line overscan", Range(1,2)) = 1.2
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
            float _BroadSpecular;
            float3 _AmbientLight;
            float4 cC;
            float4 cBb;
            float4 cBg;
            float4 cBr;
            float4 cAb;
            float4 cAg;
            float4 cAr;
            float3 _DepthBand;
            float _PhaseLineOverscan;
            float _PhaseLineFrequency;

            float _Reflectivity;
            samplerCUBE _ThemedCube;
            sampler2D _SpecMap;
            sampler2D _MainTex;
            float g_datapadBrightness;
            sampler2D _PhaseLineTex;
            sampler2D _NoiseTex;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 uv2 : TEXCOORD2;
                float3 uv4 : TEXCOORD4;
                float4 uv5 : TEXCOORD5;
                float2 uv6 : TEXCOORD6;
                float2 uv7 : TEXCOORD7;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float3 tmpvar_1;
                tmpvar_1 = normalize(v.normal);
                float2 tmpvar_2;
                float3 tmpvar_3;
                float3 tmpvar_4;
                float4 tmpvar_5;
                float2 tmpvar_6;
                float2 tmpvar_7;
                float4 tmpvar_8;
                tmpvar_8.w = 1.0;
                tmpvar_8.xyz = v.vertex.xyz;
                float4 tmpvar_9;
                tmpvar_9 = (UnityObjectToClipPos(tmpvar_8));
                float4 tmpvar_10;
                tmpvar_10.x = tmpvar_9.x;
                tmpvar_10.y = tmpvar_9.y;
                tmpvar_10.z = (tmpvar_9.z * _DepthBand.x);
                tmpvar_10.w = tmpvar_9.w;
                float2 tmpvar_11;
                tmpvar_11 = v.uv.xy;
                tmpvar_2 = tmpvar_11;
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
                float4 tmpvar_16;
                tmpvar_16.w = 1.0;
                tmpvar_16.xyz = tmpvar_15;
                float3 x2_17;
                float3 x1_18;
                float4 tmpvar_19;
                tmpvar_19.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_19.w = _AmbientLight.x;
                x1_18.x = dot ((cAr + tmpvar_19), tmpvar_16);
                float4 tmpvar_20;
                tmpvar_20.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_20.w = _AmbientLight.y;
                x1_18.y = dot ((cAg + tmpvar_20), tmpvar_16);
                float4 tmpvar_21;
                tmpvar_21.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_21.w = _AmbientLight.z;
                x1_18.z = dot ((cAb + tmpvar_21), tmpvar_16);
                float4 tmpvar_22;
                tmpvar_22 = (tmpvar_15.xyzz * tmpvar_15.yzzx);
                x2_17.x = dot (cBr, tmpvar_22);
                x2_17.y = dot (cBg, tmpvar_22);
                x2_17.z = dot (cBb, tmpvar_22);
                float3 tmpvar_23;
                tmpvar_23 = (((x1_18 + x2_17) + (cC.xyz * ((tmpvar_15.x * tmpvar_15.x) - (tmpvar_15.y * tmpvar_15.y)))) * _BroadSpecular);
                tmpvar_3 = tmpvar_23;
                tmpvar_4 = tmpvar_15;
                float tmpvar_24;
                tmpvar_24 = clamp (((tmpvar_10.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_25;
                tmpvar_25.xyz = (_FogParams - (_FogParams * tmpvar_24));
                tmpvar_25.w = tmpvar_24;
                tmpvar_5 = tmpvar_25;
                float2 tmpvar_26;
                tmpvar_26.x = frac((157.079 * _Time.y));
                tmpvar_26.y = frac((135.914 * _Time.y));
                float2 tmpvar_27;
                tmpvar_27 = ((v.uv.xy * 2.0) + tmpvar_26);
                tmpvar_6 = tmpvar_27;
                float2 tmpvar_28;
                tmpvar_28 = (((((v.uv.y * 1024.0) / 245.0) - 4.17959) + (frac((_Time.y * _PhaseLineFrequency)) * _PhaseLineOverscan)));
                tmpvar_7 = tmpvar_28;
                o.pos = tmpvar_10;
                o.uv = tmpvar_2;
                o.uv2 = tmpvar_3;
                o.uv4 = tmpvar_4;
                o.uv5 = tmpvar_5;
                o.uv7 = tmpvar_6;
                o.uv6 = tmpvar_7;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 diffusemap_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.uv);
                diffusemap_1.w = tmpvar_2.w;
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_SpecMap, i.uv);
                diffusemap_1.xyz = (tmpvar_2.xyz + tex2D (_PhaseLineTex, i.uv6).xyz);
                diffusemap_1.xyz = (diffusemap_1.xyz * (tex2D (_NoiseTex, i.uv7).xyz * g_datapadBrightness));
                float4 tmpvar_4;
                tmpvar_4.w = 1.0;
                tmpvar_4.xyz = ((((diffusemap_1.xyz + (i.uv2 * tmpvar_3.xyz)) + ((texCUBE (_ThemedCube, i.uv4).xyz * _Reflectivity) * tmpvar_3.xyz)) * i.uv5.w) + i.uv5.xyz);
                return tmpvar_4;
            }
            ENDCG
        }
    }
}