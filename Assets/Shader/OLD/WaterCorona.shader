// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Corona/Water/Corona" {
    Properties {
        _ColorControl ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
        _BumpMap ("Waves Normalmap ", 2D) = "" {}
        _WaveScale ("Wave scale", Range(0.02,0.15)) = 0.063
        WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
    }
    SubShader { 
        Tags { "QUEUE"="Geometry+997" "RenderType"="Opaque" }
        Pass {
            Tags { "QUEUE"="Geometry+997" "RenderType"="Opaque" }
            Fog { Mode Off }
            Blend One SrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _WaveOffset;
            float4 _WaveScale4;
            float3 _DepthBand;

            sampler2D _ColorControl;
            sampler2D _BumpMap;

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
            };

            v2f vert(appdata_t v)
            {
                v2f o;

                float3 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2.w = 1.0;
                tmpvar_2.xyz = v.vertex.xyz;
                float4 tmpvar_3;
                tmpvar_3 = (UnityObjectToClipPos(tmpvar_2));
                float4 tmpvar_4;
                tmpvar_4.x = tmpvar_3.x;
                tmpvar_4.y = tmpvar_3.y;
                tmpvar_4.z = ((tmpvar_3.z * _DepthBand.z) + (tmpvar_3.w * _DepthBand.y));
                tmpvar_4.w = tmpvar_3.w;
                float3 tmpvar_5;
                tmpvar_5 = (mul(unity_ObjectToWorld, v.vertex)).xyz;
                float3 tmpvar_6;
                tmpvar_6 = (_WorldSpaceCameraPos - tmpvar_5).xzy;
                tmpvar_1 = tmpvar_6;
                o.pos = tmpvar_4;
                o.uv = ((tmpvar_5.xz * _WaveScale4.xy) + _WaveOffset.xy);
                o.uv1 = ((tmpvar_5.xz * _WaveScale4.zw) + _WaveOffset.zw);
                o.uv2 = tmpvar_1;

                return o;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                float fresnel_1;
                float2 viewDir_2;
                float2 tmpvar_3;
                tmpvar_3 = normalize(i.uv2);
                viewDir_2 = tmpvar_3;
                float3 tmpvar_4;
                tmpvar_4 = ((tex2D (_BumpMap, i.uv).xyz + tex2D (_BumpMap, i.uv1).xyz) - 1.0);
                float tmpvar_5;
                tmpvar_5 = dot (viewDir_2, tmpvar_4);
                fresnel_1 = tmpvar_5;
                return tex2D (_ColorControl, float2(fresnel_1, 0));
            }
            ENDCG
        }
    }
}