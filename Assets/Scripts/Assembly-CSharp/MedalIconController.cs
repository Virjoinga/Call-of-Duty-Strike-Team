using UnityEngine;

public class MedalIconController : IconControllerBase
{
	public enum MedalIcon
	{
		Star = 0,
		Tick = 1,
		XP = 2,
		Clock = 3,
		Skull = 4,
		GMGGrenade = 10,
		GMGToken = 11,
		GMGHeadshot = 12,
		GMGSkull = 13,
		GMGSwords = 14,
		GMGTimeTrial = 15,
		GMGDomination = 16
	}

	protected override void Awake()
	{
		base.Awake();
		AlphaBlack = new Color(0.3f, 0.3f, 0.3f, 0.6f);
	}

	public void SetMedal(MedalIcon medal, bool veteran, bool earned)
	{
		base.Index = (int)(medal + (veteran ? SectionData.numMedalSlots : 0));
		base.Available = earned;
		CalculateSimpleSpriteSettings();
	}
}
