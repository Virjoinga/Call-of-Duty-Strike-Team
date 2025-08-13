Shader "Corona/Water/Corona" 
{
    Properties
    {
        _ColorControl ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
        _BumpMap ("Waves Normalmap ", 2D) = "" {}
        _WaveScale ("Wave scale", Range(0.02,0.15)) = 0.063
        WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
    }
    SubShader
    {
        Tags { "QUEUE"="Geometry+997" "RenderType"="Opaque" }
        Pass
        {
            Tags { "QUEUE"="Geometry+997" "RenderType"="Opaque" }
            Fog { Mode Off }
            Blend One SrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
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
                float3 texcoord2 : TEXCOORD2;
                float2 texcoord1 : TEXCOORD1;
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
                float3 tmpvar_5;
                tmpvar_5 = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 tmpvar_6;
                tmpvar_6 = (_WorldSpaceCameraPos - tmpvar_5).xzy;
                tmpvar_1 = tmpvar_6;
                o.vertex = tmpvar_4;
                o.texcoord0 = ((tmpvar_5.xz * _WaveScale4.xy) + _WaveOffset.xy);
                o.texcoord1 = ((tmpvar_5.xz * _WaveScale4.zw) + _WaveOffset.zw);
                o.texcoord2 = tmpvar_1;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float fresnel_1;
                float3 viewDir_2;
                float3 tmpvar_3;
                tmpvar_3 = normalize(i.texcoord2);
                viewDir_2 = tmpvar_3;
                float3 tmpvar_4;
                tmpvar_4 = ((tex2D (_BumpMap, i.texcoord0).xyz + tex2D (_BumpMap, i.texcoord1).xyz) - 1.0);
                float tmpvar_5;
                tmpvar_5 = dot (viewDir_2, tmpvar_4);
                fresnel_1 = tmpvar_5;
                return tex2D (_ColorControl, fresnel_1);
            }
            ENDCG
        }
    }
    
}