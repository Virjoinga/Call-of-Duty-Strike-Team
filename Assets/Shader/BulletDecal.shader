Shader "Corona/Effects/BulletDecal" {
    Properties {
        _MainTex ("Base", 2D) = "white" {}
        _TintColor ("TintColor", Color) = (1,1,1,1)
    }
    SubShader { 
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZWrite Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _TintColor;
            float3 _DepthBand;

            sampler2D _MainTex;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
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
                o.color = (v.color * _TintColor);
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                return (tex2D (_MainTex, i.uv) * i.color);
            }
            ENDCG
        }
    }
}