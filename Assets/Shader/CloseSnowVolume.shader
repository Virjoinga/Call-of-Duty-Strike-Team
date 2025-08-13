Shader "Corona/Effects/CloseSnowVolume" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Size ("Size", Float) = 0.01
        _Alpha ("Alpha", Float) = 1
        _Color ("Main Color", Color) = (1,1,1,1)
        _CentreFadeSpeed ("CentreFadeSpeed", Float) = 0.25
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float _CentreFadeSpeed;
            float4 _Color;
            float _Alpha;
            float _Size;
            float4 g_closeSnowWindData;
            float4 g_closeSnowFade;
            float4 g_closeSnowOffset;
            float4 g_closeSnowData2;
            float4 g_closeSnowData;
            float4 g_cameraForward;
            float4 g_cameraUp;
            float4 g_cameraRight;
            float4 g_cameraPos;
            float3 _DepthBand;
            float3 _SpecDir;
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
                float4 color : COLOR;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1.w = 0.0;
                tmpvar_1.xyz = v.normal;
                float2 tmpvar_2;
                tmpvar_2 = v.texcoord0.xy;
                float4 tmpvar_3;
                float3 lightDir_4;
                float3 pos_5;
                float3 tmpvar_6;
                tmpvar_6.x = g_closeSnowData.w;
                tmpvar_6.y = g_closeSnowData2.x;
                tmpvar_6.z = g_closeSnowData.w;
                float3 tmpvar_7;
                tmpvar_7.x = g_closeSnowOffset.w;
                tmpvar_7.y = g_closeSnowData2.y;
                tmpvar_7.z = g_closeSnowOffset.w;
                float3 tmpvar_8;
                tmpvar_8 = ((frac((((v.vertex.xyz + (tmpvar_1 * g_closeSnowWindData.x).xyz) * tmpvar_6) + g_closeSnowData.xyz)) * tmpvar_7) + g_closeSnowOffset.xyz);
                float3 tmpvar_9;
                tmpvar_9 = (tmpvar_8 - g_cameraPos.xyz);
                float tmpvar_10;
                tmpvar_10 = sqrt(dot (tmpvar_9, tmpvar_9));
                float tmpvar_11;
                tmpvar_11 = (clamp (((tmpvar_10 * (_CentreFadeSpeed * 2.0)) - ((6.0 * _CentreFadeSpeed) + 1.0)), 0.0, 1.0) * clamp ((10.0 - (sqrt(dot (tmpvar_9.xz, tmpvar_9.xz)) * (g_closeSnowData.w * 20.0))), 0.0, 1.0));
                float tmpvar_12;
                tmpvar_12 = min ((_Size * clamp (((tmpvar_10 * (_CentreFadeSpeed * 4.0)) - (12.0 * _CentreFadeSpeed)), 0.0, 1.0)), (dot (tmpvar_9, g_cameraForward.xyz) * 0.3));
                float tmpvar_13;
                tmpvar_13 = ((v.color.x * (4.0 * _Time.y)) - (2.0 * _Time.y));
                float tmpvar_14;
                tmpvar_14 = sin(tmpvar_13);
                float tmpvar_15;
                tmpvar_15 = cos(tmpvar_13);
                float2 tmpvar_16;
                tmpvar_16 = ((v.texcoord0.xy * (2.0 * tmpvar_12)) - tmpvar_12);
                pos_5 = (tmpvar_8 + ((((tmpvar_16.x * tmpvar_15) - (tmpvar_16.y * tmpvar_14)) * g_cameraRight.xyz) + (((tmpvar_16.y * tmpvar_15) + (tmpvar_16.x * tmpvar_14)) * g_cameraUp.xyz)));
                float tmpvar_17;
                tmpvar_17 = ((g_closeSnowFade.x * clamp (((tmpvar_10 * g_closeSnowFade.w) + g_closeSnowFade.z), 0.0, 1.0)) + g_closeSnowFade.y);
                float tmpvar_18;
                if ((tmpvar_17 < 0.25)) {
                tmpvar_18 = tmpvar_17;
                }
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                return (tex2D (_MainTex, i.texcoord0) * i.color);
            }
            ENDCG
        }
    }
    
}