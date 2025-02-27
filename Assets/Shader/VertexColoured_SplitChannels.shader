Shader "Sprite/Vertex Colored, Split Channels" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _AlphaTex ("Base (A)", 2D) = "white" {}
    }
    SubShader { 
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Pass {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _Color;

            sampler2D _AlphaTex;
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
                float4 tmpvar_2;
                tmpvar_2 = (v.color * _Color);
                tmpvar_1 = tmpvar_2;
                o.pos = (UnityObjectToClipPos(v.vertex));
                o.color = tmpvar_1;
                o.uv = v.uv.xy;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                tmpvar_1.xyz = tex2D (_MainTex, i.uv).xyz;
                tmpvar_1.w = tex2D (_AlphaTex, i.uv).x;
                return (tmpvar_1 * i.color);
            }
            ENDCG
        }
    }
}