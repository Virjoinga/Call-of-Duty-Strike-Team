using UnityEngine;

public class EventOnAlerted : EventDescriptor
{
	private Actor mActorRef;

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
		Actor component = gameObj.GetComponent<Actor>();
		if (component != null)
		{
			mActorRef = component;
			if (GameplayController.Instance() != null)
			{
				GameplayController.Instance().OnEnemyAlarmed += OnEnemyAlarmed;
			}
		}
		else
		{
			Debug.LogWarning("No actor associated with on alerted event");
		}
	}

	private void OnEnemyAlarmed(object sender)
	{
		Actor actor = sender as Actor;
		if (actor == mActorRef)
		{
			FireEvent();
		}
	}

	public override void DeInitialise()
	{
		if (GameplayController.Instance() != null)
		{
			GameplayController.Instance().OnEnemyAlarmed -= OnEnemyAlarmed;
		}
	}

	public void OnDestroy()
	{
		DeInitialise();
	}
}
