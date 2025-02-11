using UnityEngine;

public class Condition_InRange : Condition
{
	public GameObject targetA;

	private ActorWrapper targetAWrapper;

	public GameObject targetB;

	private ActorWrapper targetBWrapper;

	public float range;

	private Vector3 position1 = Vector3.zero;

	private Vector3 position2 = Vector3.zero;

	public void Start()
	{
		if (targetA != null)
		{
			targetAWrapper = targetA.GetComponentInChildren<ActorWrapper>();
		}
		if (targetB != null)
		{
			targetBWrapper = targetB.GetComponentInChildren<ActorWrapper>();
		}
	}

	public override bool Value()
	{
		if (!GetPositionsFromActors())
		{
			return false;
		}
		return (position1 - position2).sqrMagnitude < range * range;
	}

	private void OnDrawGizmosSelected()
	{
		if (targetA != null && targetB != null)
		{
			GetPositionsFromActors();
			Gizmos.color = Color.red;
			Vector3 normalized = (position2 - position1).normalized;
			Vector3 vector = position1 + range * normalized;
			Gizmos.color = Color.green;
			Gizmos.DrawLine(position1, vector);
			Gizmos.DrawWireSphere(vector, 0.1f);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(vector, position2);
		}
	}

	private bool GetPositionsFromActors()
	{
		if (targetA != null)
		{
			if (targetAWrapper != null)
			{
				Actor actor = targetAWrapper.GetActor();
				if (actor != null)
				{
					position1 = actor.transform.position;
				}
				else
				{
					position1 = targetA.transform.position;
				}
			}
			else
			{
				position1 = targetA.transform.position;
			}
			if (targetB != null)
			{
				if (targetBWrapper != null)
				{
					Actor actor2 = targetBWrapper.GetActor();
					if (actor2 != null)
					{
						position2 = actor2.transform.position;
					}
					else
					{
						position2 = targetB.transform.position;
					}
				}
				else
				{
					position2 = targetB.transform.position;
				}
				return true;
			}
			return false;
		}
		return false;
	}
}
