Shader "Corona/Water/Waves" 
{
    Properties
    {
        _MainTex1 ("Texture 1", 2D) = "white" {}
        _MainTex2 ("Texture 2", 2D) = "white" {}
        _Waves1 ("Wave distortion 1", 2D) = "" {}
        _Waves2 ("Wave distortion 2", 2D) = "" {}
        _TintColor ("TintColor", Color) = (1,1,1,1)
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
            float4 _Waves2_ST;
            float4 _Waves1_ST;
            float4 _MainTex2_ST;
            float4 _MainTex1_ST;
            float3 _DepthBand;
            float4 _TintColor;
            sampler2D _Waves2;
            sampler2D _Waves1;
            sampler2D _MainTex2;
            sampler2D _MainTex1;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 texcoord2 : TEXCOORD2;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float2 tmpvar_1;
                float2 tmpvar_2;
                float4 tmpvar_3;
                float4 tmpvar_4;
                tmpvar_4.w = 1.0;
                tmpvar_4.xyz = v.vertex.xyz;
                float4 tmpvar_5;
                tmpvar_5 = UnityObjectToClipPos(tmpvar_4);
                float4 tmpvar_6;
                tmpvar_6.x = tmpvar_5.x;
                tmpvar_6.y = tmpvar_5.y;
                tmpvar_6.z = ((tmpvar_5.z * _DepthBand.z) + (tmpvar_5.w * _DepthBand.y));
                tmpvar_6.w = tmpvar_5.w;
                float2 tmpvar_7;
                tmpvar_7 = ((v.texcoord0.xy * _Waves1_ST.xy) + _Waves1_ST.zw);
                tmpvar_1 = tmpvar_7;
                float2 tmpvar_8;
                tmpvar_8 = ((v.texcoord0.xy * _Waves2_ST.xy) + _Waves2_ST.zw);
                tmpvar_2 = tmpvar_8;
                float2 tmpvar_9;
                tmpvar_9 = ((v.texcoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
                tmpvar_3.xy = tmpvar_9;
                float2 tmpvar_10;
                tmpvar_10 = ((v.texcoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
                tmpvar_3.zw = tmpvar_10;
                tmpvar_3.yw = (tmpvar_3.yw - 0.5);
                o.vertex = tmpvar_6;
                o.texcoord0 = tmpvar_1;
                o.texcoord1 = tmpvar_2;
                o.texcoord2 = tmpvar_3;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 mainUVs_1;
                float2 offsets_2;
                offsets_2.x = tex2D (_Waves1, i.texcoord0).w;
                offsets_2.y = tex2D (_Waves2, i.texcoord1).w;
                mainUVs_1.xz = i.texcoord2.xz;
                mainUVs_1.yw = (i.texcoord2.yw + offsets_2);
                return (max (tex2D (_MainTex1, mainUVs_1.xy), tex2D (_MainTex2, mainUVs_1.zw)) * _TintColor);
            }
            ENDCG
        }
    }
    Fallback Off
}