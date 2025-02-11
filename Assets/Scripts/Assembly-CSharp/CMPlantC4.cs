public class CMPlantC4 : InterfaceableObject
{
	public string Label = string.Empty;

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void PopulateMenuItems()
	{
		if (GameplayController.Instance().AnySelectedAllowedCMOption(ContextMenuOptionType.SetUpExplosives))
		{
			AddCallableMethod(Label, "S_CMPlantC4", ContextMenuIcons.C4);
		}
	}

	protected override void SetIconForDefaultOption()
	{
		base.SetIconForDefaultOption();
		if (mContextBlip != null && GameplayController.Instance().AnySelectedAllowedCMOption(ContextMenuOptionType.SetUpExplosives))
		{
			SetDefaultMethodForFirstPerson(Label, "S_CMPlantC4", ContextMenuIcons.C4);
		}
	}

	public void S_CMPlantC4()
	{
		OrdersHelper.OrderPlantC4(base.transform);
		CleaupAfterSelection();
	}

	private void CleaupAfterSelection()
	{
		CommonHudController.Instance.ClearContextMenu();
	}
}
