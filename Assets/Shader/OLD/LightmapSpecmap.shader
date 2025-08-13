Shader "Corona/Lightmap/[Spec]" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _SpecPower ("Specular Power", Range(0,50)) = 10
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
            sampler2D _SpecMap;
            sampler2D _MainTex;
            float _SpecPower;
            float3 _SpecDir;
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 texcoord5 : TEXCOORD5;
                float3 texcoord3 : TEXCOORD3;
                float3 texcoord2 : TEXCOORD2;
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
                float3 tmpvar_4;
                float4 tmpvar_5;
                float4 tmpvar_6;
                tmpvar_6.w = 1.0;
                tmpvar_6.xyz = v.vertex.xyz;
                float4 tmpvar_7;
                tmpvar_7 = UnityObjectToClipPos(tmpvar_6);
                float4 tmpvar_8;
                tmpvar_8.x = tmpvar_7.x;
                tmpvar_8.y = tmpvar_7.y;
                tmpvar_8.z = ((tmpvar_7.z * _DepthBand.z) + (tmpvar_7.w * _DepthBand.y));
                tmpvar_8.w = tmpvar_7.w;
                float2 tmpvar_9;
                tmpvar_9 = v.texcoord0.xy;
                tmpvar_2 = tmpvar_9;
                float2 tmpvar_10;
                tmpvar_10 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                tmpvar_3 = tmpvar_10;
                float4 tmpvar_11;
                tmpvar_11.w = 1.0;
                tmpvar_11.xyz = _WorldSpaceCameraPos;
                float3 tmpvar_12;
                tmpvar_12 = normalize((mul(unity_WorldToObject, tmpvar_11).xyz - v.vertex.xyz));
                tmpvar_4 = tmpvar_12;
                float tmpvar_13;
                tmpvar_13 = clamp (((tmpvar_8.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_14;
                tmpvar_14.xyz = (_FogParams - (_FogParams * tmpvar_13));
                tmpvar_14.w = tmpvar_13;
                fog_1.xyz = tmpvar_14.xyz;
                fog_1.w = (tmpvar_13 * 2.0);
                tmpvar_5 = fog_1;
                o.vertex = tmpvar_8;
                o.texcoord0 = tmpvar_2;
                o.texcoord1 = tmpvar_3;
                o.texcoord2 = tmpvar_4;
                o.texcoord3 = normalize(v.normal);
                o.texcoord5 = tmpvar_5;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float3 lightDir_1;
                float3 tmpvar_2;
                float3 dir_3;
                dir_3 = _SpecDir;
                float4 tmpvar_4;
                tmpvar_4.w = 0.0;
                tmpvar_4.xyz = dir_3;
                tmpvar_2 = mul(unity_WorldToObject, tmpvar_4).xyz;
                lightDir_1 = tmpvar_2;
                float tmpvar_5;
                float spec_6;
                float tmpvar_7;
                tmpvar_7 = max (0.0, dot (normalize((lightDir_1 + normalize(i.texcoord2))), normalize(i.texcoord3)));
                spec_6 = tmpvar_7;
                float tmpvar_8;
                tmpvar_8 = pow (spec_6, _SpecPower);
                spec_6 = tmpvar_8;
                tmpvar_5 = tmpvar_8;
                float4 tmpvar_9;
                tmpvar_9.w = 1.0;
                tmpvar_9.xyz = ((((tex2D (_MainTex, i.texcoord0).xyz + (tmpvar_5.xxx * tex2D (_SpecMap, i.texcoord0).xyz)) * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.texcoord1).xyz) * i.texcoord5.w) + i.texcoord5.xyz);
                return tmpvar_9;
            }
            ENDCG
        }
    }
    Fallback Off
}