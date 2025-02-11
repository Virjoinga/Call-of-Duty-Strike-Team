using UnityEngine;

public class EventOnMove : EventDescriptor
{
	public BaseCharacter.MovementStyle MovementType = BaseCharacter.MovementStyle.Run;

	public override void Start()
	{
		base.Start();
	}

	public void OnMove(Actor a)
	{
		if (a.realCharacter.MovementStyleRequested == MovementType)
		{
			FireEvent();
		}
	}

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
	}
}
