using UnityEngine;

public class EventOnInRange : EventDescriptor
{
	public GameObject RangedObject;

	public float DistanceToCheck = 25f;

	private float mDistanceCheckSqr;

	private Actor mActorRef;

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

	public override void Start()
	{
		base.Start();
		ActorWrapper componentInChildren = RangedObject.GetComponentInChildren<ActorWrapper>();
		if (componentInChildren != null)
		{
			componentInChildren.TargetActorChanged += Setup;
		}
		mDistanceCheckSqr = DistanceToCheck * DistanceToCheck;
	}

	private void Setup(object sender)
	{
		GameObject gameObject = sender as GameObject;
		if (gameObject != null)
		{
			RangedObject = gameObject;
		}
	}

	public void Update()
	{
		if (!(mActorRef == null))
		{
			float num = Vector3.SqrMagnitude(RangedObject.transform.position - mActorRef.transform.position);
			if (num < mDistanceCheckSqr)
			{
				FireEvent();
			}
		}
	}
}
