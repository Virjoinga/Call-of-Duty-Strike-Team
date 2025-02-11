using UnityEngine;

public class Condition_Spawned : Condition
{
	private bool mSpawned;

	private void Awake()
	{
		mSpawned = false;
	}

	private void OnSpawned(GameObject spawned)
	{
		mSpawned = true;
	}

	public override bool Value()
	{
		return mSpawned;
	}
}
