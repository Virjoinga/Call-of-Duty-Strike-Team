using UnityEngine;

public class MissionCompleteSoldierPanelController : MenuScreenBlade
{
	public SpriteText NameText;

	public SpriteText KillsXpText;

	public SpriteText BonusXpText;

	public SpriteText KillsXpValueText;

	public SpriteText BonusXpValueText;

	public SpriteText TapToClaimMsg;

	public SpriteText TapToClaimText;

	public PackedSprite TapToClaimBg;

	private PackedSprite[] SoldierImages;

	private PackedSprite[] MiaKiaImages;

	private CommonBackgroundBox mBox;

	private PackedSprite ActiveImage;

	private string mID;

	private bool mPresent;

	public override void Awake()
	{
		base.Awake();
		mBox = base.transform.GetComponentInChildren<CommonBackgroundBox>();
		Transform transform = base.transform.FindChild("SoldierPlacement");
		Transform transform2 = transform.FindChild("Content");
		Transform transform3 = transform2.FindChild("SoldierImages");
		SoldierImages = transform3.GetComponentsInChildren<PackedSprite>();
		Transform transform4 = transform2.FindChild("KIAMIA");
		MiaKiaImages = transform4.GetComponentsInChildren<PackedSprite>();
		if (TapToClaimBg != null)
		{
			TapToClaimBg.gameObject.SetActive(false);
		}
	}

	public void Setup(int index, SoldierSettings soldier, MissionData mission)
	{
		Setup(index, soldier.Name, soldier.Present, mission, index);
	}

	public void Setup(int index, string soldierName, bool soldierPresent, MissionData mission, int imageIndex)
	{
		MissionData.eEnvironment environment = MissionData.eEnvironment.Arctic;
		mPresent = soldierPresent;
		mID = StatsManager.ConvertSoldierIndexToId(index);
		if (mission != null)
		{
			environment = mission.Environment;
		}
		int xPFromKillsForCharacter = StatsHelper.GetXPFromKillsForCharacter(mID);
		int xPFromBonusesForCharacter = StatsHelper.GetXPFromBonusesForCharacter(mID);
		int numKillsForCharacter = StatsHelper.GetNumKillsForCharacter(mID);
		string text = Language.Get("S_KILLS");
		string text2 = Language.Get("S_RESULT_XP");
		UpdateText(NameText, soldierName);
		UpdateText(KillsXpValueText, xPFromKillsForCharacter + text2);
		UpdateText(BonusXpValueText, xPFromBonusesForCharacter + text2);
		UpdateText(KillsXpText, text + "x" + numKillsForCharacter);
		SetSoldierImage(imageIndex, environment);
		UpdateSoldierStatus(false);
		UpdateTapToClaim(xPFromKillsForCharacter + xPFromBonusesForCharacter);
	}

	public void UpdateStatus()
	{
		bool kia = StatsHelper.WasCharacterKIA(mID);
		UpdateSoldierStatus(kia);
		if (mBox != null)
		{
			mBox.RefreshPlacements = true;
			mBox.Resize();
		}
	}

	public void PlayerPurchasedXP()
	{
		UpdateSoldierStatus(false);
	}

	private void SetSoldierImage(int index, MissionData.eEnvironment environment)
	{
		PackedSprite[] soldierImages = SoldierImages;
		foreach (PackedSprite packedSprite in soldierImages)
		{
			bool flag = (packedSprite.name.Contains(environment.ToString()) && packedSprite.name.Contains(index.ToString())) || packedSprite.name == "Background";
			packedSprite.gameObject.SetActive(flag);
			if (packedSprite.name != "Background")
			{
				packedSprite.SetColor((!mPresent) ? ColourChart.GreyedOut : Color.white);
			}
			if (flag && packedSprite.name != "Background")
			{
				ActiveImage = packedSprite;
			}
		}
	}

	private void UpdateSoldierStatus(bool kia)
	{
		PackedSprite[] miaKiaImages = MiaKiaImages;
		foreach (PackedSprite packedSprite in miaKiaImages)
		{
			bool flag = kia && packedSprite.name == "KIA_image";
			packedSprite.gameObject.SetActive(flag);
		}
		ActiveImage.SetColor((!kia && mPresent) ? Color.white : ColourChart.GreyedOut);
		int xPFromKillsForCharacter = StatsHelper.GetXPFromKillsForCharacter(mID);
		int xPFromBonusesForCharacter = StatsHelper.GetXPFromBonusesForCharacter(mID);
		bool flag2 = kia && mPresent && xPFromKillsForCharacter + xPFromBonusesForCharacter > 0;
		TapToClaimBg.gameObject.SetActive(flag2);
		if (NameText != null && KillsXpText != null && BonusXpText != null && KillsXpValueText != null && BonusXpValueText != null)
		{
			NameText.gameObject.SetActive(!flag2);
			KillsXpText.gameObject.SetActive(!flag2);
			BonusXpText.gameObject.SetActive(!flag2);
			KillsXpValueText.gameObject.SetActive(!flag2);
			BonusXpValueText.gameObject.SetActive(!flag2);
		}
		BoxCollider componentInChildren = base.transform.GetComponentInChildren<BoxCollider>();
		if (componentInChildren != null)
		{
			componentInChildren.enabled = !componentInChildren.enabled;
			componentInChildren.enabled = !componentInChildren.enabled;
		}
	}

	private void UpdateText(SpriteText text, string str)
	{
		if (text != null)
		{
			text.Text = str;
		}
	}

	private void UpdateTapToClaim(int totalxp)
	{
		if (TapToClaimText != null && TapToClaimBg != null && totalxp > 0 && mPresent)
		{
			int purchaseSoliderCost = SwrveServerVariables.Instance.PurchaseSoliderCost;
			char c = CommonHelper.HardCurrencySymbol();
			string text = string.Format("{0}{1}", purchaseSoliderCost, c);
			TapToClaimText.Text = text;
			TapToClaimBg.gameObject.SetActive(false);
		}
		if (TapToClaimMsg != null)
		{
			TapToClaimMsg.Text = Language.Get("S_TAP_TO_CLAIM");
		}
	}
}
