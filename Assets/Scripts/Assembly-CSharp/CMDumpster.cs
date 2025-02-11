public class CMDumpster : InterfaceableObject
{
	private Dumpster mDumpsterHandle;

	private MenuButton mEnterButton;

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void Start()
	{
		base.Start();
		mContextBlip.IsAllowedInFirstPerson = true;
		mDumpsterHandle = GetComponent<Dumpster>();
	}

	protected override void PopulateMenuItems()
	{
		if (mDumpsterHandle.IsOccupied)
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
		if (mDumpsterHandle.IsOccupied)
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
		else if (mEnterButton != null && mDumpsterHandle != null)
		{
			Actor nearestSelected = OrdersHelper.GetNearestSelected(GameplayController.instance, mDumpsterHandle.transform.position);
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
			OrdersHelper.OrderEnter(gameplayController, mDumpsterHandle);
		}
		CleaupAfterSelection();
	}

	public void S_CMHideBody()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController != null)
		{
			OrdersHelper.OrderHideBody(gameplayController, mDumpsterHandle);
		}
		CleaupAfterSelection();
	}

	private bool IsCarryingBody()
	{
		GameplayController gameplayController = GameplayController.Instance();
		return gameplayController != null && gameplayController.IsAnySelectedCarryingAEnemy();
	}
}
