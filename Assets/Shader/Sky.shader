Shader "Corona/Sky/Sky" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader { 
        LOD 200
        Tags { "QUEUE"="Background" }
        Pass {
            Fog { Mode Off }
            Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;

                float2 tmpvar_1;
                float4 trans_2;
                float4 tmpvar_3;
                tmpvar_3.w = 0.0;
                tmpvar_3.xyz = v.vertex.xyz;
                float4 tmpvar_4;
                tmpvar_4.w = 1.0;
                tmpvar_4.xyz = mul(unity_ObjectToWorld, tmpvar_3).xyz;
                float4 tmpvar_5;
                tmpvar_5 = (UnityObjectToClipPos(tmpvar_4)).xyww;
                trans_2.xyw = tmpvar_5.xyw;
                trans_2.z = (tmpvar_5.z * 0.999999);
                float2 tmpvar_6;
                tmpvar_6 = v.uv.xy;
                tmpvar_1 = tmpvar_6;
                o.pos = trans_2;
                o.uv = tmpvar_1;

                return o;
            }
            half4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.uv);
                tmpvar_1 = tmpvar_2;
                return tmpvar_1;
            }
            ENDCG
        }
    }
}