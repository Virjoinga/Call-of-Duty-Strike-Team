Shader "Hidden/ProbeFading" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
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
            sampler2D _MainTex;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 uv1 : TEXCOORD1;
                float4 uv5 : TEXCOORD5;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                float3 tmpvar_2;
                float4 tmpvar_3;
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
                float4 tmpvar_8;
                tmpvar_8.w = 0.0;
                tmpvar_8.xyz = normalize(v.normal);
                float3 tmpvar_9;
                tmpvar_9 = normalize(mul(unity_ObjectToWorld, tmpvar_8).xyz);
                float4 tmpvar_10;
                tmpvar_10.w = 1.0;
                tmpvar_10.xyz = tmpvar_9;
                float3 tmpvar_11;
                float3 x2_12;
                float3 x1_13;
                float4 tmpvar_14;
                tmpvar_14.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_14.w = _AmbientLight.x;
                x1_13.x = dot ((cAr + tmpvar_14), tmpvar_10);
                float4 tmpvar_15;
                tmpvar_15.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_15.w = _AmbientLight.y;
                x1_13.y = dot ((cAg + tmpvar_15), tmpvar_10);
                float4 tmpvar_16;
                tmpvar_16.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_16.w = _AmbientLight.z;
                x1_13.z = dot ((cAb + tmpvar_16), tmpvar_10);
                float4 tmpvar_17;
                tmpvar_17 = (tmpvar_9.xyzz * tmpvar_9.yzzx);
                x2_12.x = dot (cBr, tmpvar_17);
                x2_12.y = dot (cBg, tmpvar_17);
                x2_12.z = dot (cBb, tmpvar_17);
                tmpvar_11 = ((x1_13 + x2_12) + (cC.xyz * ((tmpvar_9.x * tmpvar_9.x) - (tmpvar_9.y * tmpvar_9.y))));
                tmpvar_2 = tmpvar_11;
                float tmpvar_18;
                tmpvar_18 = clamp (((tmpvar_6.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_19;
                tmpvar_19.xyz = (_FogParams - (_FogParams * tmpvar_18));
                tmpvar_19.w = tmpvar_18;
                tmpvar_3 = tmpvar_19;
                o.pos = tmpvar_6;
                o.uv = tmpvar_1;
                o.uv1 = tmpvar_2;
                o.uv5 = tmpvar_3;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                tmpvar_1.xyz = (((tex2D (_MainTex, i.uv).xyz * i.uv1) * i.uv5.w) + i.uv5.xyz);
                tmpvar_1.w = _Opacity;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}