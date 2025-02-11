using UnityEngine;

public class EventOnMoveChanged : EventDescriptor
{
	private Actor mActorRef;

	private BaseCharacter.MovementStyle mCurrentStyle;

	private bool mIsMoving;

	public BaseCharacter.MovementStyle MovementTypeToTest = BaseCharacter.MovementStyle.Run;

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
		Actor component = gameObj.GetComponent<Actor>();
		if (component != null)
		{
			mActorRef = component;
		}
		else
		{
			Debug.LogWarning("Cant Find actor for event");
		}
	}

	public void Update()
	{
		if (!(mActorRef != null))
		{
			return;
		}
		if (mActorRef.realCharacter.IsMoving() != mIsMoving)
		{
			mIsMoving = mActorRef.realCharacter.IsMoving();
			if (mIsMoving)
			{
				mCurrentStyle = mActorRef.realCharacter.MovementStyleActive;
				if (MovementTypeToTest == mCurrentStyle)
				{
					FireEvent();
				}
			}
		}
		if (mIsMoving && mCurrentStyle != mActorRef.realCharacter.MovementStyleActive)
		{
			mCurrentStyle = mActorRef.realCharacter.MovementStyleActive;
			if (mCurrentStyle == MovementTypeToTest)
			{
				FireEvent();
			}
		}
	}
}
