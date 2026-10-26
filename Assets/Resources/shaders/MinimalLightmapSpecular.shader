Shader "Hidden/MinimalLightmapSpecular"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecDir ("Specular Dir", Vector) = (-0.66,0.333,0.66,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "RenderType"="Opaque" }
            Fog {
            Mode Linear
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "../../Shader/CoronaLegacy.cginc"
            float3 _DepthBand;
            float3 _SpecDir;
            // float4x4 _Object2World;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float3 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                  float2 tmpvar_1;
                  float3 tmpvar_2;
                  float4 tmpvar_3;
                  tmpvar_3.w = 1.0;
                  tmpvar_3.xyz = v.vertex.xyz;
                  float4 tmpvar_4;
                  tmpvar_4 = CoronaObjectToClipPos(tmpvar_3);
                  float4 tmpvar_5;
                  tmpvar_5.x = tmpvar_4.x;
                  tmpvar_5.y = tmpvar_4.y;
                  tmpvar_5.z = ((tmpvar_4.z * _DepthBand.z) + (tmpvar_4.w * _DepthBand.y));
                  tmpvar_5.w = tmpvar_4.w;
                  float3x3 tmpvar_6;
                  tmpvar_6[0] = unity_ObjectToWorld[0].xyz;
                  tmpvar_6[1] = unity_ObjectToWorld[1].xyz;
                  tmpvar_6[2] = unity_ObjectToWorld[2].xyz;
                  float3 tmpvar_7;
                  tmpvar_7 = mul(tmpvar_6, normalize(v.normal));
                  float2 tmpvar_8;
                  tmpvar_8 = v.texcoord0.xy;
                  tmpvar_1 = tmpvar_8;
                  float2 tmpvar_9;
                  tmpvar_9 = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                  tmpvar_2.xy = tmpvar_9;
                  float3 tmpvar_10;
                  tmpvar_10 = (mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos);
                  float tmpvar_11;
                  tmpvar_11 = max (0.0, dot (normalize((tmpvar_10 - ((tmpvar_7 * 2.0) * dot (tmpvar_10, tmpvar_7)))), _SpecDir));
                  tmpvar_2.z = tmpvar_11;
                  tmpvar_2.z = (tmpvar_2.z * (tmpvar_2.z * tmpvar_2.z));
                  o.vertex = CoronaClipToNative(tmpvar_5);
                  o.texcoord0 = tmpvar_1;
                  o.texcoord1 = tmpvar_2;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                  float4 tmpvar_1;
                  tmpvar_1 = tex2D (_MainTex, i.texcoord0);
                  float4 tmpvar_2;
                  tmpvar_2.w = 1.0;
                  tmpvar_2.xyz = ((tmpvar_1.xyz * (2.0 * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.texcoord1.xy).xyz)) + (i.texcoord1.z * tmpvar_1.w));
                  return tmpvar_2;
            }
            ENDCG
        }
    }
    Fallback Off
}
