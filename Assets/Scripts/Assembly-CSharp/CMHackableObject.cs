public class CMHackableObject : InterfaceableObject
{
	private HackableObject mHackableObject;

	public string ActionLabel = "S_CMHack";

	public string CancelLabel = "S_CMCancel";

	public string ResumeLabel = "S_CMResumeHack";

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void Start()
	{
		base.Start();
		mContextBlip.IsAllowedInFirstPerson = true;
		mHackableObject = GetComponentInChildren<HackableObject>();
	}

	protected override void PopulateMenuItems()
	{
		if (!(mHackableObject == null) && ShouldPopulateMenuItems())
		{
			if (mHackableObject.IsInProgress)
			{
				AddCallableMethod(CancelLabel, "S_CMCancel", ContextMenuIcons.Hack);
			}
			else if (!mHackableObject.IsInUse)
			{
				AddCallableMethod((!(mHackableObject.Hacked0To1 > 0f)) ? ActionLabel : ResumeLabel, "S_CMHack", ContextMenuIcons.Hack);
			}
			if (ShouldShowKillOption() && ContextMenuVisualiser.ContextMenuDebugOptions)
			{
				AddCallableMethod("S_CMKill", ContextMenuIcons.Debug);
			}
		}
	}

	protected override void SetIconForDefaultOption()
	{
		if (!(mHackableObject == null) && ShouldPopulateMenuItems())
		{
			base.SetIconForDefaultOption();
			if (mContextBlip != null && !mHackableObject.IsInUse)
			{
				SetDefaultMethodForFirstPerson(ActionLabel.ToUpper(), "S_CMHack", ContextMenuIcons.Hack);
			}
		}
	}

	public void S_CMHack()
	{
		if (mHackableObject != null && !mHackableObject.IsInProgress)
		{
			if (mHackableObject.Hacked0To1 > 0f)
			{
				mHackableObject.ResumeHacking();
			}
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController != null)
			{
				OrdersHelper.OrderHack(gameplayController, mHackableObject);
			}
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMCancel()
	{
		if (mHackableObject != null)
		{
			mHackableObject.CancelHacking();
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMKill()
	{
		Actor component = GetComponent<Actor>();
		if (component != null)
		{
			component.realCharacter.Kill("ActOfGod");
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void EnableInteraction()
	{
		mHackableObject.EnableInteraction();
	}

	public void DisableInteraction()
	{
		mHackableObject.DisableInteraction();
	}

	protected virtual bool ShouldPopulateMenuItems()
	{
		return true;
	}

	protected virtual bool ShouldShowKillOption()
	{
		return false;
	}
}
