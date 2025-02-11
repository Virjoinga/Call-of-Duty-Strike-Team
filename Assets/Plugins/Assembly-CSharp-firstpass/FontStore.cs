using System.Collections.Generic;
using UnityEngine;

public static class FontStore
{
	private static SpriteFont[] fonts = new SpriteFont[0];

	private static Dictionary<string, TextAsset> x2FontDefDict = new Dictionary<string, TextAsset>();

	public static void PopulateHiResFonts(TextAsset[] x2FontDef)
	{
		for (int i = 0; i < x2FontDef.Length; i++)
		{
			x2FontDefDict.Add(x2FontDef[i].name, x2FontDef[i]);
		}
	}

	private static TextAsset GetFontDefForResolution(TextAsset fontDef)
	{
		string key = fontDef.name + "_@x2";
		TextAsset value;
		if (x2FontDefDict.TryGetValue(key, out value))
		{
			return value;
		}
		return fontDef;
	}

	public static SpriteFont GetFont(TextAsset fontDef)
	{
		if (fontDef == null)
		{
			return null;
		}
		fontDef = GetFontDefForResolution(fontDef);
		for (int i = 0; i < fonts.Length; i++)
		{
			if (fonts[i].fontDef == fontDef)
			{
				if (!Application.isPlaying)
				{
					fonts[i] = new SpriteFont(fontDef);
				}
				return fonts[i];
			}
		}
		SpriteFont spriteFont = new SpriteFont(fontDef);
		AddFont(spriteFont);
		return spriteFont;
	}

	private static void AddFont(SpriteFont f)
	{
		SpriteFont[] array = new SpriteFont[fonts.Length + 1];
		fonts.CopyTo(array, 0);
		array[fonts.Length] = f;
		fonts = array;
	}
}
