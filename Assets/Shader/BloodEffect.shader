Shader "Corona/Effects/BloodEffect" 
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float2 _TexOffset;
            float2 _PosOffset;
            float _Alpha;
            float4 _Severity;
            sampler2D _MaskTex;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
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
                float4 pos_1;
                float4 tmpvar_2;
                float2 tmpvar_3;
                pos_1.zw = v.vertex.zw;
                float2 tmpvar_4;
                tmpvar_4 = (v.vertex.xy + (v.texcoord1.xy * _PosOffset));
                pos_1.xy = tmpvar_4;
                tmpvar_2 = pos_1;
                float2 tmpvar_5;
                tmpvar_5 = (v.texcoord0.xy + (v.texcoord1.xy * _TexOffset));
                tmpvar_3 = tmpvar_5;
                o.vertex = tmpvar_2;
                o.texcoord0 = tmpvar_3;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 colour_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.texcoord0);
                colour_1.xyz = tmpvar_2.xyz;
                colour_1.w = (tmpvar_2.w + (dot (tex2D (_MaskTex, i.texcoord0), _Severity) - 1.0));
                colour_1.w = (colour_1.w * _Alpha);
                return colour_1;
            }
            ENDCG
        }
    }
    Fallback Off
}