using UnityEngine;

public class BaseActorComponent : MonoBehaviour
{
	public Actor myActor;

	public BaseActorComponent Connect(Actor actor)
	{
		actor.Connect(this);
		return this;
	}
}
