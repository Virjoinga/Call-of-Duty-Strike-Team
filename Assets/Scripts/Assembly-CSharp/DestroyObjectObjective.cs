using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectObjective : MissionObjective
{
	[HideInInspector]
	public List<ExplodableObject> ObjectsToDestroy = new List<ExplodableObject>();

	public DestroyObjectData m_destroyInterface;

	private int numberArmed;

	public override void Start()
	{
		base.Start();
		foreach (ExplodableObject item in ObjectsToDestroy)
		{
			if (item != null)
			{
				if (m_destroyInterface.ArmOnly)
				{
					item.AddArmEventHandler(OnArmEventHandler);
				}
				item.AddExplodeEventHandler(OnExplodeEventHandler);
				item.CanExplode = !m_destroyInterface.DestroyOnMessage;
				base.gameObject.transform.position = item.gameObject.transform.position;
				MoveToBoundsY(item.gameObject);
			}
		}
		UpdateBlipTarget();
		numberArmed = ObjectsToDestroy.Count;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		foreach (ExplodableObject item in ObjectsToDestroy)
		{
			if (item != null)
			{
				if (m_destroyInterface.ArmOnly)
				{
					item.RemoveArmEventHandler(OnArmEventHandler);
				}
				else
				{
					item.RemoveExplodeEventHandler(OnExplodeEventHandler);
				}
			}
		}
	}

	public void Explode()
	{
		foreach (ExplodableObject item in ObjectsToDestroy)
		{
			if (item != null)
			{
				item.Explode();
			}
		}
	}

	private void OnExplodeEventHandler(object sender)
	{
		ObjectsToDestroy.Remove(sender as ExplodableObject);
		if (!m_destroyInterface.ArmOnly && ObjectsToDestroy.Count == 0)
		{
			Pass();
		}
	}

	private void OnArmEventHandler(object sender)
	{
		numberArmed--;
		if (numberArmed == 0)
		{
			Pass();
		}
	}
}
