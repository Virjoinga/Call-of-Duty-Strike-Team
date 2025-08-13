Shader "Corona/Effects/MonitorScreen" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(0,2)) = 2
        _PhaseLineTex ("Phase Line Texture", 2D) = "black" {}
        _PhaseLineFrequency ("Phase line frequency", Range(0,1)) = 0.1
    }
    SubShader { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Tags { "RenderType"="Opaque" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _FogRange;
            float3 _FogParams;
            float3 _DepthBand;
            float _PhaseLineFrequency;

            sampler2D _MainTex;
            float _Brightness;
            sampler2D _PhaseLineTex;
            sampler2D _NoiseTex;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uv5 : TEXCOORD5;
                float2 uv6 : TEXCOORD6;
                float2 uv7 : TEXCOORD7;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                float4 tmpvar_2;
                float2 tmpvar_3;
                float2 tmpvar_4;
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
                float2 tmpvar_8;
                tmpvar_8 = v.uv.xy;
                tmpvar_1 = tmpvar_8;
                float tmpvar_9;
                tmpvar_9 = clamp (((tmpvar_7.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_10;
                tmpvar_10.xyz = (_FogParams - (_FogParams * tmpvar_9));
                tmpvar_10.w = tmpvar_9;
                tmpvar_2 = tmpvar_10;
                float tmpvar_11;
                tmpvar_11 = v.uv.x;
                tmpvar_3.x = tmpvar_11;
                float tmpvar_12;
                tmpvar_12 = (v.uv.y + frac((135.914 * _Time.y)));
                tmpvar_3.y = tmpvar_12;
                float2 tmpvar_13;
                tmpvar_13 = (((v.uv.y + frac((_Time.y * _PhaseLineFrequency))) * 4.0));
                tmpvar_4 = tmpvar_13;
                o.pos = tmpvar_7;
                o.uv = tmpvar_1;
                o.uv5 = tmpvar_2;
                o.uv7 = tmpvar_3;
                o.uv6 = tmpvar_4;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 diffusemap_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.uv);
                diffusemap_1.w = tmpvar_2.w;
                diffusemap_1.xyz = (tmpvar_2.xyz + tex2D (_PhaseLineTex, i.uv6).xyz);
                diffusemap_1.xyz = (diffusemap_1.xyz * (tex2D (_NoiseTex, i.uv7).xyz * _Brightness));
                float4 tmpvar_3;
                tmpvar_3.w = 1.0;
                tmpvar_3.xyz = ((diffusemap_1.xyz * i.uv5.w) + i.uv5.xyz);
                return tmpvar_3;
            }
            ENDCG
        }
    }
}