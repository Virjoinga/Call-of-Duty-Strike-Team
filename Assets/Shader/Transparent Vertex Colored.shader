Shader "Transparent/Vertex Colored" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_Emission ("Emmisive Color", Color) = (0,0,0,0)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	SubShader { 
		Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
		Pass {
			Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
			Material {
				Ambient [_Color]
				Diffuse [_Color]
				Emission [_Emission]
			}
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			SetTexture [_MainTex] { combine texture * primary, texture alpha * primary alpha }
			SetTexture [_MainTex] { ConstantColor [_Color] combine previous * constant double, previous alpha * constant alpha }
		}
	}
}