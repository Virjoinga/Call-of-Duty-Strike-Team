// Upgrade NOTE: replaced 'glstate_matrix_mvp' with 'UNITY_MATRIX_MVP'

Shader "Corona/FX/Additive" 
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
        _TintColor ("TintColor", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" "Reflection"="RenderReflectionTransparentAdd" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" "Reflection"="RenderReflectionTransparentAdd" }
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _MainTex_ST;
            float3 _DepthBand;
            float4 _TintColor;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float2 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2.w = 1.0;
                tmpvar_2.xyz = v.vertex.xyz;
                float4 tmpvar_3;
                tmpvar_3 = (UNITY_MATRIX_MVP * tmpvar_2);
                float4 tmpvar_4;
                tmpvar_4.x = tmpvar_3.x;
                tmpvar_4.y = tmpvar_3.y;
                tmpvar_4.z = ((tmpvar_3.z * _DepthBand.z) + (tmpvar_3.w * _DepthBand.y));
                tmpvar_4.w = tmpvar_3.w;
                float2 tmpvar_5;
                tmpvar_5 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                tmpvar_1 = tmpvar_5;
                o.vertex = tmpvar_4;
                o.texcoord0 = tmpvar_1;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                return (tex2D (_MainTex, i.texcoord0) * _TintColor);
            }
            ENDCG
        }
    }
    Fallback Off
}