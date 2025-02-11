using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CMMysteryCache))]
public class MysteryCache : MonoBehaviour
{
	public enum Reward
	{
		Invincible = 0,
		XP = 1,
		Speed = 2,
		Grenade1 = 3,
		Claymore1 = 4,
		Medkit1 = 5,
		Bomb = 6,
		Grenade3 = 7,
		Claymore3 = 8,
		Medkit3 = 9,
		Ammo = 10,
		Time = 11,
		Max = 12
	}

	private static string[] mRewardDesc = new string[12]
	{
		"S_MYSTERY_REWARD_1", "S_MYSTERY_REWARD_2", "S_MYSTERY_REWARD_3", "S_MYSTERY_REWARD_4", "S_MYSTERY_REWARD_5", "S_MYSTERY_REWARD_6", "S_MYSTERY_REWARD_7", "S_MYSTERY_REWARD_8", "S_MYSTERY_REWARD_9", "S_MYSTERY_REWARD_10",
		"S_MYSTERY_REWARD_11", "S_MYSTERY_REWARD_12"
	};

	private AnimationClip mOpenBoxClip;

	public GameObject ObjectiveBlipPrefab;

	public MysteryCacheData m_Interface;

	public EquipmentDescriptor MysteryDescriptor;

	[HideInInspector]
	public bool mActive;

	public AnimationClip UseCrateAnim;

	private Animation mCratePlayer;

	private ObjectiveBlip mBlip;

	private ContextMenuDistanceManager mCMDM;

	private CMMysteryCache mCMAC;

	private GameObject mIcon;

	private List<Reward> mRewardsAwarded;

	private bool mDoingInGamePurchaseFlow;

	private PurchaseFlowHelper.PurchaseData m_MysteryData;

	private Actor m_PurchaseActor;

	private CMMysteryCache mContextMenu;

	private void Start()
	{
		mCMDM = IncludeDisabled.GetComponentInChildren<ContextMenuDistanceManager>(base.gameObject);
		mCMAC = IncludeDisabled.GetComponentInChildren<CMMysteryCache>(base.gameObject);
		mCratePlayer = base.transform.parent.gameObject.GetComponentInChildren<Animation>();
		if ((bool)mCratePlayer)
		{
			foreach (AnimationState item in mCratePlayer)
			{
				if (item != null)
				{
					mOpenBoxClip = item.clip;
					break;
				}
			}
			if (mOpenBoxClip != null)
			{
				mCratePlayer.enabled = false;
				mCratePlayer.enabled = true;
				mCratePlayer.Play(mOpenBoxClip.name);
				mCratePlayer.animation[mOpenBoxClip.name].speed = -1f;
			}
			else
			{
				Debug.LogWarning("Failed to find the required animation for " + base.name);
			}
		}
		CMMysteryCache component = GetComponent<CMMysteryCache>();
		if (component != null && component.CrateModel != null)
		{
			Component[] componentsInChildren = component.CrateModel.GetComponentsInChildren(typeof(Collider), true);
			Component[] array = componentsInChildren;
			foreach (Component component2 in array)
			{
				if (!component2.rigidbody && !component2.gameObject.isStatic)
				{
					Rigidbody rigidbody = component2.gameObject.AddComponent<Rigidbody>();
					rigidbody.isKinematic = true;
				}
			}
		}
		mIcon = Object.Instantiate(EffectsController.Instance.Effects.MysteryBoxIcon) as GameObject;
		mIcon.transform.parent = base.gameObject.transform;
		mIcon.SetActive(false);
		Deactivate();
	}

	private void Awake()
	{
		mRewardsAwarded = new List<Reward>();
	}

