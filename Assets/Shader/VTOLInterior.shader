Shader "Corona/Effects/VTOL Interior" 
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
        _EmissiveTex ("Emissive Mask (RGB)", 2D) = "black" {}
        _TintColor ("TintColor", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
        Pass
        {
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _MainTex_ST;
            float3 _DepthBand;
            float4 _LightColour3;
            float4 _LightColour2;
            float4 _LightColour1;
            float4 _LightColour0;
            float4 _LightAttenSq;
            float4 _LightPosZ;
            float4 _LightPosY;
            float4 _LightPosX;
            float3 _EffectsAmbient;
            sampler2D _EmissiveTex;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
                float4 color : COLOR;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float3 texcoord3 : TEXCOORD3;
                float3 texcoord2 : TEXCOORD2;
                float3 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float3 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2.w = 1.0;
                tmpvar_2.xyz = v.vertex.xyz;
                float4 tmpvar_3;
                tmpvar_3 = UnityObjectToClipPos(tmpvar_2);
                float4 tmpvar_4;
                tmpvar_4.x = tmpvar_3.x;
                tmpvar_4.y = tmpvar_3.y;
                tmpvar_4.z = ((tmpvar_3.z * _DepthBand.z) + (tmpvar_3.w * _DepthBand.y));
                tmpvar_4.w = tmpvar_3.w;
                float4 tmpvar_5;
                tmpvar_5.w = 0.0;
                tmpvar_5.xyz = normalize(v.normal);
                float3 tmpvar_6;
                tmpvar_6 = v.color.xyz;
                tmpvar_1 = tmpvar_6;
                o.vertex = tmpvar_4;
                o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                o.texcoord1 = tmpvar_1;
                o.texcoord2 = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.texcoord3 = normalize(mul(unity_ObjectToWorld, tmpvar_5).xyz);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2 = (_LightPosX - i.texcoord2.x);
                float4 tmpvar_3;
                tmpvar_3 = (_LightPosY - i.texcoord2.y);
                float4 tmpvar_4;
                tmpvar_4 = (_LightPosZ - i.texcoord2.z);
                float4 tmpvar_5;
                tmpvar_5 = (((tmpvar_2 * tmpvar_2) + (tmpvar_3 * tmpvar_3)) + (tmpvar_4 * tmpvar_4));
                float4 tmpvar_6;
                tmpvar_6 = (max (float4(0.0, 0.0, 0.0, 0.0), ((((tmpvar_2 * i.texcoord3.x) + (tmpvar_3 * i.texcoord3.y)) + (tmpvar_4 * i.texcoord3.z)) * rsqrt(tmpvar_5))) * (1.0/((1.0 + (tmpvar_5 * _LightAttenSq)))));
                float4 tmpvar_7;
                tmpvar_7 = tex2D (_MainTex, i.texcoord0);
                float4 tmpvar_8;
                tmpvar_8 = tex2D (_EmissiveTex, i.texcoord0);
                float4 tmpvar_9;
                tmpvar_9.w = 1.0;
                tmpvar_9.xyz = ((tmpvar_7.xyz * (i.texcoord1 * (_EffectsAmbient + ((((_LightColour0.xyz * tmpvar_6.x) + (_LightColour1.xyz * tmpvar_6.y)) + (_LightColour2.xyz * tmpvar_6.z)) + (_LightColour3.xyz * tmpvar_6.w))))) + tmpvar_8.xyz);
                tmpvar_1 = tmpvar_9;
                return tmpvar_1;
            }
            ENDCG
        }
    }
    Fallback Off
}