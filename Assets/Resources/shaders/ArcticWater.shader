Shader "Corona/Water/Arctic"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SunVec ("SunVector", Vector) = (-0.6,0.3,0.75,1)
        _Ripple ("Ripple1", Vector) = (0.05,0.1,0.1,0.1)
        _Ripple2 ("Ripple2", Vector) = (0.065,0.11,0.054,0.11)
        baseWaterCol ("Base Water Col", Color) = (0.7,0.9,1,1)
        subWaterCol ("SubWaterCol", Color) = (0.6,0.6,0.8,1)
        _MainTex ("NormalMap", 2D) = "white" {}
        RippleAnim ("RippleAnim", Vector) = (1000,1000,1000,1000)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "../../Shader/CoronaLegacy.cginc"

            sampler2D _MainTex;
            float3 _DepthBand;
            float4 _Ripple, _Ripple2, _SunVec, _Color;
            float4 baseWaterCol, subWaterCol;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 ripples : TEXCOORD0;
                float4 pcopy : TEXCOORD1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                float4 pos = CoronaObjectToClipPos(float4(v.vertex.xyz, 1.0));
                pos.z = pos.z * _DepthBand.z + pos.w * _DepthBand.y;
                o.vertex = CoronaClipToNative(pos);
                o.pcopy = mul(unity_ObjectToWorld, v.vertex);
                o.pcopy.w = _Time.y * 0.2;
                o.ripples = float4(dot(o.pcopy.xz, _Ripple.xy), dot(o.pcopy.xz, _Ripple.zw),
                                  dot(o.pcopy.xz, _Ripple2.xy), dot(o.pcopy.xz, _Ripple2.zw));
                o.ripples += float4(0.1, 0.11, 0.02, 0.025) * _Time.w * 2.0;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // The exported xll_mod helper uses signed remainder, as does fmod.
                float4 modrip = fmod(i.ripples, 1.0);
                float4 remrip = i.ripples - modrip;
                float4 dmd = modrip * 2.0 + modrip * modrip * modrip * 4.0 - modrip * modrip * 6.0;
                float3 eye = normalize(i.pcopy.xyz - _WorldSpaceCameraPos);
                float2 uv = i.pcopy.xz + eye.zx * dmd.x + i.pcopy.ww;
                float2 uv2 = i.pcopy.xz + eye.zx * dmd.x + i.pcopy.ww * 0.9;
                float3 normal = (tex2D(_MainTex, (remrip.xx + uv) * 0.1).xzy - 0.5) * dmd.x * 5.0
                              + (tex2D(_MainTex, (remrip.ww + uv2) * 0.052).yzx - 0.5) * dmd.w * 5.0;
                normal.y = 1.0;
                normal.xz += _Ripple.xy * dmd.x + _Ripple.zw * dmd.y + _Ripple2.xy * dmd.z + _Ripple2.zw * dmd.w;
                normal = normalize(normal);
                float3 reflection = reflect(eye, normal);
                float b = -1.0 - dot(eye, normal);
                b *= b * b;
                b = -1.0 - b;
                float3 color = baseWaterCol.xyz + subWaterCol.xyz * min(0.0, b);
                float sun = max(0.0, dot(_SunVec.xyz, reflection) * 200.0 - 198.0);
                return float4(color + _Color.xyz * sun, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
