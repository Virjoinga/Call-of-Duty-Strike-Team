using UnityEngine;

public class CMPlayerSoldier : CMSoldier
{
	protected override bool validInFirstPerson()
	{
		return UseBlip;
	}

	protected override void Start()
	{
		base.Start();
		target = base.AssociatedObject.GetComponent<Actor>();
		if (target != null)
		{
			UseBlip = target.behaviour.PlayerControlled;
			if (target.behaviour.PlayerControlled)
			{
				invocationType = InvocationTypeEnum.Immediate;
			}
			target.health.OnHealthChange += HandleTargethealthOnHealthChange;
			target.health.OnHealthMaxed += HandleTargethealthOnHealthMaxed;
		}
		base.IgnoreTutorialLock = true;
	}

	private void HandleTargethealthOnHealthMaxed(object sender, HealthComponent.HeathChangeEventArgs args)
	{
		if (mContextBlip == null)
		{
			CreateBlip();
		}
	}

	private void HandleTargethealthOnHealthChange(object sender, HealthComponent.HeathChangeEventArgs args)
	{
		if (!target.behaviour.PlayerControlled)
		{
			return;
		}
		if (target != null && (target.health.IsMortallyWounded() || target.health.Health <= 0f))
		{
			if (!UseBlip)
			{
				UseBlip = true;
				CreateBlip();
			}
			UseBlip = true;
			if (mContextBlip != null)
			{
				mContextBlip.IsAllowedInFirstPerson = true;
				mContextBlip.renderer.enabled = true;
				mContextBlip.OffsetTarget = target.baseCharacter.Ragdoll.Pelvis.Bone.transform;
				mContextBlip.OffsetTargetOffset = Vector3.zero;
				CanBeTurnedOn = true;
				TurnOn();
			}
		}
		else if (!target.health.IsMortallyWounded() && base.collider != null)
		{
			base.collider.enabled = true;
		}
	}

	public void HideBlip()
	{
		if (base.ContextBlip != null)
		{
			base.ContextBlip.renderer.enabled = false;
		}
	}

	public override void Update()
	{
		if (target.realCharacter.IsDead() || target.realCharacter.IsMortallyWounded())
		{
			if ((target.realCharacter.CanBeCarried() || target.realCharacter.CanBeHealed()) && !target.realCharacter.IsBeingCarried && !target.health.IsReviving)
			{
				if (base.collider != null)
				{
				}
				UseBlip = true;
				ShowBlip(true);
			}
			else
			{
				if (base.collider != null)
				{
					base.collider.enabled = false;
				}
				ShowBlip(false);
				UseBlip = false;
			}
			base.InvocationType = InvocationTypeEnum.Immediate;
		}
		else if (base.ContextBlip != null)
		{
			base.ContextBlip.renderer.enabled = false;
			if (!base.ContextBlip.IsOnScreen)
			{
				base.ContextBlip.collider.enabled = false;
			}
		}
		AdjustCollider(target);
		base.Update();
	}

	private void OnDestroy()
	{
		if (target != null)
		{
			target.health.OnHealthChange -= HandleTargethealthOnHealthChange;
			target.health.OnHealthMaxed -= HandleTargethealthOnHealthMaxed;
		}
	}

	protected override void PopulateMenuItems()
	{
		TBFAssert.DoAssert(target != null, "CMPlayerSoldier is not registered to an Actor!");
		GameplayController gameplayController = GameplayController.Instance();
		bool flag = true;
		if (gameplayController.IsSelected(target))
		{
			if (ContextMenuVisualiser.ContextMenuDebugOptions)
			{
				AddCallableMethod("S_CMKill", ContextMenuIcons.Debug);
				AddCallableMethod("S_CMDamage", ContextMenuIcons.Debug);
				flag = false;
			}
		}
		else if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Hack) && target.awareness.ChDefCharacterType == CharacterType.AutonomousGroundRobot && target.realCharacter.IsDead())
		{
			AddCallableMethod("S_CMRepair", ContextMenuIcons.Repair);
			flag = false;
		}
		else
		{
			if (!target.behaviour.PlayerControlled && gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Carry) && target.realCharacter.CanBeCarried())
			{
				AddCallableMethod("S_CMCarry", ContextMenuIcons.PickupBody);
				flag = false;
			}
			if (target.realCharacter.IsMortallyWounded() && target.behaviour.PlayerControlled && target.realCharacter.CanBeHealed())
			{
				AddCallableMethod("S_CMHeal", ContextMenuIcons.Heal);
				flag = false;
			}
		}
		if (flag && target.behaviour.PlayerControlled)
		{
			GameplayController.Instance().ProcessUnitSelectLogic(target.gameObject);
		}
	}

	protected override void SetIconForDefaultOption()
	{
		base.SetIconForDefaultOption();
		if (mContextBlip != null && target != null)
		{
			if (target == GameController.Instance.mFirstPersonActor)
			{
				HideBlip();
			}
			else if (target.behaviour.PlayerControlled && target.realCharacter.IsMortallyWounded() && target.realCharacter.CanBeHealed() && !target.health.IsReviving)
			{
				SetDefaultMethodForFirstPerson("S_CMHEAL", "S_CMHeal", ContextMenuIcons.Heal);
			}
		}
	}

	public void S_CMCarry()
	{
		if (target != null && target.realCharacter.CanBeCarried())
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderCarry(gameplayController, target);
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMHeal()
	{
		PlayerSquadManager instance = PlayerSquadManager.Instance;
		if (target != null && instance != null && target.realCharacter.CanBeHealed())
		{
			PlayerSquadManager.Instance.UseMedKit(target, false);
		}
		CommonHudController.Instance.ClearContextMenu();
		DeleteBlip();
	}

	public void S_CMFollow()
	{
		Actor component = base.gameObject.GetComponent<Actor>();
		if (component != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderFollow(gameplayController, component);
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMDefend()
	{
		if (target != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderDefend(gameplayController, target);
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMHalt()
	{
		GameplayController gameplayController = GameplayController.Instance();
		OrdersHelper.OrderHoldPosition(gameplayController);
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMKill()
	{
		if (target != null)
		{
			target.realCharacter.Kill("MeleeDeath");
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMDamage()
	{
		if (target != null)
		{
			target.health.ForceCriticallyInjured();
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMExit()
	{
		if (target != null)
		{
			HidingPlace dockedContainer = target.realCharacter.DockedContainer;
			TBFAssert.DoAssert(dockedContainer != null, "Trying to Exit but not inside anything");
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController != null)
			{
				OrdersHelper.OrderExit(gameplayController, dockedContainer);
			}
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMDrop()
	{
		GameplayController gameplayController = GameplayController.Instance();
		OrdersHelper.OrderDrop(gameplayController, base.transform.position);
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMRepair()
	{
		if (target != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderRepair(gameplayController, target);
		}
		CommonHudController.Instance.ClearContextMenu();
	}
}
