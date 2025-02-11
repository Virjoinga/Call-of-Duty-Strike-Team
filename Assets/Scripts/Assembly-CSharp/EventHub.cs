using System;
using UnityEngine;

public class EventHub : MonoBehaviour
{
	public delegate void OnStartMissionEventHandler(object sender, Events.StartMission args);

	public delegate void OnEndMissionEventHandler(object sender, Events.EndMission args);

	public delegate void OnMissionUnlockedEventHandler(object sender, Events.MissionUnlocked args);

	public delegate void OnNewFlashpointActiveEventHandler(object sender, Events.NewFlashpointActive args);

	public delegate void OnWeaponFiredEventHandler(object sender, Events.WeaponFired args);

	public delegate void OnKillEventHandler(object sender, Events.Kill args);

	public delegate void OnGrenadeThrownEventHandler(object sender, Events.GrenadeThrown args);

	public delegate void OnAchievementCompleteHandler(object sender, Events.AchievementCompleted args);

	public delegate void OnObjectiveCompleteHandler(object sender, Events.ObjectiveCompleted args);

	public delegate void OnIntelCollectedHandler(object sender, Events.IntelCollected args);

	public delegate void OnMedalEarnedHandler(object sender, Events.MedalEarned args);

	public delegate void OnXPEarnedHandler(object sender, Events.XPEarned args);

	public delegate void OnHardCurrencyChangedHandler(object sender, Events.HardCurrencyChanged args);

	public delegate void OnPurchaseArmourHandler(object sender, Events.PurchaseArmour args);

	public delegate void OnPurchaseEquipmentHandler(object sender, Events.PurchaseEquipment args);

	public delegate void OnPerkUnlockedHandler(object sender, Events.PerkUnlocked args);

	public delegate void OnSpecOpsWaveCompleteHandler(object sender, Events.SpecOpsWaveComplete args);

	public delegate void OnSpecOpsWaveStartedHandler(object sender, Events.SpecOpsWaveStarted args);

	public delegate void OnAmmoCacheUsedHandler(object sender, Events.AmmoCacheUsed args);

	public delegate void OnMysteryCacheUsedHandler(object sender, Events.MysteryCacheUsed args);

	public delegate void OnAmmoCollectedHandler(object sender, Events.AmmoCollected args);

	public delegate void OnCharacterHealedHandler(object sender, Events.CharacterHealed args);

	public delegate void OnShareHandler(object sender, Events.Share args);

	public delegate void OnHeliDestroyedHandler();

	public delegate void OnChallengeJoinedHandler(object sender, Events.ChallengeJoined args);

	public delegate void OnChallengeCompletedHandler(object sender, Events.ChallengeCompleted args);

	public delegate void OnGMGScoreAdded(object sender, Events.GMGScoreAdded args);

	public delegate void OnGameplayMinutePassed(object sender, Events.GameplayMinutePassed args);

	public static EventHub smInstance;

	public static EventHub Instance
	{
		get
		{
			return smInstance;
		}
	}

	public event OnStartMissionEventHandler OnStartMission;

	public event OnEndMissionEventHandler OnEndMission;

	public event OnMissionUnlockedEventHandler OnMissionUnlocked;

	public event OnNewFlashpointActiveEventHandler OnNewFlashpointActive;

	public event OnWeaponFiredEventHandler OnWeaponFired;

	public event OnKillEventHandler OnKill;

	public event OnGrenadeThrownEventHandler OnGrenadeThrown;

	public event OnAchievementCompleteHandler OnAchievementCompleted;

	public event OnObjectiveCompleteHandler OnObjectiveCompleted;

	public event OnIntelCollectedHandler OnIntelCollected;

	public event OnMedalEarnedHandler OnMedalEarned;

	public event OnXPEarnedHandler OnXPEarned;

	public event OnHardCurrencyChangedHandler OnHardCurrencyChanged;

	public event OnPurchaseArmourHandler OnPurchaseArmour;

	public event OnPurchaseEquipmentHandler OnPurchaseEquipment;

	public event OnPerkUnlockedHandler OnPerkUnlocked;

	public event OnSpecOpsWaveCompleteHandler OnSpecOpsWaveComplete;

	public event OnSpecOpsWaveStartedHandler OnSpecOpsWaveStarted;

	public event OnAmmoCacheUsedHandler OnAmmoCacheUsed;

	public event OnMysteryCacheUsedHandler OnMysteryCacheUsed;

	public event OnAmmoCollectedHandler OnAmmoCollected;

	public event OnCharacterHealedHandler OnCharacterHealed;

	public event OnShareHandler OnShare;

	public event OnHeliDestroyedHandler XPHeliDestroyed;

	public event OnChallengeJoinedHandler ChallengeJoined;

	public event OnChallengeCompletedHandler ChallengeCompleted;

	public event OnGMGScoreAdded GMGScoreAdded;

