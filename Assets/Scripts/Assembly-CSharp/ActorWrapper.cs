using UnityEngine;

public class ActorWrapper : MonoBehaviour
{
	public delegate void TargetActorChangedEventHandler(object sender);

	private GameObject mActorObject;

	public event TargetActorChangedEventHandler TargetActorChanged;

	private void OnSpawned(GameObject spawned)
	{
		mActorObject = spawned;
		if (this.TargetActorChanged != null)
		{
			this.TargetActorChanged(mActorObject);
		}
	}

	public Actor GetActor()
	{
		if (mActorObject == null)
		{
			return null;
		}
		return mActorObject.GetComponent<Actor>();
	}

	public GameObject GetGameObject()
	{
		return mActorObject;
	}
}
