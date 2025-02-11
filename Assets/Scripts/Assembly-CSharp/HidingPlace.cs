using UnityEngine;

public class HidingPlace : MonoBehaviour
{
	public SetPieceModule EnterSetPiece;

	public SetPieceModule ExitSetPiece;

	public SetPieceModule HideBodySetPiece;

	protected InterfaceableObject mContextMenu;

	public GameObject Model;

	public Transform SetPieceLocation;

	private Actor mOccupier;

	private bool mIsACarriedBody;

	public bool IsOccupied
	{
		get
		{
			return mOccupier != null;
		}
	}

	public Actor Occupier
	{
		get
		{
			return mOccupier;
		}
	}

	public void SetOccupier(Actor actor, bool isACarriedBody)
	{
		if (!(actor == null))
		{
			mOccupier = actor;
			mIsACarriedBody = isACarriedBody;
			if (mIsACarriedBody)
			{
				DeactivateBlip();
			}
			else
			{
				DisableBlip();
			}
		}
	}

	public void ClearOccupier()
	{
		mOccupier = null;
		if (mIsACarriedBody)
		{
			ActivateBlip();
		}
		else
		{
			EnableBlip();
		}
	}

	public void EnableBlip()
	{
		if (mContextMenu != null)
		{
			mContextMenu.UseBlip = true;
			mContextMenu.enabled = true;
		}
	}

	public void DisableBlip()
	{
		if (mContextMenu != null)
		{
			mContextMenu.UseBlip = false;
			mContextMenu.enabled = false;
		}
	}

	public void ActivateBlip()
	{
		if (mContextMenu != null)
		{
			mContextMenu.Activate();
		}
	}

	public void DeactivateBlip()
	{
		if (mContextMenu != null)
		{
			mContextMenu.Deactivate();
		}
	}
}
