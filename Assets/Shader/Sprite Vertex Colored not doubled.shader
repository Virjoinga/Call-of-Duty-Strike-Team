Shader "Sprite/Vertex Colored not doubled" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
	SubShader { 
		Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
		Pass {
			Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
			Material {
				Diffuse [_Color]
			}
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			SetTexture [_MainTex] { combine texture * primary, texture alpha * primary alpha }
			SetTexture [_MainTex] { ConstantColor [_Color] combine previous * constant, previous alpha * constant alpha }
		}
	}
}