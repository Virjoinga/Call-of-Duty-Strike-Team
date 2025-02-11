using System.Collections;
using UnityEngine;

public class GMGSniper_TS : MonoBehaviour
{
	public GameObject Actor;

	public WeaponDescriptor WeaponA;

	public WeaponDescriptor WeaponB;

	private WeaponDescriptor mWeaponInstanceA;

	private WeaponDescriptor mWeaponInstanceB;

	private IWeapon mWeaponA;

	private IWeapon mWeaponB;

	public float TakeDamageMultiplier;

	public float AltTakeDamageMultiplier;

	private float currentWeapon;

	private void Start()
	{
		ActorWrapper componentInChildren = Actor.GetComponentInChildren<ActorWrapper>();
		if (!(componentInChildren == null))
		{
			Actor actor = componentInChildren.GetActor();
			if (!(actor == null))
			{
				mWeaponInstanceA = Object.Instantiate(WeaponA) as WeaponDescriptor;
				mWeaponInstanceA.UnlimitedAmmo = true;
				mWeaponInstanceB = Object.Instantiate(WeaponB) as WeaponDescriptor;
				mWeaponInstanceB.UnlimitedAmmo = true;
				mWeaponA = mWeaponInstanceA.Create(actor.model, 1f, 1f, 1f);
				mWeaponB = mWeaponInstanceB.Create(actor.model, 1f, 1f, 1f);
				GlobalBalanceTweaks.DifficultySettingsModifier = 1;
				StartCoroutine(Init());
			}
		}
	}

	private IEnumerator Init()
	{
		ActorWrapper actorWrapper = null;
		while (actorWrapper == null)
		{
			yield return null;
			actorWrapper = Actor.GetComponentInChildren<ActorWrapper>();
		}
		Actor actor = null;
		while (actor == null)
		{
			yield return null;
			actor = actorWrapper.GetActor();
			actor.awareness.airborne = true;
		}
		actor.weapon.SwapTo(mWeaponA, 1f);
		if (actor.health != null)
		{
			actor.health.TakeDamageModifier = TakeDamageMultiplier;
		}
		CommonHudController.Instance.PreventWeaponSwap = true;
	}

	public void SwitchWeapon()
	{
		ActorWrapper componentInChildren = Actor.GetComponentInChildren<ActorWrapper>();
		if (componentInChildren == null)
		{
			return;
		}
		Actor actor = componentInChildren.GetActor();
		if (actor == null)
		{
			return;
		}
		actor.realCharacter.IsAimingDownSights = false;
		if (currentWeapon == 0f)
		{
			actor.weapon.SwapTo(mWeaponB, 1f);
			currentWeapon = 1f;
			if (actor.health != null)
			{
				actor.health.TakeDamageModifier = AltTakeDamageMultiplier;
			}
		}
		else if (currentWeapon == 1f)
		{
			actor.weapon.SwapTo(mWeaponA, 1f);
			currentWeapon = 0f;
			if (actor.health != null)
			{
				actor.health.TakeDamageModifier = TakeDamageMultiplier;
			}
		}
	}

	public void Deactivate()
	{
		ActorWrapper componentInChildren = Actor.GetComponentInChildren<ActorWrapper>();
		if (!(componentInChildren == null))
		{
			Actor actor = componentInChildren.GetActor();
			if (!(actor == null))
			{
				GameController.Instance.IsLockedToFirstPerson = false;
				GameController.Instance.IsLockedToCurrentCharacter = false;
				actor.realCharacter.StopMovingManually();
				actor.realCharacter.SetReferenceFrame(null);
				FirstPersonCamera firstPersonCamera = actor.baseCharacter.FirstPersonCamera;
				firstPersonCamera.ClearConstraints();
				CommonHudController.Instance.PreventWeaponSwap = false;
			}
		}
	}
}
