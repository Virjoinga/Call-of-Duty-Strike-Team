using UnityEngine;

public class Condition_AllSpawnedDead : Condition
{
	private bool mAnySpawned;

	private int mSpawnedAlive;

	private void Awake()
	{
		mSpawnedAlive = 0;
	}

	private void OnSpawned(GameObject spawned)
	{
		mAnySpawned = true;
		HealthComponent component = spawned.GetComponent<HealthComponent>();
		if (component != null && !component.HealthEmpty)
		{
			mSpawnedAlive++;
			component.OnHealthEmpty += delegate
			{
				mSpawnedAlive--;
			};
		}
	}

	public override bool Value()
	{
		return mAnySpawned && mSpawnedAlive == 0;
	}
}
