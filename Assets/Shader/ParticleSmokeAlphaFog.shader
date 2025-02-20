Shader "Custom/ParticleSmokeAlphaFog" {
    Properties {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
        _FogRangeFog ("Fog Range", Range(0,1)) = 0.2
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
            float _FogRangeFog;
            float4 _FogRange;
            float3 _FogParams;
            float3 _DepthBand;

            float4 _TintColor;
            sampler2D _MainTex;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uv2 : TEXCOORD2;
                fixed4 color : COLOR;
            };

            v2f vert(appdata_t v)
            {
                v2f o;

                float4 tmpvar_1;
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
                float tmpvar_5;
                tmpvar_5 = clamp (((tmpvar_4.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRangeFog, 1.0);
                float4 tmpvar_6;
                tmpvar_6.xyz = (_FogParams - (_FogParams * tmpvar_5));
                tmpvar_6.w = tmpvar_5;
                tmpvar_1 = tmpvar_6;
                o.pos = tmpvar_4;
                o.color = v.color;
                o.uv = ((v.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                o.uv2 = tmpvar_1;

                return o;
            }
            half4 frag(v2f i) : SV_TARGET
            {
                float4 colour_1;
                float4 tmpvar_2;
                tmpvar_2 = (((2.0 * i.color) * _TintColor) * tex2D (_MainTex, i.uv));
                colour_1.w = tmpvar_2.w;
                colour_1.xyz = ((tmpvar_2.xyz * i.uv2.w) + i.uv2.xyz);
                return colour_1;
            }
            ENDCG
        }
    }
}