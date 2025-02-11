using UnityEngine;

public class ActionComponent : MonoBehaviour
{
	public enum ActionType
	{
		MustSee = 0,
		PreferToSee = 1,
		Ignore = 2
	}

	public ActionType Type = ActionType.PreferToSee;

	private bool CanPlay()
	{
		return InteractionsManager.Instance.CanIPlayAction(base.gameObject, null);
	}

	private bool CanControlCamera()
	{
		return InteractionsManager.Instance.CanIControlTheCamera(base.gameObject, null, null);
	}
}
