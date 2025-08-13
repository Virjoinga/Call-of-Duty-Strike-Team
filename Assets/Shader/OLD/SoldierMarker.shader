Shader "Corona/Effects/SoldierMarker" {
    Properties {
        _MainTex ("Base", 2D) = "white" {}
        _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
    }
    SubShader { 
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZTest Always
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 _Color;
            float3 _DepthBand;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata_t v) {
                v2f o;
                
                float4 tmpvar_1;
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
                float3x3 tmpvar_5;
                tmpvar_5[0] = UNITY_MATRIX_MVP[0].xyz;
                tmpvar_5[1] = UNITY_MATRIX_MVP[1].xyz;
                tmpvar_5[2] = UNITY_MATRIX_MVP[2].xyz;
                float tmpvar_6;
                tmpvar_6 = (1.0 - abs(mul(tmpvar_5, normalize(v.normal)).z));
                tmpvar_1.w = tmpvar_6;
                tmpvar_1.w = (tmpvar_1.w * tmpvar_1.w);
                tmpvar_1.xyz = ((tmpvar_1.w + 0.5));
                tmpvar_1.w = ((tmpvar_1.w * 0.5) + 0.5);
                float tmpvar_7;
                tmpvar_7 = ((v.vertex.z + _Color.w) + 100.0);
                float tmpvar_8;
                tmpvar_8 = frac(abs(tmpvar_7));
                float tmpvar_9;
                if ((tmpvar_7 >= 0.0)) {
                    tmpvar_9 = tmpvar_8;
                } else {
                    tmpvar_9 = -(tmpvar_8);
                };
                float tmpvar_10;
                tmpvar_10 = (1.0 - (tmpvar_9 * 2.0));
                float tmpvar_11;
                tmpvar_11 = (tmpvar_1.w * ((tmpvar_10 * (tmpvar_10 * 0.7)) + 0.3));
                tmpvar_1.w = tmpvar_11;
                o.pos = tmpvar_4;
                o.color = tmpvar_1;
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = _Color.xyz;
                return (tmpvar_1 * i.color);
            }
            ENDCG
        }
    }
}