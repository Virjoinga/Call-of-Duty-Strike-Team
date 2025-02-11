using UnityEngine;

public static class DebugDraw
{
	private static Material mLineMaterial;

	public static Material LineMaterial
	{
		get
		{
			if (mLineMaterial == null)
			{
				mLineMaterial = new Material("Shader \"Lines/Colored Blended\" {SubShader { Pass {  Blend SrcAlpha OneMinusSrcAlpha  ZWrite Off Cull Off Fog { Mode Off }  BindChannels { Bind \"vertex\", vertex Bind \"color\", color }} } }");
				mLineMaterial.hideFlags = HideFlags.HideAndDontSave;
				mLineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
			}
			return mLineMaterial;
		}
	}
}
