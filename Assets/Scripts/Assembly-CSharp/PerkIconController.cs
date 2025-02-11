public class PerkIconController : IconControllerBase
{
	public void SetPerk(PerkType perk, bool pro, bool available)
	{
		if (perk == PerkType.Perk1Greed || perk == PerkType.Perk2Greed || perk == PerkType.Perk3Greed)
		{
			pro = false;
		}
		base.Index = (int)(perk + (pro ? 23 : 0));
		base.Available = available;
		CalculateSimpleSpriteSettings();
	}
}
