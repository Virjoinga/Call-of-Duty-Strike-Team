Shader "Corona/Lightmap/[Emissive]" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _EmissiveTex ("Emissive Mask (RGB)", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "RenderType"="Opaque" }
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _FogRange;
            float3 _FogParams;
            float3 _DepthBand;
            sampler2D _EmissiveTex;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 texcoord5 : TEXCOORD5;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 fog_1;
                float2 tmpvar_2;
                float2 tmpvar_3;
                float4 tmpvar_4;
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
                float2 tmpvar_8;
                tmpvar_8 = v.texcoord0.xy;
                tmpvar_2 = tmpvar_8;
                float2 tmpvar_9;
                tmpvar_9 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_3 = tmpvar_9;
                float tmpvar_10;
                tmpvar_10 = clamp (((tmpvar_7.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_11;
                tmpvar_11.xyz = (_FogParams - (_FogParams * tmpvar_10));
                tmpvar_11.w = tmpvar_10;
                fog_1.xyz = tmpvar_11.xyz;
                fog_1.w = (tmpvar_10 * 2.0);
                tmpvar_4 = fog_1;
                o.vertex = tmpvar_7;
                o.texcoord0 = tmpvar_2;
                o.texcoord1 = tmpvar_3;
                o.texcoord5 = tmpvar_4;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = ((((tex2D (_MainTex, i.texcoord0).xyz * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.texcoord1).xyz) * i.texcoord5.w) + i.texcoord5.xyz) + tex2D (_EmissiveTex, i.texcoord0).xyz);
                return tmpvar_1;
            }
            ENDCG
        }
    }
    Fallback Off
}