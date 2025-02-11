using System.Collections;
using UnityEngine;

public class SendMessageCommand : Command
{
	[HideInInspector]
	public GameObject target;

	public GuidRef Target;

	public string Message;

	public string StringParam;

	public GameObject GameObjectParam;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (Target != null)
		{
			if (GameObjectParam != null)
			{
				Container.SendMessageWithParam(Target, Message, GameObjectParam, base.gameObject);
			}
			else if (StringParam != string.Empty)
			{
				Container.SendMessageWithParam(Target, Message, StringParam, base.gameObject);
			}
			else
			{
				Container.SendMessage(Target, Message, base.gameObject);
			}
		}
		yield break;
	}

	public override void ResolveGuidLinks()
	{
		if (target != null)
		{
			Target.SetObjectWithGuid(target);
		}
		Target.ResolveLink();
	}
}
