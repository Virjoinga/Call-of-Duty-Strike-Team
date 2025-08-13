Shader "Corona/Sky/Sky" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        LOD 200
        Tags { "QUEUE"="Background" }
        Pass
        {
            Tags { "QUEUE"="Background" }
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
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
                tmpvar_4.xyz = UnityObjectToViewPos(tmpvar_3).xyz;
                float4 tmpvar_5;
                tmpvar_5 = mul(UNITY_MATRIX_P, tmpvar_4).xyww;
                trans_2.xyw = tmpvar_5.xyw;
                trans_2.z = (tmpvar_5.z * 0.999999);
                float2 tmpvar_6;
                tmpvar_6 = v.texcoord0.xy;
                tmpvar_1 = tmpvar_6;
                o.vertex = trans_2;
                o.texcoord0 = tmpvar_1;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.texcoord0);
                tmpvar_1 = tmpvar_2;
                return tmpvar_1;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}