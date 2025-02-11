using UnityEngine;

public class CMMysteryCache : InterfaceableObject
{
	public SetPieceModule UseSetPiece;

	public GameObject CrateModel;

	private MysteryCache mMysteryCache;

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void Start()
	{
		base.Start();
		mMysteryCache = base.gameObject.GetComponent<MysteryCache>();
		if (!(mContextBlip != null))
		{
		}
	}

	protected override void PopulateMenuItems()
	{
		if (!(mMysteryCache == null))
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
		if (mContextBlip != null && !(mMysteryCache == null))
		{
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Open))
			{
				SetDefaultMethodForFirstPerson("S_MYSTERY_INTERACT", "S_CMOpen", ContextMenuIcons.OpenDoor, GMGData.Instance.MysteryCost);
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
		if (mMysteryCache != null)
		{
			Actor nearestSelected = OrdersHelper.GetNearestSelected(gameplayController, base.transform.position);
			if (nearestSelected != null)
			{
				mMysteryCache.TryPurchase(nearestSelected, this);
			}
		}
		CleaupAfterSelection();
	}
}
