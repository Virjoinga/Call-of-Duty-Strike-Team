using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDropManager : SingletonMonoBehaviour, iSwrveUpdatable
{
	public bool DisableAmmoPickups;

	public int DropRate = 100;

	public int StandardRifleAmount;

	public int StandardSniperAmount;

	public int StandardLMGAmount;

	public int StandardShotgunAmount;

	public int StandardSMGAmount;

	public float VeteranModifier;

	public float SpecOpsModifier;

	public float ScavengerPerkRegularMission;

	public float ScavengerPerkVeteranMission;

	public float ScavengerPerkSpecOps;

	public float ScavengerPerkRegularMissionPro;

	public float ScavengerPerkVeteranMissionPro;

	public float ScavengerPerkSpecOpsPro;

	public int MaxClipsMission;

	public int MaxClipsSpecOps;

	private float AmmoBuy_Shotgun = 0.6f;

	private float AmmoBuy_SniperRifle = 0.6f;

	private float AmmoBuy_LightMachineGun = 0.3f;

	private float AmmoBuy_AssaultRifle = 0.5f;

	private float AmmoBuy_SMG = 0.5f;

	private float AmmoStart_Shotgun = 0.7f;

	private float AmmoStart_SniperRifle = 0.7f;

	private float AmmoStart_LightMachineGun = 0.4f;

	private float AmmoStart_AssaultRifle = 0.6f;

	private float AmmoStart_SMG = 0.6f;

	public int BaseAmmoCost = 1;

	public int IncAmmoCost = 1;

	public List<AmmoPickup> ActiveAmmoClips { get; set; }

	public static AmmoDropManager Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<AmmoDropManager>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<TitleBarController>() != null;
		}
	}

	protected override void Awake()
	{
		Object.DontDestroyOnLoad(this);
		base.Awake();
		ActiveAmmoClips = new List<AmmoPickup>();
	}

	public void ShowHideClips(bool Show)
	{
		foreach (AmmoPickup activeAmmoClip in ActiveAmmoClips)
		{
			if (activeAmmoClip != null && activeAmmoClip.AmmoActive)
			{
				activeAmmoClip.gameObject.SetActive(Show);
			}
		}
	}

	public void Reset()
	{
		for (int i = 0; i < ActiveAmmoClips.Count; i++)
		{
			if (ActiveAmmoClips[i] != null)
			{
				Object.Destroy(ActiveAmmoClips[i].gameObject);
			}
		}
		ActiveAmmoClips.Clear();
		DisableAmmoPickups = false;
	}

	public void DropAmmo(Vector3 actorNavPos, bool isStealthKill)
	{
		if (DisableAmmoPickups)
		{
			return;
		}
		int num = Random.Range(1, 100);
		if ((num <= DropRate) ? true : false)
		{
			float wait = 0f;
			if (isStealthKill)
			{
				wait = 3f;
			}
			Vector3 pos = base.transform.position + Vector3.up;
			UnityEngine.AI.NavMeshHit hit;
			if (UnityEngine.AI.NavMesh.SamplePosition(actorNavPos, out hit, 10f, 1))
			{
				pos = hit.position;
			}
			StartCoroutine(DropAmmo(wait, pos));
		}
	}

	private IEnumerator DropAmmo(float wait, Vector3 pos)
	{
		yield return new WaitForSeconds(wait);
		bool ammoReused = false;
		for (int i = 0; i < ActiveAmmoClips.Count; i++)
		{
			if (!ActiveAmmoClips[i].AmmoActive)
			{
				ActiveAmmoClips[i].Init(pos);
				AmmoPickup ap3 = ActiveAmmoClips[i];
				ActiveAmmoClips.RemoveAt(i);
				ActiveAmmoClips.Insert(ActiveAmmoClips.Count, ap3);
				ammoReused = true;
				break;
			}
		}
		if (ammoReused)
		{
			yield break;
		}
		bool maxAmmoClips = false;
		if (ActStructure.Instance.CurrentMissionIsSpecOps())
		{
			if (ActiveAmmoClips.Count >= MaxClipsSpecOps)
			{
				maxAmmoClips = true;
			}
		}
		else if (ActiveAmmoClips.Count >= MaxClipsMission)
		{
			maxAmmoClips = true;
		}
		if (maxAmmoClips)
		{
			ActiveAmmoClips[0].Reset();
			yield return new WaitForEndOfFrame();
			ActiveAmmoClips[0].Init(pos);
			AmmoPickup ap2 = ActiveAmmoClips[0];
			ActiveAmmoClips.RemoveAt(0);
			ActiveAmmoClips.Insert(ActiveAmmoClips.Count, ap2);
		}
		else
		{
			GameObject ammo = (GameObject)Object.Instantiate(Resources.Load("AmmoPickup"));
			if (ammo != null)
			{
				AmmoPickup ap = ammo.GetComponent<AmmoPickup>();
				ap.Init(pos);
				ActiveAmmoClips.Add(ap);
			}
		}
	}

	public void PickUpAmmo(Actor actor, AmmoPickup ammoPickup)
	{
		WeaponAmmo weaponAmmo = actor.weapon.PrimaryWeapon.GetWeaponAmmo();
		if (weaponAmmo != null && weaponAmmo.Pickup(CalculateAmount(actor)))
		{
			WeaponSFX.Instance.PickUpAmmo.Play(actor.gameObject);
			ammoPickup.Reset();
		}
	}

	private int CalculateAmount(Actor actor)
	{
		int num = 0;
		switch (actor.weapon.PrimaryWeapon.GetClass())
		{
		case WeaponDescriptor.WeaponClass.AssaultRifle:
			num = StandardRifleAmount;
			break;
		case WeaponDescriptor.WeaponClass.LightMachineGun:
			num = StandardLMGAmount;
			break;
		case WeaponDescriptor.WeaponClass.Shotgun:
			num = StandardShotgunAmount;
			break;
		case WeaponDescriptor.WeaponClass.SniperRifle:
			num = StandardSniperAmount;
			break;
		case WeaponDescriptor.WeaponClass.SubMachineGun:
			num = StandardSMGAmount;
			break;
		}
		float num2 = 1f;
		bool flag = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.Scavenger) == 1f;
		if (ActStructure.Instance.CurrentMissionIsSpecOps())
		{
			num = (int)((float)num * SpecOpsModifier);
			num2 = ((!flag) ? ScavengerPerkSpecOps : ScavengerPerkSpecOpsPro);
		}
		else if (ActStructure.Instance.CurrentMissionMode == DifficultyMode.Veteran)
		{
			num = (int)((float)num * VeteranModifier);
			num2 = ((!flag) ? ScavengerPerkVeteranMission : ScavengerPerkVeteranMissionPro);
		}
		else
		{
			num2 = ((!flag) ? ScavengerPerkRegularMission : ScavengerPerkRegularMissionPro);
		}
		if (GameSettings.Instance.HasPerk(PerkType.Scavenger))
		{
			num2 = GMGBalanceTweaks.Instance.GMGModifier_ScavengerPerk(num2);
			int num3 = num;
			num = (int)((float)num * num2);
			int num4 = num - num3;
			if (num4 < 2 && flag)
			{
				num = num3 + 2;
			}
			else if (num4 < 1 && !flag)
			{
				num = num3 + 1;
			}
		}
		return num;
	}

	public float AmmoCacheReward(WeaponDescriptor.WeaponClass wc)
	{
		switch (wc)
		{
		case WeaponDescriptor.WeaponClass.Shotgun:
			return AmmoBuy_Shotgun;
		case WeaponDescriptor.WeaponClass.SniperRifle:
			return AmmoBuy_SniperRifle;
		case WeaponDescriptor.WeaponClass.LightMachineGun:
			return AmmoBuy_LightMachineGun;
		case WeaponDescriptor.WeaponClass.AssaultRifle:
			return AmmoBuy_AssaultRifle;
		case WeaponDescriptor.WeaponClass.SubMachineGun:
			return AmmoBuy_SMG;
		default:
			return 0.5f;
		}
	}

	public float StartingAmmoPerc(WeaponDescriptor.WeaponClass wc)
	{
		switch (wc)
		{
		case WeaponDescriptor.WeaponClass.Shotgun:
			return AmmoStart_Shotgun;
		case WeaponDescriptor.WeaponClass.SniperRifle:
			return AmmoStart_SniperRifle;
		case WeaponDescriptor.WeaponClass.LightMachineGun:
			return AmmoStart_LightMachineGun;
		case WeaponDescriptor.WeaponClass.AssaultRifle:
			return AmmoStart_AssaultRifle;
		case WeaponDescriptor.WeaponClass.SubMachineGun:
			return AmmoStart_SMG;
		default:
			return 0.6f;
		}
	}

	public void UpdateFromSwrve()
	{
		Dictionary<string, string> resourceDictionary;
		if (Bedrock.GetRemoteUserResources("Ammo", out resourceDictionary))
		{
			AmmoBuy_Shotgun = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "AmmoBuy_" + WeaponDescriptor.WeaponClass.Shotgun, AmmoBuy_Shotgun);
			AmmoBuy_SniperRifle = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "AmmoBuy_" + WeaponDescriptor.WeaponClass.SniperRifle, AmmoBuy_SniperRifle);
			AmmoBuy_LightMachineGun = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "AmmoBuy_" + WeaponDescriptor.WeaponClass.LightMachineGun, AmmoBuy_LightMachineGun);
			AmmoBuy_AssaultRifle = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "AmmoBuy_" + WeaponDescriptor.WeaponClass.AssaultRifle, AmmoBuy_AssaultRifle);
			AmmoBuy_SMG = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "AmmoBuy_" + WeaponDescriptor.WeaponClass.SubMachineGun, AmmoBuy_SMG);
			AmmoStart_Shotgun = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "AmmoStart_" + WeaponDescriptor.WeaponClass.Shotgun, AmmoStart_Shotgun);
			AmmoStart_SniperRifle = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "AmmoStart_" + WeaponDescriptor.WeaponClass.SniperRifle, AmmoStart_SniperRifle);
			AmmoStart_LightMachineGun = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "AmmoStart_" + WeaponDescriptor.WeaponClass.LightMachineGun, AmmoStart_LightMachineGun);
			AmmoStart_AssaultRifle = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "AmmoStart_" + WeaponDescriptor.WeaponClass.AssaultRifle, AmmoStart_AssaultRifle);
			AmmoStart_SMG = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "AmmoStart_" + WeaponDescriptor.WeaponClass.SubMachineGun, AmmoStart_SMG);
			StandardShotgunAmount = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "AmmoBody_" + WeaponDescriptor.WeaponClass.Shotgun, StandardShotgunAmount);
			StandardSniperAmount = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "AmmoBody_" + WeaponDescriptor.WeaponClass.SniperRifle, StandardSniperAmount);
			StandardLMGAmount = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "AmmoBody_" + WeaponDescriptor.WeaponClass.LightMachineGun, StandardLMGAmount);
			StandardRifleAmount = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "AmmoBody_" + WeaponDescriptor.WeaponClass.AssaultRifle, StandardRifleAmount);
			StandardSMGAmount = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "AmmoBody_" + WeaponDescriptor.WeaponClass.SubMachineGun, StandardSMGAmount);
			BaseAmmoCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "Cost_base", BaseAmmoCost);
			IncAmmoCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "Cost_increase", IncAmmoCost);
		}
	}
}
