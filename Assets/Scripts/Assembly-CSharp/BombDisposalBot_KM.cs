using System.Collections;
using UnityEngine;

public class BombDisposalBot_KM : MonoBehaviour
{
	private UnityEngine.AI.NavMeshAgent agent;

	public GameObject Target;

	public GameObject ProgressBlipRef;

	public float TopSpeed;

	private HackingBlip ProgressBlip;

	private Destructible ds;

	private float my_Health = 1f;

	private float CurrSpeed;

	private bool isActive;

	public GameObject Actor;

	private void Start()
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.SetDestination(Target.transform.position);
		if (!(ProgressBlipRef != null))
		{
			return;
		}
		ProgressBlipRef = Object.Instantiate(ProgressBlipRef) as GameObject;
		if (ProgressBlipRef != null)
		{
			ProgressBlip = ProgressBlipRef.GetComponent<HackingBlip>();
			if (ProgressBlip != null)
			{
				ProgressBlip.Target = base.transform;
			}
		}
		ProgressBlip.ShowBlip();
		ds = base.gameObject.GetComponentInChildren<Destructible>();
		StartCoroutine(Init());
	}

	private IEnumerator Init()
	{
		ActorWrapper actorWrapper = null;
		while (actorWrapper == null)
		{
			yield return null;
			actorWrapper = Actor.GetComponentInChildren<ActorWrapper>();
		}
		Actor actor = null;
		while (actor == null)
		{
			yield return null;
			actor = actorWrapper.GetActor();
		}
		Transform referenceFrame = base.transform;
		actor.realCharacter.SetReferenceFrame(referenceFrame);
		SkinnedMeshRenderer mySkin = actor.model.GetComponentInChildren<SkinnedMeshRenderer>();
		mySkin.enabled = false;
	}

	private void Update()
	{
		if (isActive)
		{
			ActorWrapper componentInChildren = Actor.GetComponentInChildren<ActorWrapper>();
			if (componentInChildren == null)
			{
				return;
			}
			Actor actor = componentInChildren.GetActor();
			if (actor == null)
			{
				return;
			}
			actor.SetPosition(base.transform.position);
		}
		CurrSpeed = Mathf.Lerp(CurrSpeed, TopSpeed, Time.deltaTime * 1f);
		agent.SetDestination(Target.transform.position);
		if (ds != null)
		{
			ProgressBlip.SetProgress(my_Health);
			ProgressBlip.ShowBlip();
		}
		agent.speed = CurrSpeed;
	}

	public void Begin()
	{
		isActive = !isActive;
		CommonHudController.Instance.MissionTimer.StartTimer();
		CommonHudController.Instance.ShowScore(true);
	}

	public void ReactToExplosion()
	{
		CurrSpeed = 0.1f;
	}

	public void ResumeBot()
	{
		TopSpeed = 1.75f;
		agent.Resume();
	}

	public void StopBot()
	{
		TopSpeed = 0f;
	}

	public void StopTimer()
	{
		CommonHudController.Instance.MissionTimer.PauseTimer();
	}

	public void Destroyed()
	{
		CurrSpeed = 0f;
		TopSpeed = 0f;
		CommonHudController.Instance.MissionTimer.PauseTimer();
		HUDMessenger.Instance.PushMessage("YOU FAILED TO PROTECT THE BOT", true);
	}
}
