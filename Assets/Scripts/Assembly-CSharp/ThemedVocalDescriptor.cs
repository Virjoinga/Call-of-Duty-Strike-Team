using System.Collections.Generic;
using UnityEngine;

public class ThemedVocalDescriptor : ScriptableObject
{
	public BaseCharacter.Nationality Default;

	public List<ThemeVocal> ThemeVocals;

	public BaseCharacter.Nationality GetVocalForTheme(string theme)
	{
		ThemeVocal themeVocal = ThemeVocals.Find((ThemeVocal obj) => obj.Theme == theme);
		return (themeVocal == null) ? Default : themeVocal.Nationality;
	}
}
