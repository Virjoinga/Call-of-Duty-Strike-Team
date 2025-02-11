public class CMSetUpExplosives : InterfaceableObject
{
	public string Label = string.Empty;

	private ExplodableObject mExplodableObject;

	protected override void Start()
	{
		base.Start();
		mExplodableObject = GetComponent<ExplodableObject>();
	}

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void PopulateMenuItems()
	{
		if (GameplayController.Instance().AnySelectedAllowedCMOption(ContextMenuOptionType.SetUpExplosives) && mExplodableObject.ArmedBy == null)
		{
			if (string.IsNullOrEmpty(Label))
			{
				AddCallableMethod("S_CMSetUpExplosives", ContextMenuIcons.C4);
			}
			else
			{
				AddCallableMethod(Label, "S_CMSetUpExplosives", ContextMenuIcons.C4);
			}
		}
	}

	protected override void SetIconForDefaultOption()
	{
		base.SetIconForDefaultOption();
		if (mContextBlip != null && GameplayController.Instance().AnySelectedAllowedCMOption(ContextMenuOptionType.SetUpExplosives) && mExplodableObject.ArmedBy == null)
		{
			if (string.IsNullOrEmpty(Label))
			{
				SetDefaultMethodForFirstPerson("S_CMSETUPEXPLOSIVES", "S_CMSetUpExplosives", ContextMenuIcons.C4);
			}
			else
			{
				SetDefaultMethodForFirstPerson(Label, "S_CMSetUpExplosives", ContextMenuIcons.C4);
			}
		}
	}

	private void CleaupAfterSelection()
	{
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMSetUpExplosives()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (mExplodableObject != null)
		{
			mExplodableObject.SwitchToCorrectSPL();
			OrdersHelper.OrderArmExplosives(gameplayController, GetComponent<SetPieceLogic>(), null, mExplodableObject);
		}
		else
		{
			SetPieceLogic component = GetComponent<SetPieceLogic>();
			if (component != null)
			{
				Actor nearestSelected = OrdersHelper.GetNearestSelected(gameplayController, component.transform.position);
				if (nearestSelected != null)
				{
					new TaskPlantC4(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, mExplodableObject);
					OrdersHelper.OrderSetPiece(gameplayController, component, null, typeof(TaskSetPiece));
				}
			}
		}
		CleaupAfterSelection();
	}

	public void RemoveBlip()
	{
		base.enabled = false;
		if (mContextBlip != null)
		{
			mContextBlip.enabled = false;
		}
		DeleteBlip();
	}
}
