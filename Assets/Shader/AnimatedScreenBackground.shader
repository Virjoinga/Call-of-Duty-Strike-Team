Shader "Corona/HUD/AnimatedScreenBackground"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlendStart ("Blend Start", Color) = (1,1,0,1)
        _BlendMiddle ("Blend Middle", Color) = (1,1,0,1)
        _BlendEnd ("Blend End", Color) = (1,0,0,1)
        _Offset ("Offset", Range(0,1)) = 0
        _Alpha ("Alpha", Range(0,1)) = 1
        _PatternTex ("Pattern Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Tags { "Queue"="Transparent" "RenderType"="Transparent" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _PatternTex;
            float4 _BlendStart;
            float4 _BlendMiddle;
            float4 _BlendEnd;
            float _Offset;
            float _Alpha;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv1 = float2(v.uv1.x, _Offset);
                o.uv2 = float2(v.uv1.x, _Alpha);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float progress = tex2D(_PatternTex, i.uv1).r;
                float alpha = tex2D(_PatternTex, i.uv2).a;

                float blendFactor = sin(progress * 3.14159);
                float4 blendColor = lerp((progress < 0.5) ? _BlendStart : _BlendMiddle, _BlendEnd, blendFactor);
                blendColor.a = alpha;

                return tex2D(_MainTex, i.uv) * blendColor;
            }
            ENDCG
        }
    }
}