	public event OnGameplayMinutePassed GameplayMinutePassed;

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple Event Hubs");
		}
		smInstance = this;
	}

	public void Report(Events.StartMission gameEvent)
	{
		if (this.OnStartMission != null)
		{
			this.OnStartMission(this, gameEvent);
		}
	}

	public void Report(Events.EndMission gameEvent)
	{
		if (this.OnEndMission != null)
		{
			this.OnEndMission(this, gameEvent);
		}
	}

	public void Report(Events.MissionUnlocked gameEvent)
	{
		if (this.OnMissionUnlocked != null)
		{
			this.OnMissionUnlocked(this, gameEvent);
		}
	}

	public void Report(Events.NewFlashpointActive gameEvent)
	{
		if (this.OnNewFlashpointActive != null)
		{
			this.OnNewFlashpointActive(this, gameEvent);
		}
	}

	public void Report(Events.WeaponFired gameEvent)
	{
		if (this.OnWeaponFired != null)
		{
			this.OnWeaponFired(this, gameEvent);
		}
	}

	public void Report(Events.Kill gameEvent)
	{
		if (this.OnKill != null)
		{
			this.OnKill(this, gameEvent);
		}
	}

	public void Report(Events.GrenadeThrown gameEvent)
	{
		if (this.OnGrenadeThrown != null)
		{
			this.OnGrenadeThrown(this, gameEvent);
		}
	}

	public void Report(Events.AchievementCompleted gameEvent)
	{
		if (this.OnAchievementCompleted != null)
		{
			this.OnAchievementCompleted(this, gameEvent);
		}
	}

	public void Report(Events.ObjectiveCompleted gameEvent)
	{
		if (this.OnObjectiveCompleted != null)
		{
			this.OnObjectiveCompleted(this, gameEvent);
		}
	}

	public void Report(Events.IntelCollected gameEvent)
	{
		if (this.OnIntelCollected != null)
		{
			this.OnIntelCollected(this, gameEvent);
		}
	}

	public void Report(Events.MedalEarned gameEvent)
	{
		if (this.OnMedalEarned != null)
		{
			this.OnMedalEarned(this, gameEvent);
		}
	}

	public void Report(Events.XPEarned gameEvent)
	{
		if (this.OnXPEarned != null)
		{
			this.OnXPEarned(this, gameEvent);
		}
	}

	public void Report(Events.HardCurrencyChanged gameEvent)
	{
		if (this.OnHardCurrencyChanged != null)
		{
			this.OnHardCurrencyChanged(this, gameEvent);
		}
	}

	public void Report(Events.PurchaseArmour gameEvent)
	{
		if (this.OnPurchaseArmour != null)
		{
			this.OnPurchaseArmour(this, gameEvent);
		}
	}

	public void Report(Events.PurchaseEquipment gameEvent)
	{
		if (this.OnPurchaseEquipment != null)
		{
			this.OnPurchaseEquipment(this, gameEvent);
		}
	}

	public void Report(Events.PerkUnlocked gameEvent)
	{
		if (this.OnPerkUnlocked != null)
		{
			this.OnPerkUnlocked(this, gameEvent);
		}
	}

	public void Report(Events.SpecOpsWaveComplete gameEvent)
	{
		if (this.OnSpecOpsWaveComplete != null)
		{
			this.OnSpecOpsWaveComplete(this, gameEvent);
		}
	}

	public void Report(Events.SpecOpsWaveStarted gameEvent)
	{
		if (this.OnSpecOpsWaveStarted != null)
		{
			this.OnSpecOpsWaveStarted(this, gameEvent);
		}
	}

	public void Report(Events.AmmoCacheUsed gameEvent)
	{
		if (this.OnAmmoCacheUsed != null)
		{
			this.OnAmmoCacheUsed(this, gameEvent);
		}
	}

	public void Report(Events.MysteryCacheUsed gameEvent)
	{
		if (this.OnMysteryCacheUsed != null)
		{
			this.OnMysteryCacheUsed(this, gameEvent);
		}
	}

	public void Report(Events.AmmoCollected gameEvent)
	{
		if (this.OnAmmoCollected != null)
		{
			this.OnAmmoCollected(this, gameEvent);
		}
	}

	public void Report(Events.CharacterHealed gameEvent)
	{
		if (this.OnCharacterHealed != null)
		{
			this.OnCharacterHealed(this, gameEvent);
		}
	}

	public void Report(Events.Share gameEvent)
	{
		if (this.OnShare != null)
		{
			this.OnShare(this, gameEvent);
		}
	}

	public void Report(Events.XPHeliDestroyed gameEvent)
	{
		if (this.XPHeliDestroyed != null)
		{
			this.XPHeliDestroyed();
		}
	}

	public void Report(Events.ChallengeJoined args)
	{
		if (this.ChallengeJoined != null)
		{
			this.ChallengeJoined(this, args);
		}
	}

	public void Report(Events.ChallengeCompleted args)
	{
		if (this.ChallengeCompleted != null)
		{
			this.ChallengeCompleted(this, args);
		}
	}

	public void Report(Events.GMGScoreAdded args)
	{
		if (this.GMGScoreAdded != null)
		{
			this.GMGScoreAdded(this, args);
		}
	}

	public void Report(Events.GameplayMinutePassed args)
	{
		if (this.GameplayMinutePassed != null)
		{
			this.GameplayMinutePassed(this, args);
		}
	}
}
