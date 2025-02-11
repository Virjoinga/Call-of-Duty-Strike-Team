using System.Collections.Generic;
using UnityEngine;

public class RiotShieldFromDoorOverride : ContainerOverride
{
	public GameObject moveTo;

	public RiotShieldDescriptorConfig m_RiotShieldOverrideData;

	private EnemyFromDoorOverride doorOverride;

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		RoutineDescriptor routineDescriptor = cont.FindComponentOfType(typeof(RoutineDescriptor)) as RoutineDescriptor;
		MoveToDescriptor moveToDescriptor = cont.FindComponentOfType(typeof(MoveToDescriptor)) as MoveToDescriptor;
		RiotShieldDescriptor riotShieldDescriptor = cont.FindComponentOfType(typeof(RiotShieldDescriptor)) as RiotShieldDescriptor;
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
		if (m_RiotShieldOverrideData == null)
		{
			m_RiotShieldOverrideData = new RiotShieldDescriptorConfig();
		}
		if (m_RiotShieldOverrideData != null)
		{
			riotShieldDescriptor.SpawnConfig = m_RiotShieldOverrideData;
		}
		if (doorOverride == null)
		{
			doorOverride = base.transform.parent.GetComponent<EnemyFromDoorOverride>();
		}
		if (doorOverride != null && doorOverride.m_OverrideData != null)
		{
			if (doorOverride.m_OverrideData.SpecialistRoutines == null)
			{
				doorOverride.m_OverrideData.SpecialistRoutines = new List<RoutineDescriptor>();
			}
			doorOverride.m_OverrideData.SpecialistRoutines.RemoveAll((RoutineDescriptor item) => item == null);
			if (routineDescriptor != null && !doorOverride.m_OverrideData.SpecialistRoutines.Contains(routineDescriptor))
			{
				doorOverride.m_OverrideData.SpecialistRoutines.Add(routineDescriptor);
			}
		}
	}
}
