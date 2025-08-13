Shader "Corona/Effects/HologramZFill" {
    SubShader { 
        Tags { "QUEUE"="Transparent-2" }
        Pass {
            Name "PASS"
            Tags { "QUEUE"="Transparent-2" }
            Fog { Mode Off }
            ColorMask 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _DepthBand;

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;

                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = v.vertex.xyz;
                float4 tmpvar_2;
                tmpvar_2 = UnityObjectToClipPos(tmpvar_1);
                float4 tmpvar_3;
                tmpvar_3.x = tmpvar_2.x;
                tmpvar_3.y = tmpvar_2.y;
                tmpvar_3.z = ((tmpvar_2.z * _DepthBand.z) + (tmpvar_2.w * _DepthBand.y));
                tmpvar_3.w = tmpvar_2.w;
                o.vertex = tmpvar_3;

                return o;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                return (1).xxxx;
            }
            ENDCG
        }
    }
}