using System;
using UnityEngine;

public class Claymore : MonoBehaviour
{
	private const float DAMAGE_RADIUS = 10f;

	public float DetonationTime;

	private Actor mOwner;

	private float mTriggerTime;

	private bool mTriggered;

	public BaseCharacter.Nationality TeamNationality { get; set; }

	public Actor Owner
	{
		get
		{
			return mOwner;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (mTriggered)
		{
			mTriggerTime += Time.deltaTime;
			if (mTriggerTime >= DetonationTime)
			{
				RulesSystem.DoAreaOfEffectDamage(base.transform.position, 10f, 1f, mOwner.gameObject, ExplosionManager.ExplosionType.Claymore, "Claymore");
				BroadcastExplosion();
				BroadcastNoise();
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	public void PlaceMine(Actor owner, Vector3 target)
	{
		mOwner = owner;
		TeamNationality = mOwner.realCharacter.VocalAccent;
		base.transform.position = target;
	}

	public void OnTriggerEnter(Collider other)
	{
		BaseCharacter component = other.gameObject.GetComponent<BaseCharacter>();
		if (!(component == null) && component.VocalAccent != TeamNationality)
		{
			mTriggerTime = 0f;
			mTriggered = true;
		}
	}

	private void BroadcastNoise()
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (!(a.ears == null) && a.ears.CanHear && AwarenessZone.IsUnregisteredGameObjectAwarenessInSync(base.gameObject, a.gameObject))
			{
				float magnitude = (base.transform.position - a.GetPosition()).magnitude;
				if (magnitude < AudioResponseRanges.Explosion + a.ears.Range)
				{
					a.awareness.BecomeAware(mOwner, base.transform.position);
				}
			}
		}
	}

	private void BroadcastExplosion()
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.AliveMask & ~mOwner.ident);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			Vector3 vector = base.transform.position - a.GetPosition();
			float num = Vector3.Dot(a.awareness.LookDirection.normalized, vector.normalized);
			float num2 = Mathf.Cos(a.awareness.FoV * ((float)Math.PI / 180f));
			if (!(num > num2) || !(vector.sqrMagnitude < a.awareness.visionRangeSqr))
			{
				continue;
			}
			if (a.awareness.ChDefCharacterType == CharacterType.SecurityCamera)
			{
				TaskSecurityCamera taskSecurityCamera = (TaskSecurityCamera)a.tasks.GetRunningTask(typeof(TaskSecurityCamera));
				if (taskSecurityCamera != null)
				{
					taskSecurityCamera.SoundAlarm(mOwner, base.transform.position);
				}
			}
			a.awareness.BecomeAware(mOwner, base.transform.position);
		}
	}
}
