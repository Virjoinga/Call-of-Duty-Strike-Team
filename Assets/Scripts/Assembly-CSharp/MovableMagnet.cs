using UnityEngine;

public class MovableMagnet : MonoBehaviour
{
	public GameObject ObjectToTrack;

	private ActorWrapper trackAW;

	private Actor trackActor;

	private void Start()
	{
		trackAW = ObjectToTrack.GetComponentInChildren<ActorWrapper>();
	}

	public void Update()
	{
		if (trackAW != null)
		{
			if (trackActor == null)
			{
				trackActor = trackAW.GetActor();
			}
			else
			{
				base.transform.position = trackActor.transform.position;
			}
		}
		else if ((bool)ObjectToTrack)
		{
			base.transform.position = ObjectToTrack.transform.position;
		}
	}
}
