using UnityEngine;

public class CMSetPiece : InterfaceableObject
{
	public ContextMenuIcons Icon;

	public string Label;

	public GameObject OptionalTarget;

	public bool m_OnceOnly;

	private bool m_isComplete;

	private SetPieceLogic setPieceHandle;

	[HideInInspector]
	public bool IsComplete
	{
		get
		{
			return m_isComplete;
		}
		set
		{
			m_isComplete = value;
			if (m_OnceOnly && m_isComplete)
			{
				UseBlip = false;
				CanBeTurnedOn = false;
				DeleteBlip();
				base.enabled = false;
			}
		}
	}

	protected override bool validInFirstPerson()
	{
		return UseBlip;
	}

	protected override void PopulateMenuItems()
	{
		if (!m_OnceOnly || !m_isComplete)
		{
			if (OptionalTarget != null)
			{
				setPieceHandle = OptionalTarget.GetComponent<SetPieceLogic>();
			}
			else
			{
				setPieceHandle = base.gameObject.GetComponent<SetPieceLogic>();
			}
			AddCallableMethod(Label, "S_CMDoSetPiece", Icon);
		}
	}

	protected override void SetIconForDefaultOption()
	{
		if (m_OnceOnly && m_isComplete)
		{
			return;
		}
		base.SetIconForDefaultOption();
		if (mContextBlip != null)
		{
			if (OptionalTarget != null)
			{
				setPieceHandle = OptionalTarget.GetComponent<SetPieceLogic>();
			}
			else
			{
				setPieceHandle = base.gameObject.GetComponent<SetPieceLogic>();
			}
			SetDefaultMethodForFirstPerson(Label, "S_CMDoSetPiece", Icon);
		}
	}

	private void CleaupAfterSelection()
	{
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMDoSetPiece()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController != null)
		{
			if (setPieceHandle.GetNumActorsInvolved() > 1)
			{
				OrdersHelper.OrderMultiCharacterSetPiece(gameplayController, setPieceHandle, base.transform.position, this);
			}
			else
			{
				OrdersHelper.OrderSetPiece(gameplayController, setPieceHandle, this);
			}
		}
		CleaupAfterSelection();
	}
}
