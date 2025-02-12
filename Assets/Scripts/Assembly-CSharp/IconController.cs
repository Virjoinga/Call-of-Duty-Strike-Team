using UnityEngine;

public class IconController : MonoBehaviour
{
	public enum Background
	{
		Hexagon = 0,
		Dimond = 1
	}

	public enum Icon
	{
		None = 0,
		Target = 1,
		Eye = 2,
		Grenade = 3,
		Shield = 4,
		Swords = 5,
		Boot = 6
	}

	public enum SubIcon
	{
		None = 0,
		I = 1,
		II = 2,
		III = 3,
		Star = 4,
		Plus = 5
	}

	public Color BackgroundColour;

	public Color SubIconBackgroundColour;

	public Background BackgroundType;

	public Background SubIconBackgroundType;

	public Icon IconType;

	public SubIcon SubIconType;

	public bool Locked;

	public bool None;

	private PackedSprite[] Backgrounds;

	private PackedSprite[] Icons;

	private PackedSprite[] SubIconBackgrounds;

	private PackedSprite[] SubIcons;

	private Transform BackgroundTransform;

	private Transform IconTransform;

	private Transform SubIconBackgroundTransform;

	private Transform SubIconTransform;

	private PackedSprite mLocked;

	private PackedSprite mNone;

	private float mAlpha;

	public float Alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			mAlpha = value;
		}
	}

	public float GetWidth()
	{
		float result = 0f;
		if (Backgrounds.Length > 0)
		{
			result = Backgrounds[0].width;
		}
		return result;
	}

	public float GetHeight()
	{
		float result = 0f;
		if (Backgrounds.Length > 0)
		{
			result = Backgrounds[0].height;
		}
		return result;
	}

	private void Awake()
	{
		BackgroundTransform = base.transform.Find("backgrounds");
		Backgrounds = BackgroundTransform.GetComponentsInChildren<PackedSprite>();
		IconTransform = base.transform.Find("icons");
		Icons = IconTransform.GetComponentsInChildren<PackedSprite>();
		SubIconBackgroundTransform = base.transform.Find("subicons_backgrounds");
		SubIconBackgrounds = SubIconBackgroundTransform.GetComponentsInChildren<PackedSprite>();
		SubIconTransform = base.transform.Find("subicons");
		SubIcons = SubIconTransform.GetComponentsInChildren<PackedSprite>();
		PackedSprite[] componentsInChildren = GetComponentsInChildren<PackedSprite>();
		PackedSprite[] array = componentsInChildren;
		foreach (PackedSprite packedSprite in array)
		{
			if (packedSprite.name == "Locked")
			{
				mLocked = packedSprite;
			}
			else if (packedSprite.name == "None")
			{
				mNone = packedSprite;
			}
		}
	}

	public void Refresh()
	{
		SetBackground();
		SetIcon();
		SetSubIcon();
		Color color = new Color(1f, 1f, 1f, mAlpha);
		if (mLocked != null)
		{
			mLocked.gameObject.SetActive(Locked);
			mLocked.SetColor(color);
		}
		if (mNone != null)
		{
			mNone.gameObject.SetActive(None);
			mLocked.SetColor(color);
		}
		IconTransform.position = new Vector3(IconTransform.position.x, IconTransform.position.y, BackgroundTransform.position.z - 1f);
		SubIconBackgroundTransform.position = new Vector3(SubIconBackgroundTransform.position.x, SubIconBackgroundTransform.position.y, BackgroundTransform.position.z - 2f);
		SubIconTransform.position = new Vector3(SubIconTransform.position.x, SubIconTransform.position.y, BackgroundTransform.position.z - 3f);
	}

	private void SetBackground()
	{
		PackedSprite[] backgrounds = Backgrounds;
		foreach (PackedSprite packedSprite in backgrounds)
		{
			if (!Locked && IconType != 0 && packedSprite.gameObject.name == BackgroundType.ToString())
			{
				Color color = new Color(BackgroundColour.r, BackgroundColour.g, BackgroundColour.b, mAlpha);
				packedSprite.gameObject.SetActive(true);
				packedSprite.SetColor(color);
			}
			else
			{
				packedSprite.gameObject.SetActive(false);
			}
		}
	}

	private void SetIcon()
	{
		Color color = new Color(1f, 1f, 1f, mAlpha);
		PackedSprite[] icons = Icons;
		foreach (PackedSprite packedSprite in icons)
		{
			bool flag = !Locked && packedSprite.gameObject.name == IconType.ToString();
			packedSprite.gameObject.SetActive(flag);
			if (flag)
			{
				packedSprite.SetColor(color);
			}
		}
	}

	private void SetSubIcon()
	{
		Color color = new Color(1f, 1f, 1f, mAlpha);
		PackedSprite[] subIconBackgrounds = SubIconBackgrounds;
		foreach (PackedSprite packedSprite in subIconBackgrounds)
		{
			if (!Locked && SubIconType != 0 && packedSprite.gameObject.name == SubIconBackgroundType.ToString())
			{
				Color color2 = new Color(SubIconBackgroundColour.r, SubIconBackgroundColour.g, SubIconBackgroundColour.b, mAlpha);
				packedSprite.gameObject.SetActive(true);
				packedSprite.SetColor(color2);
			}
			else
			{
				packedSprite.gameObject.SetActive(false);
			}
		}
		PackedSprite[] subIcons = SubIcons;
		foreach (PackedSprite packedSprite2 in subIcons)
		{
			bool flag = !Locked && packedSprite2.gameObject.name == SubIconType.ToString();
			packedSprite2.gameObject.SetActive(flag);
			if (flag)
			{
				packedSprite2.SetColor(color);
			}
		}
	}
}
