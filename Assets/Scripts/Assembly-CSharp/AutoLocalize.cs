using UnityEngine;

public class AutoLocalize : MonoBehaviour
{
	public bool AutolocalizeChildren = true;

	private void Awake()
	{
		SpriteText component = base.gameObject.GetComponent<SpriteText>();
		if (component != null)
		{
			FixupSpriteText(component);
		}
		if (AutolocalizeChildren)
		{
			SpriteText[] componentsInChildren = base.gameObject.GetComponentsInChildren<SpriteText>();
			SpriteText[] array = componentsInChildren;
			foreach (SpriteText st in array)
			{
				FixupSpriteText(st);
			}
		}
		Object.Destroy(this);
	}

	private void FixupSpriteText(SpriteText st)
	{
		string text = st.Text;
		text = text.ToUpper();
		if (text.StartsWith("S_"))
		{
			string text2 = Get(text);
			st.Text = text2;
		}
	}

	public static string Get(string textKey)
	{
		return Language.Get(textKey);
	}
}
