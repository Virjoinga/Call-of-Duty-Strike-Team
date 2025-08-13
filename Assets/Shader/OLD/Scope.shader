Shader "Corona/HUD/Scope" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ScopeTex ("Scope (RGB)", 2D) = "white" {}
    }
    SubShader { 
        LOD 200
        Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
        Pass {
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _ScopeTex;
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
                float2 tmpvar_2;
                tmpvar_2 = v.uv.xy;
                tmpvar_1 = tmpvar_2;
                o.pos = (UnityObjectToClipPos(v.vertex));
                o.uv = tmpvar_1;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 main_1;
                float4 scope_2;
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_ScopeTex, i.uv);
                scope_2 = tmpvar_3;
                float2 tmpvar_4;
                tmpvar_4.x = 0.0;
                tmpvar_4.y = (0.1 * scope_2.w);
                float4 tmpvar_5;
                float2 P_6;
                P_6 = (i.uv + tmpvar_4);
                tmpvar_5 = tex2D (_MainTex, P_6);
                main_1 = tmpvar_5;
                return lerp (main_1, scope_2, scope_2.wwww);
            }
            ENDCG
        }
    }
}