Shader "Color Space/YCrCbtoRGB" {
	Properties {
		_YTex ("Y (RGB)", 2D) = "white" {}
		_CbTex ("Cb (RGB)", 2D) = "white" {}
		_CrTex ("Cr (RGB)", 2D) = "white" {}
	}
	SubShader { 
		Tags { "RenderType"="Opaque" }
		Pass {
			Tags { "RenderType"="Opaque" }
			Fog {
				Color (0,0,0,0)
			}
			ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _YTex_ST;

            sampler2D _YTex;
            sampler2D _CrTex;
            sampler2D _CbTex;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                o.pos = (UnityObjectToClipPos(v.vertex));
                o.uv = ((v.uv.xy * _YTex_ST.xy) + _YTex_ST.zw);
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 rgbVec;
                float4 yuvVec;
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.x = tex2D (_YTex, i.uv).x;
                tmpvar_1.y = tex2D (_CbTex, i.uv).y;
                tmpvar_1.z = tex2D (_CrTex, i.uv).z;
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
}