public class CMLocker : InterfaceableObject
{
	private Locker mLockerHandle;

	private MenuButton mEnterButton;

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void Start()
	{
		base.Start();
		mContextBlip.IsAllowedInFirstPerson = true;
		mLockerHandle = GetComponent<Locker>();
	}

	protected override void PopulateMenuItems()
	{
		if (mLockerHandle.IsOccupied)
		{
			return;
		}
		GameplayController gameplayController = GameplayController.Instance();
		if (!(gameplayController == null))
		{
			if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.HideBody) && IsCarryingBody())
			{
				AddCallableMethod("S_CMHideBody", ContextMenuIcons.Hide);
			}
			else if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Enter))
			{
				AddCallableMethod("S_CMEnter", ContextMenuIcons.Hide);
			}
		}
	}

	protected override void SetIconForDefaultOption()
	{
		base.SetIconForDefaultOption();
		if (mLockerHandle.IsOccupied)
		{
			return;
		}
		GameplayController gameplayController = GameplayController.Instance();
		if (!(gameplayController == null) && mContextBlip != null)
		{
			if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.HideBody) && IsCarryingBody())
			{
				SetDefaultMethodForFirstPerson("S_CMHIDEBODY", "S_CMHideBody", ContextMenuIcons.Hide);
			}
			else if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Enter))
			{
				SetDefaultMethodForFirstPerson("S_CMENTER", "S_CMEnter", ContextMenuIcons.Hide);
			}
		}
	}

	private bool HaveIBeenSpotted(Actor actor)
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(actor.awareness.EnemiesWhoCanSeeMe());
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.behaviour.InActiveAlertState())
			{
				return true;
			}
		}
		return false;
	}

	public override void Update()
	{
		ContextObjectBlip contextObjectBlip = GetContextObjectBlip();
		if (contextObjectBlip != null)
		{
			contextObjectBlip.ShouldBeDisabled = false;
		}
		if (GameController.Instance.IsFirstPerson)
		{
			if (contextObjectBlip != null && GameController.Instance.mFirstPersonActor != null)
			{
				contextObjectBlip.ShouldBeDisabled = HaveIBeenSpotted(GameController.Instance.mFirstPersonActor);
			}
		}
		else if (mEnterButton != null && mLockerHandle != null)
		{
			Actor nearestSelected = OrdersHelper.GetNearestSelected(GameplayController.instance, mLockerHandle.transform.position);
			if (nearestSelected != null)
			{
				mEnterButton.SetButtonActive(!HaveIBeenSpotted(nearestSelected));
			}
		}
		base.Update();
	}

	protected override void ClearMenuItems()
	{
		if (CommonHudController.Instance.ContextMenu != null)
		{
			ContextMenuBase contextMenu = CommonHudController.Instance.ContextMenu;
			mEnterButton = contextMenu.GetButton(ContextMenuIcons.Hide);
		}
		else
		{
			mEnterButton = null;
		}
		base.ClearMenuItems();
	}

	private void CleaupAfterSelection()
	{
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMEnter()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController != null)
		{
			OrdersHelper.OrderEnter(gameplayController, mLockerHandle);
		}
		CleaupAfterSelection();
	}

	public void S_CMHideBody()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController != null)
		{
			OrdersHelper.OrderHideBody(gameplayController, mLockerHandle);
		}
		CleaupAfterSelection();
	}

	private bool IsCarryingBody()
	{
		GameplayController gameplayController = GameplayController.Instance();
		return gameplayController != null && gameplayController.IsAnySelectedCarryingAEnemy();
	}
}
