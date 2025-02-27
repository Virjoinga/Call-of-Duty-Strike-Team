Shader "Hidden/ProbeSpecularPixelCheap" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        cAr ("Light Probe Ar", Color) = (1,1,1,1)
        cAg ("Light Probe Ag", Color) = (1,1,1,1)
        cAb ("Light Probe Ab", Color) = (1,1,1,1)
        cBr ("Light Probe Br", Color) = (1,1,1,1)
        cBg ("Light Probe Bg", Color) = (1,1,1,1)
        cBb ("Light Probe Bb", Color) = (1,1,1,1)
        cC ("Light Probe C", Color) = (1,1,1,1)
        _SpecDir ("Specular Dir", Vector) = (-0.66,0.333,0.66,1)
    }
    SubShader { 
        LOD 200
        Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
        Pass {
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
            Fog {
                Mode Linear
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 cC;
            float4 cBb;
            float4 cBg;
            float4 cBr;
            float4 cAb;
            float4 cAg;
            float4 cAr;
            float3 _DepthBand;

            sampler2D _MainTex;
            float3 _SpecDir;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                float4 tmpvar_2;
                float3 tmpvar_3;
                float4 tmpvar_4;
                tmpvar_4.w = 1.0;
                tmpvar_4.xyz = v.vertex.xyz;
                float4 tmpvar_5;
                tmpvar_5 = (UnityObjectToClipPos(tmpvar_4));
                float4 tmpvar_6;
                tmpvar_6.x = tmpvar_5.x;
                tmpvar_6.y = tmpvar_5.y;
                tmpvar_6.z = ((tmpvar_5.z * _DepthBand.z) + (tmpvar_5.w * _DepthBand.y));
                tmpvar_6.w = tmpvar_5.w;
                float2 tmpvar_7;
                tmpvar_7 = v.uv.xy;
                tmpvar_1 = tmpvar_7;
                float3x3 tmpvar_8;
                tmpvar_8[0] = unity_ObjectToWorld[0].xyz;
                tmpvar_8[1] = unity_ObjectToWorld[1].xyz;
                tmpvar_8[2] = unity_ObjectToWorld[2].xyz;
                float3 tmpvar_9;
                tmpvar_9 = mul(tmpvar_8, normalize(v.normal));
                float4 tmpvar_10;
                tmpvar_10.w = 1.0;
                tmpvar_10.xyz = tmpvar_9;
                float3 tmpvar_11;
                float4 normal_12;
                normal_12 = tmpvar_10;
                float vC_13;
                float3 x3_14;
                float3 x2_15;
                float3 x1_16;
                float tmpvar_17;
                tmpvar_17 = dot (cAr, normal_12);
                x1_16.x = tmpvar_17;
                float tmpvar_18;
                tmpvar_18 = dot (cAg, normal_12);
                x1_16.y = tmpvar_18;
                float tmpvar_19;
                tmpvar_19 = dot (cAb, normal_12);
                x1_16.z = tmpvar_19;
                float4 tmpvar_20;
                tmpvar_20 = (normal_12.xyzz * normal_12.yzzx);
                float tmpvar_21;
                tmpvar_21 = dot (cBr, tmpvar_20);
                x2_15.x = tmpvar_21;
                float tmpvar_22;
                tmpvar_22 = dot (cBg, tmpvar_20);
                x2_15.y = tmpvar_22;
                float tmpvar_23;
                tmpvar_23 = dot (cBb, tmpvar_20);
                x2_15.z = tmpvar_23;
                float tmpvar_24;
                tmpvar_24 = ((normal_12.x * normal_12.x) - (normal_12.y * normal_12.y));
                vC_13 = tmpvar_24;
                float3 tmpvar_25;
                tmpvar_25 = (cC.xyz * vC_13);
                x3_14 = tmpvar_25;
                tmpvar_11 = ((x1_16 + x2_15) + x3_14);
                float3 tmpvar_26;
                tmpvar_26 = (tmpvar_11 + (2.0 * UNITY_LIGHTMODEL_AMBIENT).xyz);
                tmpvar_2.xyz = tmpvar_26;
                tmpvar_2.w = ((tmpvar_2.x + tmpvar_2.y) + tmpvar_2.z);
                float3 tmpvar_27;
                tmpvar_27 = (mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos);
                float3 tmpvar_28;
                tmpvar_28 = (tmpvar_27 - (tmpvar_9 * (2.0 * dot (tmpvar_27, tmpvar_9))));
                tmpvar_3 = tmpvar_28;
                o.pos = tmpvar_6;
                o.uv = tmpvar_1;
                o.uv1 = tmpvar_2;
                o.uv2 = tmpvar_3;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 col_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.uv);
                col_1.xyz = (tmpvar_2.xyz * i.uv1.xyz);
                col_1.w = (tmpvar_2.w * clamp ((i.uv1.w - 1.0), 0.0, 1.0));
                col_1.xyz = (col_1.xyz + (pow (max (dot (normalize(i.uv2), _SpecDir), 0.0), 10.0) * col_1.w));
                return col_1;
            }
            ENDCG
        }
    }
}