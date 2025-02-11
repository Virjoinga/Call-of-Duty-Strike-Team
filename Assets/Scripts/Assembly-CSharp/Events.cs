using System;
using UnityEngine;

public class Events
{
	public class EventActor
	{
		public CharacterType CharacterType;

		public FactionHelper.Category Faction;

		public bool HealthLow;

		public string Id;

		public bool IsDead;

		public bool IsFirstPerson;

		public bool IsInCover;

		public bool IsMortallyWounded;

		public bool IsUsingFixedGun;

		public bool IsWindowLookout;

		public string Name;

		public bool PlayerControlled;

		public bool WasFirstPersonWhenMortallyWounded;

		public float WeaponAccuracyStatAdjustment;

		public WeaponDescriptor.WeaponClass WeaponClass;

		public string WeaponId;

		public bool WeaponSilenced;

		public EventActor()
		{
			CharacterType = CharacterType.Undefined;
			Faction = FactionHelper.Category.Neutral;
			HealthLow = false;
			Id = string.Empty;
			IsDead = false;
			IsFirstPerson = false;
			IsInCover = false;
			IsMortallyWounded = false;
			IsUsingFixedGun = false;
			IsWindowLookout = false;
			Name = string.Empty;
			PlayerControlled = false;
			WasFirstPersonWhenMortallyWounded = false;
			WeaponAccuracyStatAdjustment = 1f;
			WeaponClass = WeaponDescriptor.WeaponClass.None;
			WeaponId = string.Empty;
			WeaponSilenced = false;
		}
	}

	public class SimpleEvent : EventArgs
	{
		protected string m_Id;

		protected float m_TimeStamp;

		public SimpleEvent(string id)
		{
			m_Id = id;
			m_TimeStamp = Time.realtimeSinceStartup;
			TriggerSwrveEvent();
		}

		private void TriggerSwrveEvent()
		{
		}

		public string Id()
		{
			return m_Id;
		}
	}

	public class HardCurrencyChanged : SimpleEvent
	{
		public int Amount;

		public bool FromPurchase;

		public bool Freebie;

		public HardCurrencyChanged(int amount, bool fromPurchase, bool freebie)
			: base("game.mtxsocial.hardcurrencychanged")
		{
			Amount = amount;
			FromPurchase = fromPurchase;
			Freebie = freebie;
		}
	}

	public class PurchaseArmour : SimpleEvent
	{
		public EquipmentDescriptor.ArmourType PurchasedArmourType;

		public int SoliderNum;

		public PurchaseArmour(int soliderNum, EquipmentDescriptor.ArmourType purchasedArmourType)
			: base("game.mtxsocial.purchasearmour")
		{
			SoliderNum = soliderNum;
			PurchasedArmourType = purchasedArmourType;
		}
	}

	public class PurchaseEquipment : SimpleEvent
	{
		public string PurchasedEquipmentName;

		public PurchaseEquipment(string purchasedEquipmentName)
			: base("game.mtxsocial.purchaseequipment")
		{
			PurchasedEquipmentName = purchasedEquipmentName;
		}
	}

	public class PerkUnlocked : SimpleEvent
	{
		public PerkType UnlockedPerkType;

		public PerkUnlocked(PerkType unlockedPerkType)
			: base("game.mtxsocial.perkunlocked")
		{
			UnlockedPerkType = unlockedPerkType;
		}
	}

	public class Share : SimpleEvent
	{
		public Share()
			: base("game.mtxsocial.share")
		{
		}
	}

	public class StartMission : SimpleEvent
	{
		public MissionListings.eMissionID MissionId;

		public DifficultyMode MissionDifficulty;

		public int Section;

		public StartMission(MissionListings.eMissionID id, int section, DifficultyMode difficulty)
			: base("game.missionstarted")
		{
			MissionId = id;
			Section = section;
			MissionDifficulty = difficulty;
		}
	}

	public class EndMission : SimpleEvent
	{
		public bool Success;

		public bool Quit;

		public MissionListings.eMissionID MissionId;

		public DifficultyMode Mode;

		public int Section;

		public float TimePlayed;

		public EndMission(MissionListings.eMissionID id, int section, DifficultyMode mode, bool success, bool quit, float timePlayed)
			: base("game.missionend")
		{
			MissionId = id;
			Section = section;
			Success = success;
			Quit = quit;
			Mode = mode;
			TimePlayed = timePlayed;
		}
	}

	public class MissionUnlocked : SimpleEvent
	{
		public MissionUnlocked()
			: base("game.missionunlocked")
		{
		}
	}

	public class NewFlashpointActive : SimpleEvent
	{
		public int GlobalUnrestIndex;

		public NewFlashpointActive(int index)
			: base("game.newflashpoint")
		{
			GlobalUnrestIndex = index;
		}
	}

	public class WeaponFired : SimpleEvent
	{
		public EventActor Attacker;

		public EventActor Victim;

		public bool HitTarget;

		public WeaponFired(EventActor attacker, EventActor victim, bool hitTarget)
			: base("game.weaponfired")
		{
			Attacker = attacker;
			Victim = victim;
			HitTarget = hitTarget;
		}
	}

	public class GrenadeThrown : SimpleEvent
	{
		public GrenadeThrown()
			: base("game.grenadethrown")
		{
		}
	}

