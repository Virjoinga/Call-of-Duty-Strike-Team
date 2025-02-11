using System.Collections.Generic;
using UnityEngine;

public class SniperFromDoorOverride : ContainerOverride
{
	public GameObject moveTo;

	public GameObject LookAt;

	public SniperOverrideData m_SniperOverrideData;

	private EnemyFromDoorOverride doorOverride;

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		RoutineDescriptor routineDescriptor = cont.FindComponentOfType(typeof(RoutineDescriptor)) as RoutineDescriptor;
		MoveToDescriptor moveToDescriptor = cont.FindComponentOfType(typeof(MoveToDescriptor)) as MoveToDescriptor;
		SniperDescriptor sniperDescriptor = cont.FindComponentOfType(typeof(SniperDescriptor)) as SniperDescriptor;
		if (m_SniperOverrideData != null && sniperDescriptor != null)
		{
			m_SniperOverrideData.ResolveGuidLinks();
			sniperDescriptor.SniperOverrides.ResolveGuidLinks();
		}
		Component[] array = cont.FindComponentsOfType(typeof(Transform));
		if (moveTo == null)
		{
			Component[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Transform transform = (Transform)array2[i];
				if (transform.name.Equals("Move To"))
				{
					moveTo = transform.gameObject;
				}
			}
		}
		if (moveTo != null && moveToDescriptor != null)
		{
			moveToDescriptor.Target = moveTo.transform;
		}
		if (LookAt == null)
		{
			Component[] array3 = array;
			for (int j = 0; j < array3.Length; j++)
			{
				Transform transform2 = (Transform)array3[j];
				if (transform2.name.Equals("Look At"))
				{
					LookAt = transform2.gameObject;
				}
			}
		}
		if (LookAt != null && sniperDescriptor != null)
		{
			sniperDescriptor.StartLookAt = LookAt.transform;
		}
		if (m_SniperOverrideData == null)
		{
			m_SniperOverrideData = new SniperOverrideData();
		}
		if (m_SniperOverrideData.KillZones == null)
		{
			m_SniperOverrideData.KillZones = new List<KillZone>();
		}
		m_SniperOverrideData.KillZones.RemoveAll((KillZone item) => item == null);
		if (m_SniperOverrideData.KillZones.Count == 0)
		{
			Component[] array4 = array;
			for (int k = 0; k < array4.Length; k++)
			{
				Transform transform3 = (Transform)array4[k];
				if (transform3.name.Equals("Target"))
				{
					KillZone component = transform3.gameObject.GetComponent<KillZone>();
					if (component != null)
					{
						m_SniperOverrideData.KillZones.Add(component);
					}
				}
			}
		}
		if (m_SniperOverrideData.KillZones.Count >= 1 && m_SniperOverrideData.DesiredTargets.Count == 0)
		{
			m_SniperOverrideData.Targets.Clear();
			NewCoverPointManager newCoverPointManager = (NewCoverPointManager)Object.FindObjectOfType(typeof(NewCoverPointManager));
			for (int l = 0; l < newCoverPointManager.coverPoints.Length - 1; l++)
			{
				foreach (KillZone killZone in m_SniperOverrideData.KillZones)
				{
					BoxCollider component2 = killZone.GetComponent<BoxCollider>();
					if (component2 != null && newCoverPointManager.coverPoints[l] != null && component2.bounds.Contains(newCoverPointManager.coverPoints[l].gamePos))
					{
						m_SniperOverrideData.Targets.Add(newCoverPointManager.coverPoints[l].gamePos);
					}
				}
			}
		}
		if (m_SniperOverrideData != null)
		{
			sniperDescriptor.SniperOverrides = m_SniperOverrideData;
		}
		if (doorOverride == null)
		{
			doorOverride = base.transform.parent.GetComponent<EnemyFromDoorOverride>();
		}
		if (!(doorOverride != null) || doorOverride.m_OverrideData == null)
		{
			return;
		}
		if (doorOverride.m_OverrideData.SpecialistRoutines == null)
		{
			doorOverride.m_OverrideData.SpecialistRoutines = new List<RoutineDescriptor>();
		}
		doorOverride.m_OverrideData.SpecialistRoutines.RemoveAll((RoutineDescriptor item) => item == null);
		foreach (RoutineDescriptor specialistRoutine in doorOverride.m_OverrideData.SpecialistRoutines)
		{
			foreach (TaskDescriptor task in specialistRoutine.Tasks)
			{
				MoveToDescriptor moveToDescriptor2 = task as MoveToDescriptor;
				if ((bool)moveToDescriptor2)
				{
					moveToDescriptor2.Parameters.mMovementStyle = BaseCharacter.MovementStyle.AsFastAsSafelyPossible;
				}
			}
		}
		if (routineDescriptor != null && !doorOverride.m_OverrideData.SpecialistRoutines.Contains(routineDescriptor))
		{
			doorOverride.m_OverrideData.SpecialistRoutines.Add(routineDescriptor);
		}
	}
}
