using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ExplodableObjectData
{
	public enum Type
	{
		Timer = 1,
		Distance = 2
	}

	public Type DetonationType = Type.Distance;

	public ExplosionManager.ExplosionType ExplosionType;

	public bool UseMissionTimer;

	public float TimeToDetonate = 10f;

	public float TriggerRadius = 15f;

	public bool UseRemoteTrigger;

	public GameObject GlobalTriggerObject;

	public GameObject[] ExplodedObjectsToRemove;

	public GameObject[] ExplodedObjectsToShow;

	public List<ObjectMessage> MessagesOnDetonation = new List<ObjectMessage>();

	public List<ObjectMessage> MessagesOnArm = new List<ObjectMessage>();
}