	public class Kill : SimpleEvent
	{
		public EventActor Attacker;

		public EventActor Victim;

		public bool HeadShot;

		public bool OneShotKill;

		public bool LongRange;

		public string DamageType;

		public bool GrenadeKill
		{
			get
			{
				return DamageType == "Grenade";
			}
		}

		public bool ClaymoreKill
		{
			get
			{
				return DamageType == "Claymore";
			}
		}

		public bool SilentKill
		{
			get
			{
				return DamageType == "Silent" || DamageType == "SilentNeckSnap";
			}
		}

		public bool Explosion
		{
			get
			{
				return DamageType == "Explosion";
			}
		}

		public bool Knife
		{
			get
			{
				return DamageType == "Knife";
			}
		}

		public Kill(EventActor attacker, EventActor victim, string damageType, bool headShot, bool oneShotKill, bool longRange)
			: base("game.kill")
		{
			TBFAssert.DoAssert(victim != null, "Victim must be specified in a kill event");
			Attacker = attacker;
			Victim = victim;
			HeadShot = headShot;
			DamageType = damageType;
			OneShotKill = oneShotKill;
			LongRange = longRange;
			if (GrenadeKill || ClaymoreKill || SilentKill || Explosion || Knife)
			{
				Attacker.WeaponSilenced = false;
				Attacker.WeaponClass = WeaponDescriptor.WeaponClass.None;
			}
		}
	}

	public class ObjectiveCompleted : SimpleEvent
	{
		public DifficultyMode Mode;

		public bool Primary;

		public ObjectiveCompleted(DifficultyMode mode, bool primary)
			: base("game.objective")
		{
			Mode = mode;
			Primary = primary;
		}
	}

	public class IntelCollected : SimpleEvent
	{
		public IntelCollected()
			: base("game.intelcollected")
		{
		}
	}

	public class AmmoCacheUsed : SimpleEvent
	{
		public int Amount;

		public WeaponDescriptor.WeaponClass WeaponClass;

		public AmmoCacheUsed(int amount, WeaponDescriptor.WeaponClass weaponClass)
			: base("game.AmmoCacheUsed")
		{
			Amount = amount;
			WeaponClass = weaponClass;
		}
	}

	public class MysteryCacheUsed : SimpleEvent
	{
		public int Amount;

		public MysteryCacheUsed(int amount)
			: base("game.MysteryCacheUsed")
		{
			Amount = amount;
		}
	}

	public class AmmoCollected : SimpleEvent
	{
		public int Amount;

		public AmmoCollected(int amount)
			: base("game.AmmoCollected")
		{
			Amount = amount;
		}
	}

	public class CharacterHealed : SimpleEvent
	{
		public EventActor HealedCharacter;

		public CharacterHealed(EventActor healedCharacter)
			: base("game.CharacterHealed")
		{
			HealedCharacter = healedCharacter;
		}
	}

	public class SpecOpsWaveComplete : SimpleEvent
	{
		public int WaveNum;

		public SpecOpsWaveComplete(int waveNum)
			: base("game.waveComplete")
		{
			WaveNum = waveNum;
		}
	}

	public class SpecOpsWaveStarted : SimpleEvent
	{
		public int WaveNum;

		public SpecOpsWaveStarted(int waveNum)
			: base("game.waveStarted")
		{
			WaveNum = waveNum;
		}
	}

	public class GMGScoreAdded : SimpleEvent
	{
		public int Score;

		public GMGScoreAdded(int score)
			: base("game.gmgScoreAdded")
		{
			Score = score;
		}
	}

	public class GameplayMinutePassed : SimpleEvent
	{
		public float SecondsPlayed;

		public GameplayMinutePassed(float secondsPlayed)
			: base("game.gameplayMinutePassed")
		{
			SecondsPlayed = secondsPlayed;
		}
	}

	public class AchievementCompleted : SimpleEvent
	{
		public string Identifier;

		public AchievementCompleted(string identifier)
			: base("stats.achievement")
		{
			Identifier = identifier;
		}
	}

	public class MedalEarned : SimpleEvent
	{
		public MedalType MedalId;

		public DifficultyMode Mode;

		public MedalEarned(MedalType medalId, DifficultyMode mode)
			: base("stats.medalearned")
		{
			MedalId = medalId;
			Mode = mode;
		}
	}

	public class XPEarned : SimpleEvent
	{
		public int XP;

		public string Type;

		public XPEarned(int xp, string type, string attacker, EventActor victim)
			: base("stats.xpearned")
		{
			XP = xp;
			Type = type;
		}
	}

	public class XPHeliDestroyed : SimpleEvent
	{
		public XPHeliDestroyed()
			: base("stats.xphelidestroyed")
		{
		}
	}

	public class ChallengeJoined : SimpleEvent
	{
		public ChallengeJoined()
			: base("gmg.challengejoined")
		{
		}
	}

	public class ChallengeCompleted : SimpleEvent
	{
		public ChallengeCompleted()
			: base("gmg.challengecompleted")
		{
		}
	}

	public class OptionsPressed : SimpleEvent
	{
		public OptionsPressed()
			: base("game.ui.optionspressed")
		{
		}
	}

	private static EventLog m_EventLog = new EventLog();

	public static EventLog Log()
	{
		return m_EventLog;
	}
}
