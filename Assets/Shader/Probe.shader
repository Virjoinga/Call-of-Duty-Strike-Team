Shader "Corona/Probe/Base" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        LOD 300
        Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
        Pass
        {
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _FogRange;
            float3 _FogParams;
            float3 _AmbientLight;
            float4 cC;
            float4 cBb;
            float4 cBg;
            float4 cBr;
            float4 cAb;
            float4 cAg;
            float4 cAr;
            float3 _DepthBand;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 texcoord5 : TEXCOORD5;
                float3 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float2 tmpvar_1;
                float3 tmpvar_2;
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
                tmpvar_7 = v.texcoord0.xy;
                tmpvar_1 = tmpvar_7;
                float4 tmpvar_8;
                tmpvar_8.w = 0.0;
                tmpvar_8.xyz = normalize(v.normal);
                float3 tmpvar_9;
                tmpvar_9 = normalize(mul(unity_ObjectToWorld, tmpvar_8).xyz);
                float4 tmpvar_10;
                tmpvar_10.w = 1.0;
                tmpvar_10.xyz = tmpvar_9;
                float3 tmpvar_11;
                float3 x2_12;
                float3 x1_13;
                float4 tmpvar_14;
                tmpvar_14.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_14.w = _AmbientLight.x;
                x1_13.x = dot ((cAr + tmpvar_14), tmpvar_10);
                float4 tmpvar_15;
                tmpvar_15.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_15.w = _AmbientLight.y;
                x1_13.y = dot ((cAg + tmpvar_15), tmpvar_10);
                float4 tmpvar_16;
                tmpvar_16.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_16.w = _AmbientLight.z;
                x1_13.z = dot ((cAb + tmpvar_16), tmpvar_10);
                float4 tmpvar_17;
                tmpvar_17 = (tmpvar_9.xyzz * tmpvar_9.yzzx);
                x2_12.x = dot (cBr, tmpvar_17);
                x2_12.y = dot (cBg, tmpvar_17);
                x2_12.z = dot (cBb, tmpvar_17);
                tmpvar_11 = ((x1_13 + x2_12) + (cC.xyz * ((tmpvar_9.x * tmpvar_9.x) - (tmpvar_9.y * tmpvar_9.y))));
                tmpvar_2 = tmpvar_11;
                float tmpvar_18;
                tmpvar_18 = clamp (((tmpvar_6.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_19;
                tmpvar_19.xyz = (_FogParams - (_FogParams * tmpvar_18));
                tmpvar_19.w = tmpvar_18;
                tmpvar_3 = tmpvar_19;
                o.vertex = tmpvar_6;
                o.texcoord0 = tmpvar_1;
                o.texcoord1 = tmpvar_2;
                o.texcoord5 = tmpvar_3;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = (((tex2D (_MainTex, i.texcoord0).xyz * i.texcoord1) * i.texcoord5.w) + i.texcoord5.xyz);
                return tmpvar_1;
            }
            ENDCG
        }
        Pass
        {
            Name "SHADOWCASTER"
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
            Cull Off
            Fog { Mode Off }
            Offset 1, 1
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata_t
            {
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2 = UnityObjectToClipPos(v.vertex);
                tmpvar_1.xyw = tmpvar_2.xyw;
                tmpvar_1.z = (tmpvar_2.z + unity_LightShadowBias.x);
                tmpvar_1.z = lerp (tmpvar_1.z, max (tmpvar_1.z, (tmpvar_2.w * -1.0)), unity_LightShadowBias.y);
                o.vertex = tmpvar_1;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                return float4(0.0, 0.0, 0.0, 0.0);
            }
            ENDCG
        }
        Pass
        {
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _FogRange;
            float3 _FogParams;
            float3 _AmbientLight;
            float4 cC;
            float4 cBb;
            float4 cBg;
            float4 cBr;
            float4 cAb;
            float4 cAg;
            float4 cAr;
            float3 _DepthBand;
            // float4x4 _Object2World;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 texcoord5 : TEXCOORD5;
                float3 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float2 tmpvar_1;
                float3 tmpvar_2;
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
                tmpvar_7 = v.texcoord0.xy;
                tmpvar_1 = tmpvar_7;
                float4 tmpvar_8;
                tmpvar_8.w = 0.0;
                tmpvar_8.xyz = normalize(v.normal);
                float3 tmpvar_9;
                tmpvar_9 = normalize(mul(unity_ObjectToWorld, tmpvar_8).xyz);
                float4 tmpvar_10;
                tmpvar_10.w = 1.0;
                tmpvar_10.xyz = tmpvar_9;
                float3 tmpvar_11;
                float3 x2_12;
                float3 x1_13;
                float4 tmpvar_14;
                tmpvar_14.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_14.w = _AmbientLight.x;
                x1_13.x = dot ((cAr + tmpvar_14), tmpvar_10);
                float4 tmpvar_15;
                tmpvar_15.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_15.w = _AmbientLight.y;
                x1_13.y = dot ((cAg + tmpvar_15), tmpvar_10);
                float4 tmpvar_16;
                tmpvar_16.xyz = float3(0.0, 0.0, 0.0);
                tmpvar_16.w = _AmbientLight.z;
                x1_13.z = dot ((cAb + tmpvar_16), tmpvar_10);
                float4 tmpvar_17;
                tmpvar_17 = (tmpvar_9.xyzz * tmpvar_9.yzzx);
                x2_12.x = dot (cBr, tmpvar_17);
                x2_12.y = dot (cBg, tmpvar_17);
                x2_12.z = dot (cBb, tmpvar_17);
                tmpvar_11 = ((x1_13 + x2_12) + (cC.xyz * ((tmpvar_9.x * tmpvar_9.x) - (tmpvar_9.y * tmpvar_9.y))));
                tmpvar_2 = tmpvar_11;
                float tmpvar_18;
                tmpvar_18 = clamp (((tmpvar_6.z * _FogRange.x) + (_FogRange.y + 1.0)), _FogRange.z, 1.0);
                float4 tmpvar_19;
                tmpvar_19.xyz = (_FogParams - (_FogParams * tmpvar_18));
                tmpvar_19.w = tmpvar_18;
                tmpvar_3 = tmpvar_19;
                o.vertex = tmpvar_6;
                o.texcoord0 = tmpvar_1;
                o.texcoord1 = tmpvar_2;
                o.texcoord5 = tmpvar_3;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = (((tex2D (_MainTex, i.texcoord0).xyz * i.texcoord1) * i.texcoord5.w) + i.texcoord5.xyz);
                return tmpvar_1;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}