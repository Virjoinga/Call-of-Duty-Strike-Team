public class VocalSFXHelper
{
	private enum ExtraNPC
	{
		Dubois = 0,
		Spetsnaz = 1,
		ChineseVtol = 2,
		None = 3
	}

	protected static string[] CharacterNames = new string[3] { "Dubois", "Spet", "Player_VTOL" };

	public static void PainCry(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
		{
			bool flag = false;
			flag = (IsSpetsnazLevel() ? PlayVocals(c, VocalFriendlySFX.Instance.PainCry, VocalFriendly2SFX.Instance.PainCry, VocalFriendlySpetsnazSFX.Instance.PainCry, ExtraNPC.Spetsnaz) : (IsChineseVtolLevel() ? PlayVocals(c, VocalFriendlySFX.Instance.PainCry, VocalFriendly2SFX.Instance.PainCry, VocalFriendlyChineseVtolSFX.Instance.PainCry, ExtraNPC.ChineseVtol) : ((!IsDuboisLevel()) ? PlayVocals(c, VocalFriendlySFX.Instance.PainCry, VocalFriendly2SFX.Instance.PainCry, null, ExtraNPC.None) : PlayVocals(c, VocalFriendlySFX.Instance.PainCry, VocalFriendly2SFX.Instance.PainCry, VocalFriendlyDuboisSFX.Instance.PainCry, ExtraNPC.Dubois))));
			if (!flag && (bool)c.behaviour && !c.behaviour.PlayerControlled)
			{
				ImpactSFX.Instance.BulletHitBody.Play(c.gameObject);
			}
			break;
		}
		case BaseCharacter.Nationality.Russian:
			VocalRussianSFX.Instance.PainCry.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Arabic:
			VocalArabicSFX.Instance.PainCry.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Chinese:
			VocalChineseSFX.Instance.PainCry.Play(c.gameObject);
			break;
		}
	}

	public static void GMGDeath(Actor c)
	{
		SpecOpsVOSFX.Instance.S_SURVIVAL_DIALOGUE_DEATH_01.Play2D();
	}

	public static void GMGRevive(Actor c)
	{
		SpecOpsVOSFX.Instance.S_SURVIVAL_DIALOGUE_RESPAWN_01.Play2D();
	}

	public static void IntelCollected(Actor c)
	{
		CharacterSFX.Instance.IntelCollectedVO.Play2D();
	}

	public static void DeathCry(Actor c, HealthComponent.HeathChangeEventArgs hce)
	{
		if (c == null || hce.DamageType == "Script" || c.awareness.ChDefCharacterType != CharacterType.Human)
		{
			return;
		}
		if ((bool)c.baseCharacter && c.baseCharacter.VocalAccent == BaseCharacter.Nationality.Friendly && (bool)c.behaviour && !c.behaviour.PlayerControlled)
		{
			VocalFriendlySFX.Instance.DeathCry.Play(c.gameObject);
		}
		if (KillTypeHelper.IsAStealthKill(hce))
		{
			return;
		}
		if (hce.HeadShot)
		{
			if (hce.From != null)
			{
				Actor component = hce.From.GetComponent<Actor>();
				if ((bool)component && component == GameController.Instance.mFirstPersonActor)
				{
					ImpactSFX.Instance.HeadshotKill.Play2D();
				}
			}
			return;
		}
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.DeathCry, VocalFriendly2SFX.Instance.DeathCry, VocalFriendlySpetsnazSFX.Instance.DeathCry, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.DeathCry, VocalFriendly2SFX.Instance.DeathCry, VocalFriendlyChineseVtolSFX.Instance.DeathCry, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.DeathCry, VocalFriendly2SFX.Instance.DeathCry, VocalFriendlyDuboisSFX.Instance.DeathCry, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.DeathCry, VocalFriendly2SFX.Instance.DeathCry, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			VocalRussianSFX.Instance.DeathCry.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Arabic:
			VocalArabicSFX.Instance.DeathCry.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Chinese:
			VocalChineseSFX.Instance.DeathCry.Play(c.gameObject);
			break;
		}
	}

	public static void GrenadeThrown(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.GrenadeThrown, VocalFriendly2SFX.Instance.GrenadeThrown, VocalFriendlySpetsnazSFX.Instance.GrenadeThrown, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.GrenadeThrown, VocalFriendly2SFX.Instance.GrenadeThrown, VocalFriendlyChineseVtolSFX.Instance.GrenadeThrown, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.GrenadeThrown, VocalFriendly2SFX.Instance.GrenadeThrown, VocalFriendlyDuboisSFX.Instance.GrenadeThrown, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.GrenadeThrown, VocalFriendly2SFX.Instance.GrenadeThrown, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void ClaymoreDropped(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.ClaymoreDropped, VocalFriendly2SFX.Instance.ClaymoreDropped, VocalFriendlySpetsnazSFX.Instance.ClaymoreDropped, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.ClaymoreDropped, VocalFriendly2SFX.Instance.ClaymoreDropped, VocalFriendlyChineseVtolSFX.Instance.ClaymoreDropped, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.ClaymoreDropped, VocalFriendly2SFX.Instance.ClaymoreDropped, VocalFriendlyDuboisSFX.Instance.ClaymoreDropped, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.ClaymoreDropped, VocalFriendly2SFX.Instance.ClaymoreDropped, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void ManDown(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.ManDown, VocalFriendly2SFX.Instance.ManDown, VocalFriendlySpetsnazSFX.Instance.ManDown, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.ManDown, VocalFriendly2SFX.Instance.ManDown, VocalFriendlyChineseVtolSFX.Instance.ManDown, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.ManDown, VocalFriendly2SFX.Instance.ManDown, VocalFriendlyDuboisSFX.Instance.ManDown, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.ManDown, VocalFriendly2SFX.Instance.ManDown, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void KillConfirm(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.KillConfirm, VocalFriendly2SFX.Instance.KillConfirm, VocalFriendlySpetsnazSFX.Instance.KillConfirm, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.KillConfirm, VocalFriendly2SFX.Instance.KillConfirm, VocalFriendlyChineseVtolSFX.Instance.KillConfirm, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.KillConfirm, VocalFriendly2SFX.Instance.KillConfirm, VocalFriendlyDuboisSFX.Instance.KillConfirm, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.KillConfirm, VocalFriendly2SFX.Instance.KillConfirm, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void StealthKillConfirm(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			VocalFriendlySFX.Instance.StealthKillConfirm.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void GrenadeReaction(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.GrenadeReaction, VocalFriendly2SFX.Instance.GrenadeReaction, VocalFriendlySpetsnazSFX.Instance.GrenadeReaction, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.GrenadeReaction, VocalFriendly2SFX.Instance.GrenadeReaction, VocalFriendlyChineseVtolSFX.Instance.GrenadeReaction, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.GrenadeReaction, VocalFriendly2SFX.Instance.GrenadeReaction, VocalFriendlyDuboisSFX.Instance.GrenadeReaction, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.GrenadeReaction, VocalFriendly2SFX.Instance.GrenadeReaction, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			VocalRussianSFX.Instance.GrenadeReaction.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Arabic:
			VocalArabicSFX.Instance.GrenadeReaction.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Chinese:
			VocalChineseSFX.Instance.GrenadeReaction.Play(c.gameObject);
			break;
		}
	}

	public static void OrderReceived(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.OrderReceived, VocalFriendly2SFX.Instance.OrderReceived, VocalFriendlySpetsnazSFX.Instance.OrderReceived, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.OrderReceived, VocalFriendly2SFX.Instance.OrderReceived, VocalFriendlyChineseVtolSFX.Instance.OrderReceived, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.OrderReceived, VocalFriendly2SFX.Instance.OrderReceived, VocalFriendlyDuboisSFX.Instance.OrderReceived, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.OrderReceived, VocalFriendly2SFX.Instance.OrderReceived, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			VocalRussianSFX.Instance.OrderReceived.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Arabic:
			VocalArabicSFX.Instance.OrderReceived.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Chinese:
			VocalChineseSFX.Instance.OrderReceived.Play(c.gameObject);
			break;
		}
	}

	public static void SniperSpotted(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SniperSpotted, VocalFriendly2SFX.Instance.SniperSpotted, VocalFriendlySpetsnazSFX.Instance.SniperSpotted, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SniperSpotted, VocalFriendly2SFX.Instance.SniperSpotted, VocalFriendlyChineseVtolSFX.Instance.SniperSpotted, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SniperSpotted, VocalFriendly2SFX.Instance.SniperSpotted, VocalFriendlyDuboisSFX.Instance.SniperSpotted, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SniperSpotted, VocalFriendly2SFX.Instance.SniperSpotted, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void Reload(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.Reload, VocalFriendly2SFX.Instance.Reload, VocalFriendlySpetsnazSFX.Instance.Reload, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.Reload, VocalFriendly2SFX.Instance.Reload, VocalFriendlyChineseVtolSFX.Instance.Reload, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.Reload, VocalFriendly2SFX.Instance.Reload, VocalFriendlyDuboisSFX.Instance.Reload, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.Reload, VocalFriendly2SFX.Instance.Reload, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void SniperKilled(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SniperDown, VocalFriendly2SFX.Instance.SniperDown, VocalFriendlySpetsnazSFX.Instance.SniperDown, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SniperDown, VocalFriendly2SFX.Instance.SniperDown, VocalFriendlyChineseVtolSFX.Instance.SniperDown, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SniperDown, VocalFriendly2SFX.Instance.SniperDown, VocalFriendlyDuboisSFX.Instance.SniperDown, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SniperDown, VocalFriendly2SFX.Instance.SniperDown, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void Healed(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SquadRevived, VocalFriendly2SFX.Instance.SquadRevived, VocalFriendlySpetsnazSFX.Instance.SquadRevived, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SquadRevived, VocalFriendly2SFX.Instance.SquadRevived, VocalFriendlyChineseVtolSFX.Instance.SquadRevived, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SquadRevived, VocalFriendly2SFX.Instance.SquadRevived, VocalFriendlyDuboisSFX.Instance.SquadRevived, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.SquadRevived, VocalFriendly2SFX.Instance.SquadRevived, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void LostAimedShot(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			if (IsSpetsnazLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.LostAimedShot, VocalFriendly2SFX.Instance.LostAimedShot, VocalFriendlySpetsnazSFX.Instance.LostAimedShot, ExtraNPC.Spetsnaz);
			}
			else if (IsChineseVtolLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.LostAimedShot, VocalFriendly2SFX.Instance.LostAimedShot, VocalFriendlyChineseVtolSFX.Instance.LostAimedShot, ExtraNPC.ChineseVtol);
			}
			else if (IsDuboisLevel())
			{
				PlayVocals(c, VocalFriendlySFX.Instance.LostAimedShot, VocalFriendly2SFX.Instance.LostAimedShot, VocalFriendlyDuboisSFX.Instance.LostAimedShot, ExtraNPC.Dubois);
			}
			else
			{
				PlayVocals(c, VocalFriendlySFX.Instance.LostAimedShot, VocalFriendly2SFX.Instance.LostAimedShot, null, ExtraNPC.None);
			}
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void FollowMe(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			PlayVocals(c, VocalFriendlySFX.Instance.FollowMeAnswered, VocalFriendly2SFX.Instance.FollowMeAnswered, null, ExtraNPC.None);
			break;
		case BaseCharacter.Nationality.Russian:
			break;
		case BaseCharacter.Nationality.Arabic:
			break;
		case BaseCharacter.Nationality.Chinese:
			break;
		}
	}

	public static void HitBySniper(Actor c)
	{
		CharacterSFX.Instance.HitBySniper.Play(c.gameObject);
	}

	public static void BattleChatter(Actor c)
	{
		switch (c.baseCharacter.VocalAccent)
		{
		case BaseCharacter.Nationality.Friendly:
			break;
		case BaseCharacter.Nationality.Russian:
			VocalRussianSFX.Instance.OrderReceived.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Arabic:
			VocalArabicSFX.Instance.OrderReceived.Play(c.gameObject);
			break;
		case BaseCharacter.Nationality.Chinese:
			VocalChineseSFX.Instance.OrderReceived.Play(c.gameObject);
			break;
		}
	}

	private static bool PlayVocals(Actor playActor, SoundFXData pl1SFX, SoundFXData pl2SFX, SoundFXData extraSFX, ExtraNPC npc)
	{
		RealCharacter realCharacter = playActor.realCharacter;
		bool result = true;
		if (realCharacter != null && realCharacter.Id != null)
		{
			result = false;
			if (realCharacter.Id.Contains("Player_1"))
			{
				pl1SFX.Play(playActor.gameObject);
				result = true;
			}
			else if (realCharacter.Id.Contains("Player_2") || realCharacter.Id.Contains("Player_3") || realCharacter.Id.Contains("Player_4"))
			{
				if (pl2SFX.m_audioSourceData != null && pl2SFX.m_audioSourceData.Count != 0)
				{
					pl2SFX.Play(playActor.gameObject);
				}
				else
				{
					pl1SFX.Play(playActor.gameObject);
				}
				result = true;
			}
			else if (extraSFX != null && npc != ExtraNPC.None && realCharacter.Id.Contains(CharacterNames[(int)npc]))
			{
				result = true;
				if (extraSFX.m_audioSourceData != null && extraSFX.m_audioSourceData.Count != 0)
				{
					extraSFX.Play(playActor.gameObject);
				}
			}
		}
		return result;
	}

	private static bool IsSpetsnazLevel()
	{
		bool result = false;
		if (MissionSetup.Instance != null && (MissionSetup.Instance.MissionSectionNumber == 6 || MissionSetup.Instance.MissionSectionNumber == 7) && MissionSetup.Instance.Theme == "Snow")
		{
			result = true;
		}
		return result;
	}

	private static bool IsChineseVtolLevel()
	{
		bool result = false;
		if (MissionSetup.Instance != null && MissionSetup.Instance.MissionSectionNumber == 3 && MissionSetup.Instance.Theme == "Urban")
		{
			result = true;
		}
		return result;
	}

	private static bool IsDuboisLevel()
	{
		bool result = false;
		if (MissionSetup.Instance != null && (MissionSetup.Instance.MissionSectionNumber == 2 || MissionSetup.Instance.MissionSectionNumber == 3 || MissionSetup.Instance.MissionSectionNumber == 4) && MissionSetup.Instance.Theme == "Afghanistan")
		{
			result = true;
		}
		return result;
	}
}
