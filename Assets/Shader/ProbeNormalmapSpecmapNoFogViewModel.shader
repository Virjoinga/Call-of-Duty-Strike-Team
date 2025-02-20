Shader "Corona/Probe/[Normal] [Spec] [NoFog] [ViewModel]" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SpecMap ("Specular Mask (RGB)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _SpecPower ("Specular Power", Range(0,50)) = 10
    }
    SubShader { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Tags { "LightMode"="ForwardBase" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float3 _AmbientLight;
            float3 _SpecDir;

            sampler2D _MainTex;
            sampler2D _SpecMap;
            sampler2D _BumpMap;
            float _SpecPower;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
            };

            v2f vert(appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                o.lightDir = normalize(_SpecDir);
                
                float3 normal = UnityObjectToWorldNormal(v.normal);
                o.normalDir = normalize(normal);
                
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                float3 normalMap = UnpackNormal(tex2D(_BumpMap, i.uv)).rgb;
                float3 normalDir = normalize(i.normalDir + normalMap);
                
                float3 viewDir = normalize(i.viewDir);
                float3 lightDir = normalize(i.lightDir);
                float3 halfDir = normalize(viewDir + lightDir);
                
                float spec = pow(saturate(dot(normalDir, halfDir)), _SpecPower);
                
                float3 albedo = tex2D(_MainTex, i.uv).rgb;
                float3 specColor = tex2D(_SpecMap, i.uv).rgb;
                
                float3 color = albedo + spec * specColor;
                return half4(color, 1.0);
            }
            ENDCG
        }
    }
}
