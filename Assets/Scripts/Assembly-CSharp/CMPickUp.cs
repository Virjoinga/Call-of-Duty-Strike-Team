using UnityEngine;

public class CMPickUp : InterfaceableObject
{
	public SetPieceModule PickUpSetPiece;

	public SetPieceModule PickUpSetPieceOffFloor;

	public SetPieceModule PickUpSetPieceFPP;

	public GameObject IntelToPickUp;

	protected override bool validInFirstPerson()
	{
		return true;
	}

	protected override void PopulateMenuItems()
	{
		if (GameplayController.Instance().AnySelectedAllowedCMOption(ContextMenuOptionType.PickUp))
		{
			AddCallableMethod("S_CMPickUp", ContextMenuIcons.Intel);
		}
	}

	private void CleaupAfterSelection()
	{
		CommonHudController.Instance.ClearContextMenu();
	}

	protected override void SetIconForDefaultOption()
	{
		base.SetIconForDefaultOption();
		if (mContextBlip != null && GameplayController.Instance().AnySelectedAllowedCMOption(ContextMenuOptionType.PickUp))
		{
			SetDefaultMethodForFirstPerson("S_CMPICKUP", "S_CMPickUp", ContextMenuIcons.Intel);
		}
	}

	public void S_CMPickUp()
	{
		if (IntelToPickUp != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController != null)
			{
				OrdersHelper.OrderPickUp(gameplayController, this);
			}
		}
		CleaupAfterSelection();
	}

	public SetPieceModule GetSetPiece(PickUpObject pickUp, Actor actor)
	{
		if (actor == GameController.Instance.mFirstPersonActor)
		{
			return PickUpSetPieceFPP;
		}
		if (pickUp != null)
		{
			if (pickUp.m_Interface.Positioning == CollectableData.PositioningType.OnTable)
			{
				return PickUpSetPiece;
			}
			return PickUpSetPieceOffFloor;
		}
		return null;
	}
}
