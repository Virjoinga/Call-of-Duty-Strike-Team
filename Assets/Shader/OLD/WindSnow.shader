Shader "Corona/Effects/WindSnow" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float3 g_windSnowData;
            float3 _DepthBand;
            sampler2D _MainTex;
            float3 g_windSnowColour;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float color : COLOR;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float2 tmpvar_1;
                float tmpvar_2;
                float4 tmpvar_3;
                tmpvar_3.w = 1.0;
                tmpvar_3.xyz = v.vertex.xyz;
                float4 tmpvar_4;
                tmpvar_4 = UnityObjectToClipPos(tmpvar_3);
                float4 tmpvar_5;
                tmpvar_5.x = tmpvar_4.x;
                tmpvar_5.y = tmpvar_4.y;
                tmpvar_5.z = ((tmpvar_4.z * _DepthBand.z) + (tmpvar_4.w * _DepthBand.y));
                tmpvar_5.w = tmpvar_4.w;
                float tmpvar_6;
                tmpvar_6 = (v.texcoord0.x - dot (g_windSnowData, v.color.xyz));
                tmpvar_1.x = tmpvar_6;
                float tmpvar_7;
                tmpvar_7 = v.texcoord0.y;
                tmpvar_1.y = tmpvar_7;
                float tmpvar_8;
                tmpvar_8 = v.color.w;
                tmpvar_2 = tmpvar_8;
                o.vertex = tmpvar_5;
                o.texcoord0 = tmpvar_1;
                o.color = tmpvar_2;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 colour_1;
                colour_1.xyz = g_windSnowColour;
                colour_1.w = (tex2D (_MainTex, i.texcoord0) * i.color).x;
                return colour_1;
            }
            ENDCG
        }
    }
    
}