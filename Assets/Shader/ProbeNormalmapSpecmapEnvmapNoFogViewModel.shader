Shader "Corona/Probe/[Normal] [Spec] [Env] [NoFog] [ViewModel]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _SpecPower ("Specular Power", Range(0,50)) = 10
        _Reflectivity ("Reflectivity", Range(0,1)) = 0.2
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
            float4 cC;
            float4 cBb;
            float4 cBg;
            float4 cBr;
            float4 cAb;
            float4 cAg;
            float4 cAr;
            float3 _DepthBand;
            float3 _SpecDir;

            float _Reflectivity;
            samplerCUBE _ThemedCube;
            sampler2D _SpecMap;
            sampler2D _BumpMap;
            sampler2D _MainTex;
            float _SpecPower;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float3 reflDir : TEXCOORD3;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                
                o.normal = worldNormal;
                o.viewDir = viewDir;
                o.reflDir = reflect(-viewDir, worldNormal);
                
                return o;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                float3 normal = UnpackNormal(tex2D(_BumpMap, i.uv));
                float3 viewDir = normalize(i.viewDir);
                float3 reflDir = normalize(i.reflDir);
                
                float3 specMask = tex2D(_SpecMap, i.uv).rgb;
                float3 baseColor = tex2D(_MainTex, i.uv).rgb;
                
                float spec = pow(max(dot(reflect(-viewDir, normal), viewDir), 0.0), _SpecPower);
                float3 specColor = spec * specMask;
                
                float3 envColor = texCUBE(_ThemedCube, reflDir).rgb * _Reflectivity;
                
                return half4(baseColor + specColor + envColor, 1.0);
            }
            ENDCG
        }
    }
}
