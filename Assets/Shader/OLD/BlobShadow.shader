Shader "Corona/Effects/BlobShadow" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader { 
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZWrite Off
            Fog { Mode Off }
            Blend Zero SrcColor

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float3 _DepthBand;

            float4 _Color;
            sampler2D _MainTex;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float2 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2.w = 1.0;
                tmpvar_2.xyz = v.vertex.xyz;
                float4 tmpvar_3;
                tmpvar_3 = (UnityObjectToClipPos(tmpvar_2));
                float4 tmpvar_4;
                tmpvar_4.x = tmpvar_3.x;
                tmpvar_4.y = tmpvar_3.y;
                tmpvar_4.z = ((tmpvar_3.z * _DepthBand.z) + (tmpvar_3.w * _DepthBand.y));
                tmpvar_4.w = tmpvar_3.w;
                float2 tmpvar_5;
                tmpvar_5 = v.uv.xy;
                tmpvar_1 = tmpvar_5;
                o.pos = tmpvar_4;
                o.uv = tmpvar_1;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2 = lerp (float4(1.0, 1.0, 1.0, 1.0), _Color, tex2D (_MainTex, i.uv).wwww);
                tmpvar_1 = tmpvar_2;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}