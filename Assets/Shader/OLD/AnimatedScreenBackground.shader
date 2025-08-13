Shader "Corona/HUD/AnimatedScreenBackground" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlendStart ("BlendStart", Color) = (1,1,0,1)
        _BlendMiddle ("BlendMiddle", Color) = (1,1,0,1)
        _BlendEnd ("BlendEnd", Color) = (1,0,0,1)
        _Offset ("Offset", Range(0,1)) = 0
        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        LOD 200
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZWrite Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float _Alpha;
            float _Offset;
            float4 _BlendEnd;
            float4 _BlendMiddle;
            float4 _BlendStart;
            sampler2D _PatternTex;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord2 : TEXCOORD2;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float2 tmpvar_1;
                float2 tmpvar_2;
                float2 tmpvar_3;
                float2 tmpvar_4;
                tmpvar_4 = v.texcoord0.xy;
                tmpvar_1 = tmpvar_4;
                float2 tmpvar_5;
                tmpvar_5.x = v.texcoord1.x;
                tmpvar_5.y = _Offset;
                tmpvar_2 = tmpvar_5;
                float2 tmpvar_6;
                tmpvar_6.x = v.texcoord1.x;
                tmpvar_6.y = _Alpha;
                tmpvar_3 = tmpvar_6;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = tmpvar_1;
                o.texcoord1 = tmpvar_2;
                o.texcoord2 = tmpvar_3;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tileColour_1;
                float alpha_2;
                float progress_3;
                float tmpvar_4;
                tmpvar_4 = tex2D (_PatternTex, i.texcoord1).x;
                progress_3 = tmpvar_4;
                float tmpvar_5;
                tmpvar_5 = tex2D (_PatternTex, i.texcoord2).w;
                alpha_2 = tmpvar_5;
                float tmpvar_6;
                tmpvar_6 = sin((progress_3 * 3.14159));
                float4 tmpvar_7;
                if ((progress_3 < 0.5)) {
                tmpvar_7 = _BlendStart;
                }
                return tileColour_1;
            }
            ENDCG
        }
    }
    Fallback Off
}