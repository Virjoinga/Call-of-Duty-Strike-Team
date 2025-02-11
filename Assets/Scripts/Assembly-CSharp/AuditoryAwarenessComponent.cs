using System.Collections.Generic;
using UnityEngine;

public class AuditoryAwarenessComponent : BaseActorComponent
{
	public float Range;

	public bool CanHear = true;

	public bool CanBeHeard = true;

	private List<AuditoryAwarenessComponent> mICanHear;

	private List<AuditoryAwarenessComponent> mCanHearMe;

	private float mRangeSqr;

	public float RangeSqr
	{
		get
		{
			return mRangeSqr;
		}
		set
		{
			mRangeSqr = value;
		}
	}

	public List<AuditoryAwarenessComponent> ICanHearList
	{
		get
		{
			return mICanHear;
		}
	}

	private void Awake()
	{
		mICanHear = new List<AuditoryAwarenessComponent>();
		mCanHearMe = new List<AuditoryAwarenessComponent>();
	}

	private void Start()
	{
		AuditoryAwarenessManager.Instance.AddAuditoryAwarenessComponent(this);
		mRangeSqr = Range * Range;
	}

	private void OnDestroy()
	{
		AuditoryAwarenessManager.Instance.RemoveAuditoryAwarenessComponent(this);
	}

	private void Update()
	{
	}

	public bool CanIHear(AuditoryAwarenessComponent aac)
	{
		return mICanHear.Contains(aac);
	}

	public bool CanHearMe(AuditoryAwarenessComponent aac)
	{
		return mCanHearMe.Contains(aac);
	}

	public void ClearCanHear()
	{
		mICanHear.Clear();
		mCanHearMe.Clear();
	}

	public void UpdateCanHear(List<AuditoryAwarenessComponent> aaNodes)
	{
		Vector2 vector = new Vector2(base.transform.position.x, base.transform.position.z);
		foreach (AuditoryAwarenessComponent aaNode in aaNodes)
		{
			if (this == aaNode || !aaNode.CanBeHeard)
			{
				continue;
			}
			Vector2 vector2 = new Vector2(aaNode.transform.position.x, aaNode.transform.position.z);
			Vector2 vector3 = vector2 - vector;
			if (!(vector3.sqrMagnitude < mRangeSqr))
			{
				continue;
			}
			float noiseRadius = GetNoiseRadius(aaNode);
			if (noiseRadius > 0f)
			{
				float magnitude = vector3.magnitude;
				if (magnitude < noiseRadius)
				{
					mICanHear.Add(aaNode);
					aaNode.mCanHearMe.Add(this);
				}
			}
		}
	}

	public bool CanHearEvent(AuditoryAwarenessEvent aaEvent)
	{
		float sqrMagnitude = (base.transform.position - aaEvent.transform.position).sqrMagnitude;
		if (sqrMagnitude > aaEvent.Radius * aaEvent.Radius || sqrMagnitude > mRangeSqr)
		{
			return false;
		}
		return AwarenessZone.IsUnregisteredGameObjectAwarenessInSync(myActor.GetPosition(), aaEvent.Origin);
	}

	public bool CanHearEvent(AuditoryAwarenessManager.AAWorldEvent aaWorldEvent)
	{
		if (myActor == null || aaWorldEvent.Owner == null)
		{
			return false;
		}
		if (myActor == aaWorldEvent.Owner)
		{
			return false;
		}
		if (!myActor.awareness.IsEnemy(aaWorldEvent.Owner) && myActor.behaviour.alertState >= BehaviourController.AlertState.Reacting)
		{
			return false;
		}
		float sqrMagnitude = (base.transform.position - aaWorldEvent.Origin).sqrMagnitude;
		if (sqrMagnitude > aaWorldEvent.Radius * aaWorldEvent.Radius || sqrMagnitude > RangeSqr)
		{
			return false;
		}
		return AwarenessZone.IsUnregisteredGameObjectAwarenessInSync(myActor.GetPosition(), aaWorldEvent.Origin);
	}

	public void GlDebugVisualise()
	{
		float num = 1.5f;
		Vector3 position = base.transform.position;
		position.y += num;
		GL.Color(Color.yellow);
		foreach (AuditoryAwarenessComponent item in mICanHear)
		{
			if (!(item == null))
			{
				Vector3 position2 = item.transform.position;
				GL.Vertex3(position.x, position.y, position.z);
				GL.Vertex3(position2.x, position2.y, position2.z);
			}
		}
		GL.Color(Color.red);
		foreach (AuditoryAwarenessComponent item2 in mCanHearMe)
		{
			if (!(item2 == null))
			{
				Vector3 position3 = item2.transform.position;
				GL.Vertex3(position.x, position.y - num * 0.5f, position.z);
				GL.Vertex3(position3.x, position3.y, position3.z);
			}
		}
	}

	public float GetNoiseRadius(AuditoryAwarenessComponent target)
	{
		float num = 0f;
		Actor component = target.GetComponent<Actor>();
		if (component == null)
		{
			return num;
		}
		if (component.weapon.IsFiring() && !component.weapon.IsShootingSilenced() && num < AudioResponseRanges.Gunshot)
		{
			num = AudioResponseRanges.Gunshot;
		}
		if (component.baseCharacter.IsRunning() && myActor != null && myActor.realCharacter.Location == component.realCharacter.Location && num < AudioResponseRanges.Footsteps)
		{
			num = AudioResponseRanges.Footsteps;
		}
		return num;
	}
}
