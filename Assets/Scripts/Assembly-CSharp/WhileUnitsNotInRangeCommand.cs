using System.Collections;
using UnityEngine;

public class WhileUnitsNotInRangeCommand : Command
{
	public GameObject Player1;

	public GameObject Player2;

	public float Range;

	public ObjectMessage MessageToSend;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		if (Player1 == null || Player2 == null)
		{
			yield break;
		}
		Actor actor1 = null;
		Actor actor2 = null;
		ActorWrapper aw4 = Player1.GetComponentInChildren<ActorWrapper>();
		if (aw4 != null)
		{
			actor1 = aw4.GetActor();
			aw4 = null;
		}
		aw4 = Player2.GetComponentInChildren<ActorWrapper>();
		if (aw4 != null)
		{
			actor2 = aw4.GetActor();
			aw4 = null;
		}
		if (actor1 != null && actor2 != null)
		{
			if (Mathf.Abs(Vector3.Distance(actor1.transform.position, actor2.transform.position)) > Range && MessageToSend.Object != null && MessageToSend.Message != null && MessageToSend.Message.Length > 0)
			{
				Container.SendMessage(MessageToSend.Object, MessageToSend.Message, base.gameObject);
			}
			while (Mathf.Abs(Vector3.Distance(actor1.transform.position, actor2.transform.position)) > Range)
			{
				yield return null;
			}
		}
	}
}
