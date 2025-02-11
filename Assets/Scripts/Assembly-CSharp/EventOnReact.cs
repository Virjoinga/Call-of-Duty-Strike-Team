using UnityEngine;

public class EventOnReact : EventDescriptor
{
	private Actor mActorRef;

	private bool mHasAlreadyRecievedEvent;

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
		Actor component = gameObj.GetComponent<Actor>();
		if (component != null)
		{
			mActorRef = component;
			if (GameplayController.Instance() != null)
			{
				GameplayController.Instance().OnEnemyReact += OnEnemyReact;
			}
		}
		else
		{
			Debug.LogWarning("No actor associated with on alerted event");
		}
	}

	private void OnEnemyReact(object sender)
	{
		Actor actor = sender as Actor;
		if (actor == mActorRef && !mHasAlreadyRecievedEvent)
		{
			mHasAlreadyRecievedEvent = true;
			FireEvent();
		}
	}

	public override void DeInitialise()
	{
		if (GameplayController.Instance() != null)
		{
			GameplayController.Instance().OnEnemyReact -= OnEnemyReact;
		}
	}

	public void OnDestroy()
	{
		DeInitialise();
	}
}
