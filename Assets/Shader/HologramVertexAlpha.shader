Shader "Corona/Effects/Hologram Vertex Alpha" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseTexRate ("Noise Tex Rate", Range(0,4)) = 0.85
        _NoiseRate ("Noise Scroll Rate", Range(0,1)) = 0.5
        _ShimmerIntensity ("Shimmer Intensity", Range(0,0.2)) = 0.125
        _ShimmerRate ("Shimmer Rate", Range(0,400)) = 275
        _ZBias ("Z Bias", Float) = -0.0001
    }
    SubShader { 
        Tags { "QUEUE"="Transparent-1" }
        UsePass "Corona/Effects/HologramZFill/PASS"
        Pass {
            Tags { "QUEUE"="Transparent-1" }
            ZWrite Off
            Fog { Mode Off }
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _ShimmerRate;
            float _ShimmerIntensity;
            float _NoiseRate;
            float _NoiseTexRate;
            float _ZBias;
            float3 _DepthBand;

            float4 _Color;
            sampler2D _NoiseTex;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float2 uv4 : TEXCOORD4;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float4 tmpvar_1;
                float2 tmpvar_2;
                float3 tmpvar_3;
                float tmpvar_4;
                float4 tmpvar_5;
                tmpvar_5.w = 1.0;
                tmpvar_5.xyz = v.vertex.xyz;
                float4 tmpvar_6;
                tmpvar_6 = (UnityObjectToClipPos(tmpvar_5));
                float4 tmpvar_7;
                tmpvar_7.x = tmpvar_6.x;
                tmpvar_7.y = tmpvar_6.y;
                tmpvar_7.z = ((tmpvar_6.z * _DepthBand.z) + (tmpvar_6.w * _DepthBand.y));
                tmpvar_7.w = tmpvar_6.w;
                tmpvar_1.xyw = tmpvar_7.xyw;
                tmpvar_1.z = (tmpvar_7.z + _ZBias);
                float4 tmpvar_8;
                tmpvar_8.w = 1.0;
                tmpvar_8.xyz = _WorldSpaceCameraPos;
                float3 tmpvar_9;
                tmpvar_9 = normalize((mul(unity_WorldToObject, tmpvar_8).xyz - v.vertex.xyz));
                tmpvar_3 = tmpvar_9;
                float4 tmpvar_10;
                tmpvar_10.w = 0.0;
                tmpvar_10.xyz = v.vertex.xyz;
                float4 tmpvar_11;
                tmpvar_11.w = 0.0;
                tmpvar_11.xyz = v.vertex.xyz;
                float2 tmpvar_12;
                tmpvar_12.x = -((UnityObjectToViewPos(tmpvar_10)).x);
                tmpvar_12.y = mul(unity_ObjectToWorld, tmpvar_11).y;
                float2 tmpvar_13;
                tmpvar_13.x = 0.0;
                tmpvar_13.y = (-(frac(((_Time.y * _NoiseRate) * 0.015625))) * 64.0);
                float2 tmpvar_14;
                tmpvar_14 = ((tmpvar_12 * _NoiseTexRate) + tmpvar_13);
                tmpvar_2 = tmpvar_14;
                float tmpvar_15;
                tmpvar_15 = (0.25 + ((((sin((_Time.y * (_ShimmerRate / 3.0))) + sin((_Time.y * (_ShimmerRate / 5.0)))) + sin((_Time.y * (_ShimmerRate / 7.0)))) * 0.333333) * _ShimmerIntensity));
                tmpvar_4 = tmpvar_15;
                o.pos = tmpvar_1;
                o.uv = tmpvar_2;
                o.uv1 = tmpvar_3;
                o.uv2 = normalize(v.normal);
                o.uv3 = tmpvar_4;
                o.uv4 = v.color.w;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 colour_1;
                float tmpvar_2;
                tmpvar_2 = clamp ((1.0 - abs((i.uv3 - dot (normalize(i.uv2), i.uv1)))), 0.0, 1.0);
                colour_1.w = (tmpvar_2 * ((tmpvar_2 * 0.75) + 0.25));
                colour_1.w = (colour_1.w * i.uv4);
                colour_1.xyz = (((_Color.xyz * 2.0) * tex2D (_NoiseTex, i.uv).xyz) * colour_1.w);
                return colour_1;
            }
            ENDCG
        }
    }
}