using UnityEngine;

public class CMVent : CMSetPiece
{
	public Transform VentPos;

	public NavigationZone NavZone;

	private bool m_hasTriggeredSecondaryEvent;

	protected override void Start()
	{
		base.Start();
		mContextBlip.IsAllowedInFirstPerson = true;
	}

	protected override bool validInFirstPerson()
	{
		return true;
	}

	public void OnDisable()
	{
		if (!base.IsComplete || m_hasTriggeredSecondaryEvent)
		{
			return;
		}
		GameplayController instance = GameplayController.instance;
		if (instance == null)
		{
			return;
		}
		GameController instance2 = GameController.Instance;
		if (instance2 == null)
		{
			return;
		}
		Actor actor = null;
		if (instance2.IsFirstPerson && instance2.mFirstPersonActor != null)
		{
			actor = instance2.mFirstPersonActor;
		}
		else
		{
			Actor nearestSelected = OrdersHelper.GetNearestSelected(instance, base.transform.position);
			if (nearestSelected != null)
			{
				actor = nearestSelected;
			}
		}
		if (actor != null)
		{
			SetPieceModule actionSetPieceModule = actor.realCharacter.mNavigationSetPiece.GetActionSetPieceModule(NavigationSetPieceLogic.ActionSetPieceType.kWalkVent);
			if (actionSetPieceModule != null)
			{
				SetPieceLogic setPieceLogic = actor.realCharacter.CreateSetPieceLogic();
				setPieceLogic.SetModule(actionSetPieceModule);
				if (VentPos != null)
				{
					setPieceLogic.PlaceSetPiece(VentPos);
				}
				setPieceLogic.SetActor_IndexOnlyCharacters(0, actor);
				OrdersHelper.OrderSetPiece(instance, setPieceLogic, null);
				if (NavZone != null)
				{
					NavZone.Type = NavigationZone.NavigationType.VentOpen;
				}
			}
		}
		m_hasTriggeredSecondaryEvent = true;
	}
}
