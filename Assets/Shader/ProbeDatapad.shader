Shader "Corona/Effects/Datapad [ViewModel]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _PhaseLineTex ("Phase Line Texture", 2D) = "black" {}
        _Reflectivity ("Reflectivity", Range(0,1)) = 0.5
        _BroadSpecular ("Broad Specular", Range(0,1)) = 0.25
        _PhaseLineFrequency ("Phase line frequency", Range(0,5)) = 0.5
        _PhaseLineOverscan ("Phase line overscan", Range(1,2)) = 1.2
    }
    SubShader { 
        Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
        Pass {
            Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float3 _AmbientLight;
            float4 cC, cBb, cBg, cBr, cAb, cAg, cAr;
            float3 _DepthBand;
            float _PhaseLineOverscan, _PhaseLineFrequency;
            float _Reflectivity;
            float _BroadSpecular;
            samplerCUBE _ThemedCube;
            sampler2D _SpecMap, _MainTex, _PhaseLineTex, _NoiseTex;
            float g_datapadBrightness;
            float4 _FogRange;
            float3 _FogParams;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 reflDir : TEXCOORD1;
                float3 ambient : TEXCOORD2;
                float2 phaseUV : TEXCOORD3;
                float2 noiseUV : TEXCOORD4;
                float fogFactor : TEXCOORD5;
            };

            v2f vert(appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                
                o.reflDir = reflect(-viewDir, worldNormal);
                o.ambient = (_AmbientLight * _BroadSpecular);
                o.phaseUV = float2(((v.uv.y * 1024.0 / 245.0) - 4.17959) + (frac(_Time.y * _PhaseLineFrequency) * _PhaseLineOverscan),v.uv.x);  // Providing a second value for float2

                o.noiseUV = (v.uv * 2.0) + float2(frac(157.079 * _Time.y), frac(135.914 * _Time.y));
                
                float depth = o.pos.z * _DepthBand.x;
                o.fogFactor = clamp(depth * _FogRange.x + (_FogRange.y + 1.0), _FogRange.z, 1.0);
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET {
                float4 baseColor = tex2D(_MainTex, i.uv);
                float3 specMask = tex2D(_SpecMap, i.uv).rgb;
                float3 phaseColor = tex2D(_PhaseLineTex, i.phaseUV).rgb;
                float3 noiseColor = tex2D(_NoiseTex, i.noiseUV).rgb;
                float3 envColor = texCUBE(_ThemedCube, i.reflDir).rgb * _Reflectivity;
                
                float3 finalColor = (baseColor.rgb + phaseColor) * (noiseColor * g_datapadBrightness);
                finalColor += (i.ambient + (envColor * specMask));
                finalColor *= i.fogFactor;
                
                return half4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}
