using UnityEngine;

public class LoadoutCharacterPanel : MonoBehaviour
{
	public SpriteText NameText;

	public void Setup(string soldierName, WeaponDescriptor weapon)
	{
		NameText.Text = soldierName;
	}
}