	public void Activate()
	{
		GameObject gameObject = Object.Instantiate(ObjectiveBlipPrefab) as GameObject;
		mBlip = gameObject.GetComponent<ObjectiveBlip>();
		if ((bool)mBlip)
		{
			mBlip.mObjectiveText = Language.Get("S_MYSTERY");
			GameObject gameObject2 = new GameObject();
			gameObject2.transform.position = base.transform.position + new Vector3(0f, 0.5f, 0f);
			mBlip.Target = gameObject2.transform;
			ObjectiveBlip.BlipType blipType = ObjectiveBlip.BlipType.Mystery;
			mBlip.SetBlipType(blipType);
			mBlip.SwitchOn();
		}
		if (mCMDM != null)
		{
			mCMDM.enabled = true;
		}
		HUDMessenger.Instance.PushMessage(Language.Get("S_SURVIVAL_MSG_MYSTERY_01"), string.Empty, string.Empty, false);
		string text = "S_SURVIVAL_DIALOGUE_MBOPEN_0" + Random.Range(1, 3);
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(text);
		PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, MysteryBoxVOSFX.Instance, text, false, 1f);
		mActive = true;
	}

	public void Deactivate()
	{
		if ((bool)mBlip)
		{
			mBlip.SwitchOff();
		}
		if (mCMAC != null)
		{
			mCMAC.ShowBlip(false);
		}
		if (mCMDM != null)
		{
			mCMDM.enabled = false;
		}
		if (mOpenBoxClip != null)
		{
			mCratePlayer.Play(mOpenBoxClip.name);
			mCratePlayer.animation[mOpenBoxClip.name].speed = -1f;
		}
		mActive = false;
	}

	public void TryPurchase(Actor actor, CMMysteryCache cm)
	{
		if ((bool)mBlip)
		{
			mBlip.SwitchOff();
		}
		if (mOpenBoxClip != null)
		{
			mCratePlayer.Play(mOpenBoxClip.name);
			AnimationState animationState = mCratePlayer.animation[mOpenBoxClip.name];
			if (animationState != null)
			{
				animationState.time = animationState.length;
				animationState.speed = 1f;
			}
		}
		mContextMenu = cm;
		MysteryDescriptor.HardCost = GMGData.Instance.MysteryCost;
		m_MysteryData = new PurchaseFlowHelper.PurchaseData();
		m_MysteryData.ScriptToCallWithResult = this;
		m_MysteryData.MethodToCallWithResult = "DoPurchase";
		m_MysteryData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment;
		m_MysteryData.NumItems = 1;
		m_MysteryData.EquipmentItem = MysteryDescriptor;
		m_PurchaseActor = actor;
		PurchaseFlowHelper.Instance.Purchase(m_MysteryData);
		mDoingInGamePurchaseFlow = true;
	}

	private void DoPurchase()
	{
		OrdersHelper.OrderMysteryCache(GameplayController.instance, mContextMenu);
		RewardMysteryBonus(m_PurchaseActor);
	}

	public void RewardMysteryBonus(Actor a)
	{
		Reward reward = PickReward();
		StartCoroutine(AnimateIcon(reward));
		HUDMessenger.Instance.PushPriorityMessage("[#FFFF00]" + Language.Get(mRewardDesc[(int)reward]), string.Empty, string.Empty, false);
		string text = "S_SURVIVAL_DIALOGUE_MBPURCHASE_" + ((int)(reward + 1)).ToString("00");
		CommonHudController.Instance.MissionDialogueQueue.ClearDialogueQueue(true);
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(text);
		PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, MysteryBoxVOSFX.Instance, text, false, 1f);
		switch (reward)
		{
		case Reward.Invincible:
			StopCoroutine("InvincibleTimer");
			StartCoroutine(InvincibleTimer(a));
			break;
		case Reward.XP:
			StopCoroutine("XPTimer");
			GMGData.Instance.XPBonusRewardActive = true;
			StartCoroutine(XPTimer(a));
			break;
		case Reward.Speed:
			GameController.Instance.MovementSpeedMultiplier = GMGData.Instance.SpeedBoostMultiplier;
			StopCoroutine("SpeedTimer");
			StartCoroutine(SpeedTimer(a));
			break;
		case Reward.Grenade1:
			PlayerSquadManager.Instance.AwardGrenade(1);
			break;
		case Reward.Claymore1:
			PlayerSquadManager.Instance.AwardClaymore(1);
			break;
		case Reward.Medkit1:
			PlayerSquadManager.Instance.AwardMedkit(1);
			break;
		case Reward.Bomb:
		{
			List<Actor> list2 = ActorIdentIterator.AsList(GKM.EnemiesMask(0) & GKM.AliveMask);
			foreach (Actor item in list2)
			{
				item.health.ModifyHealth(m_PurchaseActor.gameObject, -10000000f, "SmartBomb", Vector3.one, false);
			}
			string msg = Language.Get("S_MYSTERY_REWARD_7").ToUpper();
			int xP = XPManager.Instance.m_XPAwards.Bonus_SmartBomb.GetXP();
			int num = 0;
			num = list2.Count * xP;
			CommonHudController.Instance.AddXpFeedback(num, msg, null);
			GMGSFX.Instance.SonicBombVar1.Play2D();
			if (GameController.Instance.FirstPersonCamera != null)
			{
				GameController.Instance.FirstPersonCamera.AddShake(1f, 4f);
			}
			break;
		}
		case Reward.Grenade3:
			PlayerSquadManager.Instance.AwardGrenade(3);
			break;
		case Reward.Claymore3:
			PlayerSquadManager.Instance.AwardClaymore(3);
			break;
		case Reward.Medkit3:
			PlayerSquadManager.Instance.AwardMedkit(3);
			break;
		case Reward.Ammo:
		{
			List<Actor> list = new List<Actor>();
			list.Add(a);
			CharacterPropertyModifier.AddAmmoPercent(list, "100");
			m_PurchaseActor.weapon.SwitchToPrimary();
			m_PurchaseActor.weapon.Reload();
			break;
		}
		case Reward.Time:
			StopCoroutine("TimeFreezeTimer");
			StartCoroutine(TimeFreezeTimer(a));
			break;
		}
	}

	private Reward PickReward()
	{
		int[] array = new int[GMGData.Instance.RewardChance.Length];
		GMGData.Instance.RewardChance.CopyTo(array, 0);
		if (GMGData.Instance.CurrentGameType != GMGData.GameType.TimeAttack && GMGData.Instance.CurrentGameType != GMGData.GameType.Domination)
		{
			array[11] = 0;
		}
		List<Actor> list = ActorIdentIterator.AsList(GKM.EnemiesMask(0) & GKM.AliveMask);
		if (list.Count <= 2)
		{
			array[6] = 0;
		}
		foreach (Reward item in mRewardsAwarded)
		{
			array[(int)item] = 0;
		}
		int num = 0;
		for (int i = 0; i < 12; i++)
		{
			num += array[i];
		}
		if (num == 0 && mRewardsAwarded.Count > 0)
		{
			mRewardsAwarded.Clear();
			return PickReward();
		}
		int num2 = Random.Range(1, num + 1);
		int num3 = 0;
		for (int j = 0; j < 12; j++)
		{
			num3 += array[j];
			if (num2 <= num3)
			{
				Reward reward = (Reward)j;
				mRewardsAwarded.Add(reward);
				return reward;
			}
		}
		Debug.LogWarning(string.Format("Something went wrong in MysteryCache.PickReward. maxRandomPoolSize={0} randomNumber={1}", num, num2));
		return Reward.Ammo;
	}

	public void PickUpMystery()
	{
		GMGData.Instance.RegisterMysteryPurchase();
		Deactivate();
		int amount = 1;
		EventHub.Instance.Report(new Events.MysteryCacheUsed(amount));
	}

	public void Update()
	{
		if (!mDoingInGamePurchaseFlow)
		{
			return;
		}
		if (TimeManager.instance.GlobalTimeState != TimeManager.State.IngamePaused)
		{
			TimeManager.instance.PauseGame();
			GameController.Instance.LockGyro = true;
		}
		else if (!MessageBoxController.Instance.IsAnyMessageActive && FrontEndController.Instance.ActiveScreen == ScreenID.None)
		{
			if (PlayerSquadManager.Instance != null)
			{
				PlayerSquadManager.Instance.SetupInventory();
			}
			TimeManager.instance.UnpauseGame();
			mDoingInGamePurchaseFlow = false;
			GameController.Instance.LockGyro = false;
		}
	}

	private IEnumerator InvincibleTimer(Actor a)
	{
		float wait = GMGData.Instance.InvincibleTimer;
		CommonHudController.Instance.SetUnitAbility(Language.Get("S_MYSTERY_REWARD_1"), wait, Mathf.Min(wait, 5f));
		GMGSFX.Instance.InvincibilityActivate.Play2D();
		bool invulnerable;
		while (wait > 0f)
		{
			wait -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
			HealthComponent health = a.health;
			invulnerable = (a.MysteryBoxInvulnerability = true);
			health.Invulnerable = invulnerable;
		}
		HealthComponent health2 = a.health;
		invulnerable = (a.MysteryBoxInvulnerability = false);
		health2.Invulnerable = invulnerable;
		CommonHudController.Instance.ClearUnitAbility();
		GMGSFX.Instance.InvincibilityDeactivate.Play2D();
	}

	private IEnumerator SpeedTimer(Actor a)
	{
		float wait = GMGData.Instance.SpeedBoostTimer;
		CommonHudController.Instance.SetUnitAbility(Language.Get("S_MYSTERY_REWARD_3"), wait, Mathf.Min(wait, 5f));
		GMGSFX.Instance.SpeedBoostActivate.Play2D();
		while (wait > 0f)
		{
			wait -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		GameController.Instance.MovementSpeedMultiplier = 1f;
		CommonHudController.Instance.ClearUnitAbility();
		GMGSFX.Instance.SpeedBoostDeactivate.Play2D();
	}

	private IEnumerator XPTimer(Actor a)
	{
		float wait = GMGData.Instance.XPBonusTimer;
		CommonHudController.Instance.SetUnitAbility(Language.Get("S_MYSTERY_REWARD_2"), wait, Mathf.Min(wait, 5f));
		GMGSFX.Instance.XPBonusActivate.Play2D();
		while (wait > 0f)
		{
			wait -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		GMGData.Instance.XPBonusRewardActive = false;
		CommonHudController.Instance.ClearUnitAbility();
		GMGSFX.Instance.XPBonusDeactivate.Play2D();
	}

	private IEnumerator TimeFreezeTimer(Actor a)
	{
		float wait = GMGData.Instance.TimeFreezeTimer;
		CommonHudController.Instance.MissionTimer.PauseTimer();
		CommonHudController.Instance.SetUnitAbility(Language.Get("S_MYSTERY_REWARD_12"), wait, Mathf.Min(wait, 5f));
		GMGSFX.Instance.TimeFreezeActivate.Play2D();
		while (wait > 0f)
		{
			wait -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		CommonHudController.Instance.MissionTimer.StartTimer();
		CommonHudController.Instance.ClearUnitAbility();
		GMGSFX.Instance.TimeFreezeDeactivate.Play2D();
	}

	private IEnumerator AnimateIcon(Reward reward)
	{
		mIcon.SetActive(true);
		PackedSprite iconPackedSprite = mIcon.GetComponent<PackedSprite>();
		int rewardIndex = 0;
		float finishTime = Time.time + 3f;
		float openingTime = Time.time + 0.75f;
		float settledTime = Time.time + 1.98f;
		float spinTimeRemaining = 0.08f;
		while (Time.time < finishTime)
		{
			if (Time.time < openingTime)
			{
				iconPackedSprite.SetFrame(0, 6);
			}
			else if (Time.time > settledTime)
			{
				iconPackedSprite.SetFrame(0, GetRewardIconFrame(reward));
			}
			else
			{
				spinTimeRemaining -= Time.deltaTime;
				if (spinTimeRemaining <= 0f)
				{
					spinTimeRemaining = 0.08f;
					rewardIndex++;
					if (rewardIndex >= 12)
					{
						rewardIndex = 0;
					}
					iconPackedSprite.SetFrame(0, GetRewardIconFrame((Reward)rewardIndex));
				}
			}
			yield return null;
		}
		mIcon.SetActive(false);
	}

	private int GetRewardIconFrame(Reward reward)
	{
		switch (reward)
		{
		case Reward.Invincible:
			return 5;
		case Reward.XP:
			return 10;
		case Reward.Speed:
			return 8;
		case Reward.Grenade1:
			return 0;
		case Reward.Claymore1:
			return 1;
		case Reward.Medkit1:
			return 3;
		case Reward.Bomb:
			return 7;
		case Reward.Grenade3:
			return 0;
		case Reward.Claymore3:
			return 1;
		case Reward.Medkit3:
			return 3;
		case Reward.Ammo:
			return 2;
		case Reward.Time:
			return 9;
		default:
			Debug.LogWarning(string.Format("Unknown Reward {0}. Returning default Sprite index", reward));
			return 6;
		}
	}
}
