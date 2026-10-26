Shader "Color Space/YCrCbtoRGB"
{
    Properties
    {
        _YTex ("Y (RGB)", 2D) = "white" {}
        _CbTex ("Cb (RGB)", 2D) = "white" {}
        _CrTex ("Cr (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "RenderType"="Opaque" }
            Fog {
            Color (0,0,0,0)
            }
            ColorMask RGB
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _YTex_ST;
            sampler2D _YTex;
            sampler2D _CrTex;
            sampler2D _CbTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                  o.vertex = UnityObjectToClipPos(v.vertex);
                  o.texcoord0 = ((v.texcoord0.xy * _YTex_ST.xy) + _YTex_ST.zw);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                  float4 rgbVec;
                  float4 yuvVec;
                  float4 tmpvar_1;
                  tmpvar_1.w = 1.0;
                  tmpvar_1.x = tex2D (_YTex, i.texcoord0).x;
                  tmpvar_1.y = tex2D (_CbTex, i.texcoord0).y;
                  tmpvar_1.z = tex2D (_CrTex, i.texcoord0).z;
                  yuvVec = tmpvar_1;
                  rgbVec.x = dot (float4(1.16438, 0.0, 1.59603, -0.870785), yuvVec);
                  rgbVec.y = dot (float4(1.16438, -0.391762, -0.812969, 0.529594), yuvVec);
                  rgbVec.z = dot (float4(1.16438, 2.01723, 0.0, -1.08139), yuvVec);
                  rgbVec.w = 1.0;
                  return rgbVec;
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}
