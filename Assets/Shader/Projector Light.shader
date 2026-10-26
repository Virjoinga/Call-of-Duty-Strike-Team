Shader "Projector/Light" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _ShadowTex ("Cookie", 2D) = "" {}
 _FalloffTex ("FallOff", 2D) = "" {}
}
SubShader {
 Pass {
  ZWrite Off
  Blend DstColor One
  ColorMask RGB
  Offset -1, -1
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #pragma multi_compile_fog
  #include "UnityCG.cginc"
  sampler2D _ShadowTex, _FalloffTex;
  float4 _Color;
  float4x4 unity_Projector, unity_ProjectorClip;
  struct v2f
  {
      float4 vertex : SV_POSITION;
      float4 cookie : TEXCOORD0;
      float4 falloff : TEXCOORD1;
      UNITY_FOG_COORDS(2)
  };
  v2f vert(float4 vertex : POSITION)
  {
      v2f o;
      o.vertex = UnityObjectToClipPos(vertex);
      o.cookie = mul(unity_Projector, vertex);
      o.falloff = mul(unity_ProjectorClip, vertex);
      UNITY_TRANSFER_FOG(o, o.vertex);
      return o;
  }
  float4 frag(v2f i) : SV_Target
  {
      float4 cookie = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.cookie));
      float4 color = cookie * _Color;
      color.a = 1.0 - cookie.a;
      color *= tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.falloff)).a;
      UNITY_APPLY_FOG_COLOR(i.fogCoord, color, float4(0, 0, 0, 1));
      return color;
  }
  ENDCG
 }
}
}
