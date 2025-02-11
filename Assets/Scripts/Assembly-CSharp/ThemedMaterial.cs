using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ThemedMaterial
{
	public Material BaseMaterial;

	public List<ThemedTexture> ThemedTextures;

	public Texture GetTextureForTheme(string theme)
	{
		ThemedTexture themedTexture = ThemedTextures.Find((ThemedTexture obj) => obj.Theme == theme);
		return (themedTexture == null) ? null : themedTexture.TextureForTheme;
	}
}
