public class CMZipLine : InterfaceableObject
{
	private ZipLine mZipLineHandle;

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void Start()
	{
		base.Start();
		mContextBlip.IsAllowedInFirstPerson = true;
		mZipLineHandle = GetComponent<ZipLine>();
	}

	protected override void PopulateMenuItems()
	{
		if (!(mZipLineHandle == null))
		{
			AddCallableMethod("S_CMZipLine", "S_CMZipLine", ContextMenuIcons.Use);
		}
	}

	protected override void SetIconForDefaultOption()
	{
		base.SetIconForDefaultOption();
		if (!(mZipLineHandle == null) && mContextBlip != null)
		{
			mZipLineHandle = base.gameObject.GetComponent<ZipLine>();
			SetDefaultMethodForFirstPerson("S_CMZIPLINE", "S_CMZipLine", ContextMenuIcons.Use);
		}
	}

	public void S_CMZipLine()
	{
		if (mZipLineHandle == null)
		{
			return;
		}
		if (base.enabled)
		{
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController != null)
			{
				OrdersHelper.OrderUseZipLine(gameplayController, mZipLineHandle);
			}
		}
		CommonHudController.Instance.ClearContextMenu();
	}
}
