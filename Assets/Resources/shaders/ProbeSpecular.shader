Shader "Hidden/ProbeSpecular" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 cAr ("Light Probe Ar", Color) = (1,1,1,1)
 cAg ("Light Probe Ag", Color) = (1,1,1,1)
 cAb ("Light Probe Ab", Color) = (1,1,1,1)
 cBr ("Light Probe Br", Color) = (1,1,1,1)
 cBg ("Light Probe Bg", Color) = (1,1,1,1)
 cBb ("Light Probe Bb", Color) = (1,1,1,1)
 cC ("Light Probe C", Color) = (1,1,1,1)
 _SpecDir ("Specular Dir", Vector) = (-0.66,0.333,0.66,1)
}
SubShader { 
 LOD 200
 Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
 }
}
Fallback "Diffuse"
}