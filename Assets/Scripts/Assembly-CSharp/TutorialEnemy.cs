using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
	public TutorialEnemyOverrideData m_TutorialEnemyOverrideData = new TutorialEnemyOverrideData();

	public void Start()
	{
		Spawner componentInChildren = GetComponentInChildren<Spawner>();
		if (componentInChildren != null)
		{
			componentInChildren.m_TutorialEnemyOverrides = m_TutorialEnemyOverrideData;
			componentInChildren.IsTutorialEnemy = true;
			return;
		}
		SpawnerDoor componentInChildren2 = GetComponentInChildren<SpawnerDoor>();
		if (componentInChildren2 != null)
		{
			componentInChildren2.m_TutorialEnemyOverrides = m_TutorialEnemyOverrideData;
			componentInChildren2.IsTutorialEnemy = true;
		}
	}
}
