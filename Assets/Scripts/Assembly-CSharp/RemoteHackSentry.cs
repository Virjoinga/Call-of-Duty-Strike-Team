using UnityEngine;

public class RemoteHackSentry : MonoBehaviour
{
	public ActorWrapper Target;

	private void Hacked(object param)
	{
		HackableObject hackableObject = param as HackableObject;
		if (hackableObject.LastHacker != null && hackableObject.LastHacker.sentryHacker != null)
		{
			Actor component = Target.GetGameObject().GetComponent<Actor>();
			if (component != null)
			{
				hackableObject.LastHacker.sentryHacker.HackSentry(component);
			}
		}
	}
}
