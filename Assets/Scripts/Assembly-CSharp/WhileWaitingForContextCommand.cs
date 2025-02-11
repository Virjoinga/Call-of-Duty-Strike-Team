using System.Collections;

public class WhileWaitingForContextCommand : Command
{
	public InteriorItem ObjectWithCM;

	public bool WaitingTurnOn;

	public override bool Blocking()
	{
		return true;
	}

	public override void ResolveGuidLinks()
	{
		if (ObjectWithCM != null)
		{
			ObjectWithCM.ResolveGuidLinks();
		}
	}

	public override IEnumerator Execute()
	{
		InterfaceableObject cm2 = null;
		if (ObjectWithCM.Interior == null)
		{
			yield break;
		}
		InteriorOverride io = ObjectWithCM.Interior.theObject.GetComponentInChildren<InteriorOverride>();
		cm2 = ((!io) ? ObjectWithCM.Interior.theObject.GetComponentInChildren<InterfaceableObject>() : ((ObjectWithCM.ContextType != 0) ? io.m_ActiveWindows[ObjectWithCM.IndexToReference].m_Object.GetComponentInChildren<InterfaceableObject>() : io.m_ActiveDoors[ObjectWithCM.IndexToReference].m_Object.GetComponentInChildren<InterfaceableObject>()));
		if (!(cm2 != null))
		{
			yield break;
		}
		while (true)
		{
			if (WaitingTurnOn)
			{
				if (cm2.ContextBlip.IsOnScreen && cm2.ContextBlip.IsSwitchedOn)
				{
					break;
				}
			}
			else if (!cm2.ContextBlip.IsOnScreen || !cm2.ContextBlip.IsSwitchedOn)
			{
				break;
			}
			yield return null;
		}
	}
}
