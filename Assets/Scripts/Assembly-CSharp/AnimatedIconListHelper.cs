using System.Collections.Generic;
using UnityEngine;

public class AnimatedIconListHelper
{
	private enum IconType
	{
		None = 0,
		Weapon = 1,
		Equipment = 2,
		Perk = 3,
		ProPerk = 4,
		Tokens = 5,
		Veteran = 6
	}

	private enum Stage
	{
		None = 0,
		FadeUp = 1,
		Show = 2,
		FadeDown = 3
	}

	private struct IconData
	{
		public IconType type;

		public PerkType perk;

		public WeaponIconController.WeaponType weapon;

		public EquipmentIconController.EquipmentType equipment;

		public string text;
	}

	private const float MAX_TIME_IN_STAGE = 1f;

	private List<IconData> mData;

	private PerkIconController mPerkIcon;

	private WeaponIconController mWeaponIcon;

	private EquipmentIconController mEquipmentIcon;

	private PackedSprite mTokensSprite;

	private PackedSprite mVeteranSprite;

	private SpriteText mItemText;

	private string mBaseString;

	private float mTimeInStage;

	private int mCurrentIcon;

	private Stage mStage;

	public int Count
	{
		get
		{
			return (mData != null) ? mData.Count : 0;
		}
	}

	public void Setup(PerkIconController perkIcon, WeaponIconController weaponIcon, EquipmentIconController equipmentIcon, PackedSprite tokensSprite, PackedSprite veteranSprite, SpriteText spriteText)
	{
		Setup(perkIcon, weaponIcon, equipmentIcon, tokensSprite, veteranSprite, spriteText, string.Empty);
	}

	public void Setup(PerkIconController perkIcon, WeaponIconController weaponIcon, EquipmentIconController equipmentIcon, PackedSprite tokensSprite, PackedSprite veteranSprite, SpriteText spriteText, string baseString)
	{
		mPerkIcon = perkIcon;
		mWeaponIcon = weaponIcon;
		mEquipmentIcon = equipmentIcon;
		mTokensSprite = tokensSprite;
		mVeteranSprite = veteranSprite;
		mItemText = spriteText;
		mBaseString = baseString;
		mData = new List<IconData>();
		EnableItems(false);
	}

	public void AddTokenIcon(int numTokens, bool useIconInString)
	{
		IconData item = default(IconData);
		item.type = IconType.Tokens;
		if (useIconInString)
		{
			char c = CommonHelper.HardCurrencySymbol();
			item.text = string.Format("{0}{1}", c, numTokens);
		}
		else
		{
			item.text = numTokens.ToString();
		}
		mData.Add(item);
	}

	public void AddVeteranIcon()
	{
		IconData item = default(IconData);
		item.type = IconType.Veteran;
		item.text = Language.Get("S_VETERAN_MODE");
		mData.Add(item);
	}

	public void AddPerkIcon(PerkType perk)
	{
		IconData item = default(IconData);
		item.type = IconType.Perk;
		item.perk = perk;
		item.text = Language.Get("S_" + perk.ToString().ToUpper());
		mData.Add(item);
	}

	public void AddProPerkIcon(PerkType perk)
	{
		IconData item = default(IconData);
		item.type = IconType.ProPerk;
		item.perk = perk;
		string format = Language.Get("S_PRO");
		string arg = Language.Get("S_" + perk.ToString().ToUpper());
		item.text = string.Format(format, arg);
		mData.Add(item);
	}

	public void AddWeaponIcon(WeaponDescriptor weapon)
	{
		IconData item = default(IconData);
		item.type = IconType.Weapon;
		item.weapon = weapon.Type;
		item.text = weapon.Name;
		mData.Add(item);
	}

	public void AddEquipmentIcon(EquipmentDescriptor equipment, int count)
	{
		IconData item = default(IconData);
		item.type = IconType.Equipment;
		item.equipment = equipment.Type;
		item.text = Language.Get(equipment.Name).ToUpper() + ((count <= 0) ? string.Empty : (" x" + count));
		mData.Add(item);
	}

	public void Begin(string baseString)
	{
		mBaseString = baseString;
		Begin();
	}

	public void Begin()
	{
		if (mData != null && mData.Count > 0)
		{
			mCurrentIcon = 0;
			EnableItems(true);
			mStage = Stage.FadeUp;
			if (mData[mCurrentIcon].type != 0)
			{
				UpdateItems(mData[mCurrentIcon]);
				mItemText.Text = ((mBaseString == null || !(mBaseString != string.Empty)) ? mData[mCurrentIcon].text : string.Format(mBaseString, mData[mCurrentIcon].text));
			}
		}
	}

