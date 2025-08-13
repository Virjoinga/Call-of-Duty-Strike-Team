Shader "Custom/ParticleSmokeAlpha" {
    Properties {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
    }
    SubShader { 
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Pass {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            BindChannels {
                Bind "vertex", Vertex
                Bind "color", Color
                Bind "texcoord", TexCoord
            }
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _MainTex_ST;
            float3 _DepthBand;

            float4 _TintColor;
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
                o.uv = ((v.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                return (((2.0 * i.color) * _TintColor) * tex2D (_MainTex, i.uv));
            }
            ENDCG
        }
    }
}