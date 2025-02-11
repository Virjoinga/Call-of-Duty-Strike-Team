using System.Collections;
using UnityEngine;

public class WhileNotPressingButtonCommand : Command
{
	public enum HUDItems
	{
		FPPADS = 0,
		FPPCrouch = 1,
		FPPFollowMe = 2,
		FPPGrenade = 3,
		FPPSnapTarget = 4,
		FPPTrigger = 5,
		FPPUnitSelect = 6,
		FPPWeaponSelect = 7,
		TPPDropBody = 8,
		TPPDropClaymore = 9,
		TPPDropGrenade = 10,
		TPPUnitSelector = 11,
		Zoom = 12,
		Move = 13,
		Look = 14,
		FPPSnapTargetLeft = 15,
		FPPSnapTargetRight = 16,
		TPPSelectAllUnits = 17,
		FPPExitMinigun = 18
	}

	public HUDItems button;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		switch (button)
		{
		default:
			yield break;
		case HUDItems.FPPADS:
		{
			IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon);
			while (weaponADS != null && weaponADS.GetADSState() != 0)
			{
				yield return null;
			}
			yield break;
		}
		case HUDItems.FPPCrouch:
		{
			BaseCharacter.Stance initialStance = GameController.Instance.mFirstPersonActor.realCharacter.GetStance();
			while (GameController.Instance.mFirstPersonActor != null && GameController.Instance.mFirstPersonActor.realCharacter.GetStance() == initialStance)
			{
				yield return null;
			}
			yield break;
		}
		case HUDItems.FPPFollowMe:
			while (!CommonHudController.Instance.FollowMeRecentlyPressed())
			{
				yield return null;
			}
			yield break;
		case HUDItems.FPPGrenade:
		{
			Weapon_Grenade grenadeWeapon = null;
			while (grenadeWeapon == null && GameController.Instance.mFirstPersonActor != null)
			{
				grenadeWeapon = (GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon as Weapon_Grenade) ?? (GameController.Instance.mFirstPersonActor.weapon.DesiredWeapon as Weapon_Grenade);
				yield return null;
			}
			yield break;
		}
		case HUDItems.FPPSnapTarget:
			while (!CommonHudController.Instance.HasSnappedToTarget)
			{
				yield return null;
			}
			yield break;
		case HUDItems.FPPSnapTargetLeft:
			while (!CommonHudController.Instance.HasSnappedToTargetLeft)
			{
				yield return null;
			}
			yield break;
		case HUDItems.FPPSnapTargetRight:
			while (!CommonHudController.Instance.HasSnappedToTargetRight)
			{
				yield return null;
			}
			yield break;
		case HUDItems.FPPTrigger:
			while (!CommonHudController.Instance.TriggerPressed)
			{
				yield return null;
			}
			yield break;
		case HUDItems.FPPUnitSelect:
		{
			Actor initialFPPActor = null;
			if (GameController.Instance.IsFirstPerson)
			{
				initialFPPActor = GameController.Instance.mFirstPersonActor;
			}
			bool quitLoop = false;
			while (!quitLoop)
			{
				if (GameController.Instance.IsFirstPerson)
				{
					if (initialFPPActor == null)
					{
						initialFPPActor = GameController.Instance.mFirstPersonActor;
					}
					else if (initialFPPActor != GameController.Instance.mFirstPersonActor)
					{
						quitLoop = true;
					}
				}
				yield return null;
			}
			yield break;
		}
		case HUDItems.FPPWeaponSelect:
		{
			IWeapon initialWeapon = null;
			if (GameController.Instance.IsFirstPerson)
			{
				initialWeapon = GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon;
			}
			bool quitLoop2 = false;
			while (!quitLoop2)
			{
				if (GameController.Instance.IsFirstPerson)
				{
					if (initialWeapon == null)
					{
						initialWeapon = GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon;
					}
					else if (initialWeapon != GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon)
					{
						quitLoop2 = true;
					}
				}
				yield return null;
			}
			yield break;
		}
		case HUDItems.TPPDropBody:
			while (!GameplayController.instance.PlacingBody)
			{
				yield return null;
			}
			yield break;
		case HUDItems.TPPDropClaymore:
			while (!GameplayController.instance.PlacingClaymore)
			{
				yield return null;
			}
			yield break;
		case HUDItems.TPPDropGrenade:
			while (!GameController.Instance.GrenadeThrowingModeActive)
			{
				yield return null;
			}
			yield break;
		case HUDItems.TPPUnitSelector:
		{
			float numOfSelected = GameplayController.instance.Selected.Count;
			Actor leader = GameplayController.instance.SelectedLeader;
			while (leader == GameplayController.instance.SelectedLeader && numOfSelected == (float)GameplayController.instance.Selected.Count)
			{
				yield return null;
			}
			yield break;
		}
		case HUDItems.Zoom:
		{
			bool isFPP = GameController.Instance.IsFirstPerson;
			while (isFPP == GameController.Instance.IsFirstPerson)
			{
				yield return null;
			}
			yield break;
		}
		case HUDItems.Look:
			while (CommonHudController.Instance.LookAmountTouch == Vector2.zero && CommonHudController.Instance.LookAmountGamepad == Vector2.zero)
			{
				yield return null;
			}
			yield break;
		case HUDItems.Move:
			while (CommonHudController.Instance.MoveAmount == Vector2.zero)
			{
				yield return null;
			}
			yield break;
		case HUDItems.FPPExitMinigun:
			break;
		}
		while (CommonHudController.Instance.ContextInteractionButton.controlState != UIButton.CONTROL_STATE.ACTIVE && GameController.Instance.IsFirstPerson && GameController.Instance.mFirstPersonActor.tasks.GetRunningTask<TaskUseFixedGun>() != null)
		{
			yield return null;
		}
	}
}
