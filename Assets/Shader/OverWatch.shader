Shader "Corona/OverWatch" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _GrainTex ("Base (RGB)", 2D) = "gray" {}
    }
    SubShader
    {
        Pass
        {
            ZTest False
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _GrainOffsetScale;
            float _Fade;
            float _Saturation;
            float _IntensitySquare;
            float _Intensity;
            sampler2D _GrainTex;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float2 tmpvar_1;
                tmpvar_1 = v.texcoord0.xy;
                float2 tmpvar_2;
                float2 tmpvar_3;
                float2 tmpvar_4;
                float4 tmpvar_5;
                tmpvar_5.zw = float2(0.0, 0.0);
                tmpvar_5.x = tmpvar_1.x;
                tmpvar_5.y = tmpvar_1.y;
                tmpvar_4 = mul(UNITY_MATRIX_TEXTURE0, tmpvar_5).xy;
                tmpvar_2 = tmpvar_4;
                float2 tmpvar_6;
                tmpvar_6 = ((v.texcoord0.xy * _GrainOffsetScale.zw) + _GrainOffsetScale.xy);
                tmpvar_3 = tmpvar_6;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = tmpvar_2;
                o.texcoord1 = tmpvar_3;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 col_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.texcoord0);
                col_1.w = tmpvar_2.w;
                col_1.xyz = lerp (dot (tmpvar_2.xyz, float3(0.3, 0.59, 0.11)), tmpvar_2.x, _Saturation);
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_GrainTex, i.texcoord1);
                col_1.xyz = (col_1.xyz + (tmpvar_3.xyz * _IntensitySquare));
                float4 tmpvar_4;
                tmpvar_4 = (col_1 + (tmpvar_3.w * _Intensity));
                col_1.w = tmpvar_4.w;
                float2 tmpvar_5;
                tmpvar_5 = (float2(0.5, 0.5) - i.texcoord0);
                float tmpvar_6;
                tmpvar_6 = dot (tmpvar_5, tmpvar_5);
                col_1.xyz = (tmpvar_4.xyz - ((tmpvar_6 * tmpvar_6) * _Fade));
                return col_1;
            }
            ENDCG
        }
    }
    Fallback Off
}