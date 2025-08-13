Shader "Corona/Lightmap/[Detail]" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DetailTex ("DetailMap(RGB)", 2D) = "white" {}
        _DetailPower ("Detail Power", Float) = 0.5
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
            float _DetailPower;
            sampler2D _DetailTex;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord6 : TEXCOORD6;
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
                o.texcoord6 = (v.texcoord0.xy * 20.0);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 diffusemap_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.texcoord0);
                diffusemap_1.w = tmpvar_2.w;
                float2 P_3;
                P_3 = (i.texcoord6 - floor(i.texcoord6));
                diffusemap_1.xyz = clamp (((tmpvar_2.xyz * tex2D (_DetailTex, P_3).xyz) * _DetailPower), 0.0, 1.0);
                float4 tmpvar_4;
                tmpvar_4.w = 1.0;
                tmpvar_4.xyz = (((diffusemap_1.xyz * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.texcoord1).xyz) * i.texcoord5.w) + i.texcoord5.xyz);
                return tmpvar_4;
            }
            ENDCG
        }
    }
    Fallback Off
}