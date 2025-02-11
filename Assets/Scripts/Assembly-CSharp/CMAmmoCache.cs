using UnityEngine;

public class CMAmmoCache : InterfaceableObject
{
	public SetPieceModule UseSetPiece;

	public GameObject CrateModel;

	private AmmoCache mAmmoCache;

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void Start()
	{
		base.Start();
		mAmmoCache = base.gameObject.GetComponent<AmmoCache>();
		if (!(mContextBlip != null))
		{
		}
	}

	protected override void PopulateMenuItems()
	{
		if (!(mAmmoCache == null))
		{
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Open))
			{
				AddCallableMethod("S_CMOpen", ContextMenuIcons.OpenDoor);
			}
		}
	}

	protected override void SetIconForDefaultOption()
	{
		base.SetIconForDefaultOption();
		if (mContextBlip != null && !(mAmmoCache == null))
		{
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Open))
			{
				SetDefaultMethodForFirstPerson("S_GMG_AMMO_REFILL", "S_CMOpen", ContextMenuIcons.OpenDoor, GMGData.Instance.GetAmmoCost());
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
		if (mAmmoCache != null)
		{
			Actor nearestSelected = OrdersHelper.GetNearestSelected(gameplayController, base.transform.position);
			if (nearestSelected != null)
			{
				mAmmoCache.TryPurchase(nearestSelected, this);
			}
		}
		CleaupAfterSelection();
	}
}
