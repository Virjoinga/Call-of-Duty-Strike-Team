using System.Collections;
using UnityEngine;

public class ForceWeaponCommand : Command
{
	public enum ForceWeapon
	{
		Primary = 0,
		Secondary = 1,
		None = 2,
		Unforce = 3
	}

	public ForceWeapon WeaponToForce;

	public bool UseFPPActor = true;

	public GameObject AltActorToForce;

	private Actor mActorToForce;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (UseFPPActor)
		{
			mActorToForce = GameController.Instance.mFirstPersonActor;
		}
		else
		{
			ActorWrapper aw = AltActorToForce.GetComponentInChildren<ActorWrapper>();
			if (aw != null)
			{
				mActorToForce = aw.GetActor();
			}
		}
		if (mActorToForce == null)
		{
			yield break;
		}
		mActorToForce.weapon.WeaponSelectionLocked = false;
		switch (WeaponToForce)
		{
		case ForceWeapon.Primary:
			mActorToForce.weapon.RaiseWeapon();
			if (mActorToForce.weapon.ActiveWeapon != mActorToForce.weapon.PrimaryWeapon)
			{
				mActorToForce.weapon.Toggle();
			}
			mActorToForce.weapon.WeaponSelectionLocked = true;
			break;
		case ForceWeapon.Secondary:
			mActorToForce.weapon.RaiseWeapon();
			if (mActorToForce.weapon.ActiveWeapon != mActorToForce.weapon.SecondaryWeapon)
			{
				mActorToForce.weapon.Toggle();
			}
			mActorToForce.weapon.WeaponSelectionLocked = true;
			break;
		case ForceWeapon.None:
			mActorToForce.weapon.LowerWeapon();
			mActorToForce.weapon.WeaponSelectionLocked = true;
			break;
		case ForceWeapon.Unforce:
			mActorToForce.weapon.RaiseWeapon();
			break;
		}
	}
}
