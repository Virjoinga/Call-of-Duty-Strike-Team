Shader "Corona/Effects/RainStrip" 
{
    Properties
    {
        _MainTex ("Drops Texture", 2D) = "white" {}
        _Drips ("Drip Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _Drips_ST;
            float4 _MainTex_ST;
            float3 _DepthBand;
            sampler2D _Drips;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord2 : TEXCOORD2;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float2 mainUV_1;
                float2 tmpvar_2;
                float2 tmpvar_3;
                float2 tmpvar_4;
                float4 tmpvar_5;
                tmpvar_5.w = 1.0;
                tmpvar_5.xyz = v.vertex.xyz;
                float4 tmpvar_6;
                tmpvar_6 = UnityObjectToClipPos(tmpvar_5);
                float4 tmpvar_7;
                tmpvar_7.x = tmpvar_6.x;
                tmpvar_7.y = tmpvar_6.y;
                tmpvar_7.z = ((tmpvar_6.z * _DepthBand.z) + (tmpvar_6.w * _DepthBand.y));
                tmpvar_7.w = tmpvar_6.w;
                mainUV_1.x = v.texcoord0.x;
                mainUV_1.y = _Time.x;
                float2 tmpvar_8;
                tmpvar_8 = ((mainUV_1 * _MainTex_ST.xy) + _MainTex_ST.zw);
                mainUV_1.x = tmpvar_8.x;
                mainUV_1.y = frac(tmpvar_8.y);
                tmpvar_2 = mainUV_1;
                float2 tmpvar_9;
                tmpvar_9 = ((v.texcoord0.xy * _Drips_ST.xy) + _Drips_ST.zw);
                tmpvar_3 = tmpvar_9;
                float2 tmpvar_10;
                tmpvar_10 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_4 = tmpvar_10;
                o.vertex = tmpvar_7;
                o.texcoord0 = tmpvar_2;
                o.texcoord1 = tmpvar_3;
                o.texcoord2 = tmpvar_4;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float2 mainUV_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_Drips, i.texcoord1);
                mainUV_1 = (i.texcoord0 + tmpvar_2.xy);
                float4 tmpvar_3;
                tmpvar_3.xyz = (2.0 * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.texcoord2).xyz);
                tmpvar_3.w = (tex2D (_MainTex, mainUV_1).x * tmpvar_2.w);
                return tmpvar_3;
            }
            ENDCG
        }
    }
    Fallback Off
}