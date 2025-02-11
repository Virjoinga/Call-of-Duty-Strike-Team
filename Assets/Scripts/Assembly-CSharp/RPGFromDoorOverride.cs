using System.Collections.Generic;
using UnityEngine;

public class RPGFromDoorOverride : ContainerOverride
{
	public TaskConfigDescriptor ConfigFlags = new TaskConfigDescriptor();

	public RPGOverrideData m_OverrideData;

	private EnemyFromDoorOverride doorOverride;

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		Component[] array = cont.FindComponentsOfType(typeof(Transform));
		if (m_OverrideData.SpawnTarget == null)
		{
			Component[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Transform transform = (Transform)array2[i];
				if (transform.name.Equals("RPG Move To Target"))
				{
					m_OverrideData.SpawnTarget = transform.gameObject;
				}
			}
		}
		if (m_OverrideData.Target == null)
		{
			Component[] array3 = array;
			for (int j = 0; j < array3.Length; j++)
			{
				Transform transform2 = (Transform)array3[j];
				if (transform2.name.Equals("RPG Shoot Target"))
				{
					m_OverrideData.Target = transform2.gameObject;
				}
			}
		}
		RPGDescriptor rPGDescriptor = cont.FindComponentOfType(typeof(RPGDescriptor)) as RPGDescriptor;
		if (rPGDescriptor != null && m_OverrideData != null)
		{
			rPGDescriptor.m_Interface = m_OverrideData;
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
			RoutineDescriptor routineDescriptor = cont.FindComponentOfType(typeof(RoutineDescriptor)) as RoutineDescriptor;
			if (routineDescriptor != null && !doorOverride.m_OverrideData.SpecialistRoutines.Contains(routineDescriptor))
			{
				doorOverride.m_OverrideData.SpecialistRoutines.Add(routineDescriptor);
			}
		}
	}
}
