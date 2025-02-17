// Upgrade NOTE: replaced 'glstate_matrix_mvp' with 'UNITY_MATRIX_MVP'

Shader "Corona/Effects/Globe" {
    Properties {
        _MainTex ("Base", 2D) = "white" {}
        _TintColor ("TintColor", Color) = (1,1,1,1)
    }
    SubShader { 
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" "Reflection"="RenderReflectionTransparentAdd" }
        Pass {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" "Reflection"="RenderReflectionTransparentAdd" }
            ZWrite Off
            Cull Front
            Fog { Mode Off }
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _MainTex_ST;
            sampler2D _MainTex;
            float4 _TintColor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;

                float2 tmpvar_1;
                float2 tmpvar_2;

                tmpvar_2 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                tmpvar_1 = tmpvar_2;
                o.vertex = (UnityObjectToClipPos(v.vertex));
                o.texcoord0 = tmpvar_1;

                return o;
            }
            half4 frag(v2f i) : SV_TARGET
            {
                return ((tex2D (_MainTex, i.texcoord0) * 0.2) * _TintColor);
            }
            ENDCG
        }
    }
}

