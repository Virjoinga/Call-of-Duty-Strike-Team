Shader "Corona/Lightmap/[WindCloth]" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _WindAmount ("Wind Amount", Float) = 0.08
        _NormalDirectionScale ("Normal Direction Scale", Float) = 1
        _WindDirectionScale ("Wind Direction Scale", Float) = 0
        _SpatialScale ("Spatial Scale", Float) = 2
        _Snap ("Snap", Float) = 1
        _HighFrequencyAmount ("High Frequency Amount", Float) = 1
        _WindSpeed0Scale ("Wind Speed 0 Scale", Float) = 1
        _WindSpeed1Scale ("Wind Speed 1 Scale", Float) = 0
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
            float _WindSpeed1Scale;
            float _WindSpeed0Scale;
            float _HighFrequencyAmount;
            float _Snap;
            float _SpatialScale;
            float _WindDirectionScale;
            float _NormalDirectionScale;
            float _WindAmount;
            float4 g_globalWindDir2;
            float4 g_globalWindDir;
            float4 g_globalWindData;
            float3 _DepthBand;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
                float4 color : COLOR;
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
                tmpvar_5 = v.vertex;
                float3 tmpvar_6;
                float4 tmpvar_7;
                tmpvar_7.w = 1.0;
                tmpvar_7.xyz = v.vertex.xyz;
                tmpvar_6 = mul(unity_ObjectToWorld, tmpvar_7).xyz;
                float tmpvar_8;
                tmpvar_8 = (_NormalDirectionScale * _WindAmount);
                float4 tmpvar_9;
                tmpvar_9.w = 0.0;
                tmpvar_9.xyz = (g_globalWindDir.xyz * (_WindDirectionScale * _WindAmount));
                float3 tmpvar_10;
                tmpvar_10 = ((normalize(v.normal) * ((v.color.x * (2.0 * tmpvar_8)) - tmpvar_8)) + mul(unity_WorldToObject, tmpvar_9).xyz);
                float2 tmpvar_11;
                tmpvar_11.x = _WindSpeed0Scale;
                tmpvar_11.y = _WindSpeed1Scale;
                float2 tmpvar_12;
                tmpvar_12.x = (dot (tmpvar_6, (g_globalWindDir.xyz * _SpatialScale)) - (dot (g_globalWindData.xy, tmpvar_11) * 20.0));
                tmpvar_12.y = (dot (tmpvar_6, (g_globalWindDir2.xyz * 4.0)) - (_Time.y * 40.0));
                float2 tmpvar_13;
                tmpvar_13 = cos(tmpvar_12);
                float tmpvar_14;
                tmpvar_14 = ((g_globalWindData.w * 0.5) + 0.5);
                float2 tmpvar_15;
                tmpvar_15 = (1.0 - pow ((1.0 - abs(tmpvar_13)), lerp (1.0, ((g_globalWindData.w * float2(2.0, 1.0)) + float2(2.0, 1.0)).x, _Snap)));
                float2 tmpvar_16;
                tmpvar_16 = min (tmpvar_13, float2(0.0, 0.0));
                float2 b_17;
                b_17 = -(tmpvar_15);
                float tmpvar_18;
                if (tmpvar_16.x) {
                tmpvar_18 = b_17.x;
                }
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = (((tex2D (_MainTex, i.texcoord0).xyz * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.texcoord1).xyz) * i.texcoord5.w) + i.texcoord5.xyz);
                return tmpvar_1;
            }
            ENDCG
        }
    }
    Fallback Off
}