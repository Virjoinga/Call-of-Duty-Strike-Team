Shader "Corona/Effects/SoldierPath" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader { 
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZTest Greater
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float3 _DepthBand;

            float _StartProp;
            float _EndProp;
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
                
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = v.vertex.xyz;
                float4 tmpvar_2;
                tmpvar_2 = (UnityObjectToClipPos(tmpvar_1));
                float4 tmpvar_3;
                tmpvar_3.x = tmpvar_2.x;
                tmpvar_3.y = tmpvar_2.y;
                tmpvar_3.z = ((tmpvar_2.z * _DepthBand.z) + (tmpvar_2.w * _DepthBand.y));
                tmpvar_3.w = tmpvar_2.w;
                o.pos = tmpvar_3;
                o.color = v.color;
                o.uv = v.uv.xy;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float2 tmpvar_1;
                tmpvar_1.y = i.uv.y;
                float4 tx_2;
                tmpvar_1.x = min (0.5, min ((i.uv.x * _StartProp), ((1.0 - i.uv.x) * _EndProp)));
                if ((i.uv.x > 0.5)) {
                    tmpvar_1.x = (1.0 - tmpvar_1.x);
                };
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_MainTex, tmpvar_1);
                tx_2 = tmpvar_3;
                return ((i.color * tx_2) * 0.2);
            }
            ENDCG
        }
    }
}