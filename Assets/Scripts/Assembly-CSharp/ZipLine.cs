using UnityEngine;

[RequireComponent(typeof(CMZipLine))]
public class ZipLine : SetPieceLogic
{
	public ZipLineData Interface;

	private bool mHasOnStartEventTriggeredBefore;

	public new void EnableInteraction()
	{
		ToggleMarker(true);
	}

	public new void DisableInteraction()
	{
		ToggleMarker(false);
	}

	private void ToggleMarker(bool on)
	{
		CMZipLine component = GetComponent<CMZipLine>();
		if (component != null)
		{
			if (on)
			{
				component.Activate();
			}
			else
			{
				component.Deactivate();
			}
		}
	}

	public void OnStart()
	{
		if (mHasOnStartEventTriggeredBefore)
		{
			return;
		}
		mHasOnStartEventTriggeredBefore = true;
		if (Interface == null || Interface.GroupObjectToCallOnStart == null || Interface.GroupObjectToCallOnStart.Count <= 0 || Interface.GroupFunctionToCallOnStart == null)
		{
			return;
		}
		string message = string.Empty;
		int count = Interface.GroupObjectToCallOnStart.Count;
		for (int i = 0; i < count; i++)
		{
			if (i < Interface.GroupFunctionToCallOnStart.Count)
			{
				message = Interface.GroupFunctionToCallOnStart[i];
			}
			Container.SendMessage(Interface.GroupObjectToCallOnStart[i], message, base.gameObject);
		}
	}
}
