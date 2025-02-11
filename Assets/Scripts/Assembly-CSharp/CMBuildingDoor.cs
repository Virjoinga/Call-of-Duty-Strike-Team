public class CMBuildingDoor : InterfaceableObject
{
	private BuildingDoor mDoor;

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void Start()
	{
		mDoor = base.gameObject.GetComponent<BuildingDoor>();
		if (mDoor.SiblingDoor != null)
		{
			CMBuildingDoor component = mDoor.SiblingDoor.gameObject.GetComponent<CMBuildingDoor>();
			if (component.mContextBlip != null)
			{
				UseBlip = false;
			}
		}
		base.Start();
		if (mContextBlip != null)
		{
			mContextBlip.WorldOffset = mDoor.ExplosivePlantPosition;
			mContextBlip.WorldOffset -= mDoor.transform.position;
		}
	}

	protected override void PopulateMenuItems()
	{
		mDoor = base.gameObject.GetComponent<BuildingDoor>();
		if (!(mDoor == null))
		{
			if (!mDoor.IsSiblingDriver && mDoor.SiblingDoor != null)
			{
				mDoor = mDoor.SiblingDoor;
			}
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Open) && mDoor.m_Interface.State == BuildingDoor.DoorSate.Closed && mDoor.m_Interface.CanOpenAndClose)
			{
				AddCallableMethod("S_CMOpen", ContextMenuIcons.OpenDoor);
			}
			if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.BreachByExplosive) && IsBreachable() && (!OrdersHelper.IsAnySelectedPlayerSquadMemberInInterior(GameplayController.Instance(), mDoor.AssociatedBuilding) || mDoor.m_Interface.IsInterior))
			{
				AddCallableMethod("S_CMBreachByExplosive", ContextMenuIcons.Breach);
			}
		}
	}

	protected override void SetIconForDefaultOption()
	{
		base.SetIconForDefaultOption();
		if (!(mContextBlip != null))
		{
			return;
		}
		mDoor = base.gameObject.GetComponent<BuildingDoor>();
		if (!(mDoor == null))
		{
			if (!mDoor.IsSiblingDriver && mDoor.SiblingDoor != null)
			{
				mDoor = mDoor.SiblingDoor;
			}
			GameplayController gameplayController = GameplayController.Instance();
			if ((mDoor.AssociatedBuilding == null || GameController.Instance.mFirstPersonActor.realCharacter.Location != mDoor.AssociatedBuilding || mDoor.m_Interface.IsInterior) && gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.BreachByExplosive) && IsBreachable())
			{
				SetDefaultMethodForFirstPerson("S_CMBREACHBYEXPLOSIVE", "S_CMBreachByExplosive", ContextMenuIcons.Breach);
			}
			else if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Open) && mDoor.m_Interface.State == BuildingDoor.DoorSate.Closed)
			{
				SetDefaultMethodForFirstPerson("S_CMOPEN", "S_CMOpen", ContextMenuIcons.OpenDoor);
			}
		}
	}

	private void CleaupAfterSelection()
	{
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMOpen()
	{
		GameplayController gameplayController = GameplayController.Instance();
		OrdersHelper.OrderOpen(gameplayController, mDoor);
		CleaupAfterSelection();
	}

	public void S_CMClose()
	{
		GameplayController gameplayController = GameplayController.Instance();
		OrdersHelper.OrderClose(gameplayController, mDoor);
		CleaupAfterSelection();
	}

	public void S_CMBreachByExplosive()
	{
		GameplayController gameplayController = GameplayController.Instance();
		OrdersHelper.OrderBreachByExplosive(gameplayController, mDoor);
		CleaupAfterSelection();
	}

	public void S_CMLock()
	{
		mDoor.Lock();
		CleaupAfterSelection();
	}

	public void S_CMUnLock()
	{
		mDoor.Unlock();
		CleaupAfterSelection();
	}

	private bool IsBreachable()
	{
		TBFAssert.DoAssert(mDoor, "CMBuildingDoor -> IsBreachable : Door reference not initialised");
		if ((mDoor.m_Interface.State == BuildingDoor.DoorSate.Closed || mDoor.m_Interface.State == BuildingDoor.DoorSate.LockedClosed) && mDoor.m_Interface.Breachable)
		{
			return true;
		}
		return false;
	}
}
