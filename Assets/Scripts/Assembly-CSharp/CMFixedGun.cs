public class CMFixedGun : InterfaceableObject
{
	private FixedGun fixedGun;

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void Start()
	{
		base.Start();
		if (mContextBlip != null)
		{
			mContextBlip.IsAllowedInFirstPerson = true;
		}
	}

	protected override void PopulateMenuItems()
	{
		fixedGun = base.gameObject.GetComponent<FixedGun>();
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.UseFixedGun) && !IsOccupied(fixedGun))
		{
			AddCallableMethod("S_CMUseFixedGun", ContextMenuIcons.Use);
		}
	}

	protected override void SetIconForDefaultOption()
	{
		base.SetIconForDefaultOption();
		if (mContextBlip != null)
		{
			fixedGun = base.gameObject.GetComponent<FixedGun>();
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController != null && gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.UseFixedGun) && !IsOccupied(fixedGun))
			{
				SetDefaultMethodForFirstPerson("S_CMUSEFIXEDGUN", "S_CMUseFixedGun", ContextMenuIcons.Use);
			}
		}
	}

	private void CleaupAfterSelection()
	{
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMUseFixedGun()
	{
		GameplayController gameplayController = GameplayController.Instance();
		OrdersHelper.OrderUseFixedGun(gameplayController, fixedGun);
		CleaupAfterSelection();
	}

	private bool IsOccupied(FixedGun fixedGun)
	{
		return fixedGun.HasGunner();
	}
}
