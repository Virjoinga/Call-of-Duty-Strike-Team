public class TaskMortallyWounded : Task
{
	private CMPlayerSoldier mContextMenu;

	public TaskMortallyWounded(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		mActor.health.CacheAndSetCanRecharge(false);
		if (SectionTypeHelper.IsAGMG())
		{
			return;
		}
		if (mActor == GameController.Instance.mFirstPersonActor)
		{
			if (GameplayController.instance.IsSelected(mActor))
			{
				GameplayController.instance.RemoveFromSelected_NoAutoSelect(mActor);
			}
			GameController.Instance.ExitFirstPerson();
		}
		mContextMenu = mActor.gameObject.GetComponent<CMPlayerSoldier>();
		if (mContextMenu != null)
		{
			mContextMenu.InvocationType = InterfaceableObject.InvocationTypeEnum.Immediate;
		}
		ActorSelectUtils.EnableActorSelectCollider(mActor, false);
	}

	public override void Update()
	{
	}

	public override bool HasFinished()
	{
		return mActor.realCharacter.IsDead() || !mActor.realCharacter.IsMortallyWounded();
	}

	public override void Finish()
	{
		if (!mActor.health.IsReviving)
		{
			mActor.health.RestoreCanRecharge();
		}
		mActor.animDirector.EnableCategory(mActor.animDirector.GetCategoryHandle("Wounded"), false, 0.1f);
		mActor.baseCharacter.EnableNavMesh(true);
		mActor.health.ModifyHealth(mActor.gameObject, mActor.health.HealthMax - mActor.health.Health, "Heal", mActor.transform.forward, false);
		if (mActor.weapon != null)
		{
			Weapon_Grenade weapon_Grenade = mActor.weapon.ActiveWeapon as Weapon_Grenade;
			if (weapon_Grenade != null)
			{
				mActor.weapon.SwitchToPrevious();
			}
		}
		if (SectionTypeHelper.IsAGMG())
		{
			if (mActor.behaviour.PlayerControlled && !GameplayController.instance.IsSelected(mActor))
			{
				GameplayController.instance.AddToSelected(mActor);
			}
			if (!mActor.realCharacter.IsDead())
			{
				mActor.firstThirdPersonWidget.ClearAnimFinishPoseHold();
			}
			return;
		}
		if (mContextMenu != null)
		{
			mContextMenu.InvocationType = InterfaceableObject.InvocationTypeEnum.Immediate;
		}
		ActorSelectUtils.EnableActorSelectCollider(mActor, true);
		ActorSelectUtils.UpdateActorSelectCollider(mActor, ActorSelectUtils.NormalColliderSettings[(int)mActor.awareness.faction]);
		if (GameController.Instance.IsFirstPerson && WorldHelper.IsSelectableActor(mActor))
		{
			if (GKM.UnitCount(GKM.PlayerControlledMask & GKM.UpAndAboutMask & GKM.SelectableMask) <= 1)
			{
				CommonHudController.Instance.HideFollowMe(true);
				CommonHudController.Instance.HideFPPUntChangeButtons(true);
			}
			else
			{
				CommonHudController.Instance.HideFollowMe(false);
				CommonHudController.Instance.HideFPPUntChangeButtons(false);
			}
		}
	}
}