	public void Update()
	{
		if (mData == null || mData.Count <= mCurrentIcon)
		{
			return;
		}
		mTimeInStage += TimeManager.DeltaTime;
		float num = 0f;
		bool flag = mData[mCurrentIcon].type == IconType.Weapon;
		bool flag2 = mData[mCurrentIcon].type == IconType.Equipment;
		bool flag3 = mData[mCurrentIcon].type == IconType.Perk || mData[mCurrentIcon].type == IconType.ProPerk;
		bool flag4 = mData[mCurrentIcon].type == IconType.Tokens;
		bool flag5 = mData[mCurrentIcon].type == IconType.Veteran;
		if (mStage == Stage.FadeUp)
		{
			num = mTimeInStage / 1f;
			if (mTimeInStage >= 1f)
			{
				mStage = Stage.Show;
				mTimeInStage = 0f;
			}
		}
		else if (mStage == Stage.Show)
		{
			num = 1f;
			if (mTimeInStage >= 1f && mData.Count > 1)
			{
				int index = NextIcon();
				mItemText.Text = ((mBaseString == null || !(mBaseString != string.Empty)) ? mData[index].text : string.Format(mBaseString, mData[index].text));
				mStage = Stage.FadeDown;
				mTimeInStage = 0f;
			}
		}
		else if (mStage == Stage.FadeDown)
		{
			num = 1f - mTimeInStage / 1f;
			if (mTimeInStage >= 1f)
			{
				mCurrentIcon = NextIcon();
				mStage = Stage.FadeUp;
				mTimeInStage = 0f;
				flag = mData[mCurrentIcon].type == IconType.Weapon;
				flag2 = mData[mCurrentIcon].type == IconType.Equipment;
				flag3 = mData[mCurrentIcon].type == IconType.Perk || mData[mCurrentIcon].type == IconType.ProPerk;
				flag4 = mData[mCurrentIcon].type == IconType.Tokens;
				flag5 = mData[mCurrentIcon].type == IconType.Veteran;
				UpdateItems(mData[mCurrentIcon]);
			}
		}
		Color color = new Color(1f, 1f, 1f, (!flag) ? 0f : num);
		Color color2 = new Color(1f, 1f, 1f, (!flag2) ? 0f : num);
		Color color3 = new Color(1f, 1f, 1f, (!flag3) ? 0f : num);
		Color color4 = new Color(1f, 1f, 1f, (!flag4) ? 0f : num);
		Color color5 = new Color(1f, 1f, 1f, (!flag5) ? 0f : num);
		mWeaponIcon.SetColor(color);
		mEquipmentIcon.SetColor(color2);
		mPerkIcon.SetColor(color3);
		if (mTokensSprite != null && mVeteranSprite != null)
		{
			mTokensSprite.SetColor(color4);
			mVeteranSprite.SetColor(color5);
		}
	}

	public void Finish()
	{
		EnableItems(false);
	}

	private void EnableItems(bool enable)
	{
		if (mItemText != null)
		{
			mItemText.Hide(!enable);
		}
		if (mWeaponIcon != null)
		{
			mWeaponIcon.Hide(!enable);
		}
		if (mEquipmentIcon != null)
		{
			mEquipmentIcon.Hide(!enable);
		}
		if (mPerkIcon != null)
		{
			mPerkIcon.Hide(!enable);
		}
		if (mTokensSprite != null)
		{
			mTokensSprite.Hide(!enable);
		}
		if (mVeteranSprite != null)
		{
			mVeteranSprite.Hide(!enable);
		}
	}

	private void UpdateItems(IconData data)
	{
		if (mWeaponIcon != null && data.type == IconType.Weapon)
		{
			mWeaponIcon.SetWeapon(data.weapon, true);
		}
		else if (mEquipmentIcon != null && data.type == IconType.Equipment)
		{
			mEquipmentIcon.SetEquipment(data.equipment, true);
		}
		else if (mPerkIcon != null && (data.type == IconType.Perk || data.type == IconType.ProPerk))
		{
			bool pro = data.type == IconType.ProPerk;
			mPerkIcon.SetPerk(data.perk, pro, true);
		}
	}

	private int NextIcon()
	{
		return (mCurrentIcon + 1) % mData.Count;
	}
}
