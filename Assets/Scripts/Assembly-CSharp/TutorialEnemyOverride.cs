using UnityEngine;

public class TutorialEnemyOverride : ContainerOverride
{
	public RoutineDescriptorData m_RoutineOverrideData = new RoutineDescriptorData();

	public SpawnerData m_SpawnerOverrideData = new SpawnerData();

	public TutorialEnemyOverrideData m_TutorialEnemyOverrideData = new TutorialEnemyOverrideData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		m_SpawnerOverrideData.ResolveGuidLinks();
		WorldHelper.KillNamedChildren(base.gameObject, "Automated");
		if (m_SpawnerOverrideData.quickDestination != null)
		{
			GameObject gameObject = new GameObject("Automated");
			TaskDescriptor taskDescriptor = m_SpawnerOverrideData.quickDestination.GenerateRoutine(ref m_RoutineOverrideData, gameObject, m_SpawnerOverrideData.AssaultParameters);
			if (taskDescriptor != null)
			{
				m_RoutineOverrideData.TaskListObject = gameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
			}
			else if (Application.isPlaying)
			{
				gameObject.transform.parent = null;
				Object.Destroy(gameObject.gameObject);
			}
			else
			{
				Object.DestroyImmediate(gameObject.gameObject);
			}
		}
		RoutineDescriptor routineDescriptor = cont.FindComponentOfType(typeof(RoutineDescriptor)) as RoutineDescriptor;
		if (routineDescriptor != null)
		{
			routineDescriptor.m_Interface = m_RoutineOverrideData;
			m_RoutineOverrideData.CopyContainerData(routineDescriptor);
		}
		Spawner spawner = cont.FindComponentOfType(typeof(Spawner)) as Spawner;
		if (spawner != null)
		{
			spawner.m_Interface = m_SpawnerOverrideData;
			m_SpawnerOverrideData.CopyContainerData(spawner);
			spawner.m_TutorialEnemyOverrides = m_TutorialEnemyOverrideData;
			spawner.IsTutorialEnemy = true;
		}
	}
}
