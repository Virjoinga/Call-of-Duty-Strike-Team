Shader "Sprite/Vertex Colored, Fast, Split Channels" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _AlphaTex ("Base (A)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _AlphaTex;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float4 color0 : COLOR0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color0 = v.color;
                o.texcoord0 = v.texcoord0.xy;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1.xyz = tex2D (_MainTex, i.texcoord0).xyz;
                tmpvar_1.w = tex2D (_AlphaTex, i.texcoord0).x;
                return (tmpvar_1 * i.color0);
            }
            ENDCG
        }
    }
    
}