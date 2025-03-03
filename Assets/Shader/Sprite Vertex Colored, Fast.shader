Shader "Sprite/Vertex Colored, Fast" {
	Properties {
		_MainTex ("Particle Texture", 2D) = "white" {}
	}
	SubShader { 
		Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
		Pass {
			BindChannels {
				Bind "vertex", Vertex
				Bind "color", Color
				Bind "texcoord", TexCoord
			}
			ZWrite Off
			Cull Off
			Fog {
				Color (0,0,0,0)
			}
			Blend SrcAlpha OneMinusSrcAlpha
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}